using AuctionHub_backend.Data.Entities;
using AuctionHub_backend.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuctionHub_backend.Data.Repos
{
    public class AuctionRepo:IAuctionRepo
    {
        private readonly AuctionDbContext _context;

        public AuctionRepo(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Auction auction)
        {
            await _context.Auction.AddAsync(auction);
        }

        public void Update(Auction auction)
        {
            _context.Auction.Update(auction);
        }

        public void Delete(Auction auction)
        {
            _context.Auction.Remove(auction);
        }

        public async Task<IEnumerable<Auction>> GetAllAsync(string? searchTerm = null)
        {
            var query = _context.Auction
                .Include(a => a.CreatedByUser)
                .AsQueryable();

            
            query = query.Where(a => !a.IsDisabled);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
               
                query = query.Where(a => a.Title.Contains(searchTerm));
            }

            return await query.ToListAsync();
        }

        public async Task<Auction?> GetByIdAsync(int id)
        {
            return await _context.Auction
                .Include(a => a.CreatedByUser)
                .Include(a => a.Bids.OrderByDescending(b => b.Amount)) 
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Auction>> GetByUserIdAsync(int userId)
        {
            return await _context.Auction
                .Where(a => a.CreatedByUserId == userId)
                .ToListAsync();
        }

        public async Task<bool> HasBidsAsync(int auctionId)
        {
            return await _context.Bid.AnyAsync(b => b.AuctionId == auctionId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

       
    }
}
