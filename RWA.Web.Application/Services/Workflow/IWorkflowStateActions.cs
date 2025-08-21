using RWA.Web.Application.Services.Workflow.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RWA.Web.Application.Services.Workflow
{
    /// <summary>
    /// Delegates for specific trigger types to provide type safety and clarity
    /// </summary>
    public delegate Task UploadTriggerCallbackDelegate(Trigger trigger, UploadResultContext context);
    public delegate Task ValidationTriggerCallbackDelegate(Trigger trigger, ValidationResultContext context);
    public delegate Task ErrorTriggerCallbackDelegate(Trigger trigger, UnexpectedErrorContext context);
    public delegate Task NextStepTriggerCallbackDelegate(Trigger trigger);

    /// <summary>
    /// Interface for scoped workflow state actions that get wired to the singleton state machine events.
    /// These actions can use scoped dependencies like DbContext.
    /// </summary>
    public interface IWorkflowStateActions
    {
        /// <summary>
        /// Sets the specific trigger callbacks that actions can use to trigger state machine transitions.
        /// This is called by the orchestrator during wiring.
        /// </summary>
        /// <param name="uploadCallback">Callback for upload-related triggers</param>
        /// <param name="validationCallback">Callback for validation-related triggers</param>
        /// <param name="errorCallback">Callback for error-related triggers</param>
        /// <param name="nextStepCallback">Callback for next step navigation triggers</param>
        void SetTriggerCallbacks(
            UploadTriggerCallbackDelegate uploadCallback,
            ValidationTriggerCallbackDelegate validationCallback,
            ErrorTriggerCallbackDelegate errorCallback,
            NextStepTriggerCallbackDelegate nextStepCallback);


        // State entry actions
        Task OnRWACategoryManagerEntryAsync();

        // State exit actions
        Task OnRWACategoryManagerExitAsync(string stepName);
        Task OnBDDManagerExitAsync(string stepName);
        Task OnRafManagerExitAsync(string stepName);
        Task OnEnrichiExportExitAsync(string stepName);

        // Internal transition actions with parameters
        Task OnTriggerUploadAsync(List<(string FileName, byte[] Content)> files);
        Task OnUploadPendingAsync(UploadResultContext context);
        Task OnUploadSuccessAsync(UploadResultContext context);
        Task OnUploadFailedAsync(UploadResultContext context);
        Task OnApplyRwaMappingsAsync(List<RWA.Web.Application.Models.Dtos.RwaMappingDto> mappings);
        Task OnApplyEquivalenceMappingsAsync(List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto> mappings);
        Task OnValidationSuccessAsync(ValidationResultContext context);
        Task OnValidationWarningAsync(ValidationResultContext context);
        Task OnValidationErrorAsync(ValidationResultContext context);
        Task OnUnexpectedErrorAsync(UnexpectedErrorContext context);
        Task OnForceNextFallbackAsync(ForceNextContext context);

        // Transition handling
        Task OnTransitionedAsync(TransitionDto transitionDto);

        // Reset functionality - performs reset operations and returns true when finished
        Task<bool> IsResetCompleteAsync();

        // Helper methods for orchestrator delegation
        Task<System.Collections.Generic.IEnumerable<RWA.Web.Application.Models.Dtos.WorkflowStepDto>> GetWorkflowStepsSnapshotAsync();
        Task<System.Collections.Generic.IEnumerable<object>> GetCategoriesForDropdownAsync();
        Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>> GetEquivalenceCandidatesForMissingRowsAsync();
        Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.MissingRowDto>> GetMissingRowsWithSuggestionsAsync();
    }
}
