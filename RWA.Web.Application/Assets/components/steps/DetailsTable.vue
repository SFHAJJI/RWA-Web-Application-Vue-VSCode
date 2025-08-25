<template>
  <v-card flat class="ma-2 pa-3">
    <v-radio-group v-model="selectedValue" @update:modelValue="onSelectionChange">
      <v-data-table
        :items="details"
        :headers="headers"
        item-value="id"
        hide-default-footer
      >
        <template v-slot:item.details1="{ item }">
          <v-radio :value="item.details1" :label="item.details1"></v-radio>
        </template>
        <template v-slot:item.details2="{ item }">
          <v-radio :value="item.details2" :label="item.details2"></v-radio>
        </template>
      </v-data-table>
    </v-radio-group>
  </v-card>
</template>

<script setup>
import { ref, defineProps, defineEmits } from 'vue';

const props = defineProps({
  details: {
    type: Array,
    required: true,
  },
});

const emit = defineEmits(['selection-changed']);

const selectedValue = ref(null);

const headers = [
  { title: 'Details 1', key: 'details1', sortable: false },
  { title: 'Details 2', key: 'details2', sortable: false },
];

function onSelectionChange(newValue) {
  if (newValue) {
    emit('selection-changed', newValue);
  }
}
</script>
