using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineStore.Data.Models;
using OnlineStore.Data.Seeding.Interfaces;
using static OnlineStore.Data.Common.OutputMessages.ErrorMessages;

namespace OnlineStore.Data.Seeding
{
	public class WishlistSeeder : BaseSeeder<WishlistSeeder>, IEntitySeeder
	{
		private readonly ApplicationDbContext _context;

		public WishlistSeeder(ILogger<WishlistSeeder> logger, ApplicationDbContext context) :
					base(logger)
		{
			this._context = context;
		}

		public async Task SeedEntityData()
		{
			await this.SeedWishlistsForUsers();
		}

		private async Task SeedWishlistsForUsers()
		{
			
			var users = await this._context
					.Users
					.Where(u => u.Wishlist == null)
					.ToListAsync();

			if (users != null && users.Count > 0)
			{

				ICollection<Wishlist> validWishlists = new List<Wishlist>();
				foreach (var user in users)
				{

					var wishlist = new Wishlist
					{
						UserId = user.Id,
						IsDeleted = false
					};

					user.Wishlist = wishlist;
					validWishlists.Add(wishlist);
				}

				await this._context.Wishlists.AddRangeAsync(validWishlists);
				await this._context.SaveChangesAsync();
				this.Logger.LogInformation($"{validWishlists.Count} new Wishlists were added to the database.");
			}
			else
			{
				this.Logger.LogInformation(NoNewEntityDataToAdd);
			}

		}
	}
}
