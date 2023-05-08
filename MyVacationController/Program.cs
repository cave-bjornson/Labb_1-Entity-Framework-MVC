using System.Net.Mime;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyVacationController.Areas.Identity;
using MyVacationController.Data;

// Toggle seeding data from command line
var seedData = args.Length == 1 && args[0].ToLower() == "seeddata";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
    options.EnableSensitiveDataLogging();
});
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 0;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<
    IUserClaimsPrincipalFactory<ApplicationUser>,
    AdditionalUserClaimsPrincipalFactory
>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policyBuilder => policyBuilder.RequireClaim("IsAdmin"));
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllersWithViews();

if (seedData)
{
    builder.Services.AddTransient<DataSeeder>();
}

var app = builder.Build();

//Seed Data
if (seedData)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using var scope = scopedFactory.CreateScope();
    var service = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await service.Seed();
    Environment.Exit(0);
}

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

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
