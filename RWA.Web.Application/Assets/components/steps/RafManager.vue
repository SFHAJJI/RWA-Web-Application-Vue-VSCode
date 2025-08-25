<template>
  <v-stepper v-model="currentStep" :items="items" show-actions>
    <template v-slot:item.1>
      <TethysStatus :step="step" />
    </template>

    <template v-slot:item.2>
      <RAFHelper />
    </template>
  </v-stepper>
</template>

<script setup>
import { ref, defineProps } from 'vue';
import TethysStatus from './TethysStatus.vue';
import RAFHelper from './RAFHelper.vue';

const props = defineProps({
  step: {
    type: Object,
    required: true,
  },
});

const currentStep = ref(1);
const items = ['Tethys Status', 'RAF Helper'];
</script>
