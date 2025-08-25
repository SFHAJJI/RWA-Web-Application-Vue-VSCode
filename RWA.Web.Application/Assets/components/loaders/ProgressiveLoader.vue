<template>
    <div class="progressive-loader">
        <v-progress-circular indeterminate color="primary" :size="20" :width="2" v-if="loading"></v-progress-circular>
        <slot v-else></slot>
    </div>
</template>

<script setup lang="ts">
defineProps({
    loading: Boolean
});
</script>

<style scoped>
.progressive-loader {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    min-width: 88px; /* Default Vuetify button width */
    min-height: 36px; /* Default Vuetify button height */
}
</style>
