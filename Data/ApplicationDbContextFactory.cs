using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApplication5.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Railway PostgreSQL bağlantı cümlesi
            var connectionString = "Host=tramway.proxy.rlwy.net;Port=33417;Database=railway;Username=postgres;Password=YoSFHeQgjLeeRErnIKzYojSZhPtHXLWv;SSL Mode=Require;Trust Server Certificate=true";

            optionsBuilder.UseNpgsql(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
