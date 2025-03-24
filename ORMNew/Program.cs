using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProductPurchaseApp
{
    public class ProductContext : DbContext
    {
        private const string ConnectionString = "Server=DESKTOP-QRJSNLP\\SQLEXPRESS;Database=ProductDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
    }

    public class Purchase
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public class ProductService
    {
        public void EnsureDatabaseCreated()
        {
            using (var context = new ProductContext())
            {
                context.Database.EnsureCreated();
            }
        }

        public void AddProduct(string productName)
        {
            using (var context = new ProductContext())
            {
                var product = new Product { Name = productName };
                context.Products.Add(product);
                context.SaveChanges();
            }
        }

        public void AddCustomer(string customerName)
        {
            using (var context = new ProductContext())
            {
                var customer = new Customer { Name = customerName };
                context.Customers.Add(customer);
                context.SaveChanges();
            }
        }

        public void PurchaseProduct(int customerId, int productId)
        {
            using (var context = new ProductContext())
            {
                var customer = context.Customers.Find(customerId);
                var product = context.Products.Find(productId);

                if (customer == null || product == null)
                {
                    Console.WriteLine("Invalid customer or product.");
                    return;
                }

                var purchase = new Purchase { CustomerId = customerId, ProductId = productId };
                context.Purchases.Add(purchase);
                context.SaveChanges();
            }
        }

        public void GetProductPurchaseStats()
        {
            using (var context = new ProductContext())
            {
                var stats = context.Products
                    .Select(p => new
                    {
                        ProductName = p.Name,
                        BuyersCount = p.Purchases.Count
                    })
                    .OrderByDescending(p => p.BuyersCount)
                    .ToList();

                foreach (var stat in stats)
                {
                    Console.WriteLine($"Products: {stat.ProductName}, Customers: {stat.BuyersCount}");
                }
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var service = new ProductService();
            service.EnsureDatabaseCreated();

            service.AddProduct("Laptop");
            service.AddProduct("Smartphone");
            service.AddCustomer("Ivan");
            service.AddCustomer("Maria");

            service.PurchaseProduct(1, 1);
            service.PurchaseProduct(2, 1);
            service.PurchaseProduct(2, 2);

            Console.WriteLine("\nData for the sellings:");
            service.GetProductPurchaseStats();
        }
    }
}
