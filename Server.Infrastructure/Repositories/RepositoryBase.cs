using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Infrastructure.Repositories.Interface;

namespace Server.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : EntityBase
    {
        protected readonly ServerDBContext _dbContext;
        public RepositoryBase(ServerDBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }
        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }        
    }
}
