<template>
    <v-data-table-server :headers="headers" :items="serverItems" :items-length="totalItems"
        :loading="loading" @update:options="loadItems">
    </v-data-table-server>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import api from '../../api';

const props = defineProps({
    headers: {
        type: Array,
        required: true
    },
    dataUrl: {
        type: String,
        required: true
    }
});

const serverItems = ref([]);
const totalItems = ref(0);
const loading = ref(true);

const loadItems = async ({ page, itemsPerPage, sortBy }) => {
    loading.value = true;
    try {
        const response = await api.post(props.dataUrl, {
            page: page || 1,
            itemsPerPage: itemsPerPage || 10,
            sortBy: sortBy || []
        });
        serverItems.value = response.data.items;
        totalItems.value = response.data.totalItems;
    } catch (error) {
        console.error('Failed to load data:', error);
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    loadItems({ page: 1, itemsPerPage: 10, sortBy: [] });
});
</script>
