using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly RwaContext _context;

        public AuditController(RwaContext context)
        {
            _context = context;
        }

        [HttpGet("inventory/count")]
        public IActionResult GetInventoryCount()
        {
            try
            {
                var count = _context.HecateInventaireNormalises.Count();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("inventory/columns")]
        public IActionResult GetInventoryColumns()
        {
            try
            {
                var columns = new[]
                {
                    new { text = "Identifiant", value = "identifiant" },
                    new { text = "Nom", value = "nom" },
                    new { text = "Source", value = "source" },
                    new { text = "Catégorie 1", value = "categorie1" },
                    new { text = "Catégorie 2", value = "categorie2" },
                    new { text = "Devise", value = "deviseDeCotation" },
                    new { text = "Valeur de Marché", value = "valeurDeMarche" },
                    new { text = "Référence Cat. RWA", value = "refCategorieRwa" },
                    new { text = "Période Clôture", value = "periodeCloture" },
                    new { text = "Date Maturité", value = "dateMaturite" },
                    new { text = "Date Expiration", value = "dateExpiration" }
                };
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("inventory/data")]
        public IActionResult GetInventoryData([FromQuery] int limit = 100)
        {
            try
            {
                var data = _context.HecateInventaireNormalises
                    .OrderByDescending(x => x.PeriodeCloture)
                    .Take(limit)
                    .Select(x => new
                    {
                        x.Identifiant,
                        x.Nom,
                        x.Source,
                        x.Categorie1,
                        x.Categorie2,
                        x.DeviseDeCotation,
                        x.ValeurDeMarche,
                        x.RefCategorieRwa,
                        x.PeriodeCloture,
                        x.DateMaturite,
                        x.DateExpiration
                    })
                    .ToList();

                return Ok(new { rows = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("history/count")]
        public IActionResult GetHistoryCount()
        {
            try
            {
                var count = _context.HecateInterneHistoriques.Count();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("history/columns")]
        public IActionResult GetHistoryColumns()
        {
            try
            {
                var columns = new[]
                {
                    new { text = "Source", value = "source" },
                    new { text = "Identifiant Origine", value = "identifiantOrigine" },
                    new { text = "Réf Cat. RWA", value = "refCategorieRwa" },
                    new { text = "Identifiant Unique", value = "identifiantUniqueRetenu" },
                    new { text = "RAF", value = "raf" },
                    new { text = "Libellé Origine", value = "libelleOrigine" },
                    new { text = "Date Échéance", value = "dateEcheance" },
                    new { text = "BBG Ticker", value = "bbgticker" },
                    new { text = "Type Dette", value = "libelleTypeDette" },
                    new { text = "Dernière MAJ", value = "lastUpdate" }
                };
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("history/data")]
        public IActionResult GetHistoryData([FromQuery] int limit = 100)
        {
            try
            {
                var data = _context.HecateInterneHistoriques
                    .OrderByDescending(x => x.LastUpdate)
                    .Take(limit)
                    .Select(x => new
                    {
                        x.Source,
                        x.IdentifiantOrigine,
                        x.RefCategorieRwa,
                        x.IdentifiantUniqueRetenu,
                        x.Raf,
                        x.LibelleOrigine,
                        x.DateEcheance,
                        x.Bbgticker,
                        x.LibelleTypeDette,
                        x.LastUpdate
                    })
                    .ToList();

                return Ok(new { rows = data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
