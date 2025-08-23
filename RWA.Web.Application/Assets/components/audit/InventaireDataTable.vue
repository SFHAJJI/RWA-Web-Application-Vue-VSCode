<template>
  <EnhancedDataTable
    :headers="auditStore.inventaireColumns"
    :items="serverItems"
    :items-length="totalItems"
    :loading="loading"
    :search="searchTrigger"
    @update:options="loadServerItems"
  >
    <template #filters>
      <div class="filter-controls">
        <v-text-field v-model="filters.IdentifiantUnique" density="compact" placeholder="Identifiant Unique..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.Raf" density="compact" placeholder="RAF..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.RefCatRWA" density="compact" placeholder="Ref Cat RWA..." hide-details class="filter-item"></v-text-field>
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
  IdentifiantUnique: '',
  Raf: '',
  RefCatRWA: ''
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

    const response = await post('/api/audit/inventory/data', requestBody);
    serverItems.value = response.data.items;
    totalItems.value = response.data.totalItems;
  } catch (error) {
    console.error("Error loading inventory data:", error);
  } finally {
    loading.value = false;
  }
};

onMounted(async () => {
  await auditStore.fetchInventaireColumns();
});
</script>

<style scoped>
.filter-controls {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 12px;
  padding: 8px 16px;
}
.filter-item {
  flex-grow: 1;
}
</style>
