using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Interceptors;
using OnlineStore.Data.Models;
using OnlineStore.Data.Repository;
using OnlineStore.Data.Repository.Interfaces;
using OnlineStore.Data.Seeding;
using OnlineStore.Data.Seeding.Interfaces;
using OnlineStore.Data.Utilities;
using OnlineStore.Data.Utilities.Interfaces;
using OnlineStore.Services.Core;
using OnlineStore.Services.Core.Admin;
using OnlineStore.Services.Core.Admin.Interfaces;
using OnlineStore.Services.Core.Identity;
using OnlineStore.Services.Core.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration
            .GetConnectionString("DbConnectionString") ?? 
                    throw new InvalidOperationException("Connection string 'DbConnectionString' not found.");

builder.Services.AddScoped<SoftDeleteInterceptor>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
	var interceptor = sp.GetRequiredService<SoftDeleteInterceptor>();
	options
		.UseSqlServer(connectionString)
		.AddInterceptors(interceptor);
});

//Here we add the required repositories for the Application Services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductRatingRepository, ProductRatingRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IProductPromotionRepository, ProductPromotionRepository>();


//Here we add the required services for the Application
builder.Services.AddSingleton<IXmlHelper, XMLHelper>();
builder.Services.AddScoped<IDbSeeder, ApplicationDbContextSeeder>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductPromotionService, ProductPromotionService>();
builder.Services.AddScoped<IAdminProductService, AdminProductService>();
builder.Services.AddScoped<IAdminProductPromotionService, AdminProductPromotionService>();
builder.Services.AddScoped<IAdminProductCategoryService, AdminProductCategoryService>();
builder.Services.AddScoped<IAdminBrandService, AdminBrandService>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IArticleService, ArticleService>();


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireUppercase = true;
		options.Password.RequireNonAlphanumeric = false;
		options.Password.RequiredLength = 6;
	})
	.AddSignInManager<SignInManager<ApplicationUser>>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

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
