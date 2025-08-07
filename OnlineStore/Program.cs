using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Interceptors;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Data.Seeding;
using OnlineStore.Data.Seeding.Interfaces;
using OnlineStore.Services.Core.Identity;
using OnlineStore.Services.Core.Interfaces;
using OnlineStore.Web.Infrastructure.Extensions;
using OnlineStore.Web.Infrastructure.Filters;

using static OnlineStore.Common.ApplicationConstants.Account;
using static OnlineStore.Web.Infrastructure.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

//Getting the application connection string.
var connectionString = builder.Configuration
            .GetConnectionString("DbConnectionString") ?? 
                    throw new InvalidOperationException("Connection string 'DbConnectionString' not found.");

//This interceptor is used to ovveride the default EF Core delete behaviour, so it is working with soft deletable entities.
builder.Services.AddScoped<SoftDeleteInterceptor>();

//Here we add and configure the app db context.
builder.Services.AddApplicationDbContext(connectionString);

builder.Services.AddScopedGenericRepositories(typeof(GenericRepository<,>));
builder.Services.AddScopedRepositories(typeof(IProductRepository).Assembly);


//This service is only used for the development seeding purposes.
builder.Services.AddScoped<IDbSeeder, ApplicationDbContextSeeder>();

builder.Services.AddScopedServices(typeof(IProductService).Assembly);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//Here we add the application custom identity.
builder.Services.AddCustomIdentity();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = LoginPath;
});

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>();

//Here we add the application custom filters.
builder.Services.AddCustomFilters(typeof(CleanEmptyAddressFilter).Assembly);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseApiSafeStatusCodeRedirects();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Here we add the guest tracking middleware, in order to support guest user functionality in the application.
app.UseGuestTracking();

app.MapControllerRoute(
	  name: "areas",
	  pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

if (app.Environment.IsDevelopment())
{
	using IServiceScope scope = app.Services.CreateScope();

	IServiceProvider services = scope.ServiceProvider;

	IDbSeeder dataProcessor = services.GetRequiredService<IDbSeeder>();
	await dataProcessor.SeedData();
}

app.Run();
