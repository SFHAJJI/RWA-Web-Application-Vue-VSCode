<template>
  <div class="raf-helper">
    <h3 class="text-h6">RAF Helper</h3>
    <br />

    <v-alert v-if="!unresolved.length" type="success" prominent border="start">
      All inventory files are validated in Tethys
      <v-btn color="success" variant="outlined" class="ml-4">Generate Fichier Enrichi</v-btn>
    </v-alert>

    <div v-else class="split">
      <!-- LEFT: KO list -->
      <section class="left">
        <div class="left-toolbar">
          <v-switch v-model="isGroupedView" label="Grouped View" color="primary" />
          <v-text-field v-model="leftFilter" density="compact" hide-details
                        prepend-inner-icon="mdi-magnify" placeholder="Filter…" />
        </div>

        <!-- Raw mode -->
        <v-data-table
          v-if="!isGroupedView"
          :items="pagedKOs"
          :headers="leftHeaders"
          item-key="NumLigne"
          fixed-header height="70vh"
          density="compact"
          :loading="loadingLeft"
          @click:row="(event, { item }) => selectKO(item)"
        >
          <template #item.Raf="{ item }">
            <StatusCell :raf="item.Raf" />
          </template>
        </v-data-table>

        <!-- Grouped mode (no nested DetailsTable) -->
        <v-list v-else class="grouped-list" style="height:70vh; overflow:auto">
          <v-list-group
            v-for="(group, key) in grouped"
            :key="key"
            :value="key"
          >
            <template #activator="{ props }">
              <v-list-item v-bind="props">
                <v-list-item-title>{{ key }}</v-list-item-title>
                <template #append>
                  <v-icon :color="isGroupComplete(group)?'success':'error'">
                    {{ isGroupComplete(group) ? 'mdi-check-circle' : 'mdi-close-circle' }}
                  </v-icon>
                </template>
              </v-list-item>
            </template>

            <v-table class="inner-table">
              <thead>
                <tr>
                  <th>NumLigne</th><th>CptTethys</th><th>IsGeneric</th><th>OK?</th><th>Raf</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="row in group" :key="row.NumLigne" @click="selectKO(row)" class="clickable">
                  <td>{{ row.NumLigne }}</td>
                  <td>{{ row.CptTethys }}</td>
                  <td>{{ row.IsGeneric }}</td>
                  <td>{{ row.IsMappingTethysSuccessful }}</td>
                  <td><StatusCell :raf="row.Raf" /></td>
                </tr>
              </tbody>
            </v-table>
          </v-list-group>
        </v-list>

        <div class="left-footer">
          <v-btn color="primary" @click="submit" :disabled="!hasChanges">
            <v-icon start>mdi-check-circle</v-icon> Submit
          </v-btn>
        </div>
      </section>

      <v-divider vertical />

      <!-- RIGHT: single reusable DetailsTable -->
      <section class="right">
        <div class="right-toolbar">
          <div class="current">
            <span v-if="selectedKO">
              Working on <strong>#{{ selectedKO.NumLigne }}</strong> – {{ selectedKO.Source }} / {{ selectedKO.Cpt }}
            </span>
            <span v-else class="muted">Select a KO on the left to begin</span>
          </div>
          <div class="actions">
            <v-btn size="small" variant="text" @click="skip">Skip</v-btn>
          </div>
        </div>

        <DetailsTable
          v-if="selectedKO"
          :prefill="prefillFromKO(selectedKO)"
          :page-size="20"
          @assign="assign"
        />
        <v-skeleton-loader v-else type="table" class="skeleton" />
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, reactive } from 'vue'
import DetailsTable from './DetailsTable.vue'
import StatusCell from './StatusCell.vue'
import { fetchFailed, assignRaf, fetchSuggestions } from '../../api/tethysApi'

const isGroupedView = ref(false)
const loadingLeft = ref(false)
const leftFilter = ref('')
const selectedKO = ref<any | null>(null)
const dirtyIds = ref(new Set())

const left = reactive({ items: [] as any[], cursor: null as string | null, total: 0, loading: false })

