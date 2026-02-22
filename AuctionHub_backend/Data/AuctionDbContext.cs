using AuctionHub_backend.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionHub_backend.Data
{
    public class AuctionDbContext : DbContext
    {
       
            public AuctionDbContext(DbContextOptions options)
                : base(options)
            {
            }

            public virtual DbSet<User> User => Set<User>();
            public virtual DbSet<Auction> Auction => Set<Auction>();
            public virtual DbSet<Bid> Bid => Set<Bid>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // User
                modelBuilder.Entity<User>()
                    .ToTable("User");

                modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email)
                    .IsUnique();

                modelBuilder.Entity<User>()
                    .Property(u => u.Role)
                    .HasMaxLength(20);

                // Auction
                modelBuilder.Entity<Auction>()
                    .ToTable("Auction");

                modelBuilder.Entity<Auction>()
                    .Property(a => a.StartingPrice)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<Auction>()
                    .HasOne(a => a.CreatedByUser)
                    .WithMany(u => u.Auctions)
                    .HasForeignKey(a => a.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Bid
                modelBuilder.Entity<Bid>()
                    .ToTable("Bid");

                modelBuilder.Entity<Bid>()
                    .Property(b => b.Amount)
                    .HasPrecision(18, 2);

                modelBuilder.Entity<Bid>()
                    .HasOne(b => b.Auction)
                    .WithMany(a => a.Bids)
                    .HasForeignKey(b => b.AuctionId)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Bid>()
                    .HasOne(b => b.User)
                    .WithMany(u => u.Bids)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ignore computed property
                modelBuilder.Entity<Auction>()
                    .Ignore(a => a.IsOpen);

                // Seed an admin user 
                modelBuilder.Entity<User>().HasData(new User
                {
                    Id = 1,
                    Name = "Admin",
                    Email = "admin@auktionhub.com",
                    PasswordHash = "admin123",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            }
        }
    
}
