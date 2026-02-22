using AuctionHub_backend.Data.Entities;

namespace AuctionHub_backend.Data.Interfaces
{
    public interface IAuctionRepo
    {
        
        Task<Auction?> GetByIdAsync(int id);
        Task<IEnumerable<Auction>> GetAllAsync(string? searchTerm = null);

        // Get all auctions created by a specific user
        Task<IEnumerable<Auction>> GetByUserIdAsync(int userId);

        Task AddAsync(Auction auction);

        void Update(Auction auction);

        void Delete(Auction auction);

        // Check if an auction has any bids (Critical for VG logic: preventing edits if bids exist)
        Task<bool> HasBidsAsync(int auctionId);
        Task AddBidAsync(Bid bid);

        Task<bool> SaveChangesAsync();

    }
}
