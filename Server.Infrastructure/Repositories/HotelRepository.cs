using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;
using Server.Infrastructure.Repositories.Interface;

namespace Server.Infrastructure.Repositories
{
    public class HotelRepository : RepositoryBase<Hotels>, IHotelRepository 
    {
        public HotelRepository(ServerDBContext dBContext) : base(dBContext) { }
        public async Task<int> GetBeCauseIdByHotelIdAsync(string hotelId) => await _dbContext.Hotels.Where(x => x.Id == hotelId).Select(x => x.BeCauseId).FirstOrDefaultAsync() ?? 0;
        public async Task<IEnumerable<Hotels>> UpdateHotelListAsync(IEnumerable<Hotels> entities)
        {
            foreach (var entity in entities)
            {
                var entry = _dbContext.Hotels.First(e => e.Id == entity.Id);
                _dbContext.Entry(entry).CurrentValues.SetValues(entity);
            }
            await _dbContext.SaveChangesAsync();
            return entities;
        }
    }
}
