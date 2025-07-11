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
using static OnlineStore.Web.Infrastructure.Extensions.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration
            .GetConnectionString("DbConnectionString") ?? 
                    throw new InvalidOperationException("Connection string 'DbConnectionString' not found.");

//This interceptor is used to ovveride the default EF Core delete behaviour.
builder.Services.AddScoped<SoftDeleteInterceptor>();

builder.Services.AddApplicationDbContext(connectionString);

builder.Services.AddScopedGenericRepositories(typeof(GenericRepository<,>));
builder.Services.AddScopedRepositories(typeof(IProductRepository).Assembly);


//This service is only used for the development seeding purposes.
builder.Services.AddScoped<IDbSeeder, ApplicationDbContextSeeder>();

builder.Services.AddScopedServices(typeof(IProductService).Assembly);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//Here we add the application identity via extension method!
builder.Services.AddCustomIdentity();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>();

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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//We are using this middleware to show application custom error views.
app.UseStatusCodePagesWithRedirects("/Home/Error?statusCode={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
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
