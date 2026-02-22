using AuctionHub_backend.Core.Interfaces;
using AuctionHub_backend.Data;
using AuctionHub_backend.Data.Dtos;
using AuctionHub_backend.Data.Entities;
using AuctionHub_backend.Data.Interfaces;
using AuctionHub_backend.Data.Repos;

namespace AuctionHub_backend.Core.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepo _auctionRepo;
        private readonly AuctionDbContext _context;

        public AuctionService(IAuctionRepo auctionRepo, AuctionDbContext context)
        {
            _auctionRepo = auctionRepo;
            _context = context;
        }

        private static bool IsOpen(Auction a, DateTime nowUtc)
        => !a.IsDisabled && a.StartDate <= nowUtc && a.EndDate > nowUtc;

        public async Task<bool> CancelLastBidAsync(int userId, int auctionId)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);
            if (auction == null || auction.IsDisabled)
                return false;

            var now = DateTime.UtcNow;

            if (auction.EndDate <= now)
                return false;

            var bids = auction.Bids
                .OrderByDescending(b => b.CreatedAt)
                .ThenByDescending(b => b.Id)
                .ToList();

            var lastBid = bids.FirstOrDefault();
            if (lastBid == null)
                return false;

            if (lastBid.UserId != userId)
                return false;

            _context.Bid.Remove(lastBid);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateAsync(int userId, AuctionCreateDto dto)
        {
            var now = DateTime.UtcNow;

           
            if (dto.EndDate <= dto.StartDate || dto.EndDate <= now)
                return false;

            var auction = new Auction
            {
                Title = dto.Title,
                Description = dto.Description,
                StartingPrice = dto.StartingPrice,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedByUserId = userId,
                IsDisabled = false
            };

            await _auctionRepo.AddAsync(auction);
            return await _auctionRepo.SaveChangesAsync();
        }

        public async Task<bool> DisableAuctionAsync(int auctionId)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);
            if (auction == null)
                return false;

            if (auction.IsDisabled)
                return true;

            auction.IsDisabled = true;
            _auctionRepo.Update(auction);
            return await _auctionRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuctionDetailDto>> GetActiveAuctionsAsync(string? searchTerm)
        {
            var all = await _auctionRepo.GetAllAsync(searchTerm);
            var now = DateTime.UtcNow;

            var active = all
                .Where(a => IsOpen(a, now))
                .Select(MapToDetailDtoWithComputedFields)
                .ToList();

            return active;
        }

        public async Task<AuctionDetailDto?> GetByIdAsync(int id, bool includeHistory)
        {
            var auction = await _auctionRepo.GetByIdAsync(id);
            if (auction == null || auction.IsDisabled)
                return null;

            var now = DateTime.UtcNow;
            var isOpen = IsOpen(auction, now);

            
            var orderedBids = auction.Bids
                .OrderByDescending(b => b.Amount)
                .ThenByDescending(b => b.CreatedAt)
                .ToList();

            var dto = MapToDetailDto(auction, orderedBids);

            dto.IsOpen = isOpen;

            if (!isOpen)
            {
               
                var winningBid = orderedBids.FirstOrDefault();
                dto.WinningBid = winningBid != null ? MapToBidDto(winningBid) : null;
                if (!includeHistory)
                {
                    dto.Bids.Clear();
                }
            }
            else
            {
                
                dto.WinningBid = orderedBids.FirstOrDefault() != null
                    ? MapToBidDto(orderedBids.First())
                    : null;
            }

            return dto;
        }

        public async Task<IEnumerable<AuctionDetailDto>> GetExpiredAuctionsAsync()
        {
            var all = await _auctionRepo.GetAllAsync(null);
            var now = DateTime.UtcNow;

            var expired = all
                .Where(a => !a.IsDisabled && a.EndDate <= now)
                .Select(MapToDetailDtoWithComputedFields)
                .ToList();

            return expired;
        }

        public async Task<bool> PlaceBidAsync(int userId, int auctionId, BidCreateDto dto)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);
            if (auction == null || auction.IsDisabled)
                return false;

            var now = DateTime.UtcNow;

            if (!IsOpen(auction, now))
                return false;

            if (auction.CreatedByUserId == userId)
                return false;

            var highestBid = auction.Bids
                .OrderByDescending(b => b.Amount)
                .ThenByDescending(b => b.CreatedAt)
                .FirstOrDefault();

            var minAmount = highestBid?.Amount ?? auction.StartingPrice;

            if (dto.Amount <= minAmount)
                return false;

            var bid = new Bid
            {
                AuctionId = auction.Id,
                UserId = userId,
                Amount = dto.Amount,
                CreatedAt = now
            };

            await _context.Bid.AddAsync(bid);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<AuctionListDto>> SearchAsync(string? searchTerm, bool? isOpen)
        {
            var all = await _auctionRepo.GetAllAsync(searchTerm);
            var now = DateTime.UtcNow;

            if (isOpen == true)
            {
                all = all.Where(a => IsOpen(a, now));
            }
            else if (isOpen == false)
            {
                all = all.Where(a => !a.IsDisabled && a.EndDate <= now);
            }
            else
            {
                all = all.Where(a => !a.IsDisabled);
            }

            var list = all
                .Select(a =>
                {
                    var highestBid = a.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
                    var currentHighest = highestBid?.Amount ?? a.StartingPrice;

                    return new AuctionListDto
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Description = a.Description,
                        StartingPrice = a.StartingPrice,
                        StartDate = a.StartDate,
                        EndDate = a.EndDate,
                        CreatedByUserId = a.CreatedByUserId,
                        CreatedByUserName = a.CreatedByUser?.Name ?? string.Empty,
                        IsOpen = IsOpen(a, now),
                        CurrentHighestBid = currentHighest
                    };
                })
                .ToList();

            return list;
        }

        public async Task<bool> UpdateAsync(int userId, int auctionId, AuctionUpdateDto dto)
        {
            var auction = await _auctionRepo.GetByIdAsync(auctionId);
            if (auction == null || auction.IsDisabled)
                return false;

            if (auction.CreatedByUserId != userId)
                return false;

            var now = DateTime.UtcNow;

            if (auction.EndDate <= now)
                return false;

            auction.Title = dto.Title;
            auction.Description = dto.Description;
            auction.EndDate = dto.EndDate;

            _auctionRepo.Update(auction);
            return await _auctionRepo.SaveChangesAsync();
        }

        private AuctionDetailDto MapToDetailDtoWithComputedFields(Auction a)
        {
            var orderedBids = a.Bids
                .OrderByDescending(b => b.Amount)
                .ThenByDescending(b => b.CreatedAt)
                .ToList();

            return MapToDetailDto(a, orderedBids);
        }

        private AuctionDetailDto MapToDetailDto(Auction a, List<Bid> orderedBids)
        {
            var highestBid = orderedBids.FirstOrDefault();
            var now = DateTime.UtcNow;

            var dto = new AuctionDetailDto
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                StartingPrice = a.StartingPrice,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                CreatedByUserId = a.CreatedByUserId,
                CreatedByUserName = a.CreatedByUser?.Name ?? string.Empty,
                CreatedByUserEmail = a.CreatedByUser?.Email ?? string.Empty,
                IsDisabled = a.IsDisabled,
                IsOpen = IsOpen(a, now),
                CurrentHighestBid = highestBid?.Amount ?? a.StartingPrice,
                Bids = orderedBids.Select(MapToBidDto).ToList()
            };

            if (highestBid != null)
            {
                dto.WinningBid = MapToBidDto(highestBid);
            }

            return dto;
        }

        private BidDto MapToBidDto(Bid b)
        {
            return new BidDto
            {
                Id = b.Id,
                AuctionId = b.AuctionId,
                UserId = b.UserId,
                UserName = b.User?.Name ?? string.Empty,
                Amount = b.Amount,
                CreatedAt = b.CreatedAt
            };
        }
    }
}
