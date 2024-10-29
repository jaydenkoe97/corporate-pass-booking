using CorporatePassBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CorporatePassBooking.Data
{
    public class ZooDbContext : DbContext
    {
        public ZooDbContext(DbContextOptions<ZooDbContext> options) : base(options) { }

        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Facility)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FacilityId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Visitor)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.VisitorId);
        }
    }
}
