<template>
    <v-container>
        <v-card>
            <v-card-text>
                <v-form ref="form" v-model="formValid">
                    <div v-if="loading" class="text-center py-8">
                        <v-progress-circular indeterminate color="primary" class="mt-4"></v-progress-circular>
                    </div>
                    <div v-else>
                        <v-data-table
                            :items="rows"
                            :headers="headers"
                            item-value="id"
                            class="elevation-1 rwa-data-table"
                            :items-per-page="10"
                            :footer-props="{ 'items-per-page-options': [5, 10, 20, -1] }"
                            :loading="loading"
                            loading-text="Loading data..."
                        >
                            <template v-for="header in headers" #[`item.${header.value}`]="{ item }">
                                <span>{{ item[header.value] }}</span>
                            </template>
                        </v-data-table>

                        <v-row class="mt-6">
                            <v-col cols="12" class="d-flex justify-space-between align-center">
                                <v-btn
                                    :disabled="!formValid || loading"
                                    color="primary"
                                    elevation="2"
                                    large
                                    @click="submit"
                                >
                                    <v-icon left>mdi-check-circle</v-icon>
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
import { getAddToBddColumns, getAddToBddData, submitAddToBdd } from '../../api/bddManagerApi';
import type { HecateInterneHistoriqueDto } from '../../types/bddManager';

const loading = ref(false);
const formValid = ref(true);
const rows = ref<HecateInterneHistoriqueDto[]>([]);
const headers = ref<any[]>([]);

const fetchData = async () => {
    loading.value = true;
    console.log('Fetching Add to BDD data...');
    try {
        const [columnsResponse, dataResponse] = await Promise.all([
            getAddToBddColumns(),
            getAddToBddData(),
        ]);
        headers.value = columnsResponse.data.map((c: any) => ({ title: c.header, value: c.field }));
        rows.value = dataResponse.data;
        console.log('Successfully fetched Add to BDD data:', { headers: headers.value, rows: rows.value });
    } catch (error) {
        console.error('Error fetching data:', error);
    } finally {
        loading.value = false;
    }
};

const submit = async () => {
    loading.value = true;
    console.log('Submitting Add to BDD data:', rows.value);
    try {
        await submitAddToBdd(rows.value);
        console.log('Successfully submitted Add to BDD data.');
    } catch (error) {
        console.error('Error submitting data:', error);
    } finally {
        loading.value = false;
    }
};

onMounted(fetchData);
</script>

<style scoped>
.rwa-data-table {
    border-radius: 12px !important;
    overflow: hidden;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1) !important;
}
</style>
