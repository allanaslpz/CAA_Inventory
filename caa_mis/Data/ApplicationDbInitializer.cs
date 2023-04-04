using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace caa_mis.Data
{
    public class ApplicationDbInitializer
    {
        public static async void Seed(IApplicationBuilder applicationBuilder)
        {
            ApplicationDbContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<ApplicationDbContext>();
            try
            {
                ////Delete the database if you need to apply a new Migration
                //context.Database.EnsureDeleted();
                //Create the database if it does not exist and apply the Migration
                context.Database.Migrate();

                //Create Roles
                var RoleManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Admin", "Supervisor", "Security" };

                IdentityResult roleResult;
                foreach (var roleName in roleNames)
                {
                    var roleExist = await RoleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                //Create Branches
                var branchRoleManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] branchNames = { "Grimsby", "Welland", "Thorold", "Niagara Falls", "St. Catharines" };

                IdentityResult branchResult;
                foreach (var branchName in branchNames)
                {
                    var branchExist = await branchRoleManager.RoleExistsAsync(branchName);
                    if (!branchExist)
                    {
                        branchResult = await branchRoleManager.CreateAsync(new IdentityRole(branchName));
                    }
                }

                //Create Users
                var userManager = applicationBuilder.ApplicationServices.CreateScope()
                    .ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                if (userManager.FindByEmailAsync("kevin.rone05@gmail.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "kevin.rone05@gmail.com",
                        Email = "kevin.rone05@gmail.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Password").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }
                if (userManager.FindByEmailAsync("m.barde04@gmail.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "m.barde04@gmail.com",
                        Email = "m.barde04@gmail.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Password").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }
                if (userManager.FindByEmailAsync("AA.lopez03@gmail.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "AA.lopez03@gmail.com",
                        Email = "AA.lopez03@gmail.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Password").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Supervisor").Wait();
                    }
                }
                if (userManager.FindByEmailAsync("tsogo02@outlook.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "tsogo02@outlook.com",
                        Email = "tsogo02@outlook.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Password").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Admin").Wait();
                    }
                }
                if (userManager.FindByEmailAsync("luwi.floor01@apple.com").Result == null)
                {
                    IdentityUser user = new IdentityUser
                    {
                        UserName = "luwi.floor01@apple.com",
                        Email = "luwi.floor01@apple.com"
                    };

                    IdentityResult result = userManager.CreateAsync(user, "Password").Result;

                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Supervisor").Wait();
                    }
                }
                //if (userManager.FindByEmailAsync("user@outlook.com").Result == null)
                //{
                //    IdentityUser user = new IdentityUser
                //    {
                //        UserName = "user@outlook.com",
                //        Email = "user@outlook.com"
                //    };

                //    IdentityResult result = userManager.CreateAsync(user, "Pa55w@rd").Result;
                //    //Not in any role
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
