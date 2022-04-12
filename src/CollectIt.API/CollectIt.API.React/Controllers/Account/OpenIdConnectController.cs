using System.Security.Claims;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace CollectIt.API.React.Controllers.Account;

[ApiController]
[Route("connect")]
public class OpenIdConnectController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly SignInManager<User> _signInManager;

    public OpenIdConnectController(UserManager userManager,
                                   SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    [HttpPost("token")] 
    [Produces("application/json")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request is null || !request.IsPasswordGrantType())
        {
            return BadRequest("Only password grant type supported");
        }

        var (password, username) = ( request.Password, request.Username );
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>
                                                          {
                                                              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                                                              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                                                  "The username/password couple is invalid."
                                                          }!);

            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            var properties = new AuthenticationProperties(new Dictionary<string, string>
                                                          {
                                                              [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                                                              [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                                                  "The username/password couple is invalid."
                                                          }!);
            return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        principal.SetScopes(new[]
                            {
                                Scopes.OpenId,
                                Scopes.Email,
                                Scopes.Profile,
                                Scopes.Roles
                            }.Intersect(request.GetScopes()));

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case Claims.Name:
                yield return Destinations.AccessToken;
                
                if (principal.HasScope(Scopes.Profile))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Email))
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (principal.HasScope(Scopes.Roles))
                    yield return Destinations.IdentityToken;

                yield break;

            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}