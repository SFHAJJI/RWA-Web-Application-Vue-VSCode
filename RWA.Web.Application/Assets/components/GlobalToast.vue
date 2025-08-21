<script setup lang="ts">
import { computed } from 'vue';
import { useToastStore } from '../stores/toast';
const toast = useToastStore();

const items = computed(() => toast.queue);

function stop(id: number) {
    toast.removeToast(id);
}
</script>

<template>
    <div>
        <template v-for="(t, idx) in items" :key="t.id">
            <v-snackbar :model-value="true" location="top right" timeout="6000" :style="{ 'margin-top': `${idx * 64}px` }" @update:modelValue="() => stop(t.id)">
                <div class="d-flex align-center">
                    <v-icon class="me-2" :color="t.type === 'warning' ? 'warning' : (t.type === 'error' ? 'error' : 'primary')">mdi-alert-circle</v-icon>
                    <span>{{ t.message }}</span>
                </div>
                <template #actions>
                    <v-btn text @click="stop(t.id)">Close</v-btn>
                </template>
            </v-snackbar>
        </template>
    </div>
</template>
