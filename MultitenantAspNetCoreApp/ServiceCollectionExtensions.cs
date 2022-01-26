using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MultitenantAspNetCoreApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMultitenancy(this IServiceCollection services)
        {
            services.AddScoped<TenantContext>();

            services.AddScoped<ITenantContext>(provider =>
                provider.GetRequiredService<TenantContext>());

            services.AddScoped<ITenantSetter>(provider =>
                provider.GetRequiredService<TenantContext>());

            services.AddScoped<ITenantStore, TenantStore>();
            return services;
        }

        public static IServiceCollection AddMultitenantAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie();

            services.AddPerRequestCookieOptions((options, httpContext) =>
            {
                var tenant = httpContext
                    .RequestServices.GetRequiredService<ITenantContext>()
                    .CurrentTenant;

                options.DataProtectionProvider = httpContext
                    .RequestServices.GetRequiredService<IDataProtectionProvider>()
                    .CreateProtector($"App.Tenants.{tenant.Name}");

                options.Cookie.Name = $"{tenant.Name}-Cookie";
            });

            return services;
        }

        private static void AddPerRequestCookieOptions(
            this IServiceCollection services,
            Action<CookieAuthenticationOptions, HttpContext> configAction)
        {
            services.AddSingleton<IOptionsMonitor<CookieAuthenticationOptions>, CookieOptionsMonitor>();
            services.AddSingleton<IConfigureOptions<CookieAuthenticationOptions>>(provider =>
            {
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return new CookieConfigureNamedOptions(httpContextAccessor, configAction);
            });
        }
    }
}