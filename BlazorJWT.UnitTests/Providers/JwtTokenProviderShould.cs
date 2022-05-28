using System;
using System.Threading.Tasks;
using BlazorJWT.Core.Providers;
using BlazorJWT.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace BlazorJWT.UnitTests.Providers;

public class JwtTokenProviderShould
{

    private IStorageProvider _storageProvider = new MemoryStorageProvider();
    private IJwtTokenProvider _tokenProvider = new JwtTokenProvider(new MemoryStorageProvider());

    [SetUp]
    public void Setup()
    {
        _storageProvider = new MemoryStorageProvider();
        _tokenProvider = new JwtTokenProvider(_storageProvider);
    }

    [Test]
    public async Task SetTokenFromUri()
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

        var token = await _storageProvider.GetToken();
        token.State.ShouldBe("STATE");
        token.AccessToken.ShouldBe("ACCESS_TOKEN");
        token.ExpiresIn.ShouldBe(3600);
        token.IdToken.ShouldBe("ID_TOKEN");
        token.TokenType.ShouldBe("bearer");
        
        uri = basePath + $"{accessToken}&{idToken}&{token_type}&{expires_in}&{state}";
        await _tokenProvider.SetTokenFromUri(uri);
        
        token = await _storageProvider.GetToken();
        token.State.ShouldBe("STATE");
        token.AccessToken.ShouldBe("ACCESS_TOKEN");
        token.ExpiresIn.ShouldBe(3600);
        token.IdToken.ShouldBe("ID_TOKEN");
        token.TokenType.ShouldBe("bearer");
    }

    [Test]
    public async Task ThrowIfStateDoesNotMatch()
    {
        var basePath = "https://www.example.com/redirect_uri#";
        var idToken = "id_token=ID_TOKEN";
        var accessToken = "access_token=ACCESS_TOKEN";
        var token_type = "token_type=bearer";
        var expires_in = "expires_in=3600";
        var state = "state=STATE";

        var uri = basePath + $"{idToken}&{accessToken}&{token_type}&{expires_in}&{state}";

        await Should.ThrowAsync<Exception>(async () =>
        {
            await _tokenProvider.SetTokenFromUri(uri);
        });
        
    }
}