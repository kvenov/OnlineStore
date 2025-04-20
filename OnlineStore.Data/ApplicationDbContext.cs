using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using System.Reflection;

namespace OnlineStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

		public ApplicationDbContext()
		{
		}

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

		public virtual DbSet<Article> Articles { get; set; } = null!;
		public virtual DbSet<ArticleCategory> ArticleCategories { get; set; } = null!;
		public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
		public virtual DbSet<ShoppingCartItem> ShoppingCartsItems { get; set; } = null!;
		public virtual DbSet<Order> Orders { get; set; } = null!;
		public virtual DbSet<OrderItem> OrdersItems { get; set; } = null!;
		public virtual DbSet<Product> Products { get; set; } = null!;
		public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
		public virtual DbSet<Brand> Brands { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
