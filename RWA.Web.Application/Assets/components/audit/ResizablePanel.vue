<template>
  <div class="resizable-panel" :style="{ width: `${panelWidth}%` }">
    <div class="resize-handle" @mousedown="startResize"></div>
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const panelWidth = ref(50);
const isResizing = ref(false);

const startResize = () => {
  isResizing.value = true;
  document.addEventListener('mousemove', handleResize);
  document.addEventListener('mouseup', stopResize);
};

const handleResize = (event: MouseEvent) => {
  if (!isResizing.value) return;
  const newWidth = (event.clientX / window.innerWidth) * 100;
  panelWidth.value = Math.min(100, Math.max(50, newWidth));
};

const stopResize = () => {
  isResizing.value = false;
  document.removeEventListener('mousemove', handleResize);
  document.removeEventListener('mouseup', stopResize);
};
</script>

<style scoped>
.resizable-panel {
  position: relative;
  overflow: hidden;
}

.resize-handle {
  position: absolute;
  top: 0;
  right: -5px;
  width: 10px;
  height: 100%;
  cursor: col-resize;
  z-index: 1003;
}
</style>
