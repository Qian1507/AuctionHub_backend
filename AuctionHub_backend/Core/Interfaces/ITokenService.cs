using AuctionHub_backend.Data.Entities;

namespace AuctionHub_backend.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
