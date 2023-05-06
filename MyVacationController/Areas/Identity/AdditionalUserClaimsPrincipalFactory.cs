using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MyVacationController.Data;

namespace MyVacationController.Areas.Identity;

public class AdditionalUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<IdentityOptions> _optionsAccessor;
    private readonly ILogger<AdditionalUserClaimsPrincipalFactory> _logger;

    public AdditionalUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<AdditionalUserClaimsPrincipalFactory> logger
    ) : base(userManager, optionsAccessor)
    {
        _userManager = userManager;
        _optionsAccessor = optionsAccessor;
        _logger = logger;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        // _logger.LogInformation("Run principal factory for user {Dump}", user.Dump());

        var identity = await base.GenerateClaimsAsync(user);

        // _logger.LogInformation("Identity: {Dump}", identity.Dump());

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
            claims.Add(new Claim("IsAdmin", ""));
        }

        identity.AddClaims(claims);
        // _logger.LogInformation("Claims should been added here {Dump}", identity.Dump());
        return identity;
    }
}
