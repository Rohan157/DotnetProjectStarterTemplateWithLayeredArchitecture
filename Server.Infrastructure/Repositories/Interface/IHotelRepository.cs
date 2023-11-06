using Server.Domain.Entities;

namespace Server.Infrastructure.Repositories.Interface
{
    public interface IHotelRepository : IAsyncRepository<Hotels>
    {
        Task<IEnumerable<Hotels>> UpdateHotelListAsync(IEnumerable<Hotels> entities);
        Task<int> GetBeCauseIdByHotelIdAsync(string hotelId);
    }
}
