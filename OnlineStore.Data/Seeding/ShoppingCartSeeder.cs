using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.DTOs;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using OnlineStore.Data.Utilities.Interfaces;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;
using static OnlineStore.Data.Utilities.EntityValidator;

namespace OnlineStore.Data.Seeding
{
	public class ShoppingCartSeeder : BaseSeeder<ShoppingCartSeeder>, IEntitySeeder, IXmlSeeder
	{
		private readonly ApplicationDbContext _context;
		private readonly IXmlHelper _xmlHelper;

		public ShoppingCartSeeder(ILogger<ShoppingCartSeeder> logger, ApplicationDbContext context, IXmlHelper xmlHelper) : 
					base(logger)
		{
			this._context = context ?? throw new ArgumentNullException(nameof(context));
			this._xmlHelper = xmlHelper ?? throw new ArgumentNullException(nameof(xmlHelper));
		}

		public override string FilePath => 
							Path.Combine(AppContext.BaseDirectory, "Files", "shoppingCarts.xml");

		public string RootName => "ShoppingCarts";

		public IXmlHelper XmlHelper => this._xmlHelper;

		public async Task SeedEntityData()
		{
			await this.ImportShoppingCartsFromXml();
		}

		private async Task ImportShoppingCartsFromXml()
		{
			string shoppingCartsXml = await File.ReadAllTextAsync(this.FilePath);


			try
			{
				ImportShoppingCartDTO[]? shoppingCartDTOs =
				this.XmlHelper.Deserialize<ImportShoppingCartDTO[]>(shoppingCartsXml, this.RootName);

				if (shoppingCartDTOs != null && shoppingCartDTOs.Length > 0)
				{
					ICollection<ShoppingCart> validShoppingCarts = new List<ShoppingCart>();

					HashSet<string> validUsersIds = (await this._context
							.Users
							.AsNoTracking()
							.Select(u => u.Id)
							.ToListAsync()).ToHashSet();

					HashSet<string> existingShoppingCartsUserIds = (await this._context
							.ShoppingCarts
							.AsNoTracking()
							.Select(sc => sc.UserId)
							.ToListAsync()).ToHashSet();

					Dictionary<int, Product> productDictionary = await _context
							.Products
							.Include(p => p.ProductDetails)
							.AsNoTracking()
							.ToDictionaryAsync(p => p.Id);



					this.Logger.LogInformation($"Found {shoppingCartDTOs.Length} ShoppingCart DTO's to process.");

					foreach (ImportShoppingCartDTO shoppingCartDto in shoppingCartDTOs)
					{

						if (!IsValid(shoppingCartDto))
						{
							string warningMessage = this.BuildEntityValidatorWarningMessage(nameof(ShoppingCart));
							this.Logger.LogWarning(warningMessage);
							continue;
						}

						bool isCreatedAtValid = DateTime.TryParse(shoppingCartDto.CreatedAt, out DateTime createdAt);

						if (!isCreatedAtValid)
						{
							this.Logger.LogWarning(EntityDataParseError);
							continue;
						}

						if (!validUsersIds.Contains(shoppingCartDto.UserId))
						{
							this.Logger.LogWarning(ReferencedEntityMissing);
							continue;
						}

						ShoppingCart? shoppingCart = null;

						if (existingShoppingCartsUserIds.Contains(shoppingCartDto.UserId))
						{
							ShoppingCart existingCart = await this._context
								.ShoppingCarts
								.Include(sc => sc.ShoppingCartItems)
								.FirstAsync(sc => sc.UserId == shoppingCartDto.UserId);

							shoppingCart = existingCart;
						}

						if (shoppingCart == null)
						{
							shoppingCart = new ShoppingCart
							{
								CreatedAt = createdAt,
								UserId = shoppingCartDto.UserId
							};
						}

						//Better way to handle existing shopping carts
						/*
						var shoppingCart = existingShoppingCartsUserIds.Contains(shoppingCartDto.UserId)
							? await this._context
								.ShoppingCarts
								.Include(sc => sc.ShoppingCartItems)
								.FirstAsync(sc => sc.UserId == shoppingCartDto.UserId)
							: new ShoppingCart
							{
								CreatedAt = createdAt,
								UserId = shoppingCartDto.UserId
							};
						*/

						foreach (ImportShoppingCartItemDTO shoppingCartItemDto in shoppingCartDto.ShoppingCartItems)
						{

							if (!IsValid(shoppingCartItemDto))
							{
								string warningMessage = this.BuildEntityValidatorWarningMessage(nameof(ShoppingCartItem));
								this.Logger.LogWarning(warningMessage);
								continue;
							}

							bool isQuantityValid = int.TryParse(shoppingCartItemDto.Quantity, out int quantity);
							bool isPriceValid = decimal.TryParse(shoppingCartItemDto.Price, out decimal price);
							bool isTotalPriceValid = decimal.TryParse(shoppingCartItemDto.TotalPrice, out decimal totalPrice);
							bool isProductIdValid = int.TryParse(shoppingCartItemDto.ProductId, out int productId);

							if (!isQuantityValid || !isPriceValid || !isTotalPriceValid || !isProductIdValid)
							{
								this.Logger.LogWarning(EntityDataParseError);
								continue;
							}

							if (!productDictionary.TryGetValue(productId, out var product))
							{
								this.Logger.LogWarning(ReferencedEntityMissing);
								continue;
							}

							if (shoppingCart.ShoppingCartItems.Any(sci => sci.ProductId == productId))
							{
								ShoppingCartItem existingItem = shoppingCart.ShoppingCartItems
									.First(sci => sci.ProductId == productId);

								existingItem.Quantity += quantity;
								existingItem.TotalPrice = existingItem.Quantity * existingItem.Price;
								continue;
							}

							ShoppingCartItem shoppingCartItem = new ShoppingCartItem
							{
								Quantity = quantity,
								Price = price,
								TotalPrice = totalPrice,
								ShoppingCart = shoppingCart,
								ProductId = productId,
							};

							shoppingCart.ShoppingCartItems.Add(shoppingCartItem);
						}

						if (!existingShoppingCartsUserIds.Contains(shoppingCartDto.UserId))
						{
							validShoppingCarts.Add(shoppingCart);
						}
					}

					if (validShoppingCarts.Count > 0)
					{
						await this._context.ShoppingCarts.AddRangeAsync(validShoppingCarts);
					}
					else
					{
						this.Logger.LogWarning(NoNewEntityDataToAdd);
					}

					await this._context.SaveChangesAsync();
					this.Logger.LogInformation("Shopping cart data processed and saved.");
				}
				else
				{
					this.Logger.LogWarning($"No ShoppingCart DTO's found in the XML file at {this.FilePath}.");
				}
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex.Message);
				throw;
			}

		}
	}
}
