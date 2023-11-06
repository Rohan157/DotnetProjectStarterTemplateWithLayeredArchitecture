using Microsoft.EntityFrameworkCore;
using Server.Domain.Entities;

namespace Server.Infrastructure
{
    public class ServerDBContext: DbContext
    {
        public ServerDBContext(DbContextOptions<ServerDBContext> options) : base(options) { }
        public DbSet<Hotels> Hotels { get; set; }
        public DbSet<BeCause> BeCause { get; set; }
    }
}
