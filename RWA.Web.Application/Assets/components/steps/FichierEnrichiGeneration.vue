<template>
    <div>
        <h3>Fichier Enrichi Generation</h3>
        <SkeletonLoader v-if="workflowStore.loading" />
        <div v-else>
            <p>Content for this step goes here.</p>
            <ProgressiveLoader :loading="workflowStore.stepLoading['fichier-enrichi']">
                <v-btn @click="triggerGeneration" color="primary">Generate</v-btn>
            </ProgressiveLoader>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useWorkflowStore } from '../../stores/workflow';
import SkeletonLoader from '../loaders/SkeletonLoader.vue';
import ProgressiveLoader from '../loaders/ProgressiveLoader.vue';

const workflowStore = useWorkflowStore();

const triggerGeneration = async () => {
    workflowStore.setStepLoading('fichier-enrichi', true);
    await workflowStore.triggerTransition('GenerateFichierEnrichi');
    workflowStore.setStepLoading('fichier-enrichi', false);
};
</script>
