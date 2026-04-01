using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tms_api.Models;

namespace tms_api.Extensions
{
    public class SeedData
    {
        public static void EnsureSeedData(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new Role();
                role.Name = "Admin";
                var roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Teacher").Result)
            {
                var role = new Role();
                role.Name = "Teacher";
                var roleResult = roleManager.CreateAsync(role).Result;
            }


            if (!roleManager.RoleExistsAsync("Student").Result)
            {
                var role = new Role();
                role.Name = "Student";
                var roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Teamlead").Result)
            {
                var studentRole = roleManager.FindByNameAsync("Student").Result;
                var role = new Role();
                role.Name = "Teamlead";
                role.ParentId = studentRole.Id;
                var roleResult = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync("Member").Result)
            {
                var studentRole = roleManager.FindByNameAsync("Student").Result;
                var role = new Role();
                role.Name = "Member";
                role.ParentId = studentRole.Id;
                var roleResult = roleManager.CreateAsync(role).Result;
            }

            if ((userManager.FindByEmailAsync("admin@tmso.com").Result) == null)
            {
                var admin = new User
                {
                    UserName = "admin",
                    Email = "admin@tmso.com",
                    FullName = "Administration",
                    CreatedDate = DateTime.Now,
                    IsDeleted = false
                };

                var result = userManager.CreateAsync(admin, "12345").Result;
                var result2 = userManager.AddToRoleAsync(admin, "Admin").Result;
            }
        }
    }
}
