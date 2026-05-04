using Microsoft.EntityFrameworkCore;
using ShoppingProject.Models;

namespace ShoppingProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}

// Four steps to add a table
// 1. Create a Model Class
// 2. Add DB Set
// 3. add-migration AddDiaryEntryTable
// 4. update-database