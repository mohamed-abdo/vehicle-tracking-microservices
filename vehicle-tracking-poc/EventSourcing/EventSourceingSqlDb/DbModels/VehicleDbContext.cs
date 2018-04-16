using Microsoft.EntityFrameworkCore;

namespace EventSourceingSQLDB.DbModels
{
    public class VehicleDbContext : DbContext
    {
        public VehicleDbContext(DbContextOptions<VehicleDbContext> options) : base(options) { }

        public DbSet<EventSourcing> EventSourcing { get; set; }
    }
}
