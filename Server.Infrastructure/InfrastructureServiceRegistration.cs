using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Infrastructure.Repositories;
using Server.Infrastructure.Repositories.Interface;

namespace Server.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ServerDBContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("Server.Infrastructure")));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IHotelRepository, HotelRepository>();
            services.AddScoped<IBeCauseRepository, BeCauseRepository>();
            return services;
        }
    }
}
