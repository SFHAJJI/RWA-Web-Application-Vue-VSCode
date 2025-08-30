using Microsoft.AspNetCore.Mvc;
using RWA.Web.Application.Models;
using RWA.Web.Application.Services.Workflow;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RWA.Web.Application.Models.Dtos;
using Newtonsoft.Json.Linq;
using RWA.Web.Application.Services;
using System.Threading;

namespace RWA.Web.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowOrchestrator _orchestrator;
        private readonly Services.Workflow.IWorkflowDbProvider _dbProvider;
        private readonly Microsoft.Extensions.Options.IOptions<RWA.Web.Application.Models.WorkflowStatusMappingOptions> _statusMapping;

        public WorkflowController(IWorkflowOrchestrator orchestrator, Services.Workflow.IWorkflowDbProvider dbProvider, Microsoft.Extensions.Options.IOptions<RWA.Web.Application.Models.WorkflowStatusMappingOptions> statusMapping)
        {
            _orchestrator = orchestrator;
            _dbProvider = dbProvider;
            _statusMapping = statusMapping;
        }
        [HttpGet("get-status")]
        [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Any)] // 5 second cache
        public ActionResult<IEnumerable<RWA.Web.Application.Models.Dtos.WorkflowStepDto>> GetWorkflowStatus()
        {
            var steps = _orchestrator.GetWorkflowStepsSnapshot();
            return Ok(steps);
        }

        // Convenience alias for clients that request /api/workflow/status
        [HttpGet("status")]
        [ResponseCache(Duration = 5, Location = ResponseCacheLocation.Any)] // 5 second cache
        public ActionResult<IEnumerable<RWA.Web.Application.Models.Dtos.WorkflowStepDto>> GetWorkflowStatusAlias()
        {
            return GetWorkflowStatus();
        }

        [HttpGet("get-categories")]
        public ActionResult<IEnumerable<object>> GetCategories()
        {
            var cats = _orchestrator.GetCategoriesForDropdown();
            return Ok(cats);
        }

        [HttpPost("post-apply-equivalences")]
        [HttpPost("apply-equivalences")]
        /// <summary>
        /// Apply equivalence mappings posted by the RWA Category Manager UI.
        /// </summary>
        /// <remarks>
        /// Called from: POST /api/workflow/apply-equivalences (or /post-apply-equivalences).
        /// Origin: user-initiated mapping changes in the UI (manual application of suggested equivalences).
        /// Calls: <see cref="IWorkflowOrchestrator.TriggerApplyEquivalenceMappingsAsync"/> to fire trigger with parameters.
        /// </remarks>
        public async Task<IActionResult> PostApplyEquivalences([FromBody] System.Collections.Generic.List<Models.Dtos.EquivalenceMappingDto> mappings)
        {
            if (mappings == null || mappings.Count == 0) return BadRequest("No mappings provided.");

            await _orchestrator.TriggerApplyEquivalenceMappingsAsync(mappings);
            // Orchestrator publishes authoritative snapshot via SignalR; return NoContent
            return NoContent();
        }

        [HttpPost("rwa-mappings")]
        /// <summary>
        /// Apply RWA category mappings submitted by the user from the RWA Category Manager step.
        /// </summary>
        /// <remarks>
        /// Called from: POST /api/workflow/rwa-mappings.
        /// Origin: user-initiated category mapping submissions in the RWA Category Manager UI.
        /// Calls: <see cref="IWorkflowOrchestrator.TriggerApplyRwaMappingsAsync"/> to fire trigger with parameters.
        /// </remarks>
        public async Task<IActionResult> PostRwaMappings([FromBody] Models.Dtos.SubmitRwaMappingsDto request)
        {
            if (request?.Mappings == null || request.Mappings.Count == 0) 
                return BadRequest("No mappings provided.");

            await _orchestrator.TriggerApplyRwaMappingsAsync(request.Mappings);
            // Orchestrator publishes authoritative snapshot via SignalR; return NoContent
            return NoContent();
        }
        [HttpGet("get-missing-rows")]
        public ActionResult<IEnumerable<Models.Dtos.RwaMappingRowDto>> GetRowsMissingCategory()
        {
            var rows = (_orchestrator)?.GetMissingRowsWithSuggestions();
            if (rows == null) return Ok(new Models.Dtos.RwaMappingRowDto[0]);
            return Ok(rows);
        }

        [HttpPost("post-upload")]
        [HttpPost("upload")]
        /// <summary>
        /// Upload Inventory file(s) and trigger server-side import/validation.
        /// </summary>
        /// <remarks>
        /// Called from: POST /api/workflow/upload (or /post-upload) when a user selects and uploads an inventory file.
        /// Origin: user-initiated file upload from the frontend. Calls <see cref="IWorkflowOrchestrator.TriggerUploadAsync"/> which attaches the parsed payload to the Upload step and runs validation (may auto-advance the workflow).
        /// </remarks>
        public async Task<IActionResult> PostUploadInventory([FromForm] List<IFormFile> files)
        {
            // Model binding may fail to populate 'files' if the form field name doesn't match.
            // Fall back to Request.Form.Files which contains any uploaded files.
            var formFiles = files;
            if ((formFiles == null || formFiles.Count == 0) && Request?.Form?.Files?.Count > 0)
            {
                formFiles = Request.Form.Files.ToList();
            }

            if (formFiles == null || formFiles.Count == 0) return BadRequest("No file provided.");

            // Process ALL files - convert to file info with content
            var fileData = new List<(string FileName, byte[] Content)>();
            foreach (var file in formFiles)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();
                fileData.Add((file.FileName, bytes));
            }

            await _orchestrator.TriggerUploadAsync(fileData);

            // Orchestrator publishes authoritative snapshot via SignalR; return NoContent
            return NoContent();
        }

        [HttpPost("post-trigger/{trigger}")]
        [HttpPost("trigger/{trigger}")]
        /// <summary>
        /// Trigger a named workflow transition (manual trigger from UI).
        /// </summary>
        /// <remarks>
        /// Called from: POST /api/workflow/trigger/{trigger} (or /post-trigger/{trigger}).
        /// Origin: user clicks a trigger button in the UI (for example, manual 'Next'/'Previous' or custom triggers). Calls <see cref="IWorkflowOrchestrator.TriggerAsync(string)"/> to fire the state-machine trigger.
        /// </remarks>
        public async Task<IActionResult> PostTriggerTransition(string trigger)
        {
            await _orchestrator.TriggerAsync(trigger);
            return NoContent();
        }

        [HttpPost("post-revalidate")]
        [HttpPost("revalidate")]
        /// <summary>
        /// Re-run validation for the current workflow step on demand.
        /// </summary>
        /// <remarks>
        /// Called from: POST /api/workflow/revalidate (or /post-revalidate).
        /// Origin: user-initiated revalidation (e.g., pressing a 'Revalidate' button). Calls <see cref="IWorkflowOrchestrator.RevalidateCurrentAsync"/> which runs validators and may advance the state machine on success.
        /// </remarks>
        public async Task<IActionResult> PostRevalidateCurrent()
        {
            await _orchestrator.RevalidateCurrentAsync();
            // Orchestrator published authoritative snapshot via SignalR; return NoContent
            return NoContent();
        }

        [HttpPost("post-force-next")]
        [HttpPost("force-next")]
        /// <summary>
        /// Force the workflow to advance to the next step regardless of validation state.
        /// </summary>
        /// <remarks>
        /// Called from: POST /api/workflow/force-next (or /post-force-next).
        /// Origin: administrative or manual override from the UI. Calls <see cref="IWorkflowOrchestrator.ForceNextAsync"/> to move the machine forward.
        /// </remarks>
        public async Task<IActionResult> PostForceNext()
        {
            await _orchestrator.ForceNextAsync();
            return NoContent();
        }

        [HttpPost("post-reset")]
        [HttpPost("reset")]
        /// <summary>
        /// Reset the workflow to its seeded initial state.
        /// </summary>
        /// <remarks>
        /// Called from: POST /api/workflow/reset (or /post-reset).
        /// Origin: manual reset action from UI or administrative tooling. Calls <see cref="IWorkflowOrchestrator.ResetAsync"/> to reseed the workflow steps.
        /// </remarks>
        public async Task<IActionResult> PostResetWorkflow()
        {
            await _orchestrator.ResetAsync();
            return NoContent();
        }

        [HttpPost("get-inventaire-normalise-by-numlignes")]
        public async Task<ActionResult<IEnumerable<HecateInventaireNormalise>>> GetInventaireNormaliseByNumLignes([FromBody] List<int> numLignes)
        {
            var rows = await _orchestrator.GetInventaireNormaliseByNumLignes(numLignes);
            return Ok(rows);
        }

        [HttpPost("post-apply-mappings")]
        /// <summary>
        /// Apply RWA mappings posted from the mapping UI and revalidate affected rows.
        /// </summary>
        /// <remarks>
        /// Called from: POST /api/workflow/post-apply-mappings.
        /// Origin: user-initiated mapping application within the RWA Category Manager step. Calls <see cref="IWorkflowOrchestrator.ApplyRwaMappingsAsync"/> to persist mappings and return validation results.
        /// </remarks>
        public async Task<IActionResult> PostApplyRwaMappings([FromBody] List<RwaMappingRowDto> mappings)
        {
            if (mappings == null || mappings.Count == 0) return BadRequest("No mappings provided.");

            await _orchestrator.ApplyRwaMappingsAsync(mappings);
            return NoContent();
        }



        [HttpPost("update-raf")]
        public async Task<IActionResult> PostUpdateRaf([FromBody] List<HecateTethysDto> items)
        {
            if (items == null || items.Count == 0) return BadRequest("No items provided.");

            await (_orchestrator).TriggerUpdateRafAsync(items);
            return NoContent();
        }

        [HttpGet("config")]
        public ActionResult<object> GetWorkflowConfig()
        {
            var cfg = new
            {
                AdvanceStatuses = _statusMapping.Value.AllAdvanceStatuses,
                ErrorStatuses = _statusMapping.Value.ErrorStatuses
            };

            return Ok(cfg);
        }

        [HttpGet("obl-validation-columns")]
        public IActionResult GetOblValidationColumns()
        {
            // Define columns for HecateInventaireNormalise
            var columns = new[]
            {
                new { field = "periodeCloture", header = "Periode Cloture" },
                new { field = "source", header = "Source" },
                new { field = "refCategorieRwa", header = "Categorie RWA" },
                new { field = "identifiantUniqueRetenu", header = "Identifiant Unique Retenu" },
                new { field = "tauxObligation", header = "Taux Obligation" },
                new { field = "dateMaturite", header = "Date Maturite" },
                new { field = "raf", header = "RAF" },
                new { field = "libelleOrigine", header = "Libelle Origine" },
                new { field = "dateFinContrat", header = "Date Fin Contrat" },
                new { field = "identifiantOrigine", header = "Identifiant Origine" },
                new { field = "valeurDeMarche", header = "Valeur De Marche" },
                new { field = "categorie1", header = "Categorie 1" },
                new { field = "categorie2", header = "Categorie 2" },
                new { field = "deviseDeCotation", header = "Devise De Cotation" },
                new { field = "dateExpiration", header = "Date Expiration" },
                new { field = "tiers", header = "Tiers" },
                new { field = "boaSj", header = "BOA SJ" },
                new { field = "boaContrepartie", header = "BOA Contrepartie" },
                new { field = "boaDefaut", header = "BOA Defaut" },
                new { field = "bloomberg", header = "Bloomberg" }
            };
            return Ok(columns);
        }

        [HttpGet("obl-validation-data")]
        public async Task<IActionResult> GetOblValidationData()
        {
            var data = await (_orchestrator).GetInvalidObligations();
            return Ok(data);
        }

        [HttpGet("add-to-bdd-columns")]
        public IActionResult GetAddToBddColumns()
        {
            // Define columns for HecateInterneHistorique
            var columns = new[]
            {
                new { field = "source", header = "Source" },
                new { field = "refCategorieRwa", header = "Categorie RWA" },
                new { field = "identifiantUniqueRetenu", header = "Identifiant Unique Retenu" },
                new { field = "raf", header = "RAF" },
                new { field = "libelleOrigine", header = "Libelle Origine" },
                new { field = "dateEcheance", header = "Date Echeance" },
                new { field = "identifiantOrigine", header = "Identifiant Origine" },
                new { field = "bbgticker", header = "BBG Ticker" },
                new { field = "libelleTypeDette", header = "Libelle Type Dette" }
            };
            return Ok(columns);
        }

        [HttpGet("add-to-bdd-data")]
        public async Task<IActionResult> GetAddToBddData()
        {
            var data = await (_orchestrator).GetItemsToAddTobdd();
            var filteredData = data.Where(i => i.AdditionalInformation.AddtoBDDDto.AddToBDD);
            var dto = filteredData.Select(i => i.ToBddHistoDto());
            return Ok(dto);
        }

        [HttpPost("submit-obl-validation")]
        public async Task<IActionResult> SubmitOblValidation([FromBody] List<HecateInventaireNormaliseDto> items)
        {
            if (items == null || items.Count == 0) return BadRequest("No items provided.");

            await (_orchestrator).TriggerUpdateObligationsAsync(items);
            return NoContent();
        }

        [HttpPost("submit-add-to-bdd")]
        public async Task<IActionResult> SubmitAddToBdd([FromBody] List<HecateInterneHistoriqueDto> items)
        {
            if (items == null || items.Count == 0) return BadRequest("No items provided.");

            await (_orchestrator).TriggerAddBddHistoriqueAsync(items);
            return NoContent();
        }

        [HttpGet("tethys-status")]
        public async Task<ActionResult<TethysStatusPage>> GetTethysStatus(
            [FromQuery] string filter = "all",
            [FromQuery] string? cursor = null,
            [FromQuery] int take = 20,
            [FromQuery] string? q = null)
        {
            var page = await _orchestrator.GetTethysStatusPageAsync(filter, cursor, take, q);
            return Ok(page);
        }

        [HttpPost("update-tethys-status")]
        public async Task<IActionResult> UpdateTethysStatus()
        {
            await _orchestrator.TriggerUpdateTethysStatusAsync();
            return Ok();
        }

        [HttpGet("tethys/search")]
        public async Task<ActionResult<RWA.Web.Application.Models.Dtos.TethysSearchPage>> Search(
          [FromQuery] string q, [FromQuery] string? cursor, [FromQuery] int take = 20, CancellationToken ct = default)
        {
            var page = await _orchestrator.SearchTethysAsync(q, cursor, Math.Clamp(take, 1, 100), ct);
            return Ok(page);
        }

        [HttpGet("tethys/suggestions/{numLigne}")]
        public async Task<ActionResult<IEnumerable<HecateTethysDto>>> Suggestions(long numLigne)
        {
            // This is a placeholder implementation. You will need to replace this with your actual suggestion logic.
            var data = await _orchestrator.GetTethysStatusAsync();
            return Ok(data.Take(5));
        }

        [HttpPost("tethys/assign")]
        public async Task<IActionResult> Assign([FromBody] AssignRafRequest req)
        {
            // This is a placeholder implementation. You will need to replace this with your actual assignment logic.
            await (_orchestrator).TriggerUpdateRafAsync(new List<HecateTethysDto> { new HecateTethysDto { NumLigne = (int)req.NumLigne, Raf = req.Raf } });
            // If all rows are validated, auto-trigger next step
            var allOk = await _dbProvider.AreAllTethysValidatedAsync();
            if (allOk)
            {
                await _orchestrator.TriggerAsync("NextRafManagerToEnrichiExport");
            }
            return Ok();
        }

        [HttpPost("tethys/assign-batch")]
        public async Task<IActionResult> AssignBatch([FromBody] RWA.Web.Application.Models.Dtos.AssignRafBatchRequest req)
        {
            if (req?.NumLignes == null || req.NumLignes.Count == 0) return BadRequest("No rows provided");
            await _dbProvider.UpdateRafForNumLignesAsync(req.NumLignes, req.Raf, req.CptTethys);
            var allOk = await _dbProvider.AreAllTethysValidatedAsync();
            if (allOk)
            {
                await _orchestrator.TriggerAsync("NextRafManagerToEnrichiExport");
            }
            return Ok();
        }

        [HttpGet("tethys/counts")]
        public async Task<ActionResult<RWA.Web.Application.Models.Dtos.TethysStatusCounts>> GetTethysCounts()
        {
            var counts = await _dbProvider.GetTethysStatusCountsAsync();
            return Ok(counts);
        }

        // BDD Match results paging (from in-memory store)
        [HttpGet("bddmatch/results")]
        public ActionResult<object> GetBddMatchResults(
            [FromServices] RWA.Web.Application.Services.BddMatch.IBddMatchStore store,
            [FromQuery] string version,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            var (items, total, processed) = store.Get(version, Math.Max(0, skip), Math.Clamp(take, 1, 200));
            return Ok(new { items, total, processed });
        }

        // BDD Match summary for the strip (processed/total and breakdown)
        [HttpGet("bddmatch/summary")]
        public ActionResult<object> GetBddMatchSummary(
            [FromServices] RWA.Web.Application.Services.BddMatch.IBddMatchStore store,
            [FromQuery] string version)
        {
            var (_, total, processed) = store.Get(version, 0, 0);
            var (allItems, _, _) = store.Get(version, 0, processed);
            int byIdU = 0, byIdO = 0, addToBdd = 0, noMatch = 0;
            foreach (var r in allItems)
            {
                if (r.MatchBy == "IdUniqueRetenu") byIdU++;
                else if (r.MatchBy == "IdOrigine") byIdO++;
                else if (r.MatchBy == "AddToBDD") addToBdd++;
                else if (r.MatchBy == "NoMatch") noMatch++;
            }
            // Duplicates not tracked in store yet; expose 0 for now.
            var duplicates = 0;
            return Ok(new { processed, total, byIdU, byIdO, addToBdd, noMatch, duplicates });
        }

        // Re-trigger matching (enqueue a new version)
        [HttpPost("bddmatch/retrigger")]
        public ActionResult<object> RetriggerBddMatch(
            [FromServices] RWA.Web.Application.Services.BddMatch.IBddMatchJobQueue queue)
        {
            var version = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            queue.Enqueue(new RWA.Web.Application.Services.BddMatch.BddMatchJob(version));
            return Ok(new { version });
        }

        // No in-memory mock helpers - workflow is persisted through the orchestrator-owned DbContext
    }
}

