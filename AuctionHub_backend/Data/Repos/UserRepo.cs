using AuctionHub_backend.Data.Entities;
using AuctionHub_backend.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace AuctionHub_backend.Data.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly AuctionDbContext _context;

        public UserRepo(AuctionDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.User.AddAsync(user);
        }     

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.User.ToListAsync();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _context.User.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.User.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
