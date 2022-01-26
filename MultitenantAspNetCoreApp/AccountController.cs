using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace MultitenantAspNetCoreApp
{
    public class AccountController : Controller
    {
        private readonly ITenantContext _tenantContext;

        public AccountController(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        [HttpGet]
        public ActionResult Login([FromQuery] string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult> Login([FromForm] LoginViewModel viewModel)
        {
            var claims = new List<Claim>
            {
                new("UserName", viewModel.UserName)
            };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return Redirect(viewModel.ReturnUrl ?? $"/{_tenantContext.CurrentTenant.Name}");
        }

        [HttpGet]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }

    public class LoginViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}