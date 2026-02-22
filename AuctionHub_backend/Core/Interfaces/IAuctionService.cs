using AuctionHub_backend.Data.Dtos;

namespace AuctionHub_backend.Core.Interfaces
{
    public interface IAuctionService
    {
        Task<IEnumerable<AuctionDetailDto>> GetActiveAuctionsAsync(string? searchTerm);
        Task<IEnumerable<AuctionListDto>> SearchAsync(string? searchTerm, bool? isOpen);
        Task<AuctionDetailDto?> GetByIdAsync(int id, bool includeHistory);
        Task<bool> CreateAsync(int userId, AuctionCreateDto dto);
        Task<bool> UpdateAsync(int userId, int auctionId, AuctionUpdateDto dto);
        Task<bool> PlaceBidAsync(int userId, int auctionId, BidCreateDto dto);
        Task<bool> CancelLastBidAsync(int userId, int auctionId);
        Task<bool> DisableAuctionAsync(int auctionId); // admin
        Task<IEnumerable<AuctionDetailDto>> GetExpiredAuctionsAsync();
    }
}
