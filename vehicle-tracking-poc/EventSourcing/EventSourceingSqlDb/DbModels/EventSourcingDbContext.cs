using Microsoft.EntityFrameworkCore;

namespace EventSourceingSQLDB.DbModels
{
    public class EventSourcingDbContext : DbContext
    {
        public EventSourcingDbContext(DbContextOptions<EventSourcingDbContext> options) : base(options) { }

        public DbSet<EventSourcing> EventSourcing { get; set; }
    }
}
