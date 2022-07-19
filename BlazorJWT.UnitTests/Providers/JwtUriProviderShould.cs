using System;
using System.Threading.Tasks;
using System.Web;
using BlazorJWT.Core.Providers;
using BlazorJWT.UnitTests.Helpers;
using NUnit.Framework;
using Shouldly;

namespace BlazorJWT.UnitTests.Providers;

public class JwtUriProviderShould
{
    private IStorageProvider _storageProvider = new MemoryStorageProvider();
    private IJwtUriProvider _uriProvider = new JwtUriProvider(new MemoryStorageProvider());

    [SetUp]
    public void Setup()
    {
        _storageProvider = new MemoryStorageProvider();
        _uriProvider = new JwtUriProvider(_storageProvider);
    }

    [Test]
    public async Task GetLoginUri()
    {
        var baseUrl = "http://whatever.com";
        var clientId = "test-client-id";
        var responseUrl = "http://whatever.com/things?with=123&x=3";

        var loginUri = await _uriProvider.GetLoginUri(baseUrl, clientId, responseUrl);

        var expected =
            $"{baseUrl}/oauth2/authorize?response_type=token&scope=profile+email+openid&client_id={clientId}&redirect_uri={HttpUtility.HtmlEncode(responseUrl)}&state={await _storageProvider.GetState()}";
        
        loginUri.ShouldBe(expected);
    }

    [Test]
    public async Task GetLogoutUri()
    {
        var baseUrl = "http://whatever.com";
        var clientId = "test-client-id";
        var responseUrl = "http://whatever.com/things?with=123&x=3";

        var logoutUri = await _uriProvider.GetLogoutUri(baseUrl, clientId, responseUrl);
        
        var expected =
            $"{baseUrl}/logout?client_id={clientId}&logout_uri={HttpUtility.HtmlEncode(responseUrl)}";

        logoutUri.ShouldBe(expected);
    }
}