using Microsoft.EntityFrameworkCore;

namespace EventSourceingSqlDb.DbModels
{
    public class VehicleDbContext : DbContext
    {
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options) { }

        public DbSet<PingEventSourcing> PingEventSource { get; set; }
    }
}
