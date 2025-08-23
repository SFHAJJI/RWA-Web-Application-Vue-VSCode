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
        <v-text-field v-model="filters.IdentifiantOrigine" density="compact" placeholder="ID Origine..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.RefCategorieRwa" density="compact" placeholder="Réf. Cat. RWA..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.Raf" density="compact" placeholder="RAF..." hide-details class="filter-item"></v-text-field>
        <v-text-field v-model="filters.IdentifiantUniqueRetenu" density="compact" placeholder="ID Unique..." hide-details class="filter-item"></v-text-field>
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
} = useDataTable('/api/audit/history/data', {
  IdentifiantOrigine: '',
  RefCategorieRwa: '',
  Raf: '',
  IdentifiantUniqueRetenu: '',
});

onMounted(async () => {
  await auditStore.fetchHistoryColumns();
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
