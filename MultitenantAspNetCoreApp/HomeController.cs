using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultitenantAspNetCoreApp
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITenantContext _tenantContext;

        public HomeController(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var loggedUserName = User.Claims.Single(x => x.Type == "UserName").Value;
            var tenantName = _tenantContext.CurrentTenant.Name;

            return View(new HomeViewModel
            {
                Message = $"User '{loggedUserName}' is logged on tenant '{tenantName}'"
            });
        }
    }

    public class HomeViewModel
    {
        public string Message { get; set; }
    }
}