using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using StandaloneRepo.ServiceModel.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ServiceStack;
using ServiceStack.Web;

namespace StandaloneRepo.Data;

public class CustomUserSession : AuthUserSession
{
    public override void PopulateFromClaims(IRequest httpReq, ClaimsPrincipal principal)
    {
        // Populate Session with data from Identity Auth Claims
        ProfileUrl = principal.FindFirstValue(JwtClaimTypes.Picture);
    }
}

/// <summary>
/// Add additional claims to the Identity Auth Cookie
/// </summary>
public class AdditionalUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<ApplicationUser,IdentityRole>(userManager, roleManager, optionsAccessor)
{
    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        var identity = (ClaimsIdentity)principal.Identity!;

        var claims = new List<Claim>();
        // Add additional claims here
        if (user.ProfileUrl != null)
        {
            claims.Add(new Claim(JwtClaimTypes.Picture, user.ProfileUrl));
        }

        //claims.Add(new Claim(JwtClaimTypes.Permissions, "Temp Perm 1"));

        if (user.Permissions?.Count > 0)
        {
            foreach(var permission in user.Permissions)
            {
                claims.Add(new Claim(JwtClaimTypes.Permission, permission));
            }
        }

        identity.AddClaims(claims);
        return principal;
    }
}