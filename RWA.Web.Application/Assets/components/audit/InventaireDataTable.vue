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
} = useDataTable('/api/audit/inventory/data', {
  IdentifiantUnique: '',
  Raf: '',
  RefCatRWA: '',
});

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
