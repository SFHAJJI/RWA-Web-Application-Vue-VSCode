<template>
  <v-card flat class="ma-2 pa-3">
    <v-data-table-server
      v-model:items-per-page="options.itemsPerPage"
      :headers="tableHeaders"
      :items="serverItems"
      :items-length="totalItems"
      :loading="loading"
      :search="search"
      @update:options="updateOptions"
      class="elevation-1 modern-table"
      density="compact"
      fixed-header
      hover
    >
      <template v-slot:top>
        <div class="filter-controls">
          <v-text-field v-model="filters.RaisonSociale" density="compact" placeholder="Search by Raison Sociale, Cpt..." hide-details clearable prepend-inner-icon="mdi-magnify"></v-text-field>
        </div>
        <v-progress-linear :active="loading" indeterminate color="primary"></v-progress-linear>
      </template>
      <template v-slot:item.actions="{ item }">
        <v-btn size="small" color="primary" @click="assignItem(item)">
          Assign
        </v-btn>
      </template>
    </v-data-table-server>
  </v-card>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, defineEmits, watch, defineProps } from 'vue';
import { useAuditStore } from '../../stores/auditStore';
import { searchTethys } from '../../api/tethysApi';

type VDataTableHeader = {
  key: string;
  value?: string;
  title: string;
  align?: 'start' | 'end' | 'center';
  sortable?: boolean;
  [key: string]: any;
};

const props = defineProps<{
  prefill: { query: string, context?: any },
  pageSize: number,
}>();

const emit = defineEmits(['assign']);
const auditStore = useAuditStore();
const search = ref('');

const options = ref({
  page: 1,
  itemsPerPage: props.pageSize || 20,
  sortBy: [],
  groupBy: [],
});

const serverItems = ref<any[]>([]);
const loading = ref(false);
const totalItems = ref(0);
const filters = ref({ RaisonSociale: '' });

async function loadServerItems() {
    loading.value = true;
    const data = await searchTethys(filters.value.RaisonSociale, undefined, options.value.itemsPerPage);
    serverItems.value = data.items;
    totalItems.value = data.total;
    loading.value = false;
}

watch(() => props.prefill, (newPrefill) => {
    if (newPrefill) {
        filters.value.RaisonSociale = newPrefill.query;
        loadServerItems();
    }
}, { immediate: true, deep: true });


let debounceTimer: any;
watch(filters, () => {
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    search.value = String(Date.now());
  }, 300);
}, { deep: true });

const tableHeaders = computed((): VDataTableHeader[] => {
  const headers: VDataTableHeader[] = auditStore.tethysColumns.map((header: any) => ({
    title: header.text,
    value: header.value,
    key: header.value,
    sortable: true,
  }));
  // Add an actions column
  headers.push({ title: 'Actions', key: 'actions', sortable: false, align: 'end' });
  return headers;
});

function assignItem(item: any) {
  if (item && item.identifiantRaf) {
    emit('assign', { raf: item.identifiantRaf });
  }
}

const updateOptions = (newOptions: any) => {
  options.value = newOptions;
  loadServerItems();
};

onMounted(async () => {
  await auditStore.fetchTethysColumns();
});
</script>

<style scoped>
.filter-controls {
  display: grid;
  grid-template-columns: 1fr; /* Single column for one search bar */
  gap: 12px;
  padding: 8px 16px;
}
.filter-item {
  flex-grow: 1;
}
.radio-label {
  white-space: nowrap;
}
.modern-table {
  border: 1px solid rgba(0, 0, 0, 0.1);
  border-radius: 8px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}
.modern-table .v-data-table-header {
  background-color: #f5f5f5;
}
.modern-table .v-data-table-header th {
  font-weight: 600;
  color: #333;
}
.modern-table tbody tr:hover {
  background-color: #f9f9f9;
}
.modern-table tbody tr:nth-of-type(odd) {
  background-color: #fafafa;
}
</style>
