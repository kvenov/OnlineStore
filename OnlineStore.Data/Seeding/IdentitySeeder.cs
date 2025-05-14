using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using static OnlineStore.Common.OutputMessages.ErrorMessages;

namespace OnlineStore.Data.Seeding
{
	public class IdentitySeeder : BaseSeeder<IdentitySeeder>, IEntitySeeder, IIdentitySeeder<ApplicationUser, IdentityRole>
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole> roleManager;


		public IdentitySeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
				RoleManager<IdentityRole> roleManager, ILogger<IdentitySeeder> logger) 
			: base(logger)
		{

			this._context = context;

			this.userManager = userManager;
			this.roleManager = roleManager;
		}

		public UserManager<ApplicationUser> UserManager =>
			this.userManager;

		public RoleManager<IdentityRole> RoleManager =>
			this.roleManager;

		public async Task SeedEntityData()
		{
			await SeedRoles();
			await SeedUsers();
		}

		private async Task SeedUsers()
		{
			await SeedUser("admin@example.com", "Admin@123", "Admin");
			await SeedUser("appManager@example.com", "Manager@789", "Manager");
			await SeedUser("appUser@example.com", "User@456", "User");
		}

		private async Task SeedRoles()
		{
			string[] roles = { "Admin", "Manager", "User" };

			foreach (var role in roles)
			{
				var roleExists = await this.RoleManager
						.RoleExistsAsync(role);

				if (!roleExists)
				{
					var result = await this.RoleManager.CreateAsync(new IdentityRole(role));
					if (!result.Succeeded)
					{
						throw new Exception(string.Format(FailedToCreateRole, role));
					}
				}
			}
		}

		private async Task SeedUser(string email, string password, string role)
		{
			ApplicationUser? user = await this.UserManager
						.FindByEmailAsync(email);

			if (user == null)
			{
				user = new ApplicationUser
				{
					UserName = email,
					Email = email,
					CreatedDate = DateTime.UtcNow
				};

				var createUserResult = await this.UserManager
						.CreateAsync(user, password);

				if (!createUserResult.Succeeded)
				{
					throw new Exception(string.Format(FailedToCreateUser, email));
				}

				var wishlist = new Wishlist
				{
					UserId = user.Id,
					IsDeleted = false
				};

				var shoppingCart = new ShoppingCart
				{
					CreatedAt = DateTime.UtcNow,
					UserId = user.Id
				};

				await this._context.Wishlists.AddAsync(wishlist);
				await this._context.ShoppingCarts.AddAsync(shoppingCart);
				await this._context.SaveChangesAsync();

				user.ShoppingCart = shoppingCart;
				user.Wishlist = wishlist;

				await this.UserManager.UpdateAsync(user);
			}

			var isInRole = await this.UserManager.IsInRoleAsync(user, role);

			if (!isInRole)
			{
				var addRoleResult = await this.UserManager.AddToRoleAsync(user, role);
				if (!addRoleResult.Succeeded)
				{
					throw new Exception(string.Format(FailedToAssignUserToRole, role, email));
				}
			}
		}
	}
}
