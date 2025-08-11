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
        public DbSet<SessionLog> SessionLogs => Set<SessionLog>();
    }
}
