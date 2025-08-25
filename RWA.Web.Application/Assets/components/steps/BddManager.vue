<template>
    <div>
        <h2>BDD Manager</h2>
        <div class="card">
            <v-container fluid v-if="!workflowStore.loading">
                <!-- OBL Validator Section -->
                <div v-if="oblValidatorPayload.length > 0" class="mb-4">
                    <v-card>
                        <v-card-title class="data-table-title error-title">
                            <v-icon color="white" class="mr-2">mdi-alert-circle</v-icon>
                            OBL Validator: Missing Information
                        </v-card-title>
                        <v-card-subtitle>The following items require Date Maturite or Taux Obligation.</v-card-subtitle>
                        <v-data-table :headers="oblValidatorHeaders" :items="oblValidatorPayload"
                            class="elevation-1 modern-table">
                            <template v-slot:item.DateMaturite="{ item }">
                                <div v-if="!item.isEditingDateMaturite" @click="item.isEditingDateMaturite = true" class="editable-cell">
                                    {{ formatDate(item.DateMaturite) }}
                                </div>
                                <v-menu v-else v-model="item.isEditingDateMaturite" :close-on-content-click="false">
                                    <template v-slot:activator="{ props }">
                                        <v-text-field :model-value="formatDate(item.DateMaturite)" label="Date Maturite"
                                            :rules="[rules.required]" v-bind="props" readonly variant="outlined" density="compact" hide-details>
                                            <template v-slot:append-inner>
                                                <v-tooltip location="top">
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
                        </v-data-table>
                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="primary" @click="updateObligations" :disabled="isUpdateDisabled">Update Obligations</v-btn>
                        </v-card-actions>
                    </v-card>
                </div>

                <!-- Add to BDD Section -->
                <div v-if="addToBDDPayload.length > 0">
                    <v-card>
                        <v-card-title class="data-table-title success-title">
                             <v-icon color="white" class="mr-2">mdi-database-plus</v-icon>
                            To Be Added to BDD
                        </v-card-title>
                         <v-card-subtitle>These items are ready to be added to the BDD.</v-card-subtitle>
                        <v-data-table :headers="addToBDDHeaders" :items="addToBDDPayload"
                            class="elevation-1 modern-table" readonly></v-data-table>
                        <v-card-actions>
                            <v-spacer></v-spacer>
                            <v-btn color="primary" @click="triggerUpdate">Add to BDD</v-btn>
                        </v-card-actions>
                    </v-card>
                </div>
            </v-container>
            <v-container v-else>
                <SkeletonLoader />
            </v-container>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import { useWorkflowStore } from '../../stores/workflow';
import SkeletonLoader from '../loaders/SkeletonLoader.vue';
import ProgressiveLoader from '../loaders/ProgressiveLoader.vue';

const props = defineProps({
    step: {
        type: Object,
        required: true
    }
});

const workflowStore = useWorkflowStore();
const oblValidatorPayload = ref([]);
const addToBDDPayload = ref([]);

const rules = {
    required: value => !!value || 'Required.',
    decimal: value => /^\d*\.?\d*$/.test(value) || 'Invalid decimal',
};

const oblValidatorHeaders = [
    { title: 'Num Ligne', value: 'NumLigne' },
    { title: 'Identifiant', value: 'Identifiant' },
    { title: 'Nom', value: 'Nom' },
    { title: 'Date Maturite', value: 'DateMaturite' },
    { title: 'Taux Obligation', value: 'TauxObligation' },
    { title: 'Periode Cloture', value: 'PeriodeCloture' },
    { title: 'Source', value: 'Source' },
    { title: 'Valeur De Marche', value: 'ValeurDeMarche' },
    { title: 'Categorie 1', value: 'Categorie1' },
    { title: 'Categorie 2', value: 'Categorie2' },
    { title: 'Devise De Cotation', value: 'DeviseDeCotation' },
    { title: 'Date Expiration', value: 'DateExpiration' },
    { title: 'Tiers', value: 'Tiers' },
    { title: 'Raf', value: 'Raf' },
    { title: 'BOA SJ', value: 'BoaSj' },
    { title: 'BOA Contrepartie', value: 'BoaContrepartie' },
    { title: 'BOA Defaut', value: 'BoaDefaut' },
    { title: 'Identifiant Origine', value: 'IdentifiantOrigine' },
    { title: 'Ref Categorie Rwa', value: 'RefCategorieRwa' },
    { title: 'Identifiant Unique Retenu', value: 'IdentifiantUniqueRetenu' },
    { title: 'Raf Enrichi', value: 'Rafenrichi' },
    { title: 'Libelle Origine', value: 'LibelleOrigine' },
    { title: 'Date Fin Contrat', value: 'DateFinContrat' },
    { title: 'Commentaires', value: 'Commentaires' },
    { title: 'Bloomberg', value: 'Bloomberg' },
];

const addToBDDHeaders = [
    { title: 'Source', value: 'Source' },
    { title: 'Identifiant Origine', value: 'IdentifiantOrigine' },
    { title: 'Ref Categorie Rwa', value: 'RefCategorieRwa' },
    { title: 'Identifiant Unique Retenu', value: 'IdentifiantUniqueRetenu' },
    { title: 'Raf', value: 'Raf' },
    { title: 'Libelle Origine', value: 'LibelleOrigine' },
    { title: 'Date Echeance', value: 'DateEcheance' },
];


