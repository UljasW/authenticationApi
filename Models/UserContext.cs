using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Models
{
    using Microsoft.EntityFrameworkCore;
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
