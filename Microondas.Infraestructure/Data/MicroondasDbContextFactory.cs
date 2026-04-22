using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Microondas.Infrastructure.Data;

public class MicroondasDbContextFactory : IDesignTimeDbContextFactory<MicroondasDbContext>
{
    public MicroondasDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MicroondasDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=MicroondasDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new MicroondasDbContext(optionsBuilder.Options);
    }
}
