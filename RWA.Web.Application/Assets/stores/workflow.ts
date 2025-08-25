import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { useToastStore } from './toast';
import axios from 'axios';
import { postJson } from '../composables/useApi';
import { useLoadingStore } from './loading';
import { HubConnectionBuilder } from '@microsoft/signalr';

export const useWorkflowStore = defineStore('workflow', () => {
    interface WorkflowStep {
        id: number;
        stepName: string;
        status: string;
        dataPayload?: string | null;
    }

    const workflowSteps = ref<WorkflowStep[]>([]);
    
    // Computed property to get current validation messages from current step's DataPayload
    const currentValidationMessages = computed(() => {
        const currentStep = workflowSteps.value.find(s => s.status && s.status.startsWith('Current'));
        if (!currentStep || !currentStep.dataPayload) {
            return [];
        }
        
        // Parse any non-empty DataPayload - don't restrict to only error/warning statuses
        try {
            const parsedPayload = JSON.parse(currentStep.dataPayload);
            return Array.isArray(parsedPayload) ? parsedPayload : [];
        } catch (ex) {
            console.warn(`[workflow.ts] Failed to parse DataPayload as validation messages:`, ex);
            return [];
        }
    });
    
    // client-side active step index used for visual navigation only
    const uiActiveIndex = ref<number | null>(null);
    // expose the reactive refs for components so they can call array methods (findIndex, etc.) directly
    // (return the original refs instead of computed wrappers so methods like .findIndex are available)
    const workflowStepsArray = workflowSteps; // used to return the ref
    const currentValidationMessagesArray = currentValidationMessages; // used to return the ref
    const connection = ref(null);
    const loading = ref(false);
    // per-step loading flags; key is a stable step identifier (component can pick its own key)
    const stepLoading = ref<Record<string, boolean>>({});
    const error = ref(null);
    const resetCounter = ref(0);
    // Centralized set of statuses that indicate a step finished and may be advanced
    const advanceStatuses = ref<string[]>(['SuccessfulFinish', 'FinishedWithWarning']);
    // Centralized set of statuses that indicate a step is in error
    const errorStatuses = ref<string[]>(['CurrentWithError']);
    // Centralized set of statuses that indicate a step has warnings
    const warningStatuses = ref<string[]>(['CurrentWithWarning']);

    function applyServerSteps(steps: WorkflowStep[]) {
        workflowSteps.value = steps;
        // reset client active index to server current step
        const cur = steps.findIndex(s => s.status && s.status.startsWith('Current'));
        uiActiveIndex.value = cur >= 0 ? cur : 0;
    }

    function getCurrentIndex(): number {
        return workflowSteps.value.findIndex(s => s.status && s.status.startsWith('Current'));
    }

    // Request to change UI active index. This enforces that the client cannot
    // navigate beyond the server-authoritative current step (no peeking into future steps).
    function requestUiActiveIndex(index: number | null | undefined) {
        if (index == null) return;
        const cur = getCurrentIndex();
        const max = cur >= 0 ? cur : Math.max(0, workflowSteps.value.length - 1);
        let idx = Math.floor(Number(index) || 0);
        if (idx < 0) idx = 0;
        if (idx > max) idx = max;
        uiActiveIndex.value = idx;
    }

    let lastFetchTime = 0;
    const FETCH_THROTTLE_MS = 2000; // Minimum 2 seconds between fetches

    async function fetchWorkflowStatus() {
        const now = Date.now();
        if (now - lastFetchTime < FETCH_THROTTLE_MS) {
            console.log('fetchWorkflowStatus throttled');
            return; // Skip if too recent
        }
        lastFetchTime = now;
        
        loading.value = true;
        error.value = null;
        try {
            const response = await fetch('/api/workflow/status');
            if (!response.ok) {
                throw new Error('Failed to fetch workflow status');
            }
            const steps = await response.json();
            applyServerSteps(Array.isArray(steps) ? steps : []);
        } catch (e: unknown) {
            if (e instanceof Error) {
                error.value = { errorMessage: e.message };
            } else {
                error.value = { errorMessage: 'An unknown error occurred' };
            }
        } finally {
            loading.value = false;
        }
    }

    async function revalidateCurrent() {
        loading.value = true;
        error.value = null;
        try {
            // POST the revalidate request and rely on SignalR to deliver the authoritative snapshot.
            await postJson('/api/workflow/revalidate', {});
            // Do not apply POST-returned DTOs here; SignalR 'ReceiveWorkflowUpdate' will update state.
        } catch (e: unknown) {
            if (e instanceof Error) {
                error.value = { errorMessage: e.message };
            } else {
                error.value = { errorMessage: 'An unknown error occurred' };
            }
        } finally {
            loading.value = false;
        }
    }

    async function forceNext() {
        loading.value = true;
        error.value = null;
        try {
            await postJson('/api/workflow/force-next', {});
            // state will be updated via SignalR
        } catch (e: unknown) {
            if (e instanceof Error) {
                error.value = { errorMessage: e.message };
            } else {
                error.value = { errorMessage: 'An unknown error occurred' };
            }
        } finally {
            loading.value = false;
        }
    }

    async function triggerTransition(trigger: string, payload: any = {}) {
        loading.value = true;
        error.value = null;
        try {
            await postJson(`/api/workflow/trigger/${trigger}`, payload);
            // rely on SignalR update
        } catch (e: unknown) {
            if (e instanceof Error) {
                error.value = { errorMessage: e.message };
            } else {
                error.value = { errorMessage: 'An unknown error occurred' };
            }
        } finally {
            loading.value = false;
        }
    }

    async function updateBdd(items: any[]) {
        loading.value = true;
        error.value = null;
        try {
            await postJson('/api/workflow/update-bdd', items);
            // rely on SignalR update
        } catch (e: unknown) {
            if (e instanceof Error) {
                error.value = { errorMessage: e.message };
            } else {
                error.value = { errorMessage: 'An unknown error occurred' };
            }
        } finally {
            loading.value = false;
        }
    }

    async function updateObligations(items: any[]) {
        loading.value = true;
        error.value = null;
        try {
            await postJson('/api/workflow/update-obligations', items);
            // rely on SignalR update
        } catch (e: unknown) {
            if (e instanceof Error) {
                error.value = { errorMessage: e.message };
            } else {
                error.value = { errorMessage: 'An unknown error occurred' };
            }
        } finally {
            loading.value = false;
        }
    }

    async function resetWorkflow() {
        loading.value = true;
        error.value = null;
        try {
            await postJson('/api/workflow/reset', {});
            // allow SignalR to provide updated steps; keep reset counter to force UI refresh when needed
            resetCounter.value++;
            stepLoading.value = {}; // Clear step loading state on reset
            // currentValidationMessages is computed from workflowSteps, so no need to clear manually
        } catch (e: unknown) {
            if (e instanceof Error) {
                error.value = { errorMessage: e.message };
            } else {
                error.value = { errorMessage: 'An unknown error occurred' };
            }
        } finally {
            loading.value = false;
        }
    }

    function setStepLoading(key: string, value: boolean) {
        if (!key) return;
        const s = { ...stepLoading.value };
        s[key] = value;
        stepLoading.value = s;
    }

    async function uploadFiles(formData: FormData) {
        // Toggle both local loading state and global posting spinner so App.vue shows progress
        loading.value = true;
        error.value = null;
        const globalLoading = useLoadingStore();
        globalLoading.isPosting = true;
        try {
            // Let the browser set the Content-Type (including the boundary) for multipart/form-data
            await axios.post('/api/workflow/upload', formData);
            // Do not apply returned DTOs here; authoritative snapshot arrives via SignalR.
        } catch (e: any) {
            const t = useToastStore();
            if (e.response && e.response.data) {
                error.value = e.response.data;
                // show toast with server-provided message when available
                try { t.pushToast({ type: 'error', message: typeof e.response.data === 'string' ? e.response.data : (e.response.data?.message || JSON.stringify(e.response.data)) }); } catch { }
                // Don't fetch status directly - let SignalR handle updates or trigger a SignalR refresh
                // const statusResponse = await fetch('/api/workflow/status');
                // workflowSteps.value = await statusResponse.json();
            } else {
                error.value = { errorMessage: 'An unknown error occurred.' };
                try { t.pushToast({ type: 'error', message: 'Upload failed: unknown error' }); } catch { }
            }
        } finally {
            loading.value = false;
            globalLoading.isPosting = false;
        }
    }

    // Generic helper: determines whether the step at the given index is considered finished/advancable
    function isStepAdvancable(index: number) {
        if (index < 0) return false;
        const step = workflowSteps.value[index];
        if (!step || !step.status) return false;
        return advanceStatuses.value.includes(step.status);
    }

    // Generic helper: determines whether the step at the given index is considered complete (finished)
    function isStepComplete(index: number) {
        if (index < 0) return false;
        const step = workflowSteps.value[index];
        if (!step || !step.status) return false;
        return advanceStatuses.value.includes(step.status);
    }

    // Generic helper: determines whether the step at the given index is in an error state
    function isStepError(index: number) {
        if (index < 0) return false;
        const step = workflowSteps.value[index];
        if (!step || !step.status) return false;
        // Check explicit configured error statuses first, then fallback to a substring match
        if (errorStatuses.value.includes(step.status)) return true;
        return step.status.toLowerCase().includes('error');
    }

    // Generic helper: determines whether the step at the given index is in a warning state
    function isStepWarning(index: number) {
        if (index < 0) return false;
        const step = workflowSteps.value[index];
        if (!step || !step.status) return false;
        // Check explicit configured warning statuses first, then fallback to a substring match
        if (warningStatuses.value.includes(step.status)) return true;
        return step.status.toLowerCase().includes('warning');
    }

    // Generic helper: determines whether the step should be displayed as having issues (error or warning)
    function isStepWithIssues(index: number) {
        return isStepError(index) || isStepWarning(index);
    }

    function initSignalR() {
        connection.value = new HubConnectionBuilder()
            .withUrl("/workflowHub")
            .withAutomaticReconnect()
            .build();

            connection.value.on("ReceiveWorkflowUpdate", (steps) => {
                console.log('Received workflow update from SignalR:', JSON.stringify(steps, null, 2));
                console.log('[workflow.ts] Received workflow update:', steps);
                console.log('[workflow.ts] Before update - Current step:', workflowSteps.value.find(s => s.status?.startsWith('Current')));
                applyServerSteps(Array.isArray(steps) ? steps : []);
                console.log('[workflow.ts] After update - Current step:', workflowSteps.value.find(s => s.status?.startsWith('Current')));
                console.log('[workflow.ts] Validation messages count:', currentValidationMessages.value.length);
            });

            // typed transition messages
        connection.value.on("ReceiveTransition", (transition) => {
            // forward as a window event so components can react
            window.dispatchEvent(new CustomEvent('workflow-transition', { detail: transition }));
            });

        connection.value.on("ReceiveToast", (toast) => {
            // use the toast store so toasts are queued and displayed globally
            try {
                const tstore = useToastStore();
                tstore.pushToast(toast || {});
            } catch (e) {
                // fallback to event if store not available
                window.dispatchEvent(new CustomEvent('workflow-toast', { detail: toast }));
            }
        });

        connection.value.start().catch(err => console.error(err));
        // initial load
        fetchWorkflowStatus();

        // Load server-provided workflow status mapping (advance/error status arrays)
        fetch('/api/workflow/config')
            .then(r => r.ok ? r.json() : null)
            .then(cfg => {
                if (cfg) {
                    if (Array.isArray(cfg.AdvanceStatuses)) advanceStatuses.value = cfg.AdvanceStatuses;
                    if (Array.isArray(cfg.ErrorStatuses)) errorStatuses.value = cfg.ErrorStatuses;
                }
            })
            .catch(() => { /* ignore config fetch failures; keep defaults */ });
    }

    function navigateTo(index: number) {
    requestUiActiveIndex(index);
    }

    function navigateNextVisual() {
    const idx = uiActiveIndex.value ?? getCurrentIndex();
    requestUiActiveIndex((idx ?? 0) + 1);
    }

    function navigatePrevVisual() {
    const idx = uiActiveIndex.value ?? getCurrentIndex();
    requestUiActiveIndex((idx ?? 0) - 1);
    }

    // Check if user can navigate to the next step (all previous steps must be completed)
    function canNavigateNext() {
        const currentIndex = uiActiveIndex.value ?? getCurrentIndex();
        if (currentIndex === null || currentIndex === undefined) return false;
        
        // Can't go beyond the last step
        if (currentIndex >= workflowSteps.value.length - 1) return false;
        
        // Check if all steps up to current are completed/advancable
        for (let i = 0; i <= currentIndex; i++) {
            if (!isStepAdvancable(i)) return false;
        }
        
        return true;
    }

    return {
        // return refs (Pinia will proxy/unref them for consumers) so code can call array helpers in script
        workflowSteps: workflowStepsArray,
        loading,
        error,
    currentValidationMessages: currentValidationMessagesArray,
    advanceStatuses,
    errorStatuses,
    stepLoading,
    setStepLoading,
        fetchWorkflowStatus,
        triggerTransition,
        resetWorkflow,
        uploadFiles,
    isStepAdvancable,
    isStepComplete,
    isStepError,
    isStepWarning,
    isStepWithIssues,
        initSignalR,
    revalidateCurrent,
    forceNext,
    updateBdd,
    updateObligations,
    // UI navigation helpers (visual only) - do not mutate server state
    uiActiveIndex,
    navigateTo,
    navigateNextVisual,
    navigatePrevVisual,
    requestUiActiveIndex,
    canNavigateNext,
    };
});
