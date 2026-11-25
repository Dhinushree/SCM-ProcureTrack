
using Microsoft.AspNetCore.Identity;

namespace DotNetAssignmentApp.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndUsersAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            // Create the two required roles
            string[] roles =
            {
                "Procurement Officer",
                "Warehouse Storekeeper"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }

            // Demo users for testing role-based access
            await CreateUserAsync(
                userManager,
                "procurement@scm.local",
                "Procure@123",
                "Procurement Officer");

            await CreateUserAsync(
                userManager,
                "warehouse@scm.local",
                "Warehouse@123",
                "Warehouse Storekeeper");
        }

        private static async Task CreateUserAsync(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    user, password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
