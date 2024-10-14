using Microsoft.AspNetCore.Identity;
using HRbackend.Models.Auth;
using HRbackend.Models.Enums;

namespace HRbackend.Seeders
{
    public class DatabaseSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DatabaseSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await CreateRolesAsync();
            await CreateAdminAsync();
        }

        // Method to seed roles based on ApplicationRoles enum
        private async Task CreateRolesAsync()
        {
            foreach (var role in Enum.GetValues(typeof(ApplicationRoles)))
            {
                var roleName = role.ToString();
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // Method to seed a single administrator user if one doesn't exist
        private async Task CreateAdminAsync()
        {
            // Define admin credentials
            var adminFirstName = "Admin";
            var adminLastName = "User";
            var adminEmail = "admin@hr.app";
            var adminPassword = "Password1@"; // Use a strong password

            // Check if admin user already exists
            var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
            if (existingAdmin == null)
            {
                // Create new admin user
                var adminUser = new ApplicationUser
                {
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true, // Assuming email confirmation is skipped for the seeder
                    Role = ApplicationRoles.Administrator
                };

                var result = await _userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // Assign Administrator role to the newly created user
                    await _userManager.AddToRoleAsync(adminUser, ApplicationRoles.Administrator.ToString());
                }
            }
        }
    }
}
