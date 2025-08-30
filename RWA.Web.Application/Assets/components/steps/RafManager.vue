<template>
    <div>
        <v-btn color="primary" @click="downloadEnrichedFile" class="mb-4">
            Download Enriched File
        </v-btn>

        <v-card>
            <v-card-text>
                <v-stepper v-model="currentStep" :items="items">
                    <template v-slot:item.1>
                        <TethysStatus @counts="val => counts.value = val" />
                    </template>

                    <template v-slot:item.2>
                        <RAFHelper :payload="payload" />
                    </template>

                    <template v-slot:actions>
                        <v-btn :disabled="currentStep === 1" @click="currentStep--">
                            Back
                        </v-btn>
                        <v-btn :disabled="disableNext" @click="currentStep++">
                            Next
                        </v-btn>
                    </template>
                </v-stepper>
            </v-card-text>
        </v-card>
    </div>
</template>

<script setup>
import { ref, defineProps, computed, onMounted, onBeforeUnmount } from 'vue';
import TethysStatus from './TethysStatus.vue';
import RAFHelper from './RAFHelper.vue';
import { useWorkflowStore } from '../../stores/workflow';
// counts arrive via TethysStatus @counts (no polling)

const props = defineProps({
    step: {
        type: Object,
        required: true,
    },
});

const workflowStore = useWorkflowStore();

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
const counts = ref({ total: 0, lookedUp: 0, pendingLookup: 0, failed: 0, successful: 0 });
const disableNext = computed(() => {
    if (currentStep.value === 1) {
        return counts.value.pendingLookup > 0;
    }
    return currentStep.value === items.length;
});

onMounted(async () => {});
onBeforeUnmount(() => {});

const triggerNext = () => {
    workflowStore.triggerTransition('NextRafManagerToEnrichiExport');
};

// Placeholder handler to avoid build errors; wire to backend when ready
function downloadEnrichedFile() {
    // TODO: call export endpoint when available
    console.info('Download Enriched File clicked');
}
</script>
