using Stateless;
using RWA.Web.Application.Services.Workflow.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RWA.Web.Application.Services.Workflow
{
    /// <summary>
    /// Singleton state machine that defines workflow states and transitions.
    /// Actions are implemented via events that scoped services can subscribe to.
    /// </summary>
    public class WorkflowStateMachine
    {
        // Events for state entry/exit actions - scoped services subscribe to these
        public event Func<Task>? UploadInventoryEntry;
        public event Func<string, Task>? UploadInventoryExit;
        public event Func<Task>? RWACategoryManagerEntry;
        public event Func<string, Task>? RWACategoryManagerExit;
        public event Func<Task>? BDDManagerEntry;
        public event Func<string, Task>? BDDManagerExit;
        public event Func<Task>? RafManagerEntry;
        public event Func<string, Task>? RafManagerExit;
        public event Func<Task>? EnrichiExportEntry;
        public event Func<string, Task>? EnrichiExportExit;
        
        // Reset event - clears database and returns to initial state
        public event Func<Task>? ResetWorkflow;
        
        // Initial bootstrap event - scoped action will determine current DB state and fire appropriate Init trigger
        public event Func<Task>? InitialBootstrap;
        
        // Reset completion check - performs reset operations and returns true when finished
        public event Func<Task<bool>>? IsResetComplete;

        // Events for internal transition actions with parameters
        public event Func<List<(string FileName, byte[] Content)>, Task>? TriggerUpload;
        public event Func<AggregateUploadResultContext, Task>? UploadPending;
        public event Func<AggregateUploadResultContext, Task>? UploadSuccess;
        public event Func<AggregateUploadResultContext, Task>? UploadFailed;
        public event Func<List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>, Task>? ApplyRwaMappings;
        public event Func<List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto>, Task>? ApplyEquivalenceMappings;
        public event Func<ValidationResultContext, Task>? ValidationSuccess;
        public event Func<ValidationResultContext, Task>? ValidationWarning;
        public event Func<ValidationResultContext, Task>? ValidationError;
        public event Func<UnexpectedErrorContext, Task>? UnexpectedError;
        public event Func<ForceNextContext, Task>? ForceNextFallback;

        // Transition event
        public event Action<TransitionDto>? Transitioned;

        private readonly StateMachine<State, Trigger> _machine;
        private StateMachine<State, Trigger>.TriggerWithParameters<List<(string FileName, byte[] Content)>>? _setUploadTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>>? _applyRwaMappingsTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto>>? _applyEquivalencesTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<AggregateUploadResultContext>? _uploadPendingTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<AggregateUploadResultContext>? _uploadSuccessTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<AggregateUploadResultContext>? _uploadFailedTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<ValidationResultContext>? _validationSuccessTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<ValidationResultContext>? _validationWarningTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<ValidationResultContext>? _validationErrorTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<UnexpectedErrorContext>? _unexpectedErrorTrigger;
        private StateMachine<State, Trigger>.TriggerWithParameters<ForceNextContext>? _forceNextFallbackTrigger;

        private State _currentState = State.InitialState;

        public WorkflowStateMachine()
        {
            _machine = new StateMachine<State, Trigger>(() => _currentState, s => _currentState = s);
            ConfigureStateMachine();
        }

        private void ConfigureStateMachine()
        {
            // Configure parameterized triggers
            _setUploadTrigger = _machine.SetTriggerParameters<List<(string FileName, byte[] Content)>>(Trigger.UploadInventoryFiles);
            _applyRwaMappingsTrigger = _machine.SetTriggerParameters<List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>>(Trigger.ApplyRwaMappings);
            _applyEquivalencesTrigger = _machine.SetTriggerParameters<List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto>>(Trigger.ApplyEquivalenceMappings);
            _uploadPendingTrigger = _machine.SetTriggerParameters<AggregateUploadResultContext>(Trigger.UploadPending);
            _uploadSuccessTrigger = _machine.SetTriggerParameters<AggregateUploadResultContext>(Trigger.UploadSuccess);
            _uploadFailedTrigger = _machine.SetTriggerParameters<AggregateUploadResultContext>(Trigger.UploadFailed);
            _validationSuccessTrigger = _machine.SetTriggerParameters<ValidationResultContext>(Trigger.ValidationSuccess);
            _validationWarningTrigger = _machine.SetTriggerParameters<ValidationResultContext>(Trigger.ValidationWarning);
            _validationErrorTrigger = _machine.SetTriggerParameters<ValidationResultContext>(Trigger.ValidationError);
            _unexpectedErrorTrigger = _machine.SetTriggerParameters<UnexpectedErrorContext>(Trigger.UnexpectedError);
            _forceNextFallbackTrigger = _machine.SetTriggerParameters<ForceNextContext>(Trigger.ForceNextFallback);

            // Configure states
            _machine.Configure(State.InitialState)
                .OnActivate(() => _machine.Fire(Trigger.LetsGo))
                .Permit(Trigger.LetsGo, State.UploadInventoryFiles); // Default fallback

            _machine.Configure(State.UploadInventoryFiles)
                .OnEntryAsync(async () => await InvokeEventAsync(UploadInventoryEntry))
                .OnExitAsync(async () => await InvokeEventAsync(UploadInventoryExit, "Upload inventory"))
                .InternalTransition<List<(string FileName, byte[] Content)>>(_setUploadTrigger!, async (files, transition) =>
                    await InvokeEventAsync(TriggerUpload, files))
                .InternalTransition<AggregateUploadResultContext>(_uploadPendingTrigger!, async (context, transition) =>
                    await InvokeEventAsync(UploadPending, context))
                .InternalTransition<AggregateUploadResultContext>(_uploadSuccessTrigger!, async (context, transition) =>
                    await InvokeEventAsync(UploadSuccess, context))
                .InternalTransition<AggregateUploadResultContext>(_uploadFailedTrigger!, async (context, transition) =>
                    await InvokeEventAsync(UploadFailed, context))
                .InternalTransition<ValidationResultContext>(_validationSuccessTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationSuccess, context))
                .InternalTransition<ValidationResultContext>(_validationWarningTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationWarning, context))
                .InternalTransition<ValidationResultContext>(_validationErrorTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationError, context))
                .InternalTransition<UnexpectedErrorContext>(_unexpectedErrorTrigger!, async (context, transition) =>
                    await InvokeEventAsync(UnexpectedError, context))
                .InternalTransition(Trigger.Reset, async () => await InvokeResetCompleteAsync())
                .Permit(Trigger.NextUploadToRWACatManager, State.RWACategoryManager);

            _machine.Configure(State.RWACategoryManager)
                .OnEntryAsync(async () => await InvokeEventAsync(RWACategoryManagerEntry))
                .OnExitAsync(async () => await InvokeEventAsync(RWACategoryManagerExit, "RWA Category Manager"))
                .PermitReentry(Trigger.ReValidate)
                .InternalTransition<List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>>(_applyRwaMappingsTrigger!, async (mappings, transition) =>
                    await InvokeEventAsync(ApplyRwaMappings, mappings))
                .InternalTransition<List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto>>(_applyEquivalencesTrigger!, async (mappings, transition) =>
                    await InvokeEventAsync(ApplyEquivalenceMappings, mappings))
                .InternalTransition<ValidationResultContext>(_validationSuccessTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationSuccess, context))
                .InternalTransition<ValidationResultContext>(_validationWarningTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationWarning, context))
                .InternalTransition<ValidationResultContext>(_validationErrorTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationError, context))
                .Permit(Trigger.GoToBDDManager, State.BDDManager)
                .Permit(Trigger.GoToUploadInventoryFiles, State.UploadInventoryFiles)
                .PermitIfAsync(Trigger.Reset, State.UploadInventoryFiles, async () => await InvokeResetCompleteAsync());

            _machine.Configure(State.BDDManager)
                .OnEntryAsync(async () => await InvokeEventAsync(BDDManagerEntry))
                .OnExitAsync(async () => await InvokeEventAsync(BDDManagerExit, "BDD Manager"))
                .PermitReentry(Trigger.ReValidate)
                .InternalTransition<ValidationResultContext>(_validationSuccessTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationSuccess, context))
                .InternalTransition<ValidationResultContext>(_validationWarningTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationWarning, context))
                .InternalTransition<ValidationResultContext>(_validationErrorTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationError, context))
                .Permit(Trigger.Next, State.RafManager)
                .Permit(Trigger.Previous, State.RWACategoryManager)
                .PermitIfAsync(Trigger.Reset, State.UploadInventoryFiles, async () => await InvokeResetCompleteAsync());

            _machine.Configure(State.RafManager)
                .OnEntryAsync(async () => await InvokeEventAsync(RafManagerEntry))
                .OnExitAsync(async () => await InvokeEventAsync(RafManagerExit, "RAF Manager"))
                .PermitReentry(Trigger.ReValidate)
                .InternalTransition<ValidationResultContext>(_validationSuccessTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationSuccess, context))
                .InternalTransition<ValidationResultContext>(_validationWarningTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationWarning, context))
                .InternalTransition<ValidationResultContext>(_validationErrorTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationError, context))
                .Permit(Trigger.Next, State.EnrichiExport)
                .Permit(Trigger.Previous, State.BDDManager)
                .PermitIfAsync(Trigger.Reset, State.UploadInventoryFiles, async () => await InvokeResetCompleteAsync());

            _machine.Configure(State.EnrichiExport)
                .OnEntryAsync(async () => await InvokeEventAsync(EnrichiExportEntry))
                .OnExitAsync(async () => await InvokeEventAsync(EnrichiExportExit, "Fichier Enrichie Generation"))
                .InternalTransition<ValidationResultContext>(_validationSuccessTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationSuccess, context))
                .InternalTransition<ValidationResultContext>(_validationWarningTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationWarning, context))
                .InternalTransition<ValidationResultContext>(_validationErrorTrigger!, async (context, transition) =>
                    await InvokeEventAsync(ValidationError, context))
                .Permit(Trigger.Previous, State.RafManager)
                .PermitIfAsync(Trigger.Reset, State.UploadInventoryFiles, async () => await InvokeResetCompleteAsync());

            // Subscribe to transition events
            _machine.OnTransitioned(tr =>
            {
                var dto = new TransitionDto
                {
                    Source = tr.Source.ToString(),
                    Destination = tr.Destination.ToString(),
                    Trigger = tr.Trigger.ToString(),
                    OccurredAtUtc = DateTime.UtcNow
                };

                Transitioned?.Invoke(dto);
            });
        }

        // Helper methods to invoke events safely
        private async Task InvokeEventAsync(Func<Task>? eventHandler)
        {
            if (eventHandler != null)
                await eventHandler.Invoke();
        }

        private async Task InvokeEventAsync<T>(Func<T, Task>? eventHandler, T parameter)
        {
            if (eventHandler != null)
                await eventHandler.Invoke(parameter);
        }

        private async Task InvokeEventAsync<T1, T2>(Func<T1, T2, Task>? eventHandler, T1 param1, T2 param2)
        {
            if (eventHandler != null)
                await eventHandler.Invoke(param1, param2);
        }

        // Helper method to invoke reset completion check
        private async Task<bool> InvokeResetCompleteAsync()
        {
            if (IsResetComplete != null)
                return await IsResetComplete.Invoke();
            return true; // Default to true if no handler
        }

        // Public interface for firing triggers
        public async Task FireAsync(Trigger trigger)
        {
            await _machine.FireAsync(trigger);
        }

        public async Task FireAsync<T>(StateMachine<State, Trigger>.TriggerWithParameters<T> trigger, T parameter)
        {
            await _machine.FireAsync(trigger, parameter);
        }

        public async Task FireAsync<T1, T2>(StateMachine<State, Trigger>.TriggerWithParameters<T1, T2> trigger, T1 param1, T2 param2)
        {
            await _machine.FireAsync(trigger, param1, param2);
        }

        /// <summary>
        /// Gets the count of handlers subscribed to critical events for debugging
        /// </summary>
        public Dictionary<string, int> GetEventHandlerCounts()
        {
            return new Dictionary<string, int>
            {
                { "TriggerUpload", TriggerUpload?.GetInvocationList()?.Length ?? 0 },
                { "UploadPending", UploadPending?.GetInvocationList()?.Length ?? 0 },
                { "UploadSuccess", UploadSuccess?.GetInvocationList()?.Length ?? 0 },
                { "ValidationSuccess", ValidationSuccess?.GetInvocationList()?.Length ?? 0 },
                { "InitialBootstrap", InitialBootstrap?.GetInvocationList()?.Length ?? 0 }
            };
        }

        // Public properties for accessing triggers
        public StateMachine<State, Trigger>.TriggerWithParameters<List<(string FileName, byte[] Content)>>? SetUploadTrigger => _setUploadTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>>? ApplyRwaMappingsTrigger => _applyRwaMappingsTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto>>? ApplyEquivalencesTrigger => _applyEquivalencesTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<AggregateUploadResultContext>? UploadPendingTrigger => _uploadPendingTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<AggregateUploadResultContext>? UploadSuccessTrigger => _uploadSuccessTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<AggregateUploadResultContext>? UploadFailedTrigger => _uploadFailedTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<ValidationResultContext>? ValidationSuccessTrigger => _validationSuccessTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<ValidationResultContext>? ValidationWarningTrigger => _validationWarningTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<ValidationResultContext>? ValidationErrorTrigger => _validationErrorTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<UnexpectedErrorContext>? UnexpectedErrorTrigger => _unexpectedErrorTrigger;
        public StateMachine<State, Trigger>.TriggerWithParameters<ForceNextContext>? ForceNextFallbackTrigger => _forceNextFallbackTrigger;

        public State CurrentState => _machine.State;
    }
}
