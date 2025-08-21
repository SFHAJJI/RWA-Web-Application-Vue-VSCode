using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RWA.Web.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace RWA.Web.Application.Controllers
{
    public class HomeController : Controller
    {
        private readonly RwaContext _context;

        public HomeController(RwaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var username = User.Identity?.Name;
            ViewBag.Username = username;
            
            // Check HECATE status based on data
            try 
            {
                var hasHecateData = _context.HecateInventaireNormalises.Any();
                ViewBag.HecateStatus = hasHecateData ? "Finished" : "Open";
            }
            catch
            {
                ViewBag.HecateStatus = "Open";
            }
            
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult MenuPrincipalHecate()
        {
            // Logique potentielle avant de rediriger
            return RedirectToAction("MenuPrincipalHecate", "HECATE");
        }
        
        [HttpPost]
        [Authorize]
        public IActionResult MenuPrincipalRWAMarket()
        {
            // Logique potentielle avant de rediriger
            return RedirectToAction("MenuPrincipalRWAMarket", "RWAMarket");
        }

    }
}
