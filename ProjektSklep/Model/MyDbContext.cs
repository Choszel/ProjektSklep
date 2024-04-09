using System.Configuration.Provider;
using System.Data.Entity;

namespace ProjektSklep.Model
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\savan\\Desktop\\STUDIA\\SEMESTR 6\\WPF\\ProjektSklep\\ProjektSklep\\StoreDB.mdf;Integrated Security=True") { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductOrder> ProductOrders{ get; set; }
    }
}
