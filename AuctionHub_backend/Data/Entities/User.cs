using System.ComponentModel.DataAnnotations;

namespace AuctionHub_backend.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }=string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; }= string.Empty;
        [MaxLength(20)]
        public string Role {  get; set; } = "User"; //Admin or User
        public bool IsActive { get; set; } =true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public List<Auction> Auctions { get; set; } = new();

        public List<Bid> Bids { get; set; }= new();

    }
}
