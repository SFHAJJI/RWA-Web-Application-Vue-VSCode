<template>
  <div>
    <h3 class="text-h6">Tethys Status</h3>
    <br />
    <v-sheet border>
      <pre>{{ step }}</pre>
    </v-sheet>
  </div>
</template>

<script setup>
import { defineProps } from 'vue';

const props = defineProps({
  step: {
    type: Object,
    required: true,
  },
});
</script>
