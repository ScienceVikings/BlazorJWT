using System;
using System.Threading.Tasks;
using BlazorJWT.Models;
using Bunit;
using NUnit.Framework;
using Shouldly;

namespace BlazorJWT.BlazoredLocalStorage.UnitTests;

public class StorageProviderShould:TestContextBase
{
    [Test]
    public async Task SetAndGetToken()
    {
        var token = new JwtToken()
        {
            AccessToken = "AccessToken",
            ExpiresIn = 3600,
            IdToken = "IdToken",
            State = "State",
            TokenType = "TokenType"
        };
        
        var storageProvider = new StorageProvider(this.AddBlazoredLocalStorage());
        await storageProvider.SetToken(token);

        var returnedToken = await storageProvider.GetToken();
        if (returnedToken == null)
        {
            throw new Exception("Returned token is unexpectedly null");
        }
        returnedToken.AccessToken.ShouldBe(token.AccessToken);
        returnedToken.ExpiresIn.ShouldBe(token.ExpiresIn);
        returnedToken.IdToken.ShouldBe(token.IdToken);
        returnedToken.State.ShouldBe(token.State);
        returnedToken.TokenType.ShouldBe(token.TokenType);
    }
    
    [Test]
    public async Task DeleteToken()
    {
        var token = new JwtToken()
        {
            AccessToken = "AccessToken",
            ExpiresIn = 3600,
            IdToken = "IdToken",
            State = "State",
            TokenType = "TokenType"
        };
        
        var storageProvider = new StorageProvider(this.AddBlazoredLocalStorage());
        await storageProvider.SetToken(token);
        await storageProvider.DeleteToken();
        var returnedToken = await storageProvider.GetToken();
        returnedToken.ShouldBeNull();
    }
    
    [Test]
    public async Task SetAndGetState()
    {
        var state = "The is the state!";
        var storageProvider = new StorageProvider(this.AddBlazoredLocalStorage());
        await storageProvider.SetState(state);
        var returnedState = await storageProvider.GetState();
        returnedState.ShouldBe(state);
    }
    
    [Test]
    public async Task DeleteState()
    {
        var state = "The is the state!";
        var storageProvider = new StorageProvider(this.AddBlazoredLocalStorage());
        await storageProvider.SetState(state);
        await storageProvider.DeleteState();
        var returnedState = await storageProvider.GetState();
        returnedState.ShouldBeNullOrEmpty();
    }
    
}
