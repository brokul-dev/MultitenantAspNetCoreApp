using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace MultitenantAspNetCoreApp
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITenantSetter tenantSetter, ITenantStore tenantStore)
        {
            (string tenantName, string realPath) = GetTenantAndPathFrom(context.Request);

            Tenant tenant = tenantStore.GetTenant(tenantName);

            if (tenant == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Request.PathBase = $"/{tenantName}";
            context.Request.Path = realPath;
            tenantSetter.CurrentTenant = tenant;

            await _next(context);
        }

        private static (string tenantName, string realPath) GetTenantAndPathFrom(HttpRequest httpRequest)
        {
            // example: https://localhost/tenant1 -> gives tenant1
            var tenantName = new Uri(httpRequest.GetDisplayUrl()).Segments.FirstOrDefault(x => x != "/")?.TrimEnd('/');

            if (!string.IsNullOrWhiteSpace(tenantName) && 
                httpRequest.Path.StartsWithSegments($"/{tenantName}", out PathString realPath))
            {
                return (tenantName, realPath);
            }

            return (null, null);
        }
    }
}