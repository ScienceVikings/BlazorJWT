using BlazorJWT.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorJWT.Core.Providers;

public interface IJwtTokenProvider
{
    Task SetTokenFromUri(string uri);
    Task SetTokenFromUri(Uri uri);
    Task<JwtToken?> GetToken();
    Task SetState(string state);
    Task DeleteToken();
}

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly IStorageProvider _storageProvider;

    public JwtTokenProvider(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
    
    public Task SetTokenFromUri(string uri)
    {
        return SetTokenFromUri(new Uri(uri));
    }

    public async Task SetTokenFromUri(Uri uri)
    {
        var query = QueryHelpers.ParseQuery(uri.Fragment.Remove(0,1));
        var token = new JwtToken();
        
        if (query.ContainsKey("access_token"))
        {
            token.AccessToken = query["access_token"];
        }
        
        if (query.ContainsKey("expires_in"))
        {
            token.ExpiresIn = int.Parse(query["expires_in"].ToString());
        }
        
        if (query.ContainsKey("id_token"))
        {
            token.IdToken = query["id_token"];
        }
        
        if (query.ContainsKey("state"))
        {
            token.State = query["state"];
        }
        
        if (query.ContainsKey("token_type"))
        {
            token.TokenType = query["token_type"];
        }
        
        var state = await _storageProvider.GetState();
        if (token.State != state)
            throw new Exception($"State does not match. Sent [{state}] Received [{token.State}]. Use IJwtTokenProvider.SetState to set the state of the user");

        await _storageProvider.SetToken(token);
    }

    public Task<JwtToken?> GetToken()
    {
        return _storageProvider.GetToken();
    }

    public async Task SetState(string state)
    {
        await _storageProvider.SetState(state);
    }

    public async Task DeleteToken()
    {
        await _storageProvider.DeleteToken();
        await _storageProvider.DeleteState();
    }
}