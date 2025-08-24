<template>
  <div>
    <h2>BDD Manager</h2>
    <div class="card">
      <v-container fluid v-if="!loading">
        <v-row>
          <v-col cols="12" md="4">
            <v-card @click="toggleTable('SuccessfulMatches')" class="mb-4" color="green-lighten-5" elevation="2">
              <v-card-title class="d-flex align-center text-green-darken-3">
                <v-icon color="green" class="mr-2">mdi-check-circle</v-icon>
                Successful Matches
              </v-card-title>
              <v-card-subtitle>{{ successfulMatches.length }} items</v-card-subtitle>
            </v-card>
          </v-col>
          <v-col cols="12" md="4">
            <v-card @click="toggleTable('FailedMatches')" class="mb-4" color="red-lighten-5" elevation="2">
              <v-card-title class="d-flex align-center text-red-darken-3">
                <v-icon color="red" class="mr-2">mdi-alert-circle</v-icon>
                Failed Matches
              </v-card-title>
              <v-card-subtitle>{{ failedMatches.length }} items</v-card-subtitle>
            </v-card>
          </v-col>
          <v-col cols="12" md="4">
            <v-card @click="toggleTable('ToBeAddedToBDD')" class="mb-4" color="blue-lighten-5" elevation="2">
              <v-card-title class="d-flex align-center text-blue-darken-3">
                <v-icon color="blue" class="mr-2">mdi-database-plus</v-icon>
                To Be Added To BDD
              </v-card-title>
              <v-card-subtitle>{{ toBeAddedToBDD.length }} items</v-card-subtitle>
            </v-card>
          </v-col>
        </v-row>

        <v-expand-transition>
          <div v-if="activeTable">
            <v-card class="mt-4">
              <v-card-title class="data-table-title">{{ activeTable.replace(/([A-Z])/g, ' $1').trim() }}</v-card-title>
              <v-data-table
                :headers="headers"
                :items="activeData"
                class="elevation-1 modern-table"
              ></v-data-table>
            </v-card>
          </div>
        </v-expand-transition>
      </v-container>
      <v-container v-else>
        <v-row>
          <v-col cols="12" md="4" v-for="i in 3" :key="i">
            <v-skeleton-loader type="card"></v-skeleton-loader>
          </v-col>
        </v-row>
      </v-container>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue';
import { useWorkflowStore } from '../../stores/workflow';

const props = defineProps({
  step: {
    type: Object,
    required: true
  }
});

const workflowStore = useWorkflowStore();
const activeTable = ref(null);
const loading = ref(true);
const successfulMatches = ref([]);
const failedMatches = ref([]);
const toBeAddedToBDD = ref([]);

const loadDataFromStore = () => {
  loading.value = true;
  const bddManagerStep = workflowStore.workflowSteps.find(s => s.stepName === 'BDD Manager');
  console.log('BDD Manager step from store:', bddManagerStep);

  if (bddManagerStep?.dataPayload && bddManagerStep.dataPayload !== '{}') {
    try {
      console.log("Raw DataPayload:", bddManagerStep.dataPayload);
      const payload = JSON.parse(bddManagerStep.dataPayload);
      console.log("Parsed Payload:", payload);
      
      successfulMatches.value = payload.SuccessfulMatches || [];
      failedMatches.value = payload.FailedMatches || [];
      toBeAddedToBDD.value = payload.ToBeAddedToBDD || [];
      
      console.log('Loaded successfulMatches:', successfulMatches.value);
      console.log('Loaded failedMatches:', failedMatches.value);
      console.log('Loaded toBeAddedToBDD:', toBeAddedToBDD.value);

    } catch (e) {
      console.error("Failed to parse DataPayload:", e);
      successfulMatches.value = [];
      failedMatches.value = [];
      toBeAddedToBDD.value = [];
    } finally {
      loading.value = false;
    }
  } else {
    console.log("No data payload found for BDD Manager step.");
    successfulMatches.value = [];
    failedMatches.value = [];
    toBeAddedToBDD.value = [];
    loading.value = false;
  }
};

onMounted(loadDataFromStore);

watch(() => {
    const step = workflowStore.workflowSteps.find(s => s.stepName === 'BDD Manager');
    return step?.dataPayload;
}, loadDataFromStore);

const headers = computed(() => {
  if (!activeTable.value || !activeData.value || activeData.value.length === 0) return [];
  return Object.keys(activeData.value[0]).map(key => ({
    title: key.replace(/([A-Z])/g, ' $1').trim(), // Add spaces before capital letters for readability
    value: key,
    sortable: true
  }));
});

const activeData = computed(() => {
  switch (activeTable.value) {
    case 'SuccessfulMatches':
      return successfulMatches.value;
    case 'FailedMatches':
      return failedMatches.value;
    case 'ToBeAddedToBDD':
      return toBeAddedToBDD.value;
    default:
      return [];
  }
});

function toggleTable(tableName) {
  activeTable.value = activeTable.value === tableName ? null : tableName;
}
</script>

<style scoped>
.card {
  background: #ffffff;
  padding: 2rem;
  border-radius: 10px;
  margin-bottom: 1rem;
}

.v-card {
    transition: all 0.3s ease-in-out;
}

.v-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 12px 20px -10px rgba(0,0,0,0.2);
}

.data-table-title {
    font-weight: 500;
    color: #333;
    background: #f9f9f9;
    border-bottom: 1px solid #eee;
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
</style>
