using AuctionHub_backend.Data.Dtos;
using AuctionHub_backend.Data.Entities;
using AuctionHub_backend.Data.Interfaces;
using AuctionHub_backend.Data.Repos;
using Microsoft.AspNetCore.Mvc;

namespace AuctionHub_backend.Core.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(UserRegisterDto dto);
        Task<AuthResponseDto?>LoginAsync(UserLoginDto dto);
        Task<bool> UpdatePasswordAsync(int userId, UpdatePasswordDto dto);
        Task<User?> GetByIdAsync(int id);

        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<bool> BanUserAsync(int id);
    }
}
