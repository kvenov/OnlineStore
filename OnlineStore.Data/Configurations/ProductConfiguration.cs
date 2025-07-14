using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
using static OnlineStore.Data.Common.Constants.EntityConstants.Product;

namespace OnlineStore.Data.Configurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> entity)
		{

			entity
				.HasKey(p => p.Id);

			entity
				.Property(p => p.Name)
				.IsRequired()
				.HasMaxLength(ProductNameMaxLength);

			entity
				.Property(p => p.Description)
				.IsRequired()
				.HasMaxLength(ProductDescriptionMaxLength);

			entity
				.Property(p => p.Price)
				.HasColumnType(ProductPriceType);

			entity
				.Property(p => p.DiscountPrice)
				.HasColumnType(ProductDiscountPriceType);

			entity
				.Property(p => p.StockQuantity)
				.IsRequired();

			entity
				.Property(p => p.AverageRating)
				.HasPrecision(3, 2);

			entity
				.Property(p => p.TotalRatings)
				.IsRequired();

			entity
				.Property(p => p.IsActive)
				.IsRequired();

			entity
				.Property(p => p.CreatedAt)
				.IsRequired()
				.HasDefaultValueSql("GETUTCDATE()");

			entity
				.Property(p => p.ImageUrl)
				.IsRequired()
				.HasMaxLength(ProductImageUrlMaxLength);

			entity
				.Property(p => p.CategoryId)
				.IsRequired(true);

			entity
				.Property(p => p.BrandId)
				.IsRequired(false);

			entity
				.HasOne(p => p.ProductDetails)
				.WithOne(pd => pd.Product)
				.HasForeignKey<ProductDetails>(pd => pd.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			entity
				.HasOne(p => p.Category)
				.WithMany(c => c.Products)
				.HasForeignKey(p => p.CategoryId)
				.OnDelete(DeleteBehavior.Restrict);

			entity
				.HasOne(p => p.Brand)
				.WithMany(b => b.Products)
				.HasForeignKey(p => p.BrandId)
				.OnDelete(DeleteBehavior.SetNull);

			entity
				.HasIndex(p => p.Name);
			entity
				.HasIndex(p => p.IsActive);
			entity
				.HasIndex(p => p.CreatedAt);
			entity
				.HasIndex(p => new { p.Price, p.DiscountPrice });
			entity
				.HasIndex(p => p.AverageRating);

			entity
				.HasQueryFilter(p => p.IsDeleted == false &&
									 (p.Brand == null || !p.Brand.IsDeleted) &&
									 (p.Category.IsDeleted == false));

			entity
				.HasData(this.GenerateProducts());

		}

		private IEnumerable<Product> GenerateProducts()
		{
			return new List<Product>
			{
				new Product
				{
					Id = 12,
					Name = "Nike Revolution 6",
					Description = "Men's Road Running Shoes",
					Price = 75.00m,
					DiscountPrice = null,
					StockQuantity = 100,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/09f3dd58-91e6-4d94-a5ea-1b2c8f7e9ff0/revolution-6-road-running-shoes-8Xf5w3.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 12,
					BrandId = 1
				},
				new Product
				{
					Id = 13,
					Name = "Adidas Ultraboost Light",
					Description = "Running shoes made for energy return.",
					Price = 190.00m,
					DiscountPrice = 170.00m,
					StockQuantity = 80,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/f228f1f8750b4a10929faf6f011c0b99_9366/Ultraboost_Light_Shoes_Black_GY9356_01_standard.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 12,
					BrandId = 2
				},
				new Product
				{
					Id = 14,
					Name = "Levi's 512 Slim Taper Fit",
					Description = "Men's Jeans with a modern slim cut.",
					Price = 89.95m,
					DiscountPrice = 69.99m,
					StockQuantity = 120,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://lsco.scene7.com/is/image/lsco/levis/clothing/005120939-front-pdp.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 3,
					BrandId = 3
				},
				new Product
				{
					Id = 15,
					Name = "Nike Club Fleece Joggers",
					Description = "Comfortable fleece joggers for everyday wear.",
					Price = 55.00m,
					DiscountPrice = null,
					StockQuantity = 95,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/9bdce8c1-1c62-40fd-84d0-d0d658d3e883/sportswear-club-fleece-joggers-QDkzGC.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 6,
					BrandId = 1
				},
				new Product
				{
					Id = 16,
					Name = "Nike Dri-FIT Training T-Shirt",
					Description = "Men's moisture-wicking training tee.",
					Price = 30.00m,
					DiscountPrice = 25.00m,
					StockQuantity = 150,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/590d39a5-36ad-434c-b073-15376b493f3a/dri-fit-training-t-shirt-Fz4gD3.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 2,
					BrandId = 1
				},
				new Product
				{
					Id = 17,
					Name = "Adidas Essentials Tee",
					Description = "Classic Adidas t-shirt for daily wear.",
					Price = 25.00m,
					DiscountPrice = null,
					StockQuantity = 90,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/10d12fc1e6e24c73bc03ad2d00cd6e4f_9366/Essentials_Logo_Tee_White_IC9319_01_laydown.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 2,
					BrandId = 2
				},
				new Product
				{
					Id = 18,
					Name = "Nike Everyday Plus Cushioned Socks",
					Description = "Comfortable crew socks for all-day wear.",
					Price = 20.00m,
					DiscountPrice = 15.00m,
					StockQuantity = 200,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/t_PDP_1280_v1/f_auto,q_auto:eco/a4f39e13-4e96-4e8d-bc71-3476bb8ac719/everyday-plus-cushioned-training-crew-socks-JmgKcT.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 8,
					BrandId = 1
				},
				new Product
				{
					Id = 19,
					Name = "Adidas Classic Backpack",
					Description = "Spacious backpack with modern design.",
					Price = 45.00m,
					DiscountPrice = 39.99m,
					StockQuantity = 80,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/39aa23ceae624ddcb6eeae8601556df5_9366/Classic_Backpack_Black_FM6876_01_standard.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 4,
					BrandId = 2
				},
				new Product
				{
					Id = 20,
					Name = "Nike Grey Hoodie",
					Description = "Nike Men’s Essential Grey Hoodie",
					Price = 74.99m,
					DiscountPrice = null,
					StockQuantity = 70,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/c_limit,w_592,f_auto/t_product_v1/fb26574f-572f-463a-a63d-df361188ed62/sportswear-club-fleece-mens-graphic-pullover-hoodie-1WcMnq.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 6,
					BrandId = 1
				},
				new Product
				{
					Id = 21,
					Name = "Levi's Black Graphic Tee",
					Description = "Levi's Men's Relaxed Fit Graphic T-Shirt",
					Price = 29.99m,
					DiscountPrice = 24.99m,
					StockQuantity = 120,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://lsco.scene7.com/is/image/lsco/161430124-front-pdp.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 2,
					BrandId = 3
				},
				new Product
				{
					Id = 22,
					Name = "Nike Navy Shorts",
					Description = "Nike Men's Navy Training Shorts",
					Price = 39.99m,
					DiscountPrice = 34.99m,
					StockQuantity = 90,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/c_limit,w_592,f_auto/t_product_v1/62231792-14aa-47c4-b5e4-ef59ec69a4ff/dri-fit-mens-training-shorts-kFv4LN.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 7,
					BrandId = 1
				},
				new Product
				{
					Id = 23,
					Name = "Adidas Grey Sweatshirt",
					Description = "Adidas Essentials Grey Sweatshirt",
					Price = 54.99m,
					DiscountPrice = null,
					StockQuantity = 85,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://assets.adidas.com/images/h_840,f_auto,q_auto,fl_lossy,c_fill,g_auto/6b0dfdfd66e24667be28af6400c33688_9366/Essentials_Logo_Fleece_Sweatshirt_Grey_IC6779_01_laydown.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 6,
					BrandId = 2
				},
				new Product
				{
					Id = 24,
					Name = "Nike Blue Ankle Socks",
					Description = "Nike Everyday Plus Cushioned Ankle Socks",
					Price = 14.99m,
					DiscountPrice = null,
					StockQuantity = 300,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/t_default/e30dcf40-0325-43aa-bfae-3e8a4cf2cb92/everyday-plus-cushioned-training-ankle-socks-M9CcJm.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 8,
					BrandId = 1
				},
				new Product
				{
					Id = 25,
					Name = "Levi's Blue Denim Jacket",
					Description = "Levi's Original Trucker Denim Jacket",
					Price = 89.99m,
					DiscountPrice = 79.99m,
					StockQuantity = 45,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://lsco.scene7.com/is/image/lsco/723340178-front-pdp.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 5,
					BrandId = 3
				},
				new Product
				{
					Id = 26,
					Name = "Adidas Sporty White Shoes",
					Description = "Adidas Ultraboost Light White Running Shoes",
					Price = 189.99m,
					DiscountPrice = 159.99m,
					StockQuantity = 60,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://assets.adidas.com/images/w_600,f_auto,q_auto/bdf5b9ecbb54425aab59af6d0113e5c7_9366/Ultraboost_Light_Shoes_White_GW6723_01_standard.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 12,
					BrandId = 2
				},
				new Product
				{
					Id = 27,
					Name = "Nike Red Sports Backpack",
					Description = "Nike Brasilia Small Training Backpack",
					Price = 39.99m,
					DiscountPrice = null,
					StockQuantity = 130,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/t_default/e126b819-b305-40a0-aeb4-36e8c9ee5ce2/brasilia-small-training-backpack-18l-LZgMnm.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 4,
					BrandId = 1
				},
				new Product
				{
					Id = 28,
					Name = "Nike Lifestyle Black Shoes",
					Description = "Nike Air Force 1 '07 Men's Shoes",
					Price = 114.99m,
					DiscountPrice = null,
					StockQuantity = 95,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/t_default/1bbebd34-3fe2-4dc7-9b98-80f9932f5a75/air-force-1-07-mens-shoes-WrLlWX.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 13,
					BrandId = 1
				},
				new Product
				{
					Id = 29,
					Name = "Levi's Slim Fit Jeans",
					Description = "Levi's 511 Slim Fit Men's Jeans",
					Price = 59.99m,
					DiscountPrice = 49.99m,
					StockQuantity = 100,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://lsco.scene7.com/is/image/lsco/045114406-front-pdp.jpg",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 3,
					BrandId = 3
				},
				new Product
				{
					Id = 30,
					Name = "Nike Dri-FIT T-Shirt",
					Description = "Nike Dri-FIT Men’s Training T-Shirt",
					Price = 34.99m,
					DiscountPrice = null,
					StockQuantity = 115,
					IsActive = true,
					CreatedAt = DateTime.UtcNow,
					ImageUrl = "https://static.nike.com/a/images/c_limit,w_592,f_auto/t_product_v1/1797b3f1-9e7a-42cb-8d92-1c9e7cf9a93c/dri-fit-mens-training-t-shirt-Q7zT4k.png",
					AverageRating = 0,
					TotalRatings = 0,
					CategoryId = 2,
					BrandId = 1
				}
			};
		}
	}
}
