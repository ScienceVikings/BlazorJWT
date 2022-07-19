using Blazored.LocalStorage;
using BlazorJWT.Core.Providers;
using BlazorJWT.Models;

namespace BlazorJWT.BlazoredLocalStorage;

public class StorageProvider:IStorageProvider
{
    private readonly ILocalStorageService _localStorageService;

    public StorageProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public Task SetToken(JwtToken jwtToken)
    {
        return _localStorageService.SetItemAsync("JwtToken", jwtToken).AsTask();
    }

    public Task<JwtToken?> GetToken()
    {
        return _localStorageService.GetItemAsync<JwtToken?>("JwtToken").AsTask();
    }

    public Task DeleteToken()
    {
        return _localStorageService.RemoveItemAsync("JwtToken").AsTask();
    }

    public Task SetState(string state)
    {
        return _localStorageService.SetItemAsync("JwtState", state).AsTask();
    }

    public Task<string> GetState()
    {
        return _localStorageService.GetItemAsync<string>("JwtState").AsTask();
    }

    public Task DeleteState()
    {
        return _localStorageService.RemoveItemAsync("JwtState").AsTask();
    }
}