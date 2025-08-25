<template>
  <div>
    <h3 class="text-h6">Tethys Status</h3>
    <br />

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
            <v-list-group v-for="(group, category) in groupedDesserts" :key="category" :value="category">
              <template v-slot:activator="{ props }">
                <v-list-item v-bind="props" :title="category"></v-list-item>
              </template>

              <v-table class="inner-table">
                <thead>
                  <tr>
                    <th class="text-left">Dessert</th>
                    <th class="text-right">Calories</th>
                    <th class="text-right">Fat (g)</th>
                    <th class="text-right">Carbs (g)</th>
                    <th class="text-right">Protein (g)</th>
                    <th class="text-left">Details</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in group" :key="item.id">
                    <td>{{ item.name }}</td>
                    <td class="text-right">{{ item.calories }}</td>
                    <td class="text-right">{{ item.fat }}</td>
                    <td class="text-right">{{ item.carbs }}</td>
                    <td class="text-right">{{ item.protein }}</td>
                    <td>{{ item.selectedDetail }}</td>
                  </tr>
                </tbody>
              </v-table>

              <DetailsTable
                :details="getDetailsForCategory(category)"
                @selection-changed="(selectedDetail) => handleGroupedSelection(category, selectedDetail)"
              />
            </v-list-group>
          </v-list>
        </div>

        <!-- Classic Table -->
        <v-data-table
          v-else
          :items="desserts"
          :headers="classicHeaders"
          item-value="id"
          show-expand
        >
          <template v-slot:expanded-row="{ columns, item }">
            <tr>
              <td :colspan="columns.length">
                <DetailsTable
                  :details="getDetailsForItem(item)"
                  @selection-changed="(selectedDetail) => handleClassicSelection(item, selectedDetail)"
                />
              </td>
            </tr>
          </template>
        </v-data-table>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import DetailsTable from './DetailsTable.vue';

const isGroupedView = ref(false);

const desserts = ref([
  { id: 1, name: 'Frozen Yogurt', category: 'Ice cream', calories: 159, fat: 6.0, carbs: 24, protein: 4.0, selectedDetail: '', details: [{ id: 1, detail: 'Source', value: 'Store A' }, { id: 2, detail: 'Batch', value: 'A123' }] },
  { id: 2, name: 'Ice cream sandwich', category: 'Ice cream', calories: 237, fat: 9.0, carbs: 37, protein: 4.3, selectedDetail: '', details: [{ id: 3, detail: 'Source', value: 'Store B' }, { id: 4, detail: 'Batch', value: 'B456' }] },
  { id: 3, name: 'Eclair', category: 'Pastry', calories: 262, fat: 16.0, carbs: 23, protein: 6.0, selectedDetail: '', details: [{ id: 5, detail: 'Source', value: 'Bakery C' }, { id: 6, detail: 'Batch', value: 'C789' }] },
  { id: 4, name: 'Cupcake', category: 'Pastry', calories: 305, fat: 3.7, carbs: 67, protein: 4.3, selectedDetail: '', details: [{ id: 7, detail: 'Source', value: 'Bakery D' }, { id: 8, detail: 'Batch', value: 'D101' }] },
  { id: 5, name: 'Gingerbread', category: 'Pastry', calories: 356, fat: 16.0, carbs: 49, protein: 3.9, selectedDetail: '', details: [{ id: 9, detail: 'Source', value: 'Bakery E' }, { id: 10, detail: 'Batch', value: 'E112' }] },
  { id: 6, name: 'Jelly bean', category: 'Candy', calories: 375, fat: 0.0, carbs: 94, protein: 0.0, selectedDetail: '', details: [{ id: 11, detail: 'Source', value: 'Factory F' }, { id: 12, detail: 'Batch', value: 'F131' }] },
  { id: 7, name: 'Lollipop', category: 'Candy', calories: 392, fat: 0.2, carbs: 98, protein: 0, selectedDetail: '', details: [{ id: 13, detail: 'Source', value: 'Factory G' }, { id: 14, detail: 'Batch', value: 'G415' }] },
  { id: 8, name: 'Honeycomb', category: 'Candy', calories: 408, fat: 3.2, carbs: 87, protein: 6.5, selectedDetail: '', details: [{ id: 15, detail: 'Source', value: 'Factory H' }, { id: 16, detail: 'Batch', value: 'H161' }] },
  { id: 9, name: 'Donut', category: 'Pastry', calories: 452, fat: 25.0, carbs: 51, protein: 4.9, selectedDetail: '', details: [{ id: 17, detail: 'Source', value: 'Bakery I' }, { id: 18, detail: 'Batch', value: 'I718' }] },
  { id: 10, name: 'KitKat', category: 'Candy', calories: 518, fat: 26.0, carbs: 65, protein: 7, selectedDetail: '', details: [{ id: 19, detail: 'Source', value: 'Factory J' }, { id: 20, detail: 'Batch', value: 'J192' }] },
].sort((a, b) => a.category.localeCompare(b.category) || a.name.localeCompare(b.name)));

const classicHeaders = [
  { title: 'Dessert (100g serving)', key: 'name', align: 'start' },
  { title: 'Category', key: 'category', align: 'start' },
  { title: 'Calories', key: 'calories', align: 'end' },
  { title: 'Fat (g)', key: 'fat', align: 'end' },
  { title: 'Carbs (g)', key: 'carbs', align: 'end' },
  { title: 'Protein (g)', key: 'protein', align: 'end' },
  { title: 'Details', key: 'selectedDetail', align: 'start' },
  { title: '', key: 'data-table-expand', align: 'end' },
];

const groupedDesserts = computed(() => {
  return desserts.value.reduce((acc, item) => {
    const category = item.category;
    if (!acc[category]) {
      acc[category] = [];
    }
    acc[category].push(item);
    return acc;
  }, {});
});

function getDetailsForItem(item) {
  return item.details || [];
}

function getDetailsForCategory(category) {
  return desserts.value.filter(d => d.category === category).flatMap(d => d.details);
}

function handleClassicSelection(item, selectedDetail) {
  const index = desserts.value.findIndex(d => d.id === item.id);
  if (index !== -1) {
    desserts.value[index].selectedDetail = selectedDetail.value;
  }
}

function handleGroupedSelection(category, selectedDetail) {
  desserts.value.forEach((d) => {
    if (d.category === category) {
      d.selectedDetail = selectedDetail.value;
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
</style>
