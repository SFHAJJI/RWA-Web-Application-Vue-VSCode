using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RWA.Web.Application.Services.Validation;
using RWA.Web.Application.Services.Workflow.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Workflow
{
    /// <summary>
    /// Scoped orchestrator that wires scoped actions to the singleton state machine.
    /// Implements IDisposable to properly unsubscribe from events when the scope ends.
    /// </summary>
    public class WorkflowOrchestrator : IWorkflowOrchestrator, IDisposable
    {
        private static int _instanceCounter = 0;
        private readonly WorkflowStateMachine _stateMachine; // Singleton
        private readonly IWorkflowStateActions _actions; // Scoped
        private readonly ILogger<WorkflowOrchestrator> _logger;
        private readonly int _instanceId;
        private bool _isInitialized = false;
        private bool _disposed = false;

        public event Action<TransitionDto>? Transitioned;

        public WorkflowOrchestrator(
            WorkflowStateMachine stateMachine,
            IWorkflowStateActions actions,
            ILogger<WorkflowOrchestrator> logger)
        {
            _instanceId = ++_instanceCounter;
            _stateMachine = stateMachine;
            _actions = actions;
            _logger = logger;
            
            var orchestratorId = GetHashCode();
            var stateMachineId = _stateMachine.GetHashCode();
            var actionsId = _actions.GetHashCode();
            
            _logger.LogWarning("🏗️ ORCHESTRATOR CONSTRUCTOR #{InstId} - OrchId: {OrchId}, StateMachine: {SmId}, Actions: {ActId}, Total Instances: {TotalCount}", 
                _instanceId, orchestratorId, stateMachineId, actionsId, _instanceCounter);
            
            // Wire events to the state machine only once (singleton pattern)
            WireActionsToStateMachine();
        }

        /// <summary>
        /// Ensures the state machine is properly initialized by activating the initial state.
        /// This triggers the bootstrap process that queries DB and transitions to current step.
        /// </summary>
        private async Task EnsureInitializedAsync()
        {
            if (!_isInitialized)
            {
                try
                {
                    // If state machine is in InitialState, activate it - this will trigger OnActivate() -> LetsGo -> InitialBootstrap
                    if (_stateMachine.CurrentState == State.InitialState)
                    {
                        await _stateMachine.FireAsync(Trigger.LetsGo);
                    }
                    _isInitialized = true;
                    _logger.LogInformation("State machine initialized successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize state machine");
                    throw;
                }
            }
        }

        /// <summary>
        /// Fires validation triggers with ValidationResultContext
        /// </summary>
        private async Task FireValidationTriggerAsync(Trigger trigger, ValidationResultContext context)
        {
            var triggerToFire = trigger switch
            {
                Trigger.ValidationSuccess => _stateMachine.ValidationSuccessTrigger,
                Trigger.ValidationWarning => _stateMachine.ValidationWarningTrigger,
                Trigger.ValidationError => _stateMachine.ValidationErrorTrigger,
                _ => null
            };

            if (triggerToFire != null)
            {
                await _stateMachine.FireAsync(triggerToFire, context);
            }
            else
            {
                _logger.LogWarning("Unsupported validation trigger {Trigger}", trigger);
            }
        }

        /// <summary>
        /// Fires upload triggers with AggregateUploadResultContext
        /// </summary>
        private async Task FireUploadTriggerAsync(Trigger trigger, AggregateUploadResultContext context)
        {
            var triggerToFire = trigger switch
            {
                Trigger.UploadPending => _stateMachine.UploadPendingTrigger,
                Trigger.UploadSuccess => _stateMachine.UploadSuccessTrigger,
                Trigger.UploadFailed => _stateMachine.UploadFailedTrigger,
                _ => null
            };

            if (triggerToFire != null)
            {
                await _stateMachine.FireAsync(triggerToFire, context);
            }
            else
            {
                _logger.LogWarning("Unsupported upload trigger {Trigger}", trigger);
            }
        }

        /// <summary>
        /// Fires next step triggers without context (for navigation between steps)
        /// </summary>
        private async Task FireNextStepTriggerAsync(Trigger trigger)
        {
            await _stateMachine.FireAsync(trigger);
        }

        /// <summary>
        /// Fires unexpected error triggers with UnexpectedErrorContext
        /// </summary>
        private async Task FireUnexpectedErrorTriggerAsync(Trigger trigger, UnexpectedErrorContext context)
        {
            if (trigger == Trigger.UnexpectedError && _stateMachine.UnexpectedErrorTrigger != null)
            {
                await _stateMachine.FireAsync(_stateMachine.UnexpectedErrorTrigger, context);
            }
            else
            {
                _logger.LogWarning("Unsupported unexpected error trigger {Trigger}", trigger);
            }
        }

        /// <summary>
    /// <summary>
    /// Wire scoped state machine events to scoped action handlers.
    /// This creates one-to-one mapping that will be unsubscribed on disposal.
    /// </summary>
    private void WireActionsToStateMachine()
    {
        var orchestratorId = GetHashCode();
        var actionsId = _actions.GetHashCode();
        var stateMachineId = _stateMachine.GetHashCode();
        
        _logger.LogWarning("🔗 WIRING EVENTS - Orchestrator {OrchId}, Actions {ActId}, StateMachine {SmId}", 
            orchestratorId, actionsId, stateMachineId);

            // State entry actions
            _stateMachine.RWACategoryManagerEntry += _actions.OnRWACategoryManagerEntryAsync;
            _stateMachine.BDDManagerEntry += _actions.OnBDDManagerEntryAsync;
            _stateMachine.RafManagerEntry += _actions.OnRafManagerEntryAsync;
            _stateMachine.EnrichiExportEntry += _actions.OnEnrichiExportEntryAsync;

            // State exit actions
            _stateMachine.RWACategoryManagerExit += _actions.OnRWACategoryManagerExitAsync;
            _stateMachine.BDDManagerExit += _actions.OnBDDManagerExitAsync;
            _stateMachine.RafManagerExit += _actions.OnRafManagerExitAsync;
            _stateMachine.EnrichiExportExit += _actions.OnEnrichiExportExitAsync;

            // Internal transition actions
            _stateMachine.TriggerUpload += _actions.OnTriggerUploadAsync;
            _stateMachine.UploadPending += _actions.OnUploadPendingAsync;
            _stateMachine.UploadSuccess += _actions.OnUploadSuccessAsync;
            _stateMachine.UploadFailed += _actions.OnUploadFailedAsync;
            _stateMachine.ApplyRwaMappings += (mappings) => _actions.OnApplyRwaMappingsAsync(mappings);
            _stateMachine.ApplyEquivalenceMappings += _actions.OnApplyEquivalenceMappingsAsync;
            _stateMachine.ValidationSuccess += _actions.OnValidationSuccessAsync;
            _stateMachine.ValidationWarning += _actions.OnValidationWarningAsync;
            _stateMachine.ValidationError += _actions.OnValidationErrorAsync;
            _stateMachine.UnexpectedError += _actions.OnUnexpectedErrorAsync;
            _stateMachine.ForceNextFallback += _actions.OnForceNextFallbackAsync;
            _stateMachine.UpdateObligations += _actions.OnUpdateObligationsAsync;
            _stateMachine.UpdateRaf += _actions.OnUpdateRafAsync;
            _stateMachine.AddBddHistorique += _actions.OnAddBddHistoriqueAsync;

            // Wire the reset completion check
            _stateMachine.IsResetComplete += _actions.IsResetCompleteAsync;

            // Wire the state machine Transitioned event to actions
            _stateMachine.Transitioned += async (transitionDto) => await _actions.OnTransitionedAsync(transitionDto);

            // Set up specific typed callbacks so actions can trigger state machine transitions
            _actions.SetTriggerCallbacks(
                // Upload callback
                async (trigger, context) => await FireUploadTriggerAsync(trigger, context),
                // Validation callback  
                async (trigger, context) => await FireValidationTriggerAsync(trigger, context),
                // Error callback
                async (trigger, context) => await FireUnexpectedErrorTriggerAsync(trigger, context),
                // Next step callback
                async (trigger) => await FireNextStepTriggerAsync(trigger)
            );
            
            // Log handler counts for critical events
            var handlerCounts = _stateMachine.GetEventHandlerCounts();
            var countsStr = string.Join(", ", handlerCounts.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            
            _logger.LogWarning("✅ WIRING COMPLETE #{InstId} - Orchestrator {OrchId}, Handler Counts: {HandlerCounts}", 
                _instanceId, orchestratorId, countsStr);
        }

        // IWorkflowOrchestrator implementation - delegates to state machine triggers
        public async Task TriggerUploadAsync(List<(string FileName, byte[] Content)> files)
        {
            var orchestratorCallId = Guid.NewGuid();
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var timestamp = DateTime.Now;
            
            _logger.LogWarning("🚀 TriggerUploadAsync CALLED in WorkflowOrchestrator - CallId: {CallId}, Thread: {ThreadId}, Files: [{FileNames}], Time: {Timestamp}", 
                orchestratorCallId, threadId, string.Join(", ", files.Select(f => f.FileName)), timestamp);
            _logger.LogWarning("📋 Stack Trace in TriggerUploadAsync:\n{StackTrace}", Environment.StackTrace);
            
            await EnsureInitializedAsync();
            if (_stateMachine.SetUploadTrigger != null)
            {
                _logger.LogWarning("🎯 Firing StateMachine SetUploadTrigger - CallId: {CallId}, Thread: {ThreadId}", 
                    orchestratorCallId, threadId);
                await _stateMachine.FireAsync(_stateMachine.SetUploadTrigger, files);
            }
            else
            {
                _logger.LogWarning("❌ SetUploadTrigger is NULL - CallId: {CallId}", orchestratorCallId);
            }
            
            _logger.LogWarning("✅ TriggerUploadAsync COMPLETED in WorkflowOrchestrator - CallId: {CallId}, Duration: {Duration}ms", 
                orchestratorCallId, (DateTime.Now - timestamp).TotalMilliseconds);
        }

        public async Task TriggerApplyRwaMappingsAsync(System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> mappings)
        {
            await EnsureInitializedAsync();
            if (_stateMachine.ApplyRwaMappingsTrigger != null)
                await _stateMachine.FireAsync<List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>>(_stateMachine.ApplyRwaMappingsTrigger, mappings);
        }

        public async Task TriggerApplyEquivalenceMappingsAsync(System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto> mappings)
        {
            await EnsureInitializedAsync();
            if (_stateMachine.ApplyEquivalencesTrigger != null)
                await _stateMachine.FireAsync(_stateMachine.ApplyEquivalencesTrigger, mappings);
        }

        public async Task TriggerAddBddHistoriqueAsync(List<RWA.Web.Application.Models.Dtos.HecateInterneHistoriqueDto> items)
        {
            await EnsureInitializedAsync();
            if (_stateMachine.AddBddHistoriqueTrigger != null)
                await _stateMachine.FireAsync(_stateMachine.AddBddHistoriqueTrigger, items);
        }

        public async Task TriggerUpdateObligationsAsync(List<RWA.Web.Application.Models.Dtos.HecateInventaireNormaliseDto> items)
        {
            await EnsureInitializedAsync();
            if (_stateMachine.UpdateObligationsTrigger != null)
                await _stateMachine.FireAsync(_stateMachine.UpdateObligationsTrigger, items);
        }
        public async Task TriggerUpdateRafAsync(List<RWA.Web.Application.Models.Dtos.HecateTethysDto> items)
        {
            await EnsureInitializedAsync();
            if (_stateMachine.UpdateRafTrigger != null)
                await _stateMachine.FireAsync(_stateMachine.UpdateRafTrigger, items);
        }

        public async Task TriggerAsync(string triggerName)
        {
            await EnsureInitializedAsync();
            if (Enum.TryParse<Trigger>(triggerName, out var trigger))
                await _stateMachine.FireAsync(trigger);
            else
                _logger.LogWarning("Unknown trigger: {TriggerName}", triggerName);
        }

        public async Task RevalidateCurrentAsync()
        {
            await EnsureInitializedAsync();
            await _stateMachine.FireAsync(Trigger.ReValidate);
        }

        public async Task ForceNextAsync()
        {
            await EnsureInitializedAsync();
            await _stateMachine.FireAsync(Trigger.ForceNext);
        }

        public async Task ResetAsync()
        {
            await EnsureInitializedAsync();
            await _stateMachine.FireAsync(Trigger.Reset);
        }

        public async Task<(ValidationResult validation, int updatedCount)> ApplyRwaMappingsAsync(System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> mappings)
        {
            await TriggerApplyRwaMappingsAsync(mappings);
            // Return success result - the actual work is done in the state machine action
            return (new ValidationResult { OverallStatus = ValidationStatus.Success }, mappings.Count);
        }

        public async Task<(ValidationResult validation, RWA.Web.Application.Models.Dtos.EquivalenceApplyResultDto result)> ApplyEquivalenceMappingsAsync(List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto> mappings)
        {
            await TriggerApplyEquivalenceMappingsAsync(mappings);
            // Return success result - the actual work is done in the state machine action
            return (new ValidationResult { OverallStatus = ValidationStatus.Success }, new RWA.Web.Application.Models.Dtos.EquivalenceApplyResultDto());
        }

        // Read helpers - delegate to scoped actions
        public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<RWA.Web.Application.Models.Dtos.WorkflowStepDto>> GetWorkflowStepsSnapshotAsync()
        {
            return await _actions.GetWorkflowStepsSnapshotAsync();
        }

        public async System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<object>> GetCategoriesForDropdownAsync()
        {
            return await _actions.GetCategoriesForDropdownAsync();
        }

        public async System.Threading.Tasks.Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>> GetEquivalenceCandidatesForMissingRowsAsync()
        {
            return await _actions.GetEquivalenceCandidatesForMissingRowsAsync();
        }

        public async System.Threading.Tasks.Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>> GetMissingRowsWithSuggestionsAsync()
        {
            return await _actions.GetMissingRowsWithSuggestionsAsync();
        }

        // Legacy synchronous methods - delegate to async versions
        public System.Collections.Generic.IEnumerable<RWA.Web.Application.Models.Dtos.WorkflowStepDto> GetWorkflowStepsSnapshot()
        {
            return GetWorkflowStepsSnapshotAsync().GetAwaiter().GetResult();
        }

        public System.Collections.Generic.IEnumerable<object> GetCategoriesForDropdown()
        {
            return GetCategoriesForDropdownAsync().GetAwaiter().GetResult();
        }

        public System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto> GetEquivalenceCandidatesForMissingRows()
        {
            return GetEquivalenceCandidatesForMissingRowsAsync().GetAwaiter().GetResult();
        }

        public System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> GetMissingRowsWithSuggestions()
        {
            return GetMissingRowsWithSuggestionsAsync().GetAwaiter().GetResult();
        }

        public Task<List<HecateInventaireNormalise>> GetInventaireNormaliseByNumLignes(List<int> numLignes)
        {
            return _actions.GetInventaireNormaliseByNumLignes(numLignes);
        }

        public Task<List<RWA.Web.Application.Models.Dtos.HecateInventaireNormaliseDto>> GetInvalidObligations()
        {
            return (_actions as WorkflowStateActions).GetInvalidObligations();
        }

        public Task<List<HecateInventaireNormalise>> GetItemsToAddTobdd()
        {
            return (_actions as WorkflowStateActions).GetItemsToAddTobdd();
        }

        public Task<List<RWA.Web.Application.Models.Dtos.HecateTethysDto>> GetTethysStatusAsync()
        {
            return (_actions as WorkflowStateActions).GetTethysStatusAsync();
        }

        public async Task TriggerUpdateTethysStatusAsync()
        {
            await (_actions as WorkflowStateActions).UpdateTethysStatusAsync();
        }

        public async Task<RWA.Web.Application.Models.Dtos.TethysStatusPage> GetTethysStatusPageAsync(string filter, string? cursor, int take)
        {
            var allItems = await (_actions as WorkflowStateActions).GetTethysStatusAsync();
            var filteredItems = allItems;

            if (filter == "failed")
            {
                filteredItems = allItems.Where(i => !i.IsMappingTethysSuccessful).ToList();
            }
            else if (filter == "successful")
            {
                filteredItems = allItems.Where(i => i.IsMappingTethysSuccessful).ToList();
            }

            var pageItems = filteredItems.Take(take).ToList();

            return new RWA.Web.Application.Models.Dtos.TethysStatusPage
            {
                Items = pageItems,
                NextCursor = null, // Placeholder for cursor logic
                Total = filteredItems.Count
            };
        }

        /// <summary>
        /// Dispose method to unsubscribe from state machine events.
        /// This prevents multiple subscriptions to the singleton state machine.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            
            try
            {
                // Unsubscribe from all events to prevent memory leaks and multiple subscriptions
                _stateMachine.RWACategoryManagerEntry -= _actions.OnRWACategoryManagerEntryAsync;
                _stateMachine.BDDManagerEntry -= _actions.OnBDDManagerEntryAsync;
                _stateMachine.RafManagerEntry -= _actions.OnRafManagerEntryAsync;
                _stateMachine.EnrichiExportEntry -= _actions.OnEnrichiExportEntryAsync;

                _stateMachine.RWACategoryManagerExit -= _actions.OnRWACategoryManagerExitAsync;
                _stateMachine.BDDManagerExit -= _actions.OnBDDManagerExitAsync;
                _stateMachine.RafManagerExit -= _actions.OnRafManagerExitAsync;
                _stateMachine.EnrichiExportExit -= _actions.OnEnrichiExportExitAsync;
                _stateMachine.TriggerUpload -= _actions.OnTriggerUploadAsync;
                _stateMachine.UploadPending -= _actions.OnUploadPendingAsync;
                _stateMachine.UploadSuccess -= _actions.OnUploadSuccessAsync;
                _stateMachine.UploadFailed -= _actions.OnUploadFailedAsync;
                _stateMachine.ApplyRwaMappings -= (mappings) => _actions.OnApplyRwaMappingsAsync(mappings);
                _stateMachine.ApplyEquivalenceMappings -= _actions.OnApplyEquivalenceMappingsAsync;
                _stateMachine.ValidationSuccess -= _actions.OnValidationSuccessAsync;
                _stateMachine.ValidationWarning -= _actions.OnValidationWarningAsync;
                _stateMachine.ValidationError -= _actions.OnValidationErrorAsync;
                _stateMachine.ForceNextFallback -= _actions.OnForceNextFallbackAsync;
                
                // Unsubscribe from Transitioned event
                _stateMachine.Transitioned -= async (transitionDto) => await _actions.OnTransitionedAsync(transitionDto);
                
                _logger.LogWarning("🗑️ ORCHESTRATOR DISPOSED #{InstId} - OrchId: {OrchId}, unsubscribed from all state machine events", 
                    _instanceId, GetHashCode());
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error occurred while disposing WorkflowOrchestrator");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}
