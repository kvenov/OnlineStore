using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration
            .GetConnectionString("DbConnectionString") ?? 
                    throw new InvalidOperationException("Connection string 'DbConnectionString' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//Here we regester the required services for the application
/*
    builder.Services.AddSingleton<IXmlHelper, XMLHelper>();
    builder.Services.AddScoped<IDbSeeder, ApplicationDbContextSeeder>();
*/

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Here think how to congigure the Identity(To use the AddIdentity() or the AddDefaultIdentity() as a methods) and which options to use!!!
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => 
            options.SignIn.RequireConfirmedAccount = false)
	.AddSignInManager<SignInManager<ApplicationUser>>()
	.AddEntityFrameworkStores<ApplicationDbContext>();

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
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
