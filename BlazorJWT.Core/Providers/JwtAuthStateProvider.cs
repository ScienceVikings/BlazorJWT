using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorJWT.Core.Providers;

public class JwtAuthStateProvider:AuthenticationStateProvider
{
    private readonly IJwtTokenProvider _tokenProvider;

    private readonly AuthenticationState _loggedOutState =
        new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    
    public JwtAuthStateProvider(IJwtTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenProvider.GetToken();
        if (!token.IsLoggedIn)
            return _loggedOutState;

        var claims = new List<Claim>()
        {
            new("id_token", token.IdToken),
            new("access_token", token.AccessToken),
            new("is_logged_in", token.IsLoggedIn.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var loggedInState = new AuthenticationState(new ClaimsPrincipal(identity));
        return loggedInState;
    }
}