<template>
    <v-container>
        <v-card>
            <v-card-text>
                <!-- Success alert when step is completed -->
                <v-alert v-if="isRwaCategorySuccessful" type="success" prominent border="start" class="mb-6">
                    <v-row align="center">
                        <v-col class="grow">
                            This step is successfully finished
                        </v-col>
                    </v-row>
                </v-alert>

                <div v-else>
                    <v-form ref="form" v-model="formValid">
                        <SkeletonLoader v-if="workflowStore.loading" />
                        <div v-else-if="rows.length === 0" class="text-center py-8">
                            <v-icon size="48" color="grey" class="mb-4">mdi-table-search</v-icon>
                            <h3 class="text-h6 grey--text mb-2">Waiting for category mapping data...</h3>
                            <p class="text-body-2 grey--text">The system is processing category mappings via SignalR.
                            </p>
                            <v-progress-circular indeterminate color="primary" class="mt-4"></v-progress-circular>
                        </div>
                        <div v-else>
                            <!-- Search field -->
                            <v-row class="mb-4">
                                <v-col cols="12" md="6">
                                    <v-text-field id="rwa-category-search" v-model="search"
                                        prepend-inner-icon="mdi-magnify" label="Search by Source, Cat1, or Cat2..."
                                        outlined dense clearable hide-details placeholder="Filter rows"></v-text-field>
                                </v-col>
                                <v-spacer></v-spacer>
                                <v-col cols="auto" class="d-flex align-center">
                                    <span class="text-caption text--secondary">
                                        {{ filteredRows.length }} / {{ rows.length }} groups
                                    </span>
                                </v-col>
                            </v-row>

                            <v-data-table id="rwa-category-table" :items="filteredRows" :headers="headers"
                                item-value="id" class="elevation-1 rwa-data-table" role="table" :items-per-page="10"
                                :footer-props="{ 'items-per-page-options': [5, 10, 20, -1] }" :item-class="getRowClass"
                                :loading="workflowStore.loading" loading-text="Loading category mappings..."
                                @update:expanded="expandRow" show-expand>
                                <template #header.Source="{ column }">
                                    <th style="width: 15%;">{{ column.title }}</th>
                                </template>
                                <template #header.Cat1="{ column }">
                                    <th style="width: 15%;">{{ column.title }}</th>
                                </template>
                                <template #header.Cat2="{ column }">
                                    <th style="width: 15%;">{{ column.title }}</th>
                                </template>
                                <template #header.categorieRwa="{ column }">
                                    <th style="width: 25%;">{{ column.title }}</th>
                                </template>
                                <template #header.typeBloomberg="{ column }">
                                    <th style="width: 30%;">{{ column.title }}</th>
                                </template>

                                <!-- Display Source column -->
                                <template #item.Source="{ item }">
                                    <span>{{ item.Source }}</span>
                                </template>

                                <!-- Display Cat1 column -->
                                <template #item.Cat1="{ item }">
                                    <span class="text-body-2">{{ item.Cat1 }}</span>
                                </template>

                                <!-- Display Cat2 column -->
                                <template #item.Cat2="{ item }">
                                    <span class="text-body-2">{{ item.Cat2 || '-' }}</span>
                                </template>

                                <!-- Categorie RWA dropdown -->
                                <template #item.categorieRwa="{ item }">
                                    <div style="display:flex; gap:8px; align-items:center">
                                        <v-select :items="categorieRwaOptions"
                                            v-model="selectedCategorieRwa[getItemKey(item)]" item-title="title"
                                            item-value="value" variant="outlined" density="compact" hide-details
                                            class="rwa-select" placeholder="Select RWA Category"
                                            prepend-inner-icon="mdi-tag-outline" clearable
                                            :error="isTouched[getItemKey(item)] && !selectedCategorieRwa[getItemKey(item)]"
                                            :error-messages="isTouched[getItemKey(item)] && !selectedCategorieRwa[getItemKey(item)] ? ['Required'] : []"
                                            @blur="() => markTouched(getItemKey(item))"
                                            @update:model-value="() => markTouched(getItemKey(item))"
                                            style="min-width:250px"
                                            :aria-label="`Categorie RWA for ${getItemKey(item)}`">
                                        </v-select>
                                    </div>
                                </template>

                                <!-- Type Bloomberg dropdown -->
                                <template #item.typeBloomberg="{ item }">
                                    <div style="display:flex; gap:8px; align-items:center">
                                        <v-select :items="typeBloombergOptions"
                                            v-model="selectedTypeBloomberg[getItemKey(item)]" item-title="title"
                                            item-value="value" variant="outlined" density="compact" hide-details
                                            class="rwa-select" placeholder="Select Bloomberg Type (Optional)"
                                            prepend-inner-icon="mdi-chart-line" clearable style="min-width:280px"
                                            :aria-label="`Type Bloomberg for ${getItemKey(item)}`">
                                        </v-select>
                                    </div>
                                </template>
                                <template #expanded-row="{ columns, item }">
                                    <tr>
                                        <td :colspan="columns.length" class="pa-0">
                                            <div class="expanded-content-container">
                                                <ExpandedRowContent :num-lignes="item.NumLignes" />
                                            </div>
                                        </td>
                                    </tr>
                                </template>
                            </v-data-table>

                            <v-row class="mt-6">
                                <v-col cols="12" class="d-flex justify-space-between align-center">
                                    <div class="d-flex gap-3">
                                        <ProgressiveLoader :loading="workflowStore.stepLoading['rwa-category-submit']">
                                            <v-btn id="rwa-category-submit-btn"
                                                :disabled="!formValid || hasValidationErrors || workflowStore.loading"
                                                color="primary" elevation="2" large @click="submit"
                                                aria-label="Submit mappings">
                                                <v-icon left>mdi-check-circle</v-icon>
                                                Submit Mappings
                                            </v-btn>
                                        </ProgressiveLoader>
                                    </div>

                                    <!-- Progress indicator -->
                                    <div v-if="filteredRows.length > 0" class="text-caption text--secondary">
                                        {{ mappedCount }} / {{ filteredRows.length }} mapped
                                        <span v-if="search && filteredRows.length !== rows.length"
                                            class="ml-2 text--disabled">
                                            ({{ totalMappedCount }} / {{ rows.length }} total)
                                        </span>
                                    </div>
                                </v-col>
                            </v-row>
                        </div>
                    </v-form>
                </div>
            </v-card-text>
        </v-card>
    </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useWorkflowStore } from '../../stores/workflow'
