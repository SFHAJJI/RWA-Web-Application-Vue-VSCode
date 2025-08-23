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
        <v-text-field v-model="filters.IdentifiantRaf" density="compact" placeholder="RAF..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.RaisonSociale" density="compact" placeholder="Raison Sociale..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.CodeIsin" density="compact" placeholder="Code ISIN..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.CodeCusip" density="compact" placeholder="Code CUSIP..." hide-details class="filter-item"></v-text-field>
      </div>
    </template>
  </EnhancedDataTable>
</template>

<script setup lang="ts">
import { ref, reactive, watch, onMounted } from 'vue';
import { post } from '../../api';
import { useAuditStore } from '../../stores/auditStore';
import EnhancedDataTable from './EnhancedDataTable.vue';

const auditStore = useAuditStore();

const serverItems = ref<any[]>([]);
const loading = ref(true);
const totalItems = ref(0);
const searchTrigger = ref('');
const filters = reactive({
  IdentifiantRaf: '',
  RaisonSociale: '',
  CodeIsin: '',
  CodeCusip: ''
});

watch(filters, () => {
  searchTrigger.value = String(Date.now());
}, { deep: true });

const loadServerItems = async (options: any) => {
  loading.value = true;
  try {
    const { page, itemsPerPage, sortBy } = options;
    const activeFilters = Object.fromEntries(
      Object.entries(filters).filter(([, value]) => value)
    );

    const requestBody = {
      page,
      pageSize: itemsPerPage,
      sortBy: sortBy.length > 0 ? sortBy[0].key : '',
      sortDesc: sortBy.length > 0 ? sortBy[0].order === 'desc' : false,
      filters: activeFilters,
    };

    const response = await post('/api/audit/tethys/data', requestBody);
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
