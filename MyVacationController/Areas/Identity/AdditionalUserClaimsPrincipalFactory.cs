using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MyVacationController.Data;

namespace MyVacationController.Areas.Identity;

public class AdditionalUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IOptions<IdentityOptions> _optionsAccessor;

    public AdditionalUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor
    ) : base(userManager, roleManager, optionsAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _optionsAccessor = optionsAccessor;
    }

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        var identity = principal.Identity as ClaimsIdentity;

        var claims = new List<Claim>();
        if (user.EmployeeId is not null)
        {
            claims.Add(new Claim("EmployeeId", user.EmployeeId.ToString()!));
        }

        if (user.GivenName is not null)
        {
            claims.Add(new Claim(ClaimTypes.GivenName, user.GivenName));
        }

        if (user.SurName is not null)
        {
            claims.Add(new Claim(ClaimTypes.Surname, user.SurName));
        }

        if (user.DateOfBirth is not null)
        {
            claims.Add(new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString()!));
        }

        if (user.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "user"));
        }

        if (identity != null)
            identity.AddClaims(claims);
        return principal;
    }
}
