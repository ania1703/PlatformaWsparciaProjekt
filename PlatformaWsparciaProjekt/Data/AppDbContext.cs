using Microsoft.EntityFrameworkCore;
using PlatformaWsparciaProjekt.Models;

namespace PlatformaWsparciaProjekt.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<HelpRequest> HelpRequests { get; set; }
        public DbSet<Senior> Seniors { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingItem> ShoppingItems { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Senior)
                .WithMany(s => s.Requests)
                .HasForeignKey(r => r.SeniorId);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Volunteer)
                .WithMany(v => v.AssignedRequests)
                .HasForeignKey(r => r.VolunteerId)
                .IsRequired(false);

            modelBuilder.Entity<HelpRequest>()
                .HasOne(hr => hr.Senior)
                .WithMany()
                .HasForeignKey(hr => hr.SeniorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HelpRequest>()
                .HasOne(hr => hr.Volunteer)
                .WithMany()
                .HasForeignKey(hr => hr.VolunteerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔧 NOWE: konfiguracja Rating, żeby uniknąć konfliktu cascade paths
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Volunteer)
                .WithMany()
                .HasForeignKey(r => r.VolunteerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
                .Ignore(r => r.RatedBy);
        }
    }
}
