using System.Security.Claims;
using System.Threading.Tasks;
using _2FA.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace _2FA.Helpers
{
    // Adds EmployeeId claim to the signed-in principal when available
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, IdentityRole>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (user.EmployeeId.HasValue)
            {
                identity.AddClaim(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));
            }

            return identity;
        }
    }
}
