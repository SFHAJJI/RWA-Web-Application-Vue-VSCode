<script setup lang="ts">
import { onMounted, computed, ref } from 'vue';
import { useWorkflowStore } from '../stores/workflow';

import AuditPanel from './AuditPanel.vue';
import UploadInventory from './steps/UploadInventory.vue';
import RwaCategoryManager from './steps/RwaCategoryManager.vue';
import BddManager from './steps/BddManager.vue';
import RafManager from './steps/RafManager.vue';
import FichierEnrichiGeneration from './steps/FichierEnrichiGeneration.vue';

const store = useWorkflowStore();

const validationHeaders = [
    { title: 'Type', key: 'Status' },          // Changed from 'status' to 'Status'
    { title: 'File', key: 'FileName' },        // Changed from 'fileName' to 'FileName'
    { title: 'Message', key: 'Message' }       // Changed from 'message' to 'Message'
];

// local state for whether to render raw JSON for a given message key
const expandedRaw = ref<Record<string, boolean>>({});

function toggleRawFor(item: any) {
    const key = detailKeyFor(item);
    expandedRaw.value[key] = !expandedRaw.value[key];
}

function isRawFor(item: any) {
    return !!expandedRaw.value[detailKeyFor(item)];
}

function detailKeyFor(item: any) {
    // attempt to produce a stable-ish key for the row
    if (!item) return '__unknown__';
    if (item.id) return `id:${item.id}`;
    if (item.message) return `msg:${item.message}`;
    return JSON.stringify(item).slice(0, 64);
}

function groupedByValidator(errorData: any): Record<string, any[]> {
    if (!errorData) return {};
    // If it's an array, try to group by a "validator" or "name" property
    if (Array.isArray(errorData)) {
        const map: Record<string, any[]> = {};
        errorData.forEach((ed: any) => {
            const key = (ed && (ed.validator || ed.name)) || 'details';
            if (!map[key]) map[key] = [];
            map[key].push(ed);
        });
        return map;
    }
    // If it's an object with subkeys, treat each key as a validator
    if (typeof errorData === 'object') {
        // If object values are arrays or objects, assume keys are validator names
        const keys = Object.keys(errorData);
        const probable = keys.length > 0 && keys.some(k => typeof errorData[k] !== 'string');
        if (probable) return errorData;
        return { details: [errorData] };
    }
    // fallback
    return { details: [errorData] };
}

// isTabularGroup removed — we normalize groups into headers/items using helper functions

function getTabularKeys(group: any): string[] {
    if (!Array.isArray(group) || group.length === 0) return [];
    const first = group[0];
    if (!first || typeof first !== 'object' || Array.isArray(first)) return [];
    return Object.keys(first);
}

function getDataTableHeaders(group: any) {
    const keys = getTabularKeys(group);
    if (keys.length > 0) return keys.map(k => ({ title: k, key: k }));
    return [{ title: 'Details', key: '_detail' }];
}

function normalizeGroupItems(group: any) {
    if (!group) return [];
    if (Array.isArray(group)) {
        // If tabular group, keep objects as-is; otherwise, wrap primitives
        return group.map(g => (g && typeof g === 'object' ? g : { _detail: g }));
    }
    return [{ _detail: group }];
}

function isAssetIdValidator(name: string | undefined) {
    return !!name && name.toLowerCase().includes('assetid');
}

function isUploadTemplateValidator(name: string | undefined) {
    return !!name && name.toLowerCase().includes('uploadtemplate');
}

function assetIdRowsFromErrorData(errorData: any) {
    if (!errorData) return [];
    if (Array.isArray(errorData)) {
        return errorData.map((row: any) => {
            const filename = row?.SavedFilePath ?? row?.FileName ?? row?.Fichier ?? row?.filename ?? row?.fileName ?? '';
            return {
                filename: filename || '(unknown file)',
                message: 'This file contains empty Asset IDs'
            };
        });
    }
    return [{ filename: '(unknown file)', message: String(errorData) }];
}

function fileNameFor(item: any): string {
    if (!item) return '';
    const fileName = item?.FileName ?? item?.fileName ?? item?.ErrorData?.SavedFilePath ?? item?.errorData?.FileName;
    return fileName || '(general validation)';  // Show fallback when filename is null/empty
}

function messageType(status: any) {
    if (!status) return '';
    // Handle numeric enum values (0,1,2) or string names
    if (typeof status === 'number') {
        if (status === 2) return 'Error';
        if (status === 1) return 'Warning';
        return 'Success';
    }
    const s = String(status).toLowerCase();
    if (s.includes('error')) return 'Error';
    if (s.includes('warning')) return 'Warning';
    return String(status);
}

