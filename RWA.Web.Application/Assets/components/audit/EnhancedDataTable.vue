<template>
  <div>
    <v-data-table
      v-if="items.length > 0"
      :headers="tableHeaders"
      :items="filteredItems"
      :loading="loading"
      class="audit-table"
      density="compact"
      :items-per-page="itemsPerPage"
      :page.sync="page"
      @update:page="page = $event"
      fixed-header
      hover
    >
      <template v-slot:headers="{ columns, getSortIcon, toggleSort }">
        <tr>
          <th v-for="column in columns" :key="column.key" :class="{ 'sortable': column.sortable }" @click="() => column.sortable && toggleSort(column)">
            <span>{{ column.title }}</span>
            <v-icon v-if="column.sortable" :icon="getSortIcon(column)" size="small"></v-icon>
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
            ></v-text-field>
          </th>
        </tr>
      </template>
      <template v-slot:bottom>
        <div class="d-flex justify-space-between align-center pa-2">
          <v-radio-group v-model="itemsPerPage" inline dense hide-details>
            <v-radio label="5" :value="5"></v-radio>
            <v-radio label="10" :value="10"></v-radio>
            <v-radio label="25" :value="25"></v-radio>
            <v-radio label="50" :value="50"></v-radio>
          </v-radio-group>
          <v-pagination
            v-model="page"
            :length="pageCount"
            :total-visible="5"
          ></v-pagination>
        </div>
      </template>
    </v-data-table>
    <div v-else-if="loading" class="table-loading">
      <v-progress-circular indeterminate color="primary"></v-progress-circular>
      <p>Chargement des données...</p>
    </div>
    <div v-else class="table-empty">
      <v-icon size="48" color="grey">mdi-database-off</v-icon>
      <p>Aucune donnée disponible</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue';

const props = defineProps({
  headers: {
    type: Array,
    required: true,
  },
  items: {
    type: Array,
    required: true,
  },
  loading: {
    type: Boolean,
    required: true,
  },
});

const page = ref(1);
const itemsPerPage = ref(10);
const filters = reactive<Record<string, string>>({});

const filteredItems = computed(() => {
  let data = props.items;
  const activeFilters = Object.keys(filters).filter(key => filters[key]);

  if (activeFilters.length === 0) {
    return data;
  }

  return data.filter(item => {
    return activeFilters.every(key => {
      const filterValue = filters[key]?.toLowerCase();
      const itemValue = String(item[key])?.toLowerCase();
      return itemValue.includes(filterValue);
    });
  });
});

const pageCount = computed(() => {
  return Math.ceil(filteredItems.value.length / itemsPerPage.value);
});

const tableHeaders = computed(() => {
  return props.headers.map((header: any) => ({
    title: header.text,
    value: header.value,
    key: header.value,
    sortable: true,
  }));
});

watch(filters, () => {
  page.value = 1;
});
</script>

<style scoped>
.filter-row th {
  padding: 8px 16px !important;
}

.filter-input .v-input__control {
  height: 32px;
}

.filter-input .v-input__slot {
  align-items: center;
}
</style>
