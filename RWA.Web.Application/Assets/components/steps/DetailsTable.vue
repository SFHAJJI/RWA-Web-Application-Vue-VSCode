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
        <div class="d-flex gap-2 justify-end">
          <v-btn size="x-small" color="primary" variant="text" @click="assignItem(item, 'IdentifiantRaf')" :disabled="!item.identifiantRaf">
            RAF
          </v-btn>
          <v-btn size="x-small" color="secondary" variant="text" @click="assignItem(item, 'RafTeteGroupeReglementaire')" :disabled="!item.rafTeteGroupeReglementaire">
            Tête
          </v-btn>
        </div>
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

const emit = defineEmits<{
  (e: 'assign', payload: { raf: string; sourceField: 'IdentifiantRaf'|'RafTeteGroupeReglementaire'; candidate?: any }): void
}>()
const auditStore = useAuditStore();
const search = ref('');

const options = ref({
  page: 1,
  itemsPerPage: props.pageSize || 20,
  sortBy: [],
  groupBy: [],
});

const serverItems = ref<any[]>([])
const loading = ref(false)
const totalItems = ref(0)
const filters = ref({ RaisonSociale: '' })
const minChars = 2

async function loadServerItems() {
  const q = (filters.value.RaisonSociale || '').trim()
  if (q.length < minChars) {
    // avoid heavy queries on empty/short strings; wait for user input
    serverItems.value = []
    totalItems.value = 0
    return
  }
  loading.value = true
  try {
    const data = await searchTethys(q, undefined, options.value.itemsPerPage)
    serverItems.value = data.items
    totalItems.value = data.total
  } finally {
    loading.value = false
  }
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
    search.value = String(Date.now())
    loadServerItems()
  }, 300)
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

function assignItem(item: any, sourceField: 'IdentifiantRaf'|'RafTeteGroupeReglementaire') {
  const raf = sourceField === 'IdentifiantRaf' ? item?.identifiantRaf : item?.rafTeteGroupeReglementaire
  if (!raf) return
  emit('assign', { raf, sourceField, candidate: item })
}

const updateOptions = (newOptions: any) => {
  options.value = newOptions;
  loadServerItems();
};

onMounted(async () => {
  await auditStore.fetchTethysColumns();
  // Initial load (prefill watcher will also trigger if provided)
  if ((filters.value.RaisonSociale || '').trim().length >= minChars) {
    loadServerItems()
  }
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
.empty-hint { color: #888; font-size: 0.9rem; padding: 8px 16px; }
</style>
