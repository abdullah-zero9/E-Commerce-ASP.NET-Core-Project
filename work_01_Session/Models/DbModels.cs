using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace work_01_Session.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public int Role { get; set; }
        public bool IsActive { get; set; }
    }
    public class Product
    {
        public Product()
        {
            this.SalesItems=new List<SalesItem>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public string Image { get; set; }
        //nev
        public virtual ICollection<SalesItem> SalesItems { get; set; }

    }
    public class Customer
    {
        public Customer()
        {
            this.SalesOrders=new List<SalesOrder>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        //nev
        public virtual ICollection<SalesOrder> SalesOrders { get; set; }

    }
    public class SalesOrder
    {
        public SalesOrder()
        {
            this.SalesItem = new List<SalesItem>();
        }
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        //fk
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ICollection<SalesItem> SalesItem { get; set; }
    }
    public class SalesItem
    {
        public int Id { get; set; }
        //fk
        [ForeignKey("SalesOrder")]
        public int SalesOrderId { get; set; }
        public virtual SalesOrder SalesOrder { get; set; }

        public double UnitPrice { get; set; }
        public double Quantity { get; set; }

        //fk
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
    public class ECommerceDbContext : DbContext
    {
        public ECommerceDbContext()
        {
        }

        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options):base(options)
        {

        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesItem> SalesItems { get; set; }
    }

}