import ExpandedRowContent from './ExpandedRowContent.vue'
import GroupingDetails from './GroupingDetails.vue'
import SkeletonLoader from '../loaders/SkeletonLoader.vue';
import ProgressiveLoader from '../loaders/ProgressiveLoader.vue';

const workflowStore = useWorkflowStore()
const rows = ref<any[]>([])
const formValid = ref(true)
const selectedCategorieRwa = ref<Record<string, string | null>>({})
const selectedTypeBloomberg = ref<Record<string, string | null>>({})
const isTouched = ref<Record<string, boolean>>({})
const search = ref<string>('')
const expandedData = ref<Record<string, any[]>>({})
const expandedLoading = ref<Record<string, boolean>>({})

// Dropdown options extracted from payload
const categorieRwaOptions = ref<any[]>([])
const typeBloombergOptions = ref<any[]>([])

// Computed property to filter rows based on search
const filteredRows = computed(() => {
    if (!search.value || search.value.trim() === '') {
        return rows.value
    }

    const searchTerm = search.value.toLowerCase().trim()
    return rows.value.filter(row => {
        const source = row.Source.toLowerCase()
        const cat1 = row.Cat1.toLowerCase()
        const cat2 = row.Cat2?.toLowerCase() || ''
        return source.includes(searchTerm) || cat1.includes(searchTerm) || cat2.includes(searchTerm)
    })
})

// Computed property for real-time mapping count of filtered rows
const mappedCount = computed(() => {
    return filteredRows.value.filter(row => {
        const itemKey = getItemKey(row)
        return selectedCategorieRwa.value[itemKey]
    }).length
})

// Computed property for total mapping count (all rows)
const totalMappedCount = computed(() => {
    return Object.keys(selectedCategorieRwa.value).filter(k => selectedCategorieRwa.value[k]).length
})

function markTouched(id: string) {
    isTouched.value[id] = true
}

// Helper function to get the correct identifiant field name
function getItemKey(item: any): string {
    return item.Source + item.Cat1 + item.Cat2
}

// Function to get row classes based on completion status
function getRowClass(item: any): string {
    const itemKey = getItemKey(item)
    const hasRwaMapping = selectedCategorieRwa.value[itemKey]

    if (hasRwaMapping) {
        return 'row-complete'
    } else {
        return 'row-incomplete'
    }
}

