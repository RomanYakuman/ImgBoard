using Microsoft.EntityFrameworkCore;

namespace MvcApp.Models
{
    public class AppContext : DbContext
    {
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public AppContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost; user=admin; password=1+H$q3P7QaQ3; database=ImgBoard;", 
                ServerVersion.AutoDetect("server=localhost; user=admin; password=1+H$q3P7QaQ3; database=ImgBoard;"));
        }
    }
}
