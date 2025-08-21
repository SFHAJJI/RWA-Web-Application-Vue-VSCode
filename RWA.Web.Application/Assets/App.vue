<script setup lang="ts">
import WorkflowView from './components/WorkflowView.vue';
import GlobalToast from './components/GlobalToast.vue';
import { useLoadingStore } from './stores/loading';

const loading = useLoadingStore();
</script>

<template>
    <v-app>
        <v-main>
            <WorkflowView />
            <GlobalToast />
            <v-progress-linear
                v-if="loading.isPosting"
                indeterminate
                color="primary"
                height="4"
                fixed
                bottom
            />
        </v-main>
    </v-app>
</template>
