<template>
    <div>
        <v-btn @click="reloadData" class="mb-4">Reload</v-btn>
        <v-btn @click="exportToExcel" class="mb-4 ml-2">Export</v-btn>
        <v-progress-linear v-if="progress > 0 && progress < totalItems" :model-value="progress" :max="totalItems" height="25">
            <template v-slot:default="{ value }">
                <strong>{{ Math.ceil(value) }}%</strong>
            </template>
        </v-progress-linear>
        <h3>Failed Mappings ({{ failedMappings.length }})</h3>
        <v-data-table :headers="headers" :items="failedMappings" class="mb-4">
            <template v-slot:item.isMappingTethysSuccessful="{ value }">
                <v-chip :color="getColor(value)" :text="value ? 'OK' : 'KO'" size="x-small"></v-chip>
            </template>
        </v-data-table>
        <h3>Successful Mappings ({{ successfulMappings.length }})</h3>
        <v-data-table :headers="headers" :items="successfulMappings">
            <template v-slot:item.isMappingTethysSuccessful="{ value }">
                <v-chip :color="getColor(value)" :text="value ? 'OK' : 'KO'" size="x-small"></v-chip>
            </template>
        </v-data-table>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { getTethysStatus, updateTethysStatus } from '../../api/tethysApi';
import { HubConnectionBuilder } from '@microsoft/signalr';
import * as XLSX from 'xlsx';

interface TethysDto {
    numLigne: number;
    source: string;
    cpt: string;
    raf: string;
    cptTethys: string;
    isGeneric: boolean;
    isMappingTethysSuccessful: boolean;
}

const payload = ref<TethysDto[]>([]);
const progress = ref(0);
const totalItems = ref(0);

const connection = new HubConnectionBuilder()
    .withUrl("/workflowHub")
    .build();

connection.on("ReceiveTethysUpdate", (tethysDto: any) => {
    console.log('Received Tethys DTO:', tethysDto);
    const index = payload.value.findIndex(item => item.numLigne === tethysDto.numLigne);
    if (index !== -1) {
        payload.value[index] = {
            numLigne: tethysDto.numLigne,
            source: tethysDto.source,
            cpt: tethysDto.cpt,
            raf: tethysDto.raf,
            cptTethys: tethysDto.cptTethys,
            isGeneric: tethysDto.isGeneric,
            isMappingTethysSuccessful: tethysDto.isMappingTethysSuccessful,
        };
    } else {
        payload.value.push({
            numLigne: tethysDto.numLigne,
            source: tethysDto.source,
            cpt: tethysDto.cpt,
            raf: tethysDto.raf,
            cptTethys: tethysDto.cptTethys,
            isGeneric: tethysDto.isGeneric,
            isMappingTethysSuccessful: tethysDto.isMappingTethysSuccessful,
        });
    }
    progress.value++;
});

connection.on("ReceiveTethysTotalItems", (total: number) => {
    totalItems.value = total;
});

const fetchData = async (initial = false) => {
    if (initial) {
        payload.value = [];
    }
    const data = await getTethysStatus("all", payload.value.length.toString(), 20);
    payload.value = [...payload.value, ...data.items];
};

onMounted(async () => {
    await connection.start();
    await fetchData();
});

const reloadData = async () => {
    progress.value = 0;
    payload.value = [];
    await updateTethysStatus();
};

const exportToExcel = () => {
    const worksheet = XLSX.utils.json_to_sheet(payload.value);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, "Tethys Status");
    XLSX.writeFile(workbook, "tethys-status.xlsx");
};

const successfulMappings = computed(() => {
    return payload.value.filter(item => item.isMappingTethysSuccessful);
});

const failedMappings = computed(() => {
    return payload.value.filter(item => !item.isMappingTethysSuccessful);
});

const headers = ref([
    { title: 'Num Ligne', key: 'numLigne' },
    { title: 'Source', key: 'source' },
    { title: 'Cpt', key: 'cpt' },
    { title: 'Raf', key: 'raf' },
    { title: 'Cpt Tethys', key: 'cptTethys' },
    { title: 'Is Generic', key: 'isGeneric' },
    { title: 'Tethys Status', key: 'isMappingTethysSuccessful' },
]);

const getColor = (isMappingTethysSuccessful) => {
    if (isMappingTethysSuccessful) return 'success'
    else return 'error'
};
</script>
