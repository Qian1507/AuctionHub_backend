using AuctionHub_backend.Data.Entities;

namespace AuctionHub_backend.Data.Interfaces
{
    public interface IUserRepo
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task<User?> GetByIdAsync(int id);

        Task<IEnumerable<User>> GetAllAsync();

        Task<bool> SaveChangesAsync();
    }
}
