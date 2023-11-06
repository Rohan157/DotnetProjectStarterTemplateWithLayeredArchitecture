using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Infrastructure.Repositories.Interface;

namespace Server.Infrastructure.Repositories
{
    public class BeCauseRepository : RepositoryBase<BeCause>, IBeCauseRepository
    {
        public BeCauseRepository(ServerDBContext dBContext) : base(dBContext) { }
        public async Task<bool> GetCertificateStatusByIdAsync(int id) => await _dbContext.BeCause.Where(x => x.Id == id).Select(x => x.IsCertified).FirstOrDefaultAsync();
        public async Task<IEnumerable<BeCause>> UpdateBeCauseListAsync(IEnumerable<BeCause> entities)
        {
            foreach (var entity in entities)
            {
                var entry = _dbContext.BeCause.First(e => e.Id == entity.Id);
                _dbContext.Entry(entry).CurrentValues.SetValues(entity);
            }
            await _dbContext.SaveChangesAsync();
            return entities;
        }
    }
}