const headers = [
    { title: 'Source', value: 'Source' },
    { title: 'Cat1', value: 'Cat1' },
    { title: 'Cat2', value: 'Cat2' },
    { title: 'Categorie RWA', value: 'categorieRwa' },
    { title: 'Type Bloomberg', value: 'typeBloomberg' }
]

// Function to load data from workflow store
function loadDataFromStore() {
    const rwaCategoryStep = workflowStore.workflowSteps.find(s => s.stepName === 'RWA Category Manager')
    console.log('RWA Category Manager step:', rwaCategoryStep)

    if (rwaCategoryStep?.dataPayload) {
        console.log('Raw dataPayload:', rwaCategoryStep.dataPayload)
        const payload = JSON.parse(rwaCategoryStep.dataPayload)
        console.log('Parsed payload:', payload)
        console.log('Payload keys:', Object.keys(payload))

        // Set rows and dropdown options from payload - use correct property names
        let idCounter = 0
        rows.value = (payload.MissingMappingRows || payload.missingMappingRows || []).map((r: any) => ({ ...r, id: idCounter++ }))
        console.log('Loaded rows:', rows.value)

        // Check for dropdown options with more detailed logging
        const rawCategorieOptions = payload.CategorieRwaOptions || payload.categorieRwaOptions || []
        const rawTypeBloombergOptions = payload.TypeBloombergOptions || payload.typeBloombergOptions || []

        console.log('Raw Categorie options from payload:', rawCategorieOptions)
        console.log('First categorie option structure:', rawCategorieOptions[0])

        categorieRwaOptions.value = rawCategorieOptions.map((c: any) => ({
            title: c.Libelle || c.libelle || c.Title || c.title,
            value: c.IdCatRwa || c.idCatRwa || c.Value || c.value || c.Id || c.id
        }))
        console.log('Mapped Categorie options:', categorieRwaOptions.value)
        console.log('categorieRwaOptions.value length:', categorieRwaOptions.value.length)

        console.log('Raw Type Bloomberg options from payload:', rawTypeBloombergOptions)
        console.log('First type bloomberg option structure:', rawTypeBloombergOptions[0])

        typeBloombergOptions.value = rawTypeBloombergOptions.map((t: any) => ({
            title: t.Libelle || t.libelle || t.Title || t.title,
            value: t.IdTypeBloomberg || t.idTypeBloomberg || t.Value || t.value || t.Id || t.id
        }))
        console.log('Mapped Type Bloomberg options:', typeBloombergOptions.value)
        console.log('typeBloombergOptions.value length:', typeBloombergOptions.value.length)

        // Initialize selection objects
        rows.value.forEach(r => {
            const key = getItemKey(r)
            selectedCategorieRwa.value[key] = r.CategorieRwaId || null
            selectedTypeBloomberg.value[key] = r.TypeBloombergId || null
            isTouched.value[key] = false
        })
    }
}

onMounted(async () => {
    workflowStore.setStepLoading('rwa-category-reload', true);
    try {
        loadDataFromStore()
    } finally {
        workflowStore.setStepLoading('rwa-category-reload', false);
    }
})

// Watch for changes in workflow steps to reactive update the component
watch(() => workflowStore.workflowSteps, () => {
    console.log('Workflow steps changed, reloading RWA Category Manager data')
    loadDataFromStore()
}, { deep: true })

const hasValidationErrors = computed(() =>
    rows.value.some(r => !selectedCategorieRwa.value[getItemKey(r)])
)

// Check if the RWA Category Manager step is successfully finished
const isRwaCategorySuccessful = computed(() => {
    const currentStep = workflowStore.workflowSteps.find(s => s.stepName === 'RWA Category Manager')
    return currentStep?.status === 'SuccessfulFinish'
})

async function submit() {
    workflowStore.setStepLoading('rwa-category-submit', true);
    try {
        // Prepare mappings from user selections (only filled rows)
        const mappings = rows.value
            .filter(r => selectedCategorieRwa.value[getItemKey(r)]) // Only submit rows with selections
            .map((r: any) => ({
                numLignes: r.NumLignes,
                source: r.Source,
                cat1: r.Cat1,
                cat2: r.Cat2,
                categorieRwaId: selectedCategorieRwa.value[getItemKey(r)],
                typeBloombergId: selectedTypeBloomberg.value[getItemKey(r)] || null
            }))

        if (mappings.length > 0) {
            // Send to server
            const response = await fetch('/api/workflow/rwa-mappings', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ mappings })
            })

            if (!response.ok) {
                throw new Error('Failed to submit mappings')
            }

            // Don't reload here - let SignalR handle the state update
        }
    } finally {
        workflowStore.setStepLoading('rwa-category-submit', false);
    }
}

