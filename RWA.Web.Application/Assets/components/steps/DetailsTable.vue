<template>
  <v-card flat class="ma-2 pa-3">
    <v-radio-group v-model="selectedValue" @update:modelValue="onSelectionChange">
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
            <v-text-field v-model="filters.IdentifiantRaf" density="compact" placeholder="RAF..." hide-details class="filter-item"></v-text-field>
            <v-text-field v-model="filters.RaisonSociale" density="compact" placeholder="Raison Sociale..." hide-details class="filter-item"></v-text-field>
            <v-text-field v-model="filters.CodeIsin" density="compact" placeholder="Code ISIN..." hide-details class="filter-item"></v-text-field>
            <v-text-field v-model="filters.CodeCusip" density="compact" placeholder="Code CUSIP..." hide-details class="filter-item"></v-text-field>
          </div>
          <v-progress-linear :active="loading" indeterminate color="primary"></v-progress-linear>
        </template>
        <template v-slot:item.identifiantRaf="{ item }">
          <v-radio v-if="item.identifiantRaf && item.identifiantRaf.trim()" :value="item.identifiantRaf" class="d-inline-block">
            <template v-slot:label>
              <span class="radio-label">{{ item.identifiantRaf }}</span>
            </template>
          </v-radio>
        </template>
        <template v-slot:item.rafTeteGroupeReglementaire="{ item }">
          <v-radio v-if="item.rafTeteGroupeReglementaire && item.rafTeteGroupeReglementaire.trim()" :value="item.rafTeteGroupeReglementaire" class="d-inline-block">
            <template v-slot:label>
              <span class="radio-label">{{ item.rafTeteGroupeReglementaire }}</span>
            </template>
          </v-radio>
        </template>
      </v-data-table-server>
    </v-radio-group>
  </v-card>
</template>

<script setup lang="ts">
import { onMounted, ref, computed, defineEmits, watch } from 'vue';
import { useAuditStore } from '../../stores/auditStore';
import { useDataTable } from '../../composables/useDataTable';

const emit = defineEmits(['selection-changed']);
const auditStore = useAuditStore();
const selectedValue = ref(null);
const search = ref('');

const options = ref({
  page: 1,
  itemsPerPage: 10,
  sortBy: [],
  groupBy: [],
});

const {
  serverItems,
  loading,
  totalItems,
  filters,
  loadServerItems,
} = useDataTable('/api/audit/tethys/data', {
  IdentifiantRaf: '',
  RaisonSociale: '',
  CodeIsin: '',
  CodeCusip: '',
});

watch(serverItems, (newItems) => {
  console.log('Server items in DetailsTable:', newItems);
});

let debounceTimer: any;
watch(filters, () => {
  clearTimeout(debounceTimer);
  debounceTimer = setTimeout(() => {
    search.value = String(Date.now());
  }, 300);
}, { deep: true });

const tableHeaders = computed(() => {
  return auditStore.tethysColumns.map((header: any) => ({
    title: header.text,
    value: header.value,
    key: header.value,
    sortable: true,
  }));
});

function onSelectionChange(newValue) {
  if (newValue) {
    emit('selection-changed', newValue);
  }
}

const updateOptions = (newOptions: any) => {
  options.value = newOptions;
  loadServerItems(newOptions);
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
