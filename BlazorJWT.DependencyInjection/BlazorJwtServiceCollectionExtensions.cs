using Blazored.LocalStorage;
using BlazorJWT.BlazoredLocalStorage;
using BlazorJWT.Core.Providers;
using Microsoft.AspNetCore.Components.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BlazorJwtServiceCollectionExtensions
    {
        public static IServiceCollection AddBlazorJwtWithStorage<T>(this IServiceCollection services) where T : class, IStorageProvider
        {
            services.AddAuthorizationCore();
            services.AddScoped<JwtAuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>(provider =>
                provider.GetRequiredService<JwtAuthStateProvider>());
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
            services.AddScoped<IJwtUriProvider, JwtUriProvider>();
            services.AddScoped<IStorageProvider, T>();
            return services;
        }
        
        public static IServiceCollection AddBlazorJwt(this IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.AddBlazorJwtWithStorage<StorageProvider>();
            return services;
        }
    }    
}

