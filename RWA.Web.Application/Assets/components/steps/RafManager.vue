<template>
    <div>
        <h3>RAF Manager</h3>
        <SkeletonLoader v-if="workflowStore.loading" />
        <div v-else>
            <p>Content for this step goes here.</p>
            <ProgressiveLoader :loading="workflowStore.stepLoading['raf-manager']">
                <v-btn @click="triggerRaf" color="primary">Process RAF</v-btn>
            </ProgressiveLoader>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useWorkflowStore } from '../../stores/workflow';
import SkeletonLoader from '../loaders/SkeletonLoader.vue';
import ProgressiveLoader from '../loaders/ProgressiveLoader.vue';

const workflowStore = useWorkflowStore();

const triggerRaf = async () => {
    workflowStore.setStepLoading('raf-manager', true);
    await workflowStore.triggerTransition('ProcessRaf');
    workflowStore.setStepLoading('raf-manager', false);
};
</script>
