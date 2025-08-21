using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RWA.Web.Application.Services.Ldap;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILdapAuthService _ldapAuh;

        public AccountController(ILdapAuthService ldapAuth)
        {
            _ldapAuh = ldapAuth;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //if (_ldapAuh.ValidateCredentials(model.Username, model.Password))
            if(true)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username)
                };
                var identity =  new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal, new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(2)
                });

                return RedirectToAction("Index", "Home");
            }
            model.ErrorMessage = "Echec de connection"; 
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Supprimez les cookies d'authentification
            await HttpContext.SignOutAsync("MyCookieAuth");

            // Redirigez vers la vue de connexion
            return RedirectToAction("Login", "Account");
        }
    }
}
