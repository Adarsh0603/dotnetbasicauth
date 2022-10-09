using APIPlay.Models;
using Microsoft.EntityFrameworkCore;

namespace APIPlay.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options ):base(options)
        {
        }
        public DbSet<User> Users { get; set; }

    }
}
