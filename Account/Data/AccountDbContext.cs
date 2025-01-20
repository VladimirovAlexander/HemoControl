using Account.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Account.Data
{
    public class AccountDbContext : IdentityDbContext<User>
    {
        DbSet<User> Users { get; set; }


        public AccountDbContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}