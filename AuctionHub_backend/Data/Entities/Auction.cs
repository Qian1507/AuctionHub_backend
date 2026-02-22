using System.ComponentModel.DataAnnotations;

namespace AuctionHub_backend.Data.Entities
{
    public class Auction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }= string.Empty;
        [Required]
        public decimal StartingPrice { get; set; }
        [Required]
        public DateTime StartDate { get; set; }= DateTime.UtcNow;
        [Required]
        public DateTime EndDate { get; set; }

        //FK to User
        public int CreatedByUserId { get; set; }

        //For Admin: if true, action is hidden/disabled
        public bool IsDisabled { get; set; }= false;

        public bool IsOpen => 
            DateTime.UtcNow >= StartDate && 
            DateTime.UtcNow <= EndDate && 
            !IsDisabled;

        //Navigation Properties

        public User CreatedByUser { get; set; } = null!;
        public List<Bid> Bids { get; set; } = new();
    }
}
