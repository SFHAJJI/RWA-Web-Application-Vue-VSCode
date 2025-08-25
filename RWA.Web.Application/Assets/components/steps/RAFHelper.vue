<template>
  <div>
    <h3 class="text-h6">RAF Helper</h3>
    <br />

    <v-alert v-if="!items || items.length === 0" type="success" prominent border="start">
      All inventory files are validated in Tethys
      <v-btn color="success" variant="outlined" class="ml-4">Generate Fichier Enrichi</v-btn>
    </v-alert>

    <div v-else>
      <v-switch
        v-model="isGroupedView"
        label="Grouped View"
        color="primary"
      ></v-switch>

      <transition name="fade" mode="out-in">
        <div :key="isGroupedView">
          <!-- Grouped View using v-list -->
          <div v-if="isGroupedView">
            <v-list lines="two" class="grouped-list">
              <v-list-group v-for="(group, groupKey) in groupedItems" :key="groupKey" :value="groupKey">
                <template v-slot:activator="{ props }">
                  <v-list-item v-bind="props" :title="groupKey"></v-list-item>
                </template>

                <v-table class="inner-table">
                  <thead>
                    <tr>
                      <th class="text-left">NumLigne</th>
                      <th class="text-left">CptTethys</th>
                      <th class="text-left">IsGeneric</th>
                      <th class="text-left">IsMappingTethysSuccessful</th>
                      <th class="text-left">Raf</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="item in group" :key="item.NumLigne">
                      <td>{{ item.NumLigne }}</td>
                      <td>{{ item.CptTethys }}</td>
                      <td>{{ item.IsGeneric }}</td>
                      <td>{{ item.IsMappingTethysSuccessful }}</td>
                      <td class="raf-cell">
                        <v-icon v-if="item.Raf" color="success" class="status-icon">mdi-check-circle</v-icon>
                        <v-icon v-else color="error" class="status-icon">mdi-close-circle</v-icon>
                        {{ item.Raf }}
                      </td>
                    </tr>
                  </tbody>
                </v-table>

                <DetailsTable
                  @selection-changed="(selectedDetail) => handleGroupedSelection(groupKey, selectedDetail)"
                />
              </v-list-group>
            </v-list>
          </div>

          <!-- Classic Table -->
          <v-data-table
            v-else
            :items="items"
            :headers="classicHeaders"
            item-value="NumLigne"
            show-expand
            class="modern-table"
          >
            <template v-slot:item.Raf="{ item }">
              <td class="raf-cell">
                <v-icon v-if="item.Raf" color="success" class="status-icon">mdi-check-circle</v-icon>
                <v-icon v-else color="error" class="status-icon">mdi-close-circle</v-icon>
                {{ item.Raf }}
              </td>
            </template>
            <template v-slot:expanded-row="{ columns, item }">
              <tr class="expanded-row-content">
                <td :colspan="columns.length">
                  <div class="details-connector"></div>
                  <DetailsTable
                    @selection-changed="(selectedDetail) => handleClassicSelection(item, selectedDetail)"
                  />
                </td>
              </tr>
            </template>
          </v-data-table>
>>>>>>> Stashed changes
        </div>
      </transition>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, defineProps } from 'vue';
import DetailsTable from './DetailsTable.vue';

const props = defineProps({
  payload: {
    type: Array,
    required: true,
  },
});

const isGroupedView = ref(false);
const items = ref(props.payload);

const classicHeaders = [
  { title: 'NumLigne', key: 'NumLigne' },
  { title: 'Source', key: 'Source' },
  { title: 'CptTethys', key: 'CptTethys' },
  { title: 'Cpt', key: 'Cpt' },
  { title: 'IsGeneric', key: 'IsGeneric' },
  { title: 'IsMappingTethysSuccessful', key: 'IsMappingTethysSuccessful' },
  { title: 'Raf', key: 'Raf' },
  { title: '', key: 'data-table-expand', align: 'end' },
];

const groupedItems = computed(() => {
  return items.value.reduce((acc, item) => {
    const groupKey = `${item.Source} - ${item.CptTethys}`;
    if (!acc[groupKey]) {
      acc[groupKey] = [];
    }
    acc[groupKey].push(item);
    return acc;
  }, {});
});

function handleClassicSelection(item, selectedValue) {
  const index = items.value.findIndex(d => d.NumLigne === item.NumLigne);
  if (index !== -1) {
    items.value[index].Raf = selectedValue;
  }
}

function handleGroupedSelection(groupKey, selectedValue) {
  const [source, cptTethys] = groupKey.split(' - ');
  items.value.forEach((d) => {
    if (d.Source === source && d.CptTethys === cptTethys) {
      d.Raf = selectedValue;
    }
  });
}
</script>

<style>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
.grouped-list .v-list-group__items {
  padding: 0 16px 16px;
}
.inner-table {
  background-color: rgba(0,0,0,0.05);
  border-radius: 4px;
  margin-bottom: 16px;
}
.modern-table {
  border: 1px solid rgba(0, 0, 0, 0.1);
  border-radius: 8px;
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
}
.modern-table .v-data-table-header {
  background-color: #f5f5f5;
}
.modern-table .v-data-table-header th {
  font-weight: 600;
  color: #333;
}
.modern-table tbody tr:hover {
  background-color: #f9f9f9;
}
.modern-table tbody tr:nth-of-type(odd) {
  background-color: #fafafa;
}
.expanded-row-content {
  position: relative;
}
.details-connector {
  position: absolute;
  top: -10px;
  left: 50%;
  width: 2px;
  height: 10px;
  background-color: #ccc;
  animation: grow-connector 0.3s ease-out;
}
@keyframes grow-connector {
  from {
    height: 0;
  }
  to {
    height: 10px;
  }
}
.raf-cell {
  position: relative;
  transition: background-color 0.3s ease;
}
.status-icon {
  position: absolute;
  top: 5px;
  right: 5px;
  font-size: 16px;
}
</style>
