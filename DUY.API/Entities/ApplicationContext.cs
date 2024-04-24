using DUY.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DUY.API.Entities
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }
        public virtual DbSet<Admin_User> Admin_User { set; get; }
        public virtual DbSet<Admin_Group> Admin_Group { set; get; }
        public virtual DbSet<Admin_Role> Admin_Role { set; get; }
        public virtual DbSet<Admin_Group_User> Admin_Group_User { set; get; }
        public virtual DbSet<Admin_Role_Group> Admin_Role_Group { set; get; }
        public virtual DbSet<ComMent> ComMents { set; get; }
        public virtual DbSet<Customer> Customer { set; get; } 
        public virtual DbSet<File> Files { set; get; }
        public virtual DbSet<Song> Songs { get; set; }
    }
}
