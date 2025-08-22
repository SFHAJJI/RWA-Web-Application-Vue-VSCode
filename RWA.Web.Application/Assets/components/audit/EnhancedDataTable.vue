<template>
  <div>
    <v-data-table-server
      v-model:items-per-page="options.itemsPerPage"
      :headers="tableHeaders"
      :items="items"
      :items-length="itemsLength"
      :loading="loading"
      :search="search"
      class="audit-table"
      density="compact"
      fixed-header
      hover
      @update:options="updateOptions"
    >
      <template v-slot:headers="{ columns }">
        <tr>
          <th v-for="column in columns" :key="column.key">
            <span>{{ column.title }}</span>
          </th>
        </tr>
        <tr class="filter-row">
          <th v-for="header in columns" :key="`${header.key}-filter`">
            <v-text-field
              v-if="header.key !== 'data-table-select' && header.key !== 'data-table-expand'"
              v-model="filters[header.key]"
              dense
              hide-details
              class="filter-input"
              :placeholder="`Filtrer ${header.title}`"
              @update:model-value="debouncedFilterUpdate"
            ></v-text-field>
          </th>
        </tr>
      </template>
    </v-data-table-server>
    <div v-if="loading && items.length === 0" class="table-loading">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
      <p>Chargement des données...</p>
    </div>
    <div v-else-if="!loading && items.length === 0" class="table-empty">
      <v-icon size="48" color="grey">mdi-database-off</v-icon>
      <p>Aucune donnée disponible</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';

// Props
const props = defineProps({
  headers: { type: Array as () => any[], required: true },
  items: { type: Array as () => any[], required: true },
  itemsLength: { type: Number, required: true },
  loading: { type: Boolean, required: true },
});

// Emits
const emit = defineEmits(['update:options']);

// State
const options = ref({
  page: 1,
  itemsPerPage: 10,
  sortBy: [],
  groupBy: [],
  search: '',
});

const filters = reactive<Record<string, string>>({});
const search = ref(''); // This will be used to trigger updates

// Computed properties
const tableHeaders = computed(() => {
  return props.headers.map((header: any) => ({
    title: header.text,
    value: header.value,
    key: header.value,
    sortable: true,
  }));
});

// Methods
const updateOptions = (newOptions: any) => {
  options.value = newOptions;
  emit('update:options', { ...newOptions, filters: { ...filters } });
};

// Debounce function
let debounceTimer: number;
const debouncedFilterUpdate = () => {
  clearTimeout(debounceTimer);
  debounceTimer = window.setTimeout(() => {
    // Trigger a change that the watcher can pick up
    updateOptions(options.value);
  }, 500); // 500ms delay
};

// Watch for option changes to emit to parent
watch(options, () => {
    // Don't emit here directly, let updateOptions handle it to include filters
}, { deep: true });

</script>

<style scoped>
.filter-row th {
  padding: 8px 16px !important;
}
.filter-input {
  max-width: 100%;
}
.table-loading, .table-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 200px;
  color: #888;
}
</style>
