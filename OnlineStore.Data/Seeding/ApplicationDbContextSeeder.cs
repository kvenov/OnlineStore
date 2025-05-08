using OnlineStore.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using OnlineStore.Data.Utilities.Interfaces;
using OnlineStore.Data.Seeding.Interfaces;
using AutoMapper;

namespace OnlineStore.Data.Seeding
{
    public class ApplicationDbContextSeeder : IDbSeeder
    {
		private readonly ApplicationDbContext _context;

		private readonly UserManager<ApplicationUser> userManager;
		private readonly RoleManager<IdentityRole<Guid>> roleManager;

		private readonly IXmlHelper _xmlHelper;

		private readonly ICollection<IEntitySeeder> entitySeeders;

		private readonly IMapper mapper;

		public ApplicationDbContextSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, 
			RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper, IXmlHelper xmlHelper)
		{
			this._context = context;

			this.userManager = userManager;
			this.roleManager = roleManager;

			this._xmlHelper = xmlHelper;
			this.mapper = mapper;

			this.entitySeeders = new List<IEntitySeeder>();
			// this.InitializeDbSeeders(movieLogger, cinemaMovieLogger, ticketLogger, watchlistLogger, identityLogger);
		}

		public async Task SeedData()
		{

			foreach (IEntitySeeder entitySeeder in this.entitySeeders)
			{
				await entitySeeder.SeedEntityData();
			}
		}

		private void InitializeDbSeeders()
		{

			//This is how should be done
			//this.entitySeeders.Add(new MoviesSeeder(this._context, movieLogger));
			//this.entitySeeders.Add(new CinemaMovieSeeder(this._context, cinemaMovieLogger, this.mapper));
			//this.entitySeeders.Add(new TicketSeeder(this._context, ticketLogger, this._xmlHelper));
			//this.entitySeeders.Add(new WatchlistSeeder(watchlistLogger, this._context, this._xmlHelper, this.userManager));
			//this.entitySeeders.Add(new IdentitySeeder(this._context, this.userManager, this.roleManager, identityLogger));

		}
	}
}
