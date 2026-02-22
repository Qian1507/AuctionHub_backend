namespace AuctionHub_backend.Data.Dtos
{
    public class AuthResponseDto
    {
        public UserResponseDto User { get; set; } = null!;
        public string Token { get; set; }=string.Empty;

    }
}
