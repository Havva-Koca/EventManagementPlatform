using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories;
using EventManagement.Data.Repositories.Implementations;
using EventManagement.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EventManagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<EventManagementDbContext>()
    .AddDefaultTokenProviders();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
using(var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = ["Admin","Organizer", "Participant"];
    foreach (var roleName in roleNames)
    {
      if(!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
    // Seed the first admin user, if it doesn't already exist.
    // Email/password are read from configuration (User Secrets locally,
    // environment variables in production) — never hardcoded.
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string? adminEmail = builder.Configuration["AdminSeed:Email"];
    string? adminPassword = builder.Configuration["AdminSeed:Password"];

    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                FullName = "System Admin",
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(adminUser, adminPassword);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();


app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