const hasWarnings = computed(() => {
    const msgs = store.currentValidationMessages || [] as any[];
    return msgs.some((m: any) => messageType(m.status) === 'Warning');
});

async function copyRaw(item: any) {
    const payload = item && (item.errorData ?? item.ErrorData ?? item);
    const text = typeof payload === 'object' ? JSON.stringify(payload, null, 2) : String(payload ?? '');
    try {
        if (navigator && (navigator as any).clipboard && (navigator as any).clipboard.writeText) {
            await (navigator as any).clipboard.writeText(text);
            // small, silent success; consider a toast in the app later
            return;
        }
    } catch (err) {
        // fall through to textarea fallback
    }

    // fallback for older browsers: append a textarea
    try {
        const ta = document.createElement('textarea');
        ta.value = text;
        ta.style.position = 'fixed';
        ta.style.left = '-9999px';
        document.body.appendChild(ta);
        ta.select();
        document.execCommand('copy');
        document.body.removeChild(ta);
    } catch (err) {
        // last resort: show JSON in a new window
        const w = window.open('', '_blank');
        if (w) {
            w.document.write('<pre>' + text.replace(/</g, '&lt;') + '</pre>');
            w.document.close();
        }
    }
}

// explicit component rendering is used in the template below

onMounted(() => {
    store.fetchWorkflowStatus();
});

function resetWithConfirm() {
    // use browser confirm for a simple accessible confirmation
    resetDialog.value = true;
}

const resetDialog = ref(false);
function doReset() {
    resetDialog.value = false;
    store.resetWorkflow();
}

</script>

