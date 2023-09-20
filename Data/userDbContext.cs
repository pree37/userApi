using Microsoft.EntityFrameworkCore;
using usersDemo.Models;

namespace usersDemo.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        // Add other DbSet properties for additional tables if needed
    }
}
