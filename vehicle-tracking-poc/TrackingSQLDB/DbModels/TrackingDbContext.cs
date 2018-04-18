using Microsoft.EntityFrameworkCore;

namespace TrackingSQLDB.DbModels
{
    public class TrackingDbContext:DbContext
    {
        public TrackingDbContext(DbContextOptions<TrackingDbContext> options) : base(options) { }

        public DbSet<Tracking> Tracking { get; set; }

    }
}
