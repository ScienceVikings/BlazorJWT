using System.Threading.Tasks;
using BlazorJWT.Core.Providers;
using BlazorJWT.Models;

namespace BlazorJWT.UnitTests.Helpers;

public class MemoryStorageProvider:IStorageProvider
{
    private JwtToken _jwtToken = new();
    private string _state = string.Empty;
    
    public Task SetToken(JwtToken jwtToken)
    {
        _jwtToken = jwtToken;
        return Task.CompletedTask;
    }

    public Task<JwtToken> GetToken()
    {
        return Task.FromResult(_jwtToken);
    }

    public Task DeleteToken()
    {
        _jwtToken = new JwtToken();
        return Task.CompletedTask;
    }

    public Task SetState(string state)
    {
        _state = state;
        return Task.CompletedTask;
    }

    public Task<string> GetState()
    {
        return Task.FromResult(_state);
    }

    public Task DeleteState()
    {
        _state = string.Empty;
        return Task.CompletedTask;
    }
}