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
        public IActionResult GetInventoryData()
        {
            try
            {
                var data = _context.HecateInventaireNormalises
                    .OrderByDescending(x => x.PeriodeCloture)
                    .Select(x => new
                    {
                        identifiant = x.Identifiant,
                        nom = x.Nom,
                        source = x.Source,
                        categorie1 = x.Categorie1,
                        categorie2 = x.Categorie2,
                        deviseDeCotation = x.DeviseDeCotation,
                        valeurDeMarche = x.ValeurDeMarche,
                        refCategorieRwa = x.RefCategorieRwa,
                        periodeCloture = x.PeriodeCloture,
                        dateMaturite = x.DateMaturite,
                        dateExpiration = x.DateExpiration
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
        public IActionResult GetHistoryData()
        {
            try
            {
                var data = _context.HecateInterneHistoriques
                    .OrderByDescending(x => x.LastUpdate)
                    .Select(x => new
                    {
                        source = x.Source,
                        identifiantOrigine = x.IdentifiantOrigine,
                        refCategorieRwa = x.RefCategorieRwa,
                        identifiantUniqueRetenu = x.IdentifiantUniqueRetenu,
                        raf = x.Raf,
                        libelleOrigine = x.LibelleOrigine,
                        dateEcheance = x.DateEcheance,
                        bbgticker = x.Bbgticker,
                        libelleTypeDette = x.LibelleTypeDette,
                        lastUpdate = x.LastUpdate
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
