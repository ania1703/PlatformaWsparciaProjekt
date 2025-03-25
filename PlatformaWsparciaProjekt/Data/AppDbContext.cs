using Microsoft.EntityFrameworkCore;
using PlatformaWsparciaProjekt.Models;

namespace PlatformaWsparciaProjekt.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Senior> Seniors { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Request> Requests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Senior)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.SeniorId);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Volunteer)
                .WithMany(v => v.AssignedRequests)
                .HasForeignKey(r => r.VolunteerId)
                .IsRequired(false);
        }
    }
}