<template>
    <v-container fluid>
    <v-stepper v-model="store.uiActiveIndex" @update:modelValue="(v) => store.requestUiActiveIndex(Number(v))">
            <v-stepper-header>
                <v-stepper-item
                    v-for="(step, index) in store.workflowSteps"
                    :key="step.id"
                    :title="step.stepName"
                    :value="index"
                    :complete="store.isStepComplete(index)"
                    :error="store.isStepWithIssues(index)"
                    :rules="[() => !store.isStepWithIssues(index)]"
                    :subtitle="store.isStepError(index) ? 'erreurs de validation' : store.isStepWarning(index) ? 'avertissements' : ''"
                    complete-icon="mdi-check"
                >
                </v-stepper-item>
            </v-stepper-header>

            <v-stepper-window>
                <v-stepper-window-item v-for="(step, index) in store.workflowSteps" :key="`${step.id}-content`" :value="index">
                    <!-- FIXED: Only show validation messages for the active stepper tab -->
                    <div v-if="index === store.uiActiveIndex && store.currentValidationMessages && store.currentValidationMessages.length">
                        <v-alert type="warning" dense>
                            This step has validation messages. Fix them or re-run validation.
                        </v-alert>

                        <!-- Vuetify data table to display validation messages -->
                        <v-data-table
                            :items="store.currentValidationMessages"
                            item-value="Message"
                            :headers="validationHeaders"
                            :group-by="[{ key: 'ValidatorName', order: 'asc' }]"
                            class="mt-2"
                            hide-default-footer
                        >
                            <template #group-header="{ item, columns, toggleGroup, isGroupOpen }">
                                <tr>
                                    <td :colspan="columns.length" class="cursor-pointer" @click="toggleGroup(item)">
                                        <div class="d-flex align-center">
                                            <v-btn :icon="isGroupOpen(item) ? '$expand' : '$next'" color="medium-emphasis" density="comfortable" size="small" variant="outlined" />
                                            <span class="ms-4">{{ item.value || '(validator)' }}</span>
                                        </div>
                                    </td>
                                </tr>
                            </template>
                            <!-- render the Type column as a small label -->
                            <template #item.Status="{ item }">
                                <div>{{ messageType(item.Status) }}</div>
                            </template>
                            <template #item.FileName="{ item }">
                                <div>{{ fileNameFor(item) }}</div>
                            </template>
                            <!-- Expanded area for per-row ErrorData -->
                            <template #item.data-table-expand="{ item }">
                                <v-card flat class="ma-2 pa-3">
                                    <div class="d-flex align-center justify-space-between">
                                        <div>
                                            <strong>Details</strong>
                                            <span class="ml-2 text--secondary">(grouped by validator when available)</span>
                                        </div>
                                        <div class="d-flex align-center">
                                            <v-switch
                                                :model-value="isRawFor(item)"
                                                :label="isRawFor(item) ? 'Raw JSON' : 'Structured'"
                                                hide-details
                                                @update:modelValue="() => toggleRawFor(item)"
                                            />
                                            <v-btn icon small class="ml-2" @click.prevent="copyRaw(item)" :title="'Copy raw JSON'">
                                                <v-icon small>mdi-content-copy</v-icon>
                                            </v-btn>
                                        </div>
                                    </div>

                                        <div class="mt-3">
                                        <div v-if="isRawFor(item)">
                                            <pre style="white-space: pre-wrap; word-break: break-word; max-height: 360px; overflow:auto;">{{ JSON.stringify(item.ErrorData, null, 2) }}</pre>
                                        </div>
                                        <div v-else>
                                            <div v-if="item.ErrorData">
                                                <div v-for="(group, validator) in groupedByValidator(item.ErrorData)" :key="validator" class="mb-3">
                                                    <div class="font-weight-medium">{{ validator }}</div>
                                                    <div>
                                                        <!-- Special-case AssetIdValidator -->
                                                        <div v-if="isAssetIdValidator(validator)">
                                                            <v-data-table dense :items="assetIdRowsFromErrorData(group)" :headers="[{ title: 'File', key: 'filename' }, { title: 'Message', key: 'message' }]" hide-default-footer class="mt-2" />
                                                        </div>
                                                        <!-- Special-case UploadTemplateValidator -->
                                                        <div v-else-if="isUploadTemplateValidator(validator)">
                                                            <v-data-table dense :items="[{ filename: (item.errorData && item.errorData.length ? (item.errorData.join(', ')) : '(expected columns)') , message: 'This file does not respect the expected template: ' + (item.errorData && item.errorData.length ? item.errorData.join(', ') : '') } ]" :headers="[{ title: 'File', key: 'filename' }, { title: 'Message', key: 'message' }]" hide-default-footer class="mt-2" />
                                                        </div>
                                                        <!-- Generic rendering -->
                                                        <div v-else>
                                                            <v-data-table
                                                                dense
                                                                :items="normalizeGroupItems(group)"
                                                                :headers="getDataTableHeaders(group)"
                                                                hide-default-footer
                                                                class="mt-2"
                                                            >
                                                                <template #item._detail="{ item }">
                                                                    <div style="padding:6px 8px;">
                                                                        <pre style="white-space: pre-wrap; word-break: break-word; margin:0">{{ typeof item._detail === 'object' ? JSON.stringify(item._detail, null, 2) : String(item._detail) }}</pre>
                                                                    </div>
                                                                </template>
                                                            </v-data-table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div v-else>
                                                <em>No extra details available</em>
                                            </div>
                                        </div>
                                    </div>
                                </v-card>
                            </template>
                        </v-data-table>

                        <div class="mt-2">
                            <v-btn color="primary" @click="store.revalidateCurrent()">Re-validate</v-btn>
                            <v-btn v-if="hasWarnings" color="secondary" @click="store.forceNext()">Force Next</v-btn>
                        </div>
                    </div>
                    <div>
                        <UploadInventory v-if="step.stepName === 'Upload inventory'" />
                        <RwaCategoryManager v-else-if="step.stepName === 'RWA Category Manager'" />
                        <BddManager v-else-if="step.stepName === 'BDD Manager'" />
                        <RafManager v-else-if="step.stepName === 'RAF Manager'" />
                        <FichierEnrichiGeneration v-else-if="step.stepName === 'Fichier Enrichie Generation'" />
                        <div v-else>
                            <p>Step component not found: {{ step.stepName }}</p>
                        </div>
                    </div>
                </v-stepper-window-item>
            </v-stepper-window>
        </v-stepper>
            <div class="d-flex justify-space-between mt-4">
            <v-btn @click="store.navigatePrevVisual()" :disabled="(store.uiActiveIndex ?? 0) === 0">Previous</v-btn>

            <!-- Reset is always available; placed between previous and next for ergonomics -->
            <v-btn color="error" class="mx-2" @click="resetWithConfirm">Reset</v-btn>

            <v-btn @click="store.navigateNextVisual()" :disabled="!store.canNavigateNext()">Next</v-btn>
        </div>
        <v-dialog v-model="resetDialog" width="480">
            <v-card>
                <v-card-title>Confirm Reset</v-card-title>
                <v-card-text>Reset workflow and clear imported inventory? This cannot be undone.</v-card-text>
                <v-card-actions>
                    <v-spacer />
                    <v-btn text @click="resetDialog = false">Cancel</v-btn>
                    <v-btn color="error" @click="doReset">Reset</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-container>

    <!-- Audit Panel - Always available -->
    <AuditPanel />
</template>
