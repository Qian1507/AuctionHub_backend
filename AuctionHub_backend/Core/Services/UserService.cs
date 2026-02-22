using AuctionHub_backend.Core.Interfaces;
using AuctionHub_backend.Data.Dtos;
using AuctionHub_backend.Data.Entities;
using AuctionHub_backend.Data.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace AuctionHub_backend.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly ITokenService _tokenService;
        public UserService(IUserRepo userRepo,ITokenService tokenService    )
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepo.GetAllAsync();
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role
            });
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepo.GetByIdAsync(id);
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null || !user.IsActive) return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            var userDto= new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            var token = _tokenService.CreateToken(user);

            return new AuthResponseDto
            {
                User = userDto,
                Token = token,
            };
        }

        public async Task<bool> RegisterAsync(UserRegisterDto dto)
        {
            // check if email already exists
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null) return false;

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "User",
                IsActive = true
            };

            await _userRepo.AddAsync(user);
            return await _userRepo.SaveChangesAsync();
        }

        public async Task<bool> UpdatePasswordAsync(int userId, UpdatePasswordDto dto)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return false;

            
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            return await _userRepo.SaveChangesAsync();
        }

        //Admin
        public async Task<bool> BanUserAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return false;
            if (!user.IsActive) return true;
            user.IsActive = false;
            return await _userRepo.SaveChangesAsync();
        }

    }
}