async function loadLeft(initial = false) {
    if (left.loading) return
    left.loading = true
    const d = await fetchFailed(initial ? undefined : left.cursor ?? undefined, 20)
    left.items = initial ? d.items : [...left.items, ...d.items]
    left.cursor = d.nextCursor ?? null
    left.total = d.total
    left.loading = false
}

onMounted(() => loadLeft(true))

const unresolved = computed(() => left.items.filter(i => !i.isMappingTethysSuccessful))
const leftHeaders = [
  { title:'NumLigne', key:'NumLigne' },
  { title:'Source', key:'Source' },
  { title:'CptTethys', key:'CptTethys' },
  { title:'Cpt', key:'Cpt' },
  { title:'IsGeneric', key:'IsGeneric' },
  { title:'OK?', key:'IsMappingTethysSuccessful' },
  { title:'Raf', key:'Raf' },
]

const pagedKOs = computed(() => {
    const f = (leftFilter.value || '').toLowerCase()
    return unresolved.value.filter(x =>
        !f || String(x.numLigne).includes(f) || (x.cptTethys?.toLowerCase().includes(f)))
})

const grouped = computed(() => {
  return unresolved.value.reduce((acc, it) => {
    const k = `${it.Source} - ${it.Cpt}`
    ;(acc[k] ||= []).push(it)
    return acc
  }, {} as Record<string, any[]>)
})

function isGroupComplete(group: any[]) {
  return group.every(i => i.Raf && i.Raf.trim() !== '')
}

function selectKO(row: any) {
  selectedKO.value = row
}

function prefillFromKO(row: any) {
  // return an object the DetailsTable can use to set initial query/filters
  // e.g. { query: row.CptTethys ?? row.Cpt, chips: [row.Source, row.Tiers] }
  return { query: row.CptTethys || row.Cpt, context: { source: row.Source, cpt: row.Cpt } }
}

async function assign(candidate: { raf: string, id?: number }) {
    if (!selectedKO.value) return
    const originalRaf = selectedKO.value.raf
    const originalStatus = selectedKO.value.isMappingTethysSuccessful

    // optimistic update
    selectedKO.value.raf = candidate.raf
    selectedKO.value.isMappingTethysSuccessful = true
    dirtyIds.value.add(selectedKO.value.numLigne)

    try {
        await assignRaf(selectedKO.value.numLigne, candidate.raf)
    } catch (error) {
        // revert on failure
        selectedKO.value.raf = originalRaf
        selectedKO.value.isMappingTethysSuccessful = originalStatus
        dirtyIds.value.delete(selectedKO.value.numLigne)
        // show error toast
    }

    // auto-advance
    const idx = unresolved.value.findIndex(i => i.numLigne === selectedKO.value.numLigne)
    selectedKO.value = unresolved.value[idx + 1] || null
}

function undo(row: any) {
  row.Raf = ''
  row.IsMappingTethysSuccessful = false
  dirtyIds.value.delete(row.NumLigne)
  selectedKO.value = row
}

const hasChanges = computed(() => dirtyIds.value.size > 0)

async function submit() {
    const changed = left.items.filter(i => dirtyIds.value.has(i.numLigne))
    for (const item of changed) {
        await assignRaf(item.numLigne, item.raf)
    }
    dirtyIds.value.clear()
}

function skip() {
  // mark item to revisit or move on
  const idx = unresolved.value.findIndex(i => i.NumLigne === selectedKO.value?.NumLigne)
  selectedKO.value = unresolved.value[idx + 1] || null
}
</script>

<style scoped>
.split { display: grid; grid-template-columns: 5fr 1px 7fr; gap: 0; }
.left, .right { display: flex; flex-direction: column; min-height: 70vh; }
.left-toolbar, .right-toolbar { position: sticky; top: 0; background: white; z-index: 1; padding: .5rem; border-bottom: 1px solid #eee; }
.left-footer { padding: .5rem; border-top: 1px solid #eee; }
.skeleton { height: 70vh; }
.clickable { cursor: pointer; }
</style>
