using Server.Domain.Entities;

namespace Server.Infrastructure.Repositories.Interface
{
    public interface IAsyncRepository<T> where T : EntityBase
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    }
}
