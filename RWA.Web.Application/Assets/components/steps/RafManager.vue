<template>
    <div>
        <v-btn color="primary" @click="triggerNext" class="mb-4">
            Next to Enrichi Export
        </v-btn>

        <v-card>
            <v-card-text>
                <v-stepper v-model="currentStep" :items="items">
                    <template v-slot:item.1>
                        <TethysStatus :payload="payload" />
                    </template>

                    <template v-slot:item.2>
                        <RAFHelper :payload="payload" />
                    </template>

                    <template v-slot:actions>
                        <v-btn :disabled="currentStep === 1" @click="currentStep--">
                            Back
                        </v-btn>
                        <v-btn :disabled="currentStep === items.length" @click="currentStep++">
                            Next
                        </v-btn>
                    </template>
                </v-stepper>
            </v-card-text>
        </v-card>
    </div>
</template>

<script setup>
import { ref, defineProps, computed } from 'vue';
import TethysStatus from './TethysStatus.vue';
import RAFHelper from './RAFHelper.vue';
import { useWorkflowStore } from '../../stores/workflow';

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

const triggerNext = () => {
    workflowStore.triggerTransition('NextRafManagerToEnrichiExport');
};
</script>
