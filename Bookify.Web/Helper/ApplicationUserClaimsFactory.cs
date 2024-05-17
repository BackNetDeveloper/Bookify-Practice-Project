using Microsoft.Extensions.Options;

namespace Bookify.Web.Helper
{
	public class ApplicationUserClaimsFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
	{
		public ApplicationUserClaimsFactory(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
		{
		}
		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
		{
			var userClaims = await base.GenerateClaimsAsync(user);
			userClaims.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName));
			return userClaims;
		}
	}
}
