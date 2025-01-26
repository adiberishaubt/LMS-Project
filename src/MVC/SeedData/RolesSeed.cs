using Library_Managment_System.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library_Managment_System.SeedData
{
    public static class RolesSeed
    {
        public static void SeedRoles(this IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!roleManager.RoleExistsAsync(Roles.Admin.ToString()).Result)
            {
                roleManager.CreateAsync(new IdentityRole { Name = Roles.Admin.ToString() }).Wait();
            }

            if (!roleManager.RoleExistsAsync(Roles.User.ToString()).Result)
            {
                roleManager.CreateAsync(new IdentityRole { Name = Roles.User.ToString() }).Wait();
            }
        }
    }
}
