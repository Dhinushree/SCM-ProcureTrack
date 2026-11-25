
using DotNetAssignmentApp.Data;
using DotNetAssignmentApp.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services
builder.Services.AddControllersWithViews();

// Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity and Roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure role-based authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProcurementAccess", policy =>
        policy.RequireRole("Procurement Officer"));

    options.AddPolicy("WarehouseAccess", policy =>
        policy.RequireRole("Warehouse Storekeeper"));
});

var app = builder.Build();

// Centralized Exception Handling Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Create required roles and demo users
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedRolesAndUsersAsync(
        scope.ServiceProvider);
}

// Database connection test
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    var canConnect = await db.Database.CanConnectAsync();

    Console.WriteLine($"Database Connected: {canConnect}");
}

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
