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
                        <div v-if="stepStatus.AddToBDDStepStatus === 'Finished'">
                            <v-alert type="success" :value="true">
                                Add to BDD step is finished.
                            </v-alert>
                        </div>
                        <div v-else>
                            <AddToBddTable v-if="currentStep === 2" />
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
import { useWorkflowStore } from '../../stores/workflow';
import OblValidationTable from './OblValidationTable.vue';
import AddToBddTable from './AddToBddTable.vue';

const workflowStore = useWorkflowStore();
const stepStatus = ref({
    OBLValidationStepStatus: '',
    AddToBDDStepStatus: ''
});
const currentStep = ref(1);
const loadingStepStatus = ref(true);

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
        } catch (e) {
            console.error("Failed to parse DataPayload:", e);
        }
    }
    loadingStepStatus.value = false;
};

onMounted(() => {
    loadStepStatus();
});

watch(() => workflowStore.workflowSteps.find(s => s.stepName === 'BDD Manager')?.dataPayload, loadStepStatus);
</script>

<style scoped>
.card {
    background: #ffffff;
    padding: 2rem;
    border-radius: 10px;
    margin-bottom: 1rem;
}
</style>
