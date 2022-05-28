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
            services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
            services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
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

