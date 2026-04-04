using Microsoft.EntityFrameworkCore;

namespace TgRobuxBot.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Product>().HasData(
            // === 40+80 combo packs (40 to 400) ===
            new Product { Id = 1,  Category = "Robux", Name = "40 Robux",  PriceUZS = 7000 },
            new Product { Id = 2,  Category = "Robux", Name = "80 Robux",  PriceUZS = 14000 },
            new Product { Id = 3,  Category = "Robux", Name = "120 Robux", PriceUZS = 21000 },
            new Product { Id = 4,  Category = "Robux", Name = "160 Robux", PriceUZS = 28000 },
            new Product { Id = 5,  Category = "Robux", Name = "200 Robux", PriceUZS = 35000 },
            new Product { Id = 6,  Category = "Robux", Name = "240 Robux", PriceUZS = 42000 },
            new Product { Id = 7,  Category = "Robux", Name = "280 Robux", PriceUZS = 49000 },
            new Product { Id = 8,  Category = "Robux", Name = "320 Robux", PriceUZS = 56000 },
            new Product { Id = 9,  Category = "Robux", Name = "360 Robux", PriceUZS = 63000 },
            new Product { Id = 10, Category = "Robux", Name = "400 Robux", PriceUZS = 70000 },
            
            // === Special 500 pack ===
            new Product { Id = 11, Category = "Robux", Name = "500 Robux", PriceUZS = 69000 },

            // === 500-base combo packs (500 + 40/80 additions) ===
            new Product { Id = 22, Category = "Robux", Name = "540 Robux", PriceUZS = 76000 },
            new Product { Id = 23, Category = "Robux", Name = "580 Robux", PriceUZS = 83000 },
            new Product { Id = 24, Category = "Robux", Name = "620 Robux", PriceUZS = 90000 },
            new Product { Id = 25, Category = "Robux", Name = "660 Robux", PriceUZS = 97000 },
            new Product { Id = 26, Category = "Robux", Name = "700 Robux", PriceUZS = 104000 },
            new Product { Id = 27, Category = "Robux", Name = "740 Robux", PriceUZS = 111000 },
            new Product { Id = 28, Category = "Robux", Name = "780 Robux", PriceUZS = 118000 },

            // === High-tier Roblox packs ===
            new Product { Id = 12, Category = "Robux", Name = "1000 Robux",        PriceUZS = 135000 },
            new Product { Id = 13, Category = "Robux", Name = "1700+300 Robux",    PriceUZS = 260000 },
            new Product { Id = 14, Category = "Robux", Name = "4500+750 Robux",    PriceUZS = 630000 },
            new Product { Id = 15, Category = "Robux", Name = "10.000+1000 Robux", PriceUZS = 1250000 },
            new Product { Id = 16, Category = "Robux", Name = "22.500+1500 Robux", PriceUZS = 2500000 },

            // === Telegram Premium ===
            new Product { Id = 17, Category = "TelegramPremium", Name = "1 oy (Login orqali)", PriceUZS = 42000 },
            new Product { Id = 18, Category = "TelegramPremium", Name = "3 oy (Gift)",         PriceUZS = 166000 },
            new Product { Id = 19, Category = "TelegramPremium", Name = "6 oy (Gift)",         PriceUZS = 220000 },
            new Product { Id = 20, Category = "TelegramPremium", Name = "12 oy (Gift)",        PriceUZS = 390000 },

            // === Telegram Stars placeholder (price calculated dynamically at 240/star) ===
            new Product { Id = 21, Category = "TelegramStars", Name = "Telegram Stars (Maxsus)", PriceUZS = 0 }
        );
    }
}
