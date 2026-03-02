using AuctionHub_backend.Data.Entities;

namespace AuctionHub_backend.Data.Interfaces
{
    public interface IAuctionRepo
    {
        
        Task<Auction?> GetByIdAsync(int id);
        Task<IEnumerable<Auction>> GetAllAsync(string? searchTerm = null);

        
        Task<IEnumerable<Auction>> GetByUserIdAsync(int userId);

        Task AddAsync(Auction auction);

        void Update(Auction auction);

        void Delete(Auction auction);

        
        Task<bool> HasBidsAsync(int auctionId);
        Task AddBidAsync(Bid bid);

        Task<bool> SaveChangesAsync();

    }
}
