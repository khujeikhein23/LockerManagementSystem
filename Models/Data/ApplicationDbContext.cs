using Microsoft.EntityFrameworkCore;
using LockerManagementSystem.Models;

namespace LockerManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<LockerRequest> LockerRequests { get; set; }
    }
}