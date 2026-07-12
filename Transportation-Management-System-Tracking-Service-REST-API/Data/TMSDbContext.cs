using Microsoft.EntityFrameworkCore;
using Transportation_Management_System_Tracking_Service_REST_API.Models;

namespace Transportation_Management_System_Tracking_Service_REST_API.Data
{
    public class TMSDbContext : DbContext
    {
        public TMSDbContext(DbContextOptions<TMSDbContext> dbContext) : base(dbContext)
        {

        }

        public DbSet<TrackingEvent> TrackingEvents { get; set; } = null!;
    }
}