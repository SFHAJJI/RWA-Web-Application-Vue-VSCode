<template>
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
    <template v-slot:top>
      <slot name="filters"></slot>
    </template>
  </v-data-table-server>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';

// Props
const props = defineProps({
  headers: { type: Array as () => any[], required: true },
  items: { type: Array as () => any[], required: true },
  itemsLength: { type: Number, required: true },
  loading: { type: Boolean, required: true },
  search: { type: String, required: true }
});

// Emits
const emit = defineEmits(['update:options']);

// State
const options = ref({
  page: 1,
  itemsPerPage: 10,
  sortBy: [],
  groupBy: [],
});

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
  emit('update:options', newOptions);
};
</script>

<style scoped>
.audit-table {
  height: 100%;
}
</style>
