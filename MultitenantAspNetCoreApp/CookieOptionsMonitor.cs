using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace MultitenantAspNetCoreApp
{
    public class CookieOptionsMonitor : IOptionsMonitor<CookieAuthenticationOptions>
    {
        private readonly IOptionsFactory<CookieAuthenticationOptions> _optionsFactory;

        public CookieOptionsMonitor(IOptionsFactory<CookieAuthenticationOptions> optionsFactory)
        {
            _optionsFactory = optionsFactory;
        }

        public CookieAuthenticationOptions CurrentValue => Get(Options.DefaultName);

        public CookieAuthenticationOptions Get(string name)
        {
            return _optionsFactory.Create(name);
        }

        public IDisposable OnChange(Action<CookieAuthenticationOptions, string> listener) => null;
    }
}