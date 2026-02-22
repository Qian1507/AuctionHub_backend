using System.Reflection;

namespace AuctionHub_backend.Data.Dtos
{
    public class AuctionUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal StartingPrice {  get; set; }
        public DateTime EndDate { get; set; }
    }
}
