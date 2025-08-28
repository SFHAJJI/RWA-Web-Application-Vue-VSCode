<template>
    <v-container>
        <v-card class="modern-card">
            <v-card-title class="headline">
                Add to BDD
            </v-card-title>
            <v-card-subtitle>
                Review the following entries before submission.
            </v-card-subtitle>
            <v-card-text>
                <v-form ref="form" v-model="formValid">
                    <div v-if="loading" class="text-center py-8">
                        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
                        <p class="mt-4 text-grey-darken-1">Loading data...</p>
                    </div>
                    <div v-else>
                        <v-toolbar flat class="mb-4 rounded-lg">
                            <v-spacer></v-spacer>
                            <v-btn @click="exportToXLSX" color="success">
                                <v-icon left>mdi-file-excel-outline</v-icon>
                                Export
                            </v-btn>
                        </v-toolbar>

                        <v-data-table
                            :items="rows"
                            :headers="headers"
                            item-value="id"
                            class="elevation-1 rwa-data-table"
                            :items-per-page="10"
                            :footer-props="{ 'items-per-page-options': [5, 10, 20, -1] }"
                            :loading="loading"
                            loading-text="Loading data..."
                            density="compact"
                        >
                            <template v-for="header in headers" #[`item.${header.value}`]="{ item }">
                                <div class="cell-content">
                                    <span v-if="header.value === 'dateEcheance'">{{ formatDateForDisplay(item.dateEcheance) }}</span>
                                    <span v-else>{{ item[header.value] }}</span>
                                </div>
                            </template>
                        </v-data-table>

                        <v-row class="mt-6">
                            <v-col cols="12" class="d-flex justify-end align-center">
                                <v-btn
                                    :disabled="!formValid || loading"
                                    color="primary"
                                    elevation="2"
                                    large
                                    @click="submit"
                                    class="submit-btn"
                                >
                                    <v-icon left>mdi-check-circle-outline</v-icon>
                                    Submit
                                </v-btn>
                            </v-col>
                        </v-row>
                    </div>
                </v-form>
            </v-card-text>
        </v-card>
    </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import * as XLSX from 'xlsx';
import dayjs from 'dayjs';
import customParseFormat from 'dayjs/plugin/customParseFormat';
import { getAddToBddColumns, getAddToBddData, submitAddToBdd } from '../../api/bddManagerApi';
import type { HecateInterneHistoriqueDto } from '../../types/bddManager';

dayjs.extend(customParseFormat);

const loading = ref(false);
const formValid = ref(true);
const rows = ref<HecateInterneHistoriqueDto[]>([]);
const headers = ref<any[]>([]);

const formatDateForDisplay = (date: string | null) => {
    if (!date) return '';
    const d = dayjs(date);
    return d.isValid() ? d.format('DD/MM/YYYY') : date.toString();
};

const exportToXLSX = () => {
    const headerTitles = headers.value.map(h => h.title);
    const headerKeys = headers.value.map(h => h.value);

    const dataToExport = rows.value.map(row => {
        const rowData: any = {};
        headerKeys.forEach(key => {
            let value = (row as any)[key];
            if (key === 'dateEcheance') {
                value = formatDateForDisplay(value);
            }
            rowData[key] = value;
        });
        return rowData;
    });

    const worksheet = XLSX.utils.json_to_sheet(dataToExport, { header: headerKeys });
    XLSX.utils.sheet_add_aoa(worksheet, [headerTitles], { origin: 'A1' });

    const colWidths = headerKeys.map((key, i) => {
        const titleWidth = headerTitles[i].length;
        const dataWidths = dataToExport.map(row => (row[key] || '').toString().length);
        return { wch: Math.max(titleWidth, ...dataWidths) + 2 };
    });
    worksheet['!cols'] = colWidths;

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Add to BDD');
    XLSX.writeFile(workbook, 'add_to_bdd_export.xlsx');
};

const fetchData = async () => {
    loading.value = true;
    try {
        const [columnsResponse, dataResponse] = await Promise.all([
            getAddToBddColumns(),
            getAddToBddData(),
        ]);
        headers.value = columnsResponse.data.map((c: any) => ({ title: c.header, value: c.field }));
        rows.value = dataResponse.data;
    } catch (error) {
        console.error('Error fetching data:', error);
    } finally {
        loading.value = false;
    }
};

const submit = async () => {
    loading.value = true;
    try {
        await submitAddToBdd(rows.value);
    } catch (error) {
        console.error('Error submitting data:', error);
    } finally {
        loading.value = false;
    }
};

onMounted(fetchData);
</script>

<style scoped>
.modern-card {
    border-radius: 16px;
    box-shadow: 0 8px 24px rgba(0,0,0,0.1);
}
.headline {
    font-weight: 500;
    color: #1976D2;
}
.rwa-data-table {
    border-radius: 12px;
    overflow: hidden;
}
.rwa-data-table :deep(th) {
    font-weight: 600 !important;
    color: #4a4a4a !important;
    background-color: #f5f5f5;
}
.rwa-data-table :deep(td) {
    vertical-align: middle;
}
.cell-content {
    padding-top: 4px;
    padding-bottom: 4px;
    min-width: 180px;
}
.submit-btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}
</style>
