using System.Threading.Tasks;
using IIASA.Repository.Context;
using IIASA.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IIASA.Services.Context
{
    public class ExtendedDbContext : DataContext
    {
        public ExtendedDbContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        public async Task<int> AddImage(Image image)
        {
            var entityEntry = await Images.AddAsync(image);

            await SaveChangesAsync();

            return entityEntry.Entity.Id;
        }

        public async Task<Image> GetImageById(int imageId)
        {
            return await Images.Where(c => c.Id == imageId).FirstOrDefaultAsync();
        }

        public async Task UpdateImage(Image image)
        {
            Entry(image).State = EntityState.Modified;

            await SaveChangesAsync();
        }
    }
}
