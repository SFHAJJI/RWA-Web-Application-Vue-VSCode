using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RWA.Web.Application.Models;
using RWA.Web.Application.Models.Dtos;
using RWA.Web.Application.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RWA.Web.Application.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly RwaContext _context;
        private readonly IMemoryCache _cache;

        public AuditController(RwaContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("inventory/count")]
        public async Task<IActionResult> GetInventoryCount(CancellationToken cancellationToken)
        {
            try
            {
                var count = await _context.HecateInventaireNormalises.AsNoTracking().CountAsync(cancellationToken);
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
                if (!_cache.TryGetValue("InventoryColumns", out var columns))
                {
                    columns = new[]
                    {
                        new { text = "Identifiant", value = "identifiant" },
                        new { text = "Identifiant Unique Retenu", value = "identifiantUniqueRetenu" },
                        new { text = "RAF", value = "raf" },
                        new { text = "Ref Cat RWA", value = "refCategorieRwa" },
                        new { text = "Nom", value = "nom" },
                        new { text = "Source", value = "source" },
                        new { text = "Catégorie 1", value = "categorie1" },
                        new { text = "Catégorie 2", value = "categorie2" },
                        new { text = "Devise", value = "deviseDeCotation" },
                        new { text = "Valeur de Marché", value = "valeurDeMarche" },
                        new { text = "Période Clôture", value = "periodeCloture" },
                        new { text = "Date Maturité", value = "dateMaturite" },
                        new { text = "Date Expiration", value = "dateExpiration" },
                        new { text = "Num Ligne", value = "numLigne" },
                        new { text = "Taux Obligation", value = "tauxObligation" },
                        new { text = "Tiers", value = "tiers" },
                        new { text = "BOA SJ", value = "boaSj" },
                        new { text = "BOA Contrepartie", value = "boaContrepartie" },
                        new { text = "BOA Defaut", value = "boaDefaut" },
                        new { text = "Identifiant Origine", value = "identifiantOrigine" },
                        new { text = "RAF Enrichi", value = "rafenrichi" },
                        new { text = "Libelle Origine", value = "libelleOrigine" },
                        new { text = "Date Fin Contrat", value = "dateFinContrat" },
                        new { text = "Commentaires", value = "commentaires" },
                        new { text = "Bloomberg", value = "bloomberg" },
                        new { text = "Ref Type Depot", value = "refTypeDepot" },
                        new { text = "Ref Type Resultat", value = "refTypeResultat" },
                        new { text = "Code Resultat", value = "codeResultat" }
                    };
                    _cache.Set("InventoryColumns", columns, TimeSpan.FromMinutes(10));
                }
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("inventory/data")]
        public async Task<IActionResult> GetInventoryData([FromBody] DataTableRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.HecateInventaireNormalises.AsNoTracking();
                var response = await query.ToDataTablesResponse(request, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("history/count")]
        public async Task<IActionResult> GetHistoryCount(CancellationToken cancellationToken)
        {
            try
            {
                if (!_cache.TryGetValue("HistoryCount", out int count))
                {
                    count = await _context.HecateInterneHistoriques.AsNoTracking().CountAsync(cancellationToken);
                    _cache.Set("HistoryCount", count, TimeSpan.FromMinutes(10));
                }
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
                if (!_cache.TryGetValue("HistoryColumns", out var columns))
                {
                    columns = new[]
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
                    _cache.Set("HistoryColumns", columns, TimeSpan.FromMinutes(10));
                }
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("history/data")]
        public async Task<IActionResult> GetHistoryData([FromBody] DataTableRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.HecateInterneHistoriques.AsNoTracking();
                var response = await query.ToDataTablesResponse(request, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("tethys/count")]
        public async Task<IActionResult> GetTethysCount(CancellationToken cancellationToken)
        {
            try
            {
                if (!_cache.TryGetValue("TethysCount", out int count))
                {
                    count = await _context.HecateTethys.AsNoTracking().CountAsync(cancellationToken);
                    _cache.Set("TethysCount", count, TimeSpan.FromMinutes(10));
                }
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
                if (!_cache.TryGetValue("TethysColumns", out var columns))
                {
                    columns = new[]
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
                    _cache.Set("TethysColumns", columns, TimeSpan.FromMinutes(10));
                }
                return Ok(columns);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("tethys/data")]
        public async Task<IActionResult> GetTethysData([FromBody] DataTableRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.HecateTethys.AsNoTracking();
                var response = await query.ToDataTablesResponse(request, cancellationToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
