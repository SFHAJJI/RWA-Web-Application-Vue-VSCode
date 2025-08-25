<template>
  <v-stepper v-model="currentStep" :items="items" show-actions>
    <template v-slot:item.1>
      <TethysStatus :payload="payload" />
    </template>

    <template v-slot:item.2>
      <RAFHelper />
    </template>
  </v-stepper>
</template>

<script setup>
import { ref, defineProps, computed } from 'vue';
import TethysStatus from './TethysStatus.vue';
import RAFHelper from './RAFHelper.vue';

const props = defineProps({
  step: {
    type: Object,
    required: true,
  },
});

const payload = computed(() => {
    try {
        const parsed = JSON.parse(props.step.dataPayload);
        return parsed.Dtos || [];
    } catch (e) {
        return [];
    }
});

const currentStep = ref(1);
const items = ['Tethys Status', 'RAF Helper'];
</script>
