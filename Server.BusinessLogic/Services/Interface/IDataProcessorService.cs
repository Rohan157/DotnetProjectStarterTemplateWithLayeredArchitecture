using Microsoft.AspNetCore.Http;

namespace Server.BusinessLogic.Services.Interface
{
    public interface IDataProcessorService
    {
        Task<bool> SaveFileAsync(IFormFile file);
        Task<bool> GetCertificationStatusAsync(string hotelId);
    }
}
