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
import { onMounted } from 'vue';
import { useAuditStore } from '../../stores/auditStore';
import { useDataTable } from '../../composables/useDataTable';
import EnhancedDataTable from './EnhancedDataTable.vue';

const auditStore = useAuditStore();

const {
  serverItems,
  loading,
  totalItems,
  searchTrigger,
  filters,
  loadServerItems,
} = useDataTable('/api/audit/tethys/data', {
  IdentifiantRaf: '',
  RaisonSociale: '',
  CodeIsin: '',
  CodeCusip: '',
});

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
