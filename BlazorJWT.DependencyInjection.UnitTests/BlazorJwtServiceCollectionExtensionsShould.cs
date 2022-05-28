using System.Threading.Tasks;
using BlazorJWT.Core.Providers;
using Bunit;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

namespace BlazorJWT.DependencyInjection.UnitTests;

public class BlazorJwtServiceCollectionExtensionsShould:TestContextBase
{
    [Test]
    public void ShouldAddServices()
    {
        Services.AddBlazorJwt();
        this.AddBlazoredLocalStorage();

        Should.NotThrow(() =>
        {
            var authStateProvider = Services.GetRequiredService<AuthenticationStateProvider>();
            var jwtTokenProvider = Services.GetRequiredService<IJwtTokenProvider>();
        });
        
    }
}