<template>
    <div class="expanded-row-container">
        <GroupingDetails :num-lignes="numLignes" />
    </div>
</template>

<script setup lang="ts">
import GroupingDetails from './GroupingDetails.vue'

defineProps({
    numLignes: {
        type: Array as () => number[],
        required: true
    }
})
</script>

<style scoped>
.expanded-row-container {
    overflow: auto;
}
</style>