async function expandRow(expandedItems: any[]) {
    const item = expandedItems[0]
    if (!item) {
        return
    }
    const key = getItemKey(item)
    if (expandedData.value[key]) {
        return
    }

    expandedLoading.value[key] = true
    try {
        const response = await fetch('/api/workflow/get-inventaire-normalise-by-numlignes', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item.NumLignes)
        })

        if (!response.ok) {
            throw new Error('Failed to fetch expanded data')
        }

        expandedData.value[key] = await response.json()
    } finally {
        expandedLoading.value[key] = false
    }
}

async function reload() {
    workflowStore.setStepLoading('rwa-category-reload', true);
    // Reload from current workflow step payload using the same logic
    loadDataFromStore()
    workflowStore.setStepLoading('rwa-category-reload', false);
}
</script>

<style scoped>
.rwa-data-table {
    border-radius: 12px !important;
    overflow: hidden;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1) !important;
}

.rwa-data-table :deep(.v-data-table__wrapper) {
    border-radius: 12px;
}

.rwa-data-table :deep(.v-data-table-header) {
    background: linear-gradient(135deg, #1976d2 0%, #1565c0 100%);
}

.rwa-data-table :deep(.v-data-table-header th) {
    color: white !important;
    font-weight: 600;
    text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
    border: none !important;
    border-right: 1px solid rgba(255, 255, 255, 0.2) !important;
}

.rwa-data-table :deep(.v-data-table-header th:last-child) {
    border-right: none !important;
}

.rwa-data-table :deep(tbody td) {
    border-right: 1px solid rgba(0, 0, 0, 0.08) !important;
    padding: 12px 16px !important;
}

.rwa-data-table :deep(tbody td:last-child) {
    border-right: none !important;
}

.rwa-data-table :deep(tbody tr:hover) {
    background: rgba(0, 0, 0, 0.04) !important;
    transform: none;
    transition: background-color 0.2s ease;
}

.rwa-data-table :deep(tbody tr) {
    transition: all 0.2s ease;
}

/* Row status indicators */
.rwa-data-table :deep(tbody tr.row-complete) {
    background: rgba(76, 175, 80, 0.08) !important;
    border-left: 4px solid #4caf50 !important;
}

.rwa-data-table :deep(tbody tr.row-incomplete) {
    background: rgba(244, 67, 54, 0.06) !important;
    border-left: 4px solid #f44336 !important;
}

.rwa-data-table :deep(tbody tr.row-complete:hover) {
    background: rgba(76, 175, 80, 0.15) !important;
}

.rwa-data-table :deep(tbody tr.row-incomplete:hover) {
    background: rgba(244, 67, 54, 0.12) !important;
}

.rwa-select :deep(.v-input__control) {
    border-radius: 8px;
}

.rwa-select :deep(.v-select__selection) {
    margin-top: 4px;
    margin-bottom: 4px;
}

.rwa-select :deep(.v-text-field__details) {
    padding-left: 12px;
    margin-top: 4px;
}

/* Animation for loading spinner */
@keyframes spin {
    0% {
        transform: rotate(0deg);
    }

    100% {
        transform: rotate(360deg);
    }
}

.mdi-spin {
    animation: spin 1s linear infinite;
}

/* Custom chip styling */
.v-chip.v-chip--outlined.v-chip--select {
    border: 1px solid rgba(25, 118, 210, 0.5);
    background: rgba(25, 118, 210, 0.05);
}

/* Progress indicator styling */
.text--secondary {
    font-weight: 500;
    background: linear-gradient(45deg, #1976d2, #42a5f5);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
    font-size: 14px !important;
}

/* Button hover effects */
.v-btn:not(.v-btn--disabled):hover {
    transform: translateY(-2px);
    transition: all 0.2s ease;
}

/* Card styling */
.v-card {
    border-radius: 16px !important;
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12) !important;
}

/* Success alert enhancement */
.v-alert.success {
    border-left: 4px solid #4caf50;
    background: linear-gradient(135deg, #e8f5e8 0%, #f1f8e9 100%);
}

.expanded-content-container {
    max-height: 400px;
    /* Or your desired max height */
    overflow-y: auto;
}
</style>
