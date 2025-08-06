using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;

namespace OnlineStore.Data.Seeding
{
	public class ShoppingCartSeeder : BaseSeeder<ShoppingCartSeeder>, IEntitySeeder
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;

		public ShoppingCartSeeder(ILogger<ShoppingCartSeeder> logger, 
								  ApplicationDbContext context, 
								  UserManager<ApplicationUser> userManager) : 
					base(logger)
		{
			this._context = context;
			this._userManager = userManager;
		}

		public async Task SeedEntityData()
		{
			await this.CreateShoppingCartsForUsers();
		}

		private async Task CreateShoppingCartsForUsers()
		{

			try
			{
				var users = await this._context
					.Users
					.Include(u => u.ShoppingCart)
					.Where(u => u.ShoppingCart == null)
					.ToListAsync();

				if (users != null && users.Count > 0)
				{
					ICollection<ShoppingCart> validShoppingCarts = new List<ShoppingCart>();

					this.Logger.LogInformation($"{users.Count} users without ShoppingCart were found.");

					foreach (var user in users)
					{
						bool isAdminOrManager = await _userManager.IsInRoleAsync(user, "Admin") || 
									await _userManager.IsInRoleAsync(user, "Manager");

						if (!isAdminOrManager)
						{
							var shoppingCart = new ShoppingCart()
							{
								UserId = user.Id,
							};

							user.ShoppingCart = shoppingCart;
							validShoppingCarts.Add(shoppingCart);
						}
					}

					await this._context.ShoppingCarts.AddRangeAsync(validShoppingCarts);
					await this._context.SaveChangesAsync();
					this.Logger.LogInformation($"{validShoppingCarts.Count} new ShoppingCarts were added to the database.");
				}
				else
				{
					this.Logger.LogInformation(NoNewEntityDataToAdd);
				}
			}
			catch (Exception ex)
			{
				this.Logger.LogError(ex.Message);
				return;
			}
		}
	}
}
