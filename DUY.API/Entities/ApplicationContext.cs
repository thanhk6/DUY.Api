using DUY.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DUY.API.Entities
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
        public virtual DbSet<ComMent> ComMents { set; get; }
        public virtual DbSet<Customer> Customer { set; get; } 
        public virtual DbSet<File> Files { set; get; }
        public virtual DbSet<Song> Songs { get; set; }
    }
}
