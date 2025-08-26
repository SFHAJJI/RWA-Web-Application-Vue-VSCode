<template>
    <div>
        <div v-if="!sortedPayload || sortedPayload.length === 0">
            <SkeletonLoader />
        </div>
        <v-data-table v-else :headers="headers" :items="sortedPayload">
            <template v-slot:item.IsMappingTethysSuccessful="{ value }">
                <v-chip :color="getColor(value)" :text="value ? 'OK' : 'KO'" size="x-small"></v-chip>
            </template>
        </v-data-table>
    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import SkeletonLoader from '../loaders/SkeletonLoader.vue';

interface TethysDto {
    NumLigne: number;
    Source: string;
    Cpt: string;
    Raf: string;
    CptTethys: string;
    IsGeneric: boolean;
    IsMappingTethysSuccessful: boolean;
}

const props = defineProps({
    payload: {
        type: Array as () => TethysDto[],
        required: true
    }
});

const sortedPayload = computed(() => {
    if (!props.payload) {
        return [];
    }
    return [...props.payload].sort((a, b) => {
        if (a.IsMappingTethysSuccessful === b.IsMappingTethysSuccessful) {
            return 0;
        }
        return a.IsMappingTethysSuccessful ? 1 : -1;
    });
});

const headers = ref([
    { title: 'Num Ligne', value: 'NumLigne' },
    { title: 'Source', value: 'Source' },
    { title: 'Cpt', value: 'Cpt' },
    { title: 'Raf', value: 'Raf' },
    { title: 'Cpt Tethys', value: 'CptTethys' },
    { title: 'Is Generic', value: 'IsGeneric' },
    { title: 'Tethys Status', value: 'IsMappingTethysSuccessful' },
]);

const getColor = (isMappingTethysSuccessful) => {
    if (isMappingTethysSuccessful) return 'success'
    else return 'error'
};
</script>
