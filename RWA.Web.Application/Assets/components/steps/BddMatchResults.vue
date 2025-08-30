<template>
  <v-card class="mt-4" v-if="version">
    <v-card-title class="text-subtitle-1">BDD Match Results</v-card-title>
    <v-card-text>
      <div class="d-flex align-center mb-2" style="gap:8px;">
        <v-chip size="small" variant="tonal">Processed: {{ processed }} / {{ total }}</v-chip>
        <v-spacer />
        <v-btn size="small" variant="text" @click="refresh" :disabled="loading">Refresh</v-btn>
      </div>
      <v-data-table :headers="headers" :items="rows" :loading="loading" density="compact">
        <template #bottom>
          <div class="d-flex align-center" style="gap:8px;">
            <v-btn size="small" variant="text" @click="loadMore" :disabled="loading || rows.length >= processed">Load more</v-btn>
            <v-spacer />
            <span class="text-caption">Showing {{ rows.length }} of {{ processed }}</span>
          </div>
        </template>
      </v-data-table>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue';
import axios from 'axios';

const props = defineProps<{ version: string | null }>();

const rows = ref<any[]>([]);
const total = ref(0);
const processed = ref(0);
const skip = ref(0);
const take = ref(50);
const loading = ref(false);

  const headers = [
  { title: 'Num Ligne', value: 'numLigne' },
  { title: 'Match By', value: 'matchBy' },
  { title: 'BDD RAF (Matched)', value: 'matchedRaf' },
  { title: 'Inv. IdUniqueRetenu', value: 'inventoryIdentifiantUniqueRetenu' },
  { title: 'Inv. RAF', value: 'inventoryRaf' },
  { title: 'AddToBDD', value: 'addToBdd' },
];

async function fetchPage(reset = false) {
  if (!props.version) return;
  loading.value = true;
  try {
    if (reset) { skip.value = 0; rows.value = []; }
    const { data } = await axios.get('/api/workflow/bddmatch/results', { params: { version: props.version, skip: skip.value, take: take.value } });
    total.value = data.total ?? 0;
    processed.value = data.processed ?? 0;
    const items = (data.items || []).map((r:any) => ({
      numLigne: r.numLigne,
      matchBy: r.matchBy,
      matchedRaf: r.matchedRaf, // BDD RAF
      inventoryIdentifiantUniqueRetenu: r.inventoryIdentifiantUniqueRetenu,
      inventoryRaf: r.inventoryRaf,
      addToBdd: r.addToBdd,
    }));
    rows.value = [...rows.value, ...items];
    skip.value += items.length;
  } finally {
    loading.value = false;
  }
}

function loadMore() { fetchPage(false); }
function refresh() { fetchPage(true); }

watch(() => props.version, () => { if (props.version) fetchPage(true); });
onMounted(() => { if (props.version) fetchPage(true); });
</script>

<style scoped>
</style>
