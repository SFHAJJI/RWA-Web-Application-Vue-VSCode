<template>
    <div>
        <h2>BDD Manager</h2>
        <div class="card">
            <v-container fluid v-if="!workflowStore.stepLoading['bdd-manager'] && !loadingStepStatus">
                <v-stepper v-model="currentStep" :items="['OBL Validation', 'Add to BDD']" class="elevation-0">
                    <template v-slot:item.1>
                        <div v-if="stepStatus.OBLValidationStepStatus === 'Finished'">
                            <v-alert type="success" :value="true">
                                OBL Validation step is finished.
                            </v-alert>
                        </div>
                        <div v-else>
                            <OblValidationTable />
                        </div>
                    </template>
                    <template v-slot:item.2>
        <!-- Summary strip -->
        <div class="d-flex align-center mb-2" style="gap:8px;" v-if="progress.total > 0 || summary.total > 0">
            <v-chip size="small" color="primary" variant="tonal">Processed: {{ summary.processed || progress.processed }} / {{ summary.total || progress.total }}</v-chip>
            <v-chip size="small" color="success" variant="tonal">IdUnique: {{ summary.byIdU }}</v-chip>
            <v-chip size="small" color="success" variant="tonal">IdOrigine: {{ summary.byIdO }}</v-chip>
            <v-chip size="small" color="info" variant="tonal">AddToBDD: {{ summary.addToBdd }}</v-chip>
            <v-chip size="small" color="warning" variant="tonal">NoMatch: {{ summary.noMatch }}</v-chip>
            <v-chip size="small" color="error" variant="tonal">Duplicates: {{ summary.duplicates }}</v-chip>
            <v-spacer />
            <v-btn size="small" variant="text" @click="retrigger">Re-run matching</v-btn>
        </div>

        <div class="mb-3" v-if="(progress.total ?? 0) > 0 || (summary.total ?? 0) > 0">
            <v-progress-linear :model-value="progress.processed" :max="progress.total" height="16">
                <strong class="pl-2">{{ Math.floor((progress.processed/progress.total)*100) }}%</strong>
            </v-progress-linear>
        </div>
                        <div v-if="stepStatus.AddToBDDStepStatus === 'Finished'">
                            <v-alert type="success" :value="true">
                                Add to BDD step is finished.
                            </v-alert>
                        </div>
                        <div v-else>
                            <AddToBddTable v-if="currentStep === 2" />
                            <BddMatchResults :version="progressVersion" />
                        </div>
                    </template>
                </v-stepper>
            </v-container>
            <v-container v-else>
                <v-skeleton-loader type="card"></v-skeleton-loader>
            </v-container>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useWorkflowStore } from '../../stores/workflow';
import OblValidationTable from './OblValidationTable.vue';
import AddToBddTable from './AddToBddTable.vue';
import BddMatchResults from './BddMatchResults.vue';

const workflowStore = useWorkflowStore();
const stepStatus = ref({
    OBLValidationStepStatus: '',
    AddToBDDStepStatus: '',
    Version: null as string | null
});
const currentStep = ref(1);
const loadingStepStatus = ref(true);
const progress = ref({ processed: 0, total: 0 });
const progressVersion = ref<string | null>(null);
const summary = ref({ processed: 0, total: 0, byIdU: 0, byIdO: 0, addToBdd: 0, noMatch: 0, duplicates: 0 });

const loadStepStatus = () => {
    loadingStepStatus.value = true;
    const bddManagerStep = workflowStore.workflowSteps.find(s => s.stepName === 'BDD Manager');
    if (bddManagerStep?.dataPayload) {
        try {
            const payload = JSON.parse(bddManagerStep.dataPayload);
            stepStatus.value = payload;
            if (payload.OBLValidationStepStatus === 'Finished') {
                currentStep.value = 2;
            } else {
                currentStep.value = 1;
            }
            if (payload.Version) {
                progressVersion.value = payload.Version;
                // fetch summary immediately so the bar/strip appears even if SignalR events were missed
                fetchSummary();
            }
        } catch (e) {
            console.error("Failed to parse DataPayload:", e);
        }
    }
    loadingStepStatus.value = false;
};

onMounted(() => {
    loadStepStatus();
    // Listen for BDD match progress
    const conn = new HubConnectionBuilder().withUrl('/workflowHub').build();
    conn.on('BddMatchProgress', (payload: any) => {
        progress.value = { processed: payload.processed ?? 0, total: payload.total ?? 0 };
        if (payload.version) progressVersion.value = payload.version;
        fetchSummary();
    });
    conn.start();
});

watch(() => workflowStore.workflowSteps.find(s => s.stepName === 'BDD Manager')?.dataPayload, loadStepStatus);

async function fetchSummary() {
  try {
    if (!progressVersion.value) return;
    const { data } = await (await import('axios')).default.get('/api/workflow/bddmatch/summary', { params: { version: progressVersion.value } });
    summary.value = data;
    progress.value = { processed: data.processed ?? progress.value.processed, total: data.total ?? progress.value.total };
  } catch (e) {
    // noop
  }
}

async function retrigger() {
  try {
    const { data } = await (await import('axios')).default.post('/api/workflow/bddmatch/retrigger');
    progressVersion.value = data.version;
    // reset UI counters
    summary.value = { processed: 0, total: 0, byIdU: 0, byIdO: 0, addToBdd: 0, noMatch: 0, duplicates: 0 } as any;
    progress.value = { processed: 0, total: 0 };
    // fetch summary immediately for the new version
    fetchSummary();
  } catch (e) { /* noop */ }
}
</script>

<style scoped>
.card {
    background: #ffffff;
    padding: 2rem;
    border-radius: 10px;
    margin-bottom: 1rem;
}
</style>
