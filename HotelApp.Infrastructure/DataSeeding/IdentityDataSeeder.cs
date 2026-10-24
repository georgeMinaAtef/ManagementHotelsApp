using HotelApp.Domain.Contracts;
using HotelApp.Infrastructure.Identity.Data;
using HotelApp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelApp.Infrastructure.DataSeeding
{
    internal class IdentityDataSeeder : IDataSeeder
    {
        private readonly HotelIdentityDbContext dbContext;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<IdentityDataSeeder> logger;

        public IdentityDataSeeder(
            HotelIdentityDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityDataSeeder> logger
            )
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.logger = logger;
        }


        public async Task SeedDataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
                if (pendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(cancellationToken);

                if (!await roleManager.Roles.AnyAsync(cancellationToken))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                    await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
                }


                if (!await userManager.Users.AnyAsync(cancellationToken))
                {
                    var admin = new ApplicationUser()
                    {
                        DisplayName = "George Mina",
                        Email = "george3mina7@gmail.com",
                        UserName = "GeorgeMina",
                        PhoneNumber = "01212659270",
                    };


                    var createUser = await userManager.CreateAsync(admin, "P@ssw0rd");

                    if (createUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "SuperAdmin");
                    }

                    else
                    {
                        var error = string.Join(';', createUser.Errors.Select(x => x.Description));
                        logger.LogWarning($"Can Not Seed Default Admin {error}");
                    }


                }

            }

            catch (Exception ex)
            {
                logger.LogError(ex, "Data Seeding Exception");
                return;
            }

        }
    }
}
