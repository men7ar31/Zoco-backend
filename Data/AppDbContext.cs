using Microsoft.EntityFrameworkCore;
using ZocoApp.Models;

namespace ZocoApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Study> Studies => Set<Study>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<SessionLog> SessionLogs { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SessionLog>()
                .HasOne(s => s.User)
                .WithMany(u => u.SessionLogs)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Study>()
            .HasOne(s => s.User)
            .WithMany(u => u.Studies)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        }

    }
}