const loadDataFromStore = () => {
    const bddManagerStep = workflowStore.workflowSteps.find(s => s.stepName === 'BDD Manager');
    console.log("BDD Manager Step:", bddManagerStep);
    if (bddManagerStep?.dataPayload) {
        try {
            const payload = JSON.parse(bddManagerStep.dataPayload);
            console.log("Parsed Payload:", payload);
            const managerPayload = payload.BDDManagerPayload;

            if (managerPayload) {
                console.log("OBLValidatorPayload from server:", managerPayload.OBLValidatorPayload);
                oblValidatorPayload.value = managerPayload.OBLValidatorPayload.map(item => ({
                    ...item,
                    DateMaturite: item.DateMaturite ? new Date(item.DateMaturite) : null,
                    TauxObligation: item.TauxObligation !== null && item.TauxObligation !== undefined ? item.TauxObligation : null,
                    isEditingDateMaturite: !item.DateMaturite,
                    isEditingTauxObligation: item.TauxObligation === null || item.TauxObligation === undefined,
                })) || [];
                console.log("Processed OBLValidatorPayload:", oblValidatorPayload.value);
                addToBDDPayload.value = managerPayload.AddToBDDPayload || [];
            } else {
                 oblValidatorPayload.value = [];
                 addToBDDPayload.value = [];
            }
        } catch (e) {
            console.error("Failed to parse DataPayload:", e);
            oblValidatorPayload.value = [];
            addToBDDPayload.value = [];
        }
    } else {
        oblValidatorPayload.value = [];
        addToBDDPayload.value = [];
    }
};

onMounted(loadDataFromStore);

watch(() => workflowStore.workflowSteps.find(s => s.stepName === 'BDD Manager')?.dataPayload, loadDataFromStore);

const isUpdateDisabled = computed(() => {
    return oblValidatorPayload.value.some(item => {
        const tauxObligation = item.TauxObligation;
        const isTauxInvalid = tauxObligation === null ||
            tauxObligation === undefined ||
            String(tauxObligation).trim() === '';
        return !item.DateMaturite || isTauxInvalid;
    });
});

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

const triggerUpdate = async () => {
    workflowStore.setStepLoading('bdd-manager', true);

    const itemsToUpdate = addToBDDPayload.value.map(item => {
        return {
            Source: item.Source,
            IdentifiantOrigine: item.IdentifiantOrigine,
            RefCategorieRwa: item.RefCategorieRwa,
            IdentifiantUniqueRetenu: item.IdentifiantUniqueRetenu,
            Raf: item.Raf,
            LibelleOrigine: item.LibelleOrigine,
            DateEcheance: item.DateEcheance
        };
    });

    await workflowStore.updateBdd(itemsToUpdate);
    workflowStore.setStepLoading('bdd-manager', false);
};

const updateObligations = async () => {
    workflowStore.setStepLoading('bdd-manager', true);

    const itemsToUpdate = oblValidatorPayload.value.map(item => {
        // Helper to format Date objects to 'YYYY-MM-DD'
        const formatDateForServer = (date) => {
            if (!date) return null;
            const d = new Date(date);
            if (isNaN(d.getTime())) return null;
            const year = d.getFullYear();
            const month = String(d.getMonth() + 1).padStart(2, '0');
            const day = String(d.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        };

        let tauxObligation = null;
        if (item.TauxObligation !== null && item.TauxObligation !== undefined && String(item.TauxObligation).trim() !== '') {
            const parsedTaux = parseFloat(String(item.TauxObligation).replace(',', '.'));
            if (!isNaN(parsedTaux)) {
                tauxObligation = parsedTaux;
            }
        }

        return {
            NumLigne: item.NumLigne,
            DateMaturite: formatDateForServer(item.DateMaturite),
            TauxObligation: tauxObligation
        };
    });

    console.log('Payload to be sent:', JSON.stringify(itemsToUpdate, null, 2));
    await workflowStore.updateObligations(itemsToUpdate);
    workflowStore.setStepLoading('bdd-manager', false);
};
</script>

<style scoped>
.card {
    background: #ffffff;
    padding: 2rem;
    border-radius: 10px;
    margin-bottom: 1rem;
}

.data-table-title {
    font-weight: 500;
    color: white;
    display: flex;
    align-items: center;
}

.error-title {
    background-color: #d32f2f; /* Vuetify Error Color */
}

.success-title {
    background-color: #388e3c; /* Vuetify Success Color */
}

.modern-table {
    border-radius: 8px;
    overflow: hidden;
}

.modern-table :deep(.v-data-table-header) {
    background-color: #f4f6f8;
}

.modern-table :deep(th) {
    color: #37474f !important;
    font-weight: 600 !important;
    text-transform: uppercase;
    font-size: 0.75rem;
}

.modern-table :deep(tbody tr:hover) {
    background-color: #e3f2fd !important;
}

.editable-cell {
    cursor: pointer;
    min-height: 40px; /* Align with text field height */
    display: flex;
    align-items: center;
    padding: 0 8px;
}

.editable-cell:hover {
    background-color: #f0f0f0;
}
</style>
