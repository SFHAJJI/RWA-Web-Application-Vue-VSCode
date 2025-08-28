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
                            <ServerSideDataTable :headers="oblValidatorHeaders" data-url="/api/workflow/obl-validation-data"
                                :columns="oblValidatorColumns" @update:items="handleOblItemsUpdate">
                                <template v-slot:item.DateMaturite="{ item }">
                                    <div v-if="!item.isEditingDateMaturite" @click="item.isEditingDateMaturite = true" class="editable-cell">
                                        {{ formatDate(item.DateMaturite) }}
                                    </div>
                                    <v-menu v-else v-model="item.isEditingDateMaturite" :close-on-content-click="false">
                                        <template v-slot:activator="{ props }">
                                            <v-text-field :model-value="formatDate(item.DateMaturite)" label="Date Maturite"
                                                :rules="[rules.required]" v-bind="props" readonly variant="outlined" density="compact" hide-details>
                                                <template v-slot:append-inner>
                                                    <v-tooltip location="top" v-if="!item.DateMaturite">
                                                        <template v-slot:activator="{ props: tooltipProps }">
                                                            <v-icon v-bind="tooltipProps" color="warning" size="x-small">mdi-alert-circle</v-icon>
                                                        </template>
                                                        <span>This field is mandatory.</span>
                                                    </v-tooltip>
                                                </template>
                                            </v-text-field>
                                        </template>
                                        <v-date-picker v-model="item.DateMaturite" @update:model-value="item.isEditingDateMaturite = false" show-adjacent-months elevation="2"></v-date-picker>
                                    </v-menu>
                                </template>
                                <template v-slot:item.TauxObligation="{ item }">
                                    <div v-if="!item.isEditingTauxObligation" @click="item.isEditingTauxObligation = true" class="editable-cell">
                                        {{ item.TauxObligation }}
                                    </div>
                                    <v-text-field v-else v-model="item.TauxObligation" label="Taux Obligation"
                                        :rules="[rules.required, rules.decimal]" variant="outlined" density="compact" hide-details
                                        type="text" @blur="item.isEditingTauxObligation = false"
                                        @keydown.enter="item.isEditingTauxObligation = false"
                                        @input="(event) => sanitizeDecimalInput(item, event.target.value)">
                                        <template v-slot:append-inner>
                                            <v-tooltip location="top">
                                                <template v-slot:activator="{ props: tooltipProps }">
                                                    <v-icon v-bind="tooltipProps" color="warning" size="x-small">mdi-alert-circle</v-icon>
                                                </template>
                                                <span>This field is mandatory and must be a decimal.</span>
                                            </v-tooltip>
                                        </template>
                                    </v-text-field>
                                </template>
                            </ServerSideDataTable>
                            <div class="d-flex justify-center mt-4">
                                <v-btn color="primary" @click="submitObligations" :disabled="modifiedObligations.length === 0">Submit Modified Obligations</v-btn>
                            </div>
                        </div>
                    </template>
                    <template v-slot:item.2>
                        <div v-if="stepStatus.AddToBDDStepStatus === 'Finished'">
                            <v-alert type="success" :value="true">
                                Add to BDD step is finished.
                            </v-alert>
                        </div>
                        <div v-else>
                            <ServerSideDataTable :headers="addToBDDHeaders" data-url="/api/workflow/add-to-bdd-data"
                                :columns="addToBDDColumns" @update:items="handleBddItemsUpdate" />
                            <div class="d-flex justify-center mt-4">
                                <v-btn color="primary" @click="submitBddItems">Add to BDD</v-btn>
                            </div>
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
import { ref, onMounted, watch, computed } from 'vue';
import { useWorkflowStore } from '../../stores/workflow';
import ProgressiveLoader from '../loaders/ProgressiveLoader.vue';
import ServerSideDataTable from './../utils/ServerSideDataTable.vue';
import api from '../../api';
import { HecateInventaireNormaliseDto, HecateInterneHistoriqueDto } from '../../types/bddManager';

const workflowStore = useWorkflowStore();
const stepStatus = ref({
    OBLValidationStepStatus: '',
    AddToBDDStepStatus: ''
});
const currentStep = ref(1);
const loadingStepStatus = ref(true);

const oblValidatorColumns = ref<{ field: string, header: string }[]>([]);
const addToBDDColumns = ref<{ field: string, header: string }[]>([]);
const modifiedObligations = ref<HecateInventaireNormaliseDto[]>([]);
const bddItems = ref<HecateInterneHistoriqueDto[]>([]);

const oblValidatorHeaders = computed(() => oblValidatorColumns.value.map(c => ({ title: c.header, value: c.field })));
const addToBDDHeaders = computed(() => addToBDDColumns.value.map(c => ({ title: c.header, value: c.field })));

const rules = {
    required: value => !!value || 'Required.',
    decimal: value => /^\d*\.?\d*$/.test(value) || 'Invalid decimal',
};

const formatDate = (date) => {
    if (!date) return '';
    const d = new Date(date);
    if (isNaN(d.getTime())) return '';
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    return `${day}/${month}/${year}`;
};

const sanitizeDecimalInput = (item, value) => {
    if (value === null || typeof value === 'undefined') {
        item.TauxObligation = null;
        return;
    }
    let stringValue = String(value);
    let sanitized = stringValue.replace(/[^0-9.]/g, '');
    const firstDot = sanitized.indexOf('.');
    if (firstDot !== -1) {
        sanitized = sanitized.slice(0, firstDot + 1) + sanitized.slice(firstDot + 1).replace(/\./g, '');
    }
    if (item.TauxObligation !== sanitized) {
        item.TauxObligation = sanitized;
    }
};

const handleOblItemsUpdate = (items) => {
    modifiedObligations.value = items;
};

const handleBddItemsUpdate = (items) => {
    bddItems.value = items;
};

const submitObligations = async () => {
    await workflowStore.updateObligations(modifiedObligations.value);
};

const submitBddItems = async () => {
    await workflowStore.updateBdd(bddItems.value);
};

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

const fetchColumns = async () => {
    try {
        const oblResponse = await api.get('/api/workflow/obl-validation-columns');
        oblValidatorColumns.value = oblResponse.data;

        const bddResponse = await api.get('/api/workflow/add-to-bdd-columns');
        addToBDDColumns.value = bddResponse.data;
    } catch (error) {
        console.error('Failed to fetch column definitions:', error);
    }
};

const fetchData = async () => {
    try {
        const oblDataResponse = await api.post('/api/workflow/obl-validation-data', {});
        console.log('OBL Validation Data:', oblDataResponse.data);

        const bddDataResponse = await api.post('/api/workflow/add-to-bdd-data', {});
        console.log('Add to BDD Data:', bddDataResponse.data);
    } catch (error) {
        console.error('Failed to fetch data:', error);
    }
};

onMounted(() => {
    loadStepStatus();
    fetchColumns();
    fetchData();
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
