using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EasyPay.Infrastructure.Database;

public class EPContextFactory : IDesignTimeDbContextFactory<EPContext>
{
    public EPContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EPContext>();
        optionsBuilder.UseNpgsql("Server=localhost;Database=easypay_db;User Id=postgres;Password=postgres;");
        return new EPContext(optionsBuilder.Options);
    }
}