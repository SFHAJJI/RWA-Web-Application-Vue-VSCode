<template>
    <v-container>
        <v-card class="modern-card">
            <v-card-title class="headline">
                OBL Validation
            </v-card-title>
            <v-card-subtitle>
                Review and complete the following entries before submission.
            </v-card-subtitle>
            <v-card-text>
                <v-form ref="form" v-model="formValid">
                    <div v-if="loading" class="text-center py-8">
                        <v-progress-circular indeterminate color="primary" size="64"></v-progress-circular>
                        <p class="mt-4 text-grey-darken-1">Loading validation data...</p>
                    </div>
                    <div v-else>
                        <v-toolbar flat class="mb-4 rounded-lg">
                            <v-toolbar-title class="text-body-2 font-weight-bold">Bulk Actions</v-toolbar-title>
                            <v-spacer></v-spacer>
                            
                            <v-text-field
                                v-model="globalTaux"
                                label="Apply Taux to All"
                                variant="outlined"
                                density="compact"
                                hide-details="auto"
                                class="mx-2"
                                style="max-width: 200px;"
                                :rules="[rules.decimal]"
                            ></v-text-field>
                            <v-btn @click="applyTauxToAll" color="secondary" :disabled="rules.decimal(globalTaux) !== true" class="mr-2">
                                <v-icon left>mdi-arrow-down-bold-box-outline</v-icon>
                                Apply Taux
                            </v-btn>

                            <v-text-field
                                v-model="globalDate"
                                label="Apply Date to All (dd/mm/yyyy)"
                                variant="outlined"
                                density="compact"
                                hide-details="auto"
                                class="mx-2"
                                style="max-width: 200px;"
                                :rules="[val => rules.date(val, undefined, earliestMinDate)]"
                            ></v-text-field>
                            <v-btn @click="applyDateToAll" color="secondary" :disabled="rules.date(globalDate, undefined, earliestMinDate) !== true" class="mr-4">
                                <v-icon left>mdi-arrow-down-bold-box-outline</v-icon>
                                Apply Date
                            </v-btn>

                            <v-btn @click="exportToXLSX" color="success">
                                <v-icon left>mdi-file-excel-outline</v-icon>
                                Export
                            </v-btn>
                        </v-toolbar>

                        <v-data-table
                            :items="rows"
                            :headers="headers"
                            item-value="numLigne"
                            class="elevation-1 rwa-data-table"
                            :items-per-page="10"
                            :footer-props="{ 'items-per-page-options': [5, 10, 20, -1] }"
                            :loading="loading"
                            loading-text="Loading data..."
                            :row-props="getRowClass"
                            density="compact"
                        >
                            <template v-for="header in headers" #[`item.${header.value}`]="{ item }">
                                <div class="cell-content">
                                    <template v-if="header.value === 'tauxObligation'">
                                        <v-text-field
                                            v-if="item.isTauxObligationInvalid"
                                            v-model="item.tauxObligation"
                                            :rules="[rules.required, rules.decimal]"
                                            variant="outlined"
                                            density="compact"
                                            hide-details="auto"
                                            placeholder="Enter rate"
                                            class="editable-field"
                                        >
                                            <template v-slot:append-inner>
                                                <v-tooltip location="top" v-if="isFieldInvalid(item, 'tauxObligation')">
                                                    <template v-slot:activator="{ props }">
                                                        <v-icon v-bind="props" color="warning" size="small">mdi-alert-circle-outline</v-icon>
                                                    </template>
                                                    <span>This field is required.</span>
                                                </v-tooltip>
                                            </template>
                                        </v-text-field>
                                        <span v-else>{{ item.tauxObligation }}</span>
                                    </template>
                                    <template v-else-if="header.value === 'dateMaturite'">
                                        <v-text-field
                                            v-if="item.isDateMaturiteInvalid"
                                            v-model="item.dateMaturite"
                                            :rules="[rules.required, val => rules.date(val, item.periodeCloture)]"
                                            variant="outlined"
                                            density="compact"
                                            hide-details="auto"
                                            placeholder="dd/mm/yyyy"
                                            class="editable-field"
                                        >
                                            <template v-slot:append-inner>
                                                <v-tooltip location="top" v-if="isFieldInvalid(item, 'dateMaturite')">
                                                    <template v-slot:activator="{ props }">
                                                        <v-icon v-bind="props" color="warning" size="small">mdi-alert-circle-outline</v-icon>
                                                    </template>
                                                    <span>Date is invalid or before closing period.</span>
                                                </v-tooltip>
                                            </template>
                                        </v-text-field>
                                        <span v-else>{{ item.dateMaturite }}</span>
                                    </template>
                                    <span v-else>{{ item[header.value] }}</span>
                                </div>
                            </template>
                        </v-data-table>

                        <v-row class="mt-6">
                            <v-col cols="12" class="d-flex justify-end align-center">
                                <v-btn
                                    :disabled="isSubmitDisabled"
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
import { ref, onMounted, computed, watch } from 'vue';
import * as XLSX from 'xlsx';
import dayjs from 'dayjs';
import customParseFormat from 'dayjs/plugin/customParseFormat';
import isSameOrAfter from 'dayjs/plugin/isSameOrAfter';
import { getOblValidationColumns, getOblValidationData, submitOblValidation } from '../../api/bddManagerApi';
import type { HecateInventaireNormaliseDto } from '../../types/bddManager';

dayjs.extend(customParseFormat);
dayjs.extend(isSameOrAfter);

const loading = ref(false);
const formValid = ref(true);
const rows = ref<HecateInventaireNormaliseDto[]>([]);
const headers = ref<any[]>([]);
const globalDate = ref<string | null>(null);
const globalTaux = ref<number | null>(null);
const submitDisabled = ref(true);

watch(rows, (newValue) => {
    submitDisabled.value = newValue.some(isRowInvalid);
}, { deep: true });

const rules = {
    required: (value: any) => !!value || 'Required.',
    decimal: (value: any) => {
        if (value === null || value === '' || value === undefined) return true;
        const pattern = /^-?\d*\.?\d+$/;
        return pattern.test(value) || 'Invalid decimal.';
    },
    date: (value: any, periodeCloture?: string, minDateOverride?: string) => {
        if (!value) return 'Required.';
        const date = dayjs(value, 'DD/MM/YYYY', true);
        if (!date.isValid()) return 'Format must be dd/mm/yyyy.';

        const minDate = minDateOverride ? dayjs(minDateOverride) : getMinDate(periodeCloture);
        if (minDate && date.isBefore(minDate, 'day')) {
            return `Date must be on or after ${minDate.format('DD/MM/YYYY')}.`;
        }
        return true;
    }
};

const getMinDate = (periodeCloture?: string) => {
    if (!periodeCloture || periodeCloture.length !== 6) return undefined;
    try {
        const month = parseInt(periodeCloture.substring(0, 2));
        const year = parseInt(periodeCloture.substring(2, 6));
        return dayjs(new Date(year, month, 0));
    } catch {
        return undefined;
    }
};

const earliestMinDate = computed(() => {
    const dates = rows.value
        .filter(r => r.isDateMaturiteInvalid)
        .map(row => getMinDate(row.periodeCloture))
        .filter(date => date !== undefined) as dayjs.Dayjs[];
    if (dates.length === 0) return undefined;
    const earliest = dayjs(Math.min(...dates.map(d => d.valueOf())));
    return earliest.format('YYYY-MM-DD');
});

const isFieldInvalid = (row: HecateInventaireNormaliseDto, field: 'tauxObligation' | 'dateMaturite') => {
    if (field === 'tauxObligation') {
        return row.isTauxObligationInvalid && (rules.required(row.tauxObligation) !== true || rules.decimal(row.tauxObligation) !== true);
    }
    if (field === 'dateMaturite') {
        return row.isDateMaturiteInvalid && (rules.required(row.dateMaturite) !== true || rules.date(row.dateMaturite, row.periodeCloture) !== true);
    }
    return false;
};

const isRowInvalid = (row: HecateInventaireNormaliseDto) => {
    return isFieldInvalid(row, 'tauxObligation') || isFieldInvalid(row, 'dateMaturite');
};

const getRowClass = (item: any) => {
    if (isRowInvalid(item.item)) {
        return { class: 'invalid-row' };
    }
    return {};
};

const isSubmitDisabled = computed(() => {
    return submitDisabled.value;
});

const formatDateForDisplay = (date: string | null) => {
    if (!date) return '';
    const d = dayjs(date);
    return d.isValid() ? d.format('DD/MM/YYYY') : date.toString();
};

const applyDateToAll = () => {
    if (!globalDate.value || rules.date(globalDate.value, undefined, earliestMinDate.value) !== true) return;
    const dateToApply = globalDate.value;
    rows.value.forEach(row => {
        if (row.isDateMaturiteInvalid) {
            const minDate = getMinDate(row.periodeCloture);
            if (minDate && dayjs(dateToApply, 'DD/MM/YYYY').isSameOrAfter(minDate, 'day')) {
                row.dateMaturite = dateToApply;
            }
        }
    });
};

const applyTauxToAll = () => {
    if (globalTaux.value === null || rules.decimal(globalTaux.value) !== true) return;
    rows.value.forEach(row => {
        if (row.isTauxObligationInvalid) {
            row.tauxObligation = globalTaux.value;
        }
    });
};

const exportToXLSX = () => {
    const headerTitles = headers.value.map(h => h.title);
    const headerKeys = headers.value.map(h => h.value);

    const dataToExport = rows.value.map(row => {
        const rowData: any = {};
        headerKeys.forEach(key => {
            rowData[key] = (row as any)[key];
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

    dataToExport.forEach((row, index) => {
        if (isRowInvalid(rows.value[index])) {
            headerKeys.forEach((key, colIndex) => {
                const cellAddress = XLSX.utils.encode_cell({ r: index + 1, c: colIndex });
                if (!worksheet[cellAddress]) worksheet[cellAddress] = { v: '' };
                worksheet[cellAddress].s = { fill: { fgColor: { rgb: "FFFF0000" } } };
            });
        }
    });

    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'OBL Validation');
    XLSX.writeFile(workbook, 'obl_validation_export.xlsx');
};

const fetchData = async () => {
    loading.value = true;
    try {
        const [columnsResponse, dataResponse] = await Promise.all([
            getOblValidationColumns(),
            getOblValidationData(),
        ]);
        
        headers.value = columnsResponse.data.map((c: any) => ({ title: c.header, value: c.field, sortable: true }));
        rows.value = dataResponse.data.map((row: HecateInventaireNormaliseDto) => ({
            ...row,
            dateMaturite: formatDateForDisplay(row.dateMaturite)
        }));
    } catch (error) {
        console.error('Error fetching data:', error);
    } finally {
        loading.value = false;
    }
};

const submit = async () => {
    loading.value = true;
    try {
        const dataToSubmit = rows.value.map(row => {
            const { isTauxObligationInvalid, isDateMaturiteInvalid, ...rest } = row as any;
            return rest;
        });
        console.log('Submitting data:', JSON.stringify(dataToSubmit, null, 2));
        await submitOblValidation(dataToSubmit);
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
.editable-field {
    transition: all 0.2s ease-in-out;
}
.submit-btn:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 8px rgba(0,0,0,0.2);
}
.invalid-row {
    background-color: rgba(255, 0, 0, 0.1) !important;
}
</style>
