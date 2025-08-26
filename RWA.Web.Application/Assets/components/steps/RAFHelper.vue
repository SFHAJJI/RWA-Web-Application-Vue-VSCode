<template>
  <div>
    <h3 class="text-h6">RAF Helper</h3>
    <br />

    <v-alert v-if="!items || items.length === 0" type="success" prominent border="start">
      All inventory files are validated in Tethys
      <v-btn color="success" variant="outlined" class="ml-4">Generate Fichier Enrichi</v-btn>
    </v-alert>

    <div v-else>
      <div v-if="areAllMappingsSuccessful">
        <v-alert type="success" prominent border="start">
          All mappings are successful. Ready to proceed.
        </v-alert>
      </div>
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
                    <v-list-item v-bind="props">
                      <v-list-item-title>{{ groupKey }}</v-list-item-title>
                      <template v-slot:append>
                        <v-icon v-if="isGroupComplete(group)" color="success">mdi-check-circle</v-icon>
                        <v-icon v-else color="error">mdi-close-circle</v-icon>
                      </template>
                    </v-list-item>
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
              :items="itemsToDisplay"
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
          </div>
        </transition>

        <v-row class="mt-6">
          <v-col cols="12" class="d-flex justify-end">
            <v-btn color="primary" elevation="2" large @click="submit">
              <v-icon left>mdi-check-circle</v-icon>
              Submit
            </v-btn>
          </v-col>
        </v-row>
      </div>
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

console.log('Tethys Payload:', props.payload);

const isGroupedView = ref(false);
const items = ref(props.payload);

const areAllMappingsSuccessful = computed(() => {
  return items.value.every(item => item.IsMappingTethysSuccessful);
});

const itemsToDisplay = computed(() => {
  return items.value.filter(item => !item.IsMappingTethysSuccessful);
});

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
  return itemsToDisplay.value.reduce((acc, item) => {
    const groupKey = `${item.Source} - ${item.Cpt}`;
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
  const [source, cpt] = groupKey.split(' - ');
  items.value.forEach((d) => {
    if (d.Source === source && d.Cpt === cpt) {
      d.Raf = selectedValue;
    }
  });
}

function isGroupComplete(group) {
  return group.every(item => item.Raf && item.Raf.trim() !== '');
}

async function submit() {
  try {
    const itemsToSubmit = items.value.map(item => {
      const newItem = {};
      for (const key in item) {
        newItem[key] = item[key] === null ? '' : item[key];
      }
      return newItem;
    });

    console.log('Submitting items:', JSON.parse(JSON.stringify(itemsToSubmit)));

    const response = await fetch('/api/workflow/update-raf', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(itemsToSubmit),
    });

    if (!response.ok) {
      throw new Error('Failed to update RAF values');
    }

    // Optionally, you can handle the success case here, e.g., show a notification.
    console.log('RAF values updated successfully');
  } catch (error) {
    console.error('Error updating RAF values:', error);
  }
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
