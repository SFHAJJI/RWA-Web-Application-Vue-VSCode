<template>
  <div class="status-cell">
    <v-icon v-if="raf" color="success" class="status-icon">mdi-check-circle</v-icon>
    <v-icon v-else color="error" class="status-icon">mdi-close-circle</v-icon>
    <span class="raf-text">{{ raf }}</span>
  </div>
</template>

<script setup>
defineProps({
  raf: {
    type: String,
    default: ''
  }
});
</script>

<style scoped>
.status-cell {
  display: flex;
  align-items: center;
}
.status-icon {
  margin-right: 8px;
}
.raf-text {
  font-family: monospace;
}
</style>
