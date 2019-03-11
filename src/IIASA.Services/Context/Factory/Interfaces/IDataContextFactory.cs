using IIASA.Repository.Context;

namespace IIASA.Services.Context.Factory.Interfaces
{
    public interface IExtendedDataContextFactory
    {
        ExtendedDbContext CreateContext();
    }
}
