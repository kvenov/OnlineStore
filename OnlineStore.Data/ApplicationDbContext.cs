using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Interfaces;
using System.Linq.Expressions;
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
		public virtual DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
		public virtual DbSet<PaymentDetails> PaymentDetails { get; set; } = null!;
		public virtual DbSet<ProductDetails> ProductDetails { get; set; } = null!;
		public virtual DbSet<Wishlist> Wishlists { get; set; } = null!;
		public virtual DbSet<WishlistItem> WishlistsItems { get; set; } = null!;
		public virtual DbSet<ProductRating> ProductsRatings { get; set; } = null!;
		public virtual DbSet<Address> Addresses { get; set; } = null!;
		public virtual DbSet<RecentlyViewedProduct> RecentlyViewedProducts { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			foreach (var entityType in builder.Model.GetEntityTypes())
			{
				if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
				{
					var parameter = Expression
							.Parameter(entityType.ClrType, "e");
					var isDeletedProperty = Expression
							.Property(parameter, nameof(ISoftDeletable.IsDeleted));

					var filter = Expression.Lambda(
						Expression
							.Equal(isDeletedProperty, Expression.Constant(false)),
								   parameter
					);

					builder
							.Entity(entityType.ClrType).HasQueryFilter(filter);
				}
			}


			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
