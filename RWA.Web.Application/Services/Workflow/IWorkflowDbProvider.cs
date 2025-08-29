using System.Collections.Generic;
using RWA.Web.Application.Models;
using RWA.Web.Application.Models.Dtos;

namespace RWA.Web.Application.Services.Workflow
{
    public interface IWorkflowDbProvider
    {
        Task<IEnumerable<WorkflowStep>> GetAllWorkflowStepsOrderedAsync();
        Task<IEnumerable<object>> GetCategoriesForDropdownAsync();
        Task<WorkflowStep?> GetStepByNameAsync(string stepName);
        Task<WorkflowStep?> GetCurrentStepAsync();
        Task<System.Collections.Generic.List<WorkflowStep>> GetStepsSnapshotAsync();
        Task AddRangeWorkflowStepsAsync(IEnumerable<WorkflowStep> steps);
        Task SaveChangesAsync();
        Task UpdateStepStatusAndDataAsync(string stepName, string status, string dataPayload);
        Task UpdateStepStatusAsync(string stepName, string status);
        // Additional helper operations moved from the orchestrator so DB
        // responsibilities are centralized in the provider implementation.
        Task<int> AutoMapExactMatchesAsync();
        Task<int> ApplyRwaMappingsAsync(System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> mappings);
        Task<int> PersistInventoryRowsAsync(System.Collections.Generic.IEnumerable<RWA.Web.Application.Models.HecateInventaireNormalise> rows);
        Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>> GetMissingRowsWithSuggestionsAsync();
        Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>> GetEquivalenceCandidatesForMissingRowsAsync();
        Task<RWA.Web.Application.Models.Dtos.EquivalenceApplyResultDto> ApplyEquivalenceMappingsAsync(System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto> mappings);
        Task<System.Collections.Generic.List<WorkflowStep>> ForceNextFallbackAndGetStepsAsync();
        System.Threading.Tasks.Task<System.Collections.Generic.List<WorkflowStep>> ResetWorkflowAsync();
        
        // Reset specific operations
        Task ClearInventoryTableAsync();
        Task InitializeWorkflowStepsAsync();
        Task<System.Collections.Generic.List<WorkflowStep>> GetWorkflowStepsAsync();
        
        // RWA Category Manager specific methods
        Task<RWA.Web.Application.Models.Dtos.RwaCategoryManagerPayloadDto> ProcessRwaCategoryMappingAsync();
        Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.CategorieRwaOptionDto>> GetCategorieRwaOptionsAsync();
        Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.TypeBloombergOptionDto>> GetTypeBloombergOptionsAsync();
        Task<List<HecateInventaireNormalise>> GetInventaireNormaliseByNumLignes(List<int> numLignes);

        // BDD Manager specific methods
        Task<List<HecateInventaireNormalise>> GetAllInventaireNormaliseAsync();
        Task<List<HecateInventaireNormalise>> GetAllInventaireNormaliseAsNoTrackingAsync();
        Task<List<HecateInterneHistorique>> GetAllHecateInterneHistoriqueAsync();
        Task<HecateInterneHistorique> FindMatchInHistoriqueAsync(System.Linq.Expressions.Expression<System.Func<HecateInterneHistorique, bool>> predicate);
        Task AddBddHistoriqueAsync(List<HecateInterneHistoriqueDto> items);
        Task UpdateObligationsAsync(List<RWA.Web.Application.Models.Dtos.HecateInventaireNormaliseDto> items);

        // RAF Manager specific methods
        Task<List<HecateContrepartiesTransparence>> GetHecateContrepartiesTransparenceAsync();
        Task UpdateInventaireNormaliseRangeAsync(List<HecateInventaireNormalise> items);
        Task<List<HecateTethy>> GetTethysDataByRafAsync(List<string> rafs);
        Task<bool> TethysExistsAsync(string raf, CancellationToken ct = default);
        Task<HashSet<string>> GetExistingTethysRafsAsync(List<string> rafs);
        Task<int> GetTethysCountAsync();
        Task UpdateTethysStatusForNumLignesAsync(List<int> numLignes, bool status);
        Task UpdateRafAsync(List<HecateTethysDto> items);
        Task UpdateRafForNumLignesAsync(List<int> numLignes, string raf, string? cptTethys = null);
        Task<bool> AreAllRafsCompletedAsync();
        Task<HecateTethysPayload> GetTethysMappingPayloadAsync();
        Task<RWA.Web.Application.Models.Dtos.TethysStatusCounts> GetTethysStatusCountsAsync();
        Task<bool> AreAllTethysValidatedAsync();

        // Tethys search (right pane)
        Task<RWA.Web.Application.Models.Dtos.TethysSearchPage> SearchTethysAsync(string q, string? cursor, int take, System.Threading.CancellationToken ct = default);
    }
}
