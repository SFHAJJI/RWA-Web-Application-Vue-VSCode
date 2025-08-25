<template>
  <v-card flat class="ma-2 pa-3">
    <v-radio-group v-model="selectedId" @update:modelValue="onSelectionChange">
      <v-data-table
        :items="details"
        :headers="headers"
        item-value="id"
        hide-default-footer
      >
        <template v-slot:item.selector="{ item }">
          <v-radio :value="item.id"></v-radio>
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

const selectedId = ref(null);

const headers = [
  { title: '', key: 'selector', sortable: false, width: '50px' },
  { title: 'ID', key: 'id' },
  { title: 'Detail', key: 'detail' },
  { title: 'Value', key: 'value' },
];

function onSelectionChange(newId) {
  const selectedItem = props.details.find(d => d.id === newId);
  if (selectedItem) {
    emit('selection-changed', selectedItem);
  }
}
</script>
