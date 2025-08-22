<template>
  <EnhancedDataTable
    :headers="auditStore.tethysColumns"
    :items="serverItems"
    :items-length="totalItems"
    :loading="loading"
    :search="searchTrigger"
    @update:options="loadServerItems"
  >
    <template #filters>
      <div class="filter-controls">
        <v-text-field v-model="filters.identifiantRaf" density="compact" placeholder="RAF..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.raisonSociale" density="compact" placeholder="Raison Sociale..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.codeIsin" density="compact" placeholder="Code ISIN..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.codeCusip" density="compact" placeholder="Code CUSIP..." hide-details class="filter-item"></v-text-field>
      </div>
    </template>
  </EnhancedDataTable>
</template>

<script setup lang="ts">
import { ref, reactive, watch, onMounted } from 'vue';
import axios from 'axios';
import { useAuditStore } from '../../stores/auditStore';
import EnhancedDataTable from './EnhancedDataTable.vue';

const auditStore = useAuditStore();

const serverItems = ref<any[]>([]);
const loading = ref(true);
const totalItems = ref(0);
const searchTrigger = ref('');
const filters = reactive({
  identifiantRaf: '',
  raisonSociale: '',
  codeIsin: '',
  codeCusip: ''
});

watch(filters, () => {
  searchTrigger.value = String(Date.now());
}, { deep: true });

const loadServerItems = async (options: any) => {
  loading.value = true;
  try {
    const { page, itemsPerPage, sortBy } = options;
    const requestBody = {
      page,
      pageSize: itemsPerPage,
      sortBy: sortBy.length > 0 ? sortBy[0].key : null,
      sortDesc: sortBy.length > 0 ? sortBy[0].order === 'desc' : false,
      filters: filters,
    };
    const response = await axios.post('/api/audit/tethys/data', requestBody);
    serverItems.value = response.data.items;
    totalItems.value = response.data.totalItems;
  } catch (error) {
    console.error("Error loading tethys data:", error);
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  await auditStore.fetchTethysColumns();
  // Initial load
  loading.value = true;
  try {
    const response = await axios.get('/api/audit/tethys/initial-data');
    serverItems.value = response.data.items;
    totalItems.value = response.data.totalItems;
  } catch (error) {
    console.error("Error on initial load for tethys:", error);
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
.filter-controls {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
  padding: 8px 16px;
}
.filter-item {
  flex-grow: 1;
}
</style>
