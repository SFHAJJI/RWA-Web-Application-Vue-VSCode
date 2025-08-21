using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RWA.Web.Application.Services.Ldap;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Controllers
{
    public class MarketController : Controller
    {

        public MarketController()
        {
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            RWAMarketViewModel viewModel = new RWAMarketViewModel();
            viewModel.OnGet();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Filter(RWAMarketViewModel model)
        {
            //if (_ldapAuh.ValidateCredentials(model.Username, model.Password))
            model.OnPost();
            return View("Index", model);
        }

    }
}
