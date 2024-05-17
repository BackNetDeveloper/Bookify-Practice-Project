using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.DataSeeding
{
	public class DefaultUsers
	{
		public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
		{
			var admin = new ApplicationUser()
			{
				UserName = "Atwan",
				FullName = "AhmedAtwan",
				Email = "atwan@bookify.com",
				EmailConfirmed = true
			};
			var user = await userManager.FindByEmailAsync(admin.Email);
			if (user is null)
			{
				await userManager.CreateAsync(admin, "P@ssword12345");
				await userManager.AddToRoleAsync(admin,SystemRoles.Admin);
			}
		}
	}
}
