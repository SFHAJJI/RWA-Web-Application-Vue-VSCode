<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useWorkflowStore } from '../../stores/workflow';
import { useToastStore } from '../../stores/toast';
import { validateFiles } from '../../validation/validateInventoryFilenames';
import SkeletonLoader from '../loaders/SkeletonLoader.vue';

const store = useWorkflowStore();
const files = ref([]);
const snackbarMessage = ref('');
const validationResult = ref<any | null>(null);
const filesValid = ref(true);

// Check if current upload step is successfully finished
const isUploadSuccessful = computed(() => {
    const uploadStep = store.workflowSteps.find(s => s.stepName === 'Upload Inventory');
    return uploadStep?.status === 'SuccessfulFinish';
});

// validate immediately when files change so we can show the toast without waiting for upload click
// Watch the files ref deeply and also respond to fileName changes from v-file-input
watch(files, (newFiles) => {
    const res = validateFiles(newFiles || []);
    // avoid re-firing the same toast if validation result is unchanged
    if (!validationResult.value || JSON.stringify(validationResult.value.badNames) !== JSON.stringify(res.badNames)) {
        validationResult.value = res;
        filesValid.value = res.valid;
        if (!res.valid) {
            snackbarMessage.value = `Invalid filenames: ${res.badNames.join(', ')}`;
            const t = useToastStore();
            t.pushToast({ type: 'warning', message: snackbarMessage.value });
        }
    }
}, { deep: true });

// Clear local file selection when the server reset clears the upload payload or after successful upload
// Clear files when the upload step payload is cleared, and when the store.resetCounter increments
watch(() => store.workflowSteps, (steps) => {
    const uploadStep = (steps || []).find((s: any) => s.stepName === 'Upload Inventory');
    if (!uploadStep || !uploadStep.dataPayload) {
        files.value = [];
    }
}, { deep: true });

watch(() => (store as any).resetCounter, () => {
    files.value = [];
});

const uploadFiles = async () => {
    // client-side validation mirror of server: validate filenames before attempting upload
    const result = validateFiles(files.value || []);
    if (!result.valid) {
        snackbarMessage.value = `Invalid filenames: ${result.badNames.join(', ')}`;
        const t = useToastStore();
        t.pushToast({ type: 'warning', message: snackbarMessage.value });
        return;
    }

    store.setStepLoading('upload-inventory', true);
    const formData = new FormData();
    for (const file of files.value) {
        // the server expects a single file parameter named 'file' (IFormFile)
        formData.append('file', file);
    }
    await store.uploadFiles(formData);
    // clear local selection after successful upload
    files.value = [];
    store.setStepLoading('upload-inventory', false);
};

const errorRows = computed(() => {
    const currentStep = store.workflowSteps.find(s => s.status.startsWith('Current'));
    if (!currentStep || !currentStep.dataPayload) return [];
    const data = JSON.parse(currentStep.dataPayload);
    return data.Messages ? data.Messages[0].ErrorData : [];
});

const validationError = computed(() => {
    const currentStep = store.workflowSteps.find(s => s.status.startsWith('Current'));
    if (!currentStep || !currentStep.dataPayload) return null;
    const data = JSON.parse(currentStep.dataPayload);
    return data.Messages ? data.Messages[0].Message : null;
});
</script>

<template>
    <v-container>
        <!-- Show success alert when upload is successfully finished -->
        <v-alert v-if="isUploadSuccessful" type="success" variant="outlined" class="mb-4">
            This step is successfully finished.
        </v-alert>

        <!-- Subtle inline loader on the button only; avoid full skeleton during validation/transition -->
        <!-- Removed global skeleton to prevent flicker between validation and next step -->

        <!-- Show upload component only when step is not successfully finished and not uploading -->
        <div v-else-if="!isUploadSuccessful">
            <v-row>
                <v-col>
                    <h3>Upload Inventory Files</h3>
                    <v-file-input v-model="files" :show-size="1000" color="deep-purple-accent-4" label="File input"
                        placeholder="Select your files" variant="outlined" counter multiple accept=".xlsx">
                        <template v-slot:selection="{ fileNames }">
                            <template v-for="(fileName, index) in fileNames" :key="fileName">
                                <v-chip class="me-2" color="deep-purple-accent-4" size="small" label closable
                                    @click:close="files.splice(index, 1)">
                                    {{ fileName }}
                                </v-chip>
                            </template>
                        </template>
                    </v-file-input>
                    <v-btn @click="uploadFiles" :disabled="files.length === 0 || !filesValid" :loading="store.stepLoading['upload-inventory']">Upload</v-btn>
                    <v-btn @click="store.triggerTransition('Revalidate')" v-if="validationError">Revalidate</v-btn>
                    <!-- global toast via window event 'workflow-toast' will be used -->
                </v-col>
            </v-row>
            <v-row v-if="validationError">
                <v-col>
                    <v-alert type="error" class="mb-4">{{ validationError }}</v-alert>
                    <v-data-table :headers="[{ title: 'Identifiant Origine', value: 'IdentifiantOrigine' }]"
                        :items="errorRows"></v-data-table>
                </v-col>
            </v-row>
        </div>
    </v-container>
</template>
