using Server.Domain.Entities;

namespace Server.Infrastructure.Repositories.Interface
{
    public interface IBeCauseRepository : IAsyncRepository<BeCause>
    {
        Task<bool> GetCertificateStatusByIdAsync(int id);
        Task<IEnumerable<BeCause>> UpdateBeCauseListAsync(IEnumerable<BeCause> entities);
    }
}
