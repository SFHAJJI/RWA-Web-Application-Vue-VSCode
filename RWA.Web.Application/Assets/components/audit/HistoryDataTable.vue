<template>
  <EnhancedDataTable
    :headers="auditStore.historyColumns"
    :items="serverItems"
    :items-length="totalItems"
    :loading="loading"
    :search="searchTrigger"
    @update:options="loadServerItems"
  >
    <template #filters>
      <div class="filter-controls">
        <v-text-field v-model="filters.identifiantOrigine" density="compact" placeholder="ID Origine..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.refCategorieRwa" density="compact" placeholder="Réf. Cat. RWA..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.raf" density="compact" placeholder="RAF..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.identifiantUniqueRetenu" density="compact" placeholder="ID Unique..." hide-details class="filter-item"></v-text-field>
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
  identifiantOrigine: '',
  refCategorieRwa: '',
  raf: '',
  identifiantUniqueRetenu: ''
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
    const response = await axios.post('/api/audit/history/data', requestBody);
    serverItems.value = response.data.items;
    totalItems.value = response.data.totalItems;
  } catch (error) {
    console.error("Error loading history data:", error);
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  await auditStore.fetchHistoryColumns();
  // Initial load
  loading.value = true;
  try {
    const response = await axios.get('/api/audit/history/initial-data');
    serverItems.value = response.data.items;
    totalItems.value = response.data.totalItems;
  } catch (error) {
    console.error("Error on initial load for history:", error);
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
