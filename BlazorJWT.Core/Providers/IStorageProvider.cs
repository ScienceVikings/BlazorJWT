using BlazorJWT.Models;

namespace BlazorJWT.Core.Providers;

public interface IStorageProvider
{
    Task SetToken(JwtToken jwtToken);
    Task<JwtToken?> GetToken();
    Task DeleteToken();
    Task SetState(string state);
    Task<string> GetState();
    Task DeleteState();
}