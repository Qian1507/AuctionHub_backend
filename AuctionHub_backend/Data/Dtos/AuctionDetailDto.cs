namespace AuctionHub_backend.Data.Dtos
{
    public class AuctionDetailDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal StartingPrice { get; set; }
       

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsOpen { get; set; }
        public bool IsDisabled { get; set; }

        public decimal CurrentHighestBid { get; set; }
        public BidDto? WinningBid { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public string CreatedByUserEmail { get; set; } =string.Empty;
        public List<BidDto> Bids { get; set; } = new();
        
    }
}
