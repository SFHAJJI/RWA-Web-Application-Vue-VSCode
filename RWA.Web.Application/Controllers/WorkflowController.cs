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

namespace RWA.Web.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowOrchestrator _orchestrator;
        private readonly Microsoft.Extensions.Options.IOptions<RWA.Web.Application.Models.WorkflowStatusMappingOptions> _statusMapping;

        public WorkflowController(IWorkflowOrchestrator orchestrator, Microsoft.Extensions.Options.IOptions<RWA.Web.Application.Models.WorkflowStatusMappingOptions> statusMapping)
        {
            _orchestrator = orchestrator;
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

        [HttpGet("get-equivalence-candidates")]
        public ActionResult<IEnumerable<Models.Dtos.EquivalenceCandidateDto>> GetEquivalenceCandidates()
        {
            var list = (_orchestrator as WorkflowOrchestrator)?.GetEquivalenceCandidatesForMissingRows();
            if (list == null) return Ok(new Models.Dtos.EquivalenceCandidateDto[0]);
            return Ok(list);
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
            var rows = (_orchestrator as WorkflowOrchestrator)?.GetMissingRowsWithSuggestions();
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
            var rows = await (_orchestrator as WorkflowOrchestrator).GetInventaireNormaliseByNumLignes(numLignes);
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

        [HttpPost("update-bdd")]
        public async Task<IActionResult> PostUpdateBdd([FromBody] List<HecateInterneHistoriqueDto> items)
        {
            if (items == null || items.Count == 0) return BadRequest("No items provided.");

            await (_orchestrator as WorkflowOrchestrator).TriggerAddBddHistoriqueAsync(items);
            return NoContent();
        }

        [HttpPost("update-obligations")]
        public async Task<IActionResult> PostUpdateObligations([FromBody] List<ObligationUpdateDto> items)
        {
            if (items == null || items.Count == 0) return BadRequest("No items provided.");

            await (_orchestrator as WorkflowOrchestrator).TriggerUpdateObligationsAsync(items);
            return NoContent();
        }

        [HttpPost("update-raf")]
        public async Task<IActionResult> PostUpdateRaf([FromBody] List<HecateTethysDto> items)
        {
            if (items == null || items.Count == 0) return BadRequest("No items provided.");

            await (_orchestrator as WorkflowOrchestrator).TriggerUpdateRafAsync(items);
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

        // No in-memory mock helpers - workflow is persisted through the orchestrator-owned DbContext
    }
}
