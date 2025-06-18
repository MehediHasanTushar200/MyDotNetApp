using hamko.Models;

using Microsoft.EntityFrameworkCore;

namespace hamko.Service
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Branch> Branches { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }
       


        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Battery> Batteries { get; set; }

        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<StockIn> StockIns { get; set; }

        public DbSet<Sales> Sales { get; set; }
        public DbSet<StockOut> StockOuts { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<Supplier> Supplier { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Product -> Brand relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product -> Category relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }


    }
}
