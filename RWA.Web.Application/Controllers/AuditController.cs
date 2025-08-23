using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RWA.Web.Application.Models;
using RWA.Web.Application.Models.Dtos;
using System.Linq.Dynamic.Core;

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

        [HttpPost("history/data")]
        public async Task<IActionResult> GetHistoryData([FromBody] DataTableRequest request)
        {
            try
            {
                var query = _context.HecateInterneHistoriques.AsQueryable();

                // Filtering
                if (request.Filters != null)
                {
                    if (request.Filters.TryGetValue("IdentifiantOrigine", out var identifiantOrigine) && !string.IsNullOrEmpty(identifiantOrigine))
                        query = query.Where(x => x.IdentifiantOrigine.ToLower().Contains(identifiantOrigine.ToLower()));
                    if (request.Filters.TryGetValue("RefCategorieRwa", out var refCategorieRwa) && !string.IsNullOrEmpty(refCategorieRwa))
                        query = query.Where(x => x.RefCategorieRwa.ToLower().Contains(refCategorieRwa.ToLower()));
                    if (request.Filters.TryGetValue("Raf", out var raf) && !string.IsNullOrEmpty(raf))
                        query = query.Where(x => x.Raf.ToLower().Contains(raf.ToLower()));
                    if (request.Filters.TryGetValue("IdentifiantUniqueRetenu", out var identifiantUniqueRetenu) && !string.IsNullOrEmpty(identifiantUniqueRetenu))
                        query = query.Where(x => x.IdentifiantUniqueRetenu.ToLower().Contains(identifiantUniqueRetenu.ToLower()));
                }

                var totalItems = await query.CountAsync();

                // Sorting
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = query.OrderBy($"{request.SortBy} {(request.SortDesc ? "descending" : "ascending")}");
                }
                else
                {
                    query = query.OrderByDescending(x => x.LastUpdate);
                }

                // Pagination
                var pagedData = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                return Ok(new DataTablesResponse<HecateInterneHistorique>
                {
                    Items = pagedData,
                    TotalItems = totalItems
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("tethys/count")]
        public IActionResult GetTethysCount()
        {
            try
            {
                var count = _context.HecateTethys.Count();
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("tethys/columns")]
        public IActionResult GetTethysColumns()
        {
            try
            {
                var columns = new[]
                {
                    new { text = "Identifiant RAF", value = "identifiantRaf" },
                    new { text = "Libelle Court", value = "libelleCourt" },
                    new { text = "Raison Sociale", value = "raisonSociale" },
                    new { text = "Pays de Residence", value = "paysDeResidence" },
                    new { text = "Pays de Nationalite", value = "paysDeNationalite" },
                    new { text = "Numero et Nom de Rue", value = "numeroEtNomDeRue" },
                    new { text = "Ville", value = "ville" },
                    new { text = "Categorie Tethys", value = "categorieTethys" },
                    new { text = "NAF NACE", value = "nafNace" },
                    new { text = "Code ISIN", value = "codeIsin" },
                    new { text = "Segment de Risque", value = "segmentDeRisque" },
                    new { text = "Segmentation BPCE", value = "segmentationBpce" },
                    new { text = "Code CUSIP", value = "codeCusip" },
                    new { text = "RAF Tete Groupe Reglementaire", value = "rafTeteGroupeReglementaire" },
                    new { text = "Nom Tete Groupe Reglementaire", value = "nomTeteGroupeReglementaire" },
                    new { text = "Date Notation Interne", value = "dateNotationInterne" },
                    new { text = "Code Notation", value = "codeNotation" },
                    new { text = "Code Conso", value = "codeConso" },
                    new { text = "Code Apparentement", value = "codeApparentement" }
                };
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("tethys/data")]
        public async Task<IActionResult> GetTethysData([FromBody] DataTableRequest request)
        {
            try
            {
                var query = _context.HecateTethys.AsQueryable();

                // Filtering
                if (request.Filters != null)
                {
                    if (request.Filters.TryGetValue("IdentifiantRaf", out var identifiantRaf) && !string.IsNullOrEmpty(identifiantRaf))
                        query = query.Where(x => x.IdentifiantRaf.ToLower().Contains(identifiantRaf.ToLower()));
                    if (request.Filters.TryGetValue("RaisonSociale", out var raisonSociale) && !string.IsNullOrEmpty(raisonSociale))
                        query = query.Where(x => x.RaisonSociale.ToLower().Contains(raisonSociale.ToLower()));
                    if (request.Filters.TryGetValue("CodeIsin", out var codeIsin) && !string.IsNullOrEmpty(codeIsin))
                        query = query.Where(x => x.CodeIsin.ToLower().Contains(codeIsin.ToLower()));
                    if (request.Filters.TryGetValue("CodeCusip", out var codeCusip) && !string.IsNullOrEmpty(codeCusip))
                        query = query.Where(x => x.CodeCusip.ToLower().Contains(codeCusip.ToLower()));
                }

                var totalItems = await query.CountAsync();

                // Sorting
                if (!string.IsNullOrEmpty(request.SortBy))
                {
                    query = query.OrderBy($"{request.SortBy} {(request.SortDesc ? "descending" : "ascending")}");
                }

                // Pagination
                var pagedData = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();

                return Ok(new DataTablesResponse<HecateTethy>
                {
                    Items = pagedData,
                    TotalItems = totalItems
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
