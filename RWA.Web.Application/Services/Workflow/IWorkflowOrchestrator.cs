using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RWA.Web.Application.Models;
using RWA.Web.Application.Models.Dtos;
using RWA.Web.Application.Services.Validation;

namespace RWA.Web.Application.Services.Workflow
{
    using Dtos;

    /// <summary>
    /// Interface for workflow orchestration without requiring explicit DbProvider parameters.
    /// The orchestrator manages its own scoped dependencies and wires actions to the singleton state machine.
    /// </summary>
    public interface IWorkflowOrchestrator
    {
        event Action<TransitionDto>? Transitioned;
        
        // Trigger-based methods (all POST endpoints should fire triggers)
        Task TriggerUploadAsync(List<(string FileName, byte[] Content)> files);
        Task TriggerApplyRwaMappingsAsync(List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> mappings);
        Task TriggerAddBddHistoriqueAsync(List<RWA.Web.Application.Models.Dtos.HecateInterneHistoriqueDto> items);
        Task TriggerUpdateObligationsAsync(List<RWA.Web.Application.Models.Dtos.HecateInventaireNormaliseDto> items);
        Task TriggerUpdateRafAsync(List<RWA.Web.Application.Models.Dtos.HecateTethysDto> items);
        Task TriggerApplyEquivalenceMappingsAsync(List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto> mappings);
        Task TriggerAsync(string triggerName);
        Task RevalidateCurrentAsync();
        Task ForceNextAsync();
        Task ResetAsync();
        
        // Legacy methods for backward compatibility
        Task<(ValidationResult validation, int updatedCount)> ApplyRwaMappingsAsync(List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> mappings);
        Task<(ValidationResult validation, RWA.Web.Application.Models.Dtos.EquivalenceApplyResultDto result)> ApplyEquivalenceMappingsAsync(List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto> mappings);
        
        // Read helpers for controllers so they don't need direct DB access
        Task<IEnumerable<RWA.Web.Application.Models.Dtos.WorkflowStepDto>> GetWorkflowStepsSnapshotAsync();
        Task<IEnumerable<object>> GetCategoriesForDropdownAsync();
        Task<List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>> GetEquivalenceCandidatesForMissingRowsAsync();
        Task<List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>> GetMissingRowsWithSuggestionsAsync();
        
        // Legacy synchronous methods for backward compatibility (will be removed in future versions)
        IEnumerable<RWA.Web.Application.Models.Dtos.WorkflowStepDto> GetWorkflowStepsSnapshot();
        IEnumerable<object> GetCategoriesForDropdown();
        List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto> GetEquivalenceCandidatesForMissingRows();
        List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> GetMissingRowsWithSuggestions();
        Task<List<HecateInventaireNormalise>> GetInventaireNormaliseByNumLignes(List<int> numLignes);
        Task<List<RWA.Web.Application.Models.Dtos.HecateInventaireNormaliseDto>> GetInvalidObligations();
        Task<List<HecateInventaireNormalise>> GetItemsToAddTobdd();
        Task<List<HecateTethysDto>> GetTethysStatusAsync();
        Task<List<HecateTethysDto>> TriggerUpdateTethysStatusAsync();
    }
}
