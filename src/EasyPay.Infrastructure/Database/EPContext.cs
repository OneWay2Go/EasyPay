using EasyPay.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyPay.Infrastructure.Database;

public class EPContext
    : DbContext
{
    EPContext(DbContextOptions<EPContext> options) : base(options){}
    
    DbSet<User> Users { get; set; }
    DbSet<Order> Orders { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Game> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(x => x.UserId);
            
            builder.HasMany(x => x.Products)
                .WithOne(x => x.Seller)
                .HasForeignKey(x => x.SellerId);
            
            builder.HasMany(x => x.Purchases)
                .WithOne(x => x.Buyer)
                .HasForeignKey(x => x.BuyerId);
            
            builder.HasMany(x => x.Sales)
                .WithOne(x => x.Seller)
                .HasForeignKey(x => x.SellerId);
        });

        modelBuilder.Entity<Product>(builder =>
        {
            builder.HasKey(x => x.ProductId);
            
            builder.HasOne(x => x.Game)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.GameId);
        });
    }
}