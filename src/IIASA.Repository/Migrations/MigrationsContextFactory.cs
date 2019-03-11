using IIASA.Repository.Context;
using IIASA.Repository.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IIASA.Repository.Migrations
{
    public class MigrationsContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseNpgsql(SettingsExtensions.GetConnectionString("Default"));

            return new DataContext(optionsBuilder.Options);
        }
    }
}
