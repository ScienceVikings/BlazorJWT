using System;
using System.Linq;
using System.Threading.Tasks;
using BlazorJWT.Core.Providers;
using BlazorJWT.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace BlazorJWT.UnitTests.Providers;

public class JwtAuthStateProviderShould
{
    private IStorageProvider _storageProvider = new MemoryStorageProvider();
    private IJwtTokenProvider _tokenProvider = new JwtTokenProvider(new MemoryStorageProvider());
    private JwtAuthStateProvider _authStateProvider =
        new JwtAuthStateProvider(new JwtTokenProvider(new MemoryStorageProvider()));

    [SetUp]
    public void Setup()
    {
        _storageProvider = new MemoryStorageProvider();
        _tokenProvider = new JwtTokenProvider(_storageProvider);
        _authStateProvider = new JwtAuthStateProvider(_tokenProvider);
    }

    [Test]
    public async Task GetLoggedInState()
    {
        var basePath = "https://www.example.com/redirect_uri#";
        var idToken = "id_token=ID_TOKEN";
        var accessToken = "access_token=ACCESS_TOKEN";
        var token_type = "token_type=bearer";
        var expires_in = "expires_in=3600";
        var state = "state=STATE";

        var uri = basePath + $"{idToken}&{accessToken}&{token_type}&{expires_in}&{state}";
        await _storageProvider.SetState("STATE");
        await _tokenProvider.SetTokenFromUri(uri);

        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        authState.User.Claims.Count().ShouldBe(3);
    }

    [Test]
    public async Task GetLoggedOutState()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        authState.User.Claims.Count().ShouldBe(0);
        
    }
}