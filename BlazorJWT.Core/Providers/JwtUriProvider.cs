using System.Web;

namespace BlazorJWT.Core.Providers;

public interface IJwtUriProvider
{
    Task<string> GetLoginUri(string baseUrl, string clientId, string responseUrl);
    Task<string> GetLogoutUri(string baseUrl, string clientId, string responseUrl);
}

public class JwtUriProvider:IJwtUriProvider
{
    private readonly IStorageProvider _storageProvider;

    public JwtUriProvider(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }
    
    public Task<string> GetLoginUri(string baseUrl, string clientId, string responseUrl)
    {
        var url = $"{baseUrl}/oauth2/authorize";
        var state = Guid.NewGuid();

        _storageProvider.SetState(state.ToString());
        
        var queryParams = new string[]
        {
            "response_type=token",
            "scope=profile+email+openid",
            $"client_id={clientId}",
            $"redirect_uri={HttpUtility.HtmlEncode(responseUrl)}",
            $"state={state}",
        };

        url += $"?{string.Join('&', queryParams)}";

        return Task.FromResult(url);
    }

    public Task<string> GetLogoutUri(string baseUrl, string clientId, string responseUrl)
    {
        var url = $"{baseUrl}/logout";
        _storageProvider.SetState("");
        var queryParams = new string[]
        {
            $"client_id={clientId}",
            $"logout_uri={HttpUtility.HtmlEncode(responseUrl)}",
        };
        url += $"?{string.Join('&', queryParams)}";
        return Task.FromResult(url);
    }
}