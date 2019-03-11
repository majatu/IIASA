using IIASA.Repository.Context;
using IIASA.Services.Context.Factory.Interfaces;
using IIASA.Services.Providers.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IIASA.Services.Context.Factory
{
    public class ExtendedDataContextFactory : IExtendedDataContextFactory
    {
        private IConnectionStringProvider _connectionStringProvider;

        public ExtendedDataContextFactory(IConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;
        }

        public ExtendedDbContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder
                //.UseLazyLoadingProxies()
                .UseNpgsql(_connectionStringProvider.ConnStringDefault);

            return new ExtendedDbContext(optionsBuilder.Options);
        }
    }
}
