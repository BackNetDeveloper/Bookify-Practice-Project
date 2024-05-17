using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.DataSeeding
{
	public class DefaultRoles
	{
		public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
		{
			if (!roleManager.Roles.Any())
			{
				await roleManager.CreateAsync(new IdentityRole() { Name = SystemRoles.Admin });
				await roleManager.CreateAsync(new IdentityRole() { Name = SystemRoles.Archive });
				await roleManager.CreateAsync(new IdentityRole() { Name = SystemRoles.Reception });
			}
		}
	}
}
