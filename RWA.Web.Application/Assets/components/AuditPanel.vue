<template>
  <div>
    <!-- Audit trigger button -->
    <div 
      class="audit-trigger" 
      :class="{ 'active': isOpen }"
      @click="togglePanel"
      v-show="!isOpen"
    >
      <v-icon size="24" color="primary">mdi-chevron-right</v-icon>
    </div>

    <!-- Backdrop overlay -->
    <div 
      class="audit-backdrop" 
      :class="{ 'active': isOpen }"
      @click="closePanel"
    ></div>

    <!-- Audit sliding panel -->
    <ResizablePanel class="audit-panel" :class="{ 'open': isOpen }">
      <!-- Panel header -->
      <div class="audit-header">
        <h3 class="audit-title">
          <v-icon class="me-2">mdi-chart-line</v-icon>
          Audit du Workflow
        </h3>
        <v-btn 
          icon 
          size="small" 
          variant="text"
          @click="closePanel"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
      </div>

      <!-- Audit cards -->
      <div class="audit-cards">
        <div 
          class="audit-card" 
          v-for="(table, index) in auditTables" 
          :key="table.name"
          :class="{ 'active': activeCard === index }"
          @click="selectCard(index)"
        >
          <div class="card-content">
            <div class="card-icon">
              <v-icon :color="table.color" size="32">{{ table.icon }}</v-icon>
            </div>
            <div class="card-info">
              <h4 class="card-title">{{ table.title }}</h4>
              <p class="card-description">{{ table.description }}</p>
              <div class="card-stats">
                <span class="stat-badge">{{ table.count }} enregistrements</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Data table section -->
      <div class="audit-table-container" v-if="activeCard !== null">
        <div class="table-content">
          <HistoryDataTable v-if="auditTables[activeCard].name === 'history'" />
          <TethysDataTable v-if="auditTables[activeCard].name === 'tethys'" />
          <InventaireDataTable v-if="auditTables[activeCard].name === 'inventory'" />
        </div>
      </div>
    </ResizablePanel>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import axios from 'axios';
import ResizablePanel from './audit/ResizablePanel.vue';
import HistoryDataTable from './audit/HistoryDataTable.vue';
import TethysDataTable from './audit/TethysDataTable.vue';
import InventaireDataTable from './audit/InventaireDataTable.vue';

interface AuditTable {
  name: string;
  title: string;
  description: string;
  icon: string;
  color: string;
  count: number;
  apiEndpoint: string;
}

// Panel state
const isOpen = ref(false);
const activeCard = ref<number | null>(null);

// Audit tables configuration
const auditTables = reactive<AuditTable[]>([
  {
    name: 'inventory',
    title: 'Inventaire Normalisé',
    description: 'Table principale - HecateInventaireNormalise',
    icon: 'mdi-database',
    color: 'primary',
    count: 0,
    apiEndpoint: '/api/audit/inventory'
  },
  {
    name: 'history', 
    title: 'Historique Interne',
    description: 'Suivi des modifications - HecateInterneHistorique',
    icon: 'mdi-history',
    color: 'secondary',
    count: 0,
    apiEndpoint: '/api/audit/history'
  },
  {
    name: 'tethys', 
    title: 'Tethys',
    description: 'Données Tethys - HecateTethy',
    icon: 'mdi-alpha-t-box',
    color: 'green',
    count: 0,
    apiEndpoint: '/api/audit/tethys'
  }
]);

// Methods
const togglePanel = () => isOpen.value ? closePanel() : openPanel();

const openPanel = async () => {
  isOpen.value = true;
  await loadTableCounts();
};

const closePanel = () => {
  isOpen.value = false;
  activeCard.value = null;
};

const selectCard = (index: number) => {
  if (activeCard.value === index) {
    activeCard.value = null;
    return;
  }
  activeCard.value = index;
};

const loadTableCounts = async () => {
  try {
    for (const table of auditTables) {
      const response = await axios.get(`${table.apiEndpoint}/count`);
      table.count = response.data.count || 0;
    }
  } catch (error) {
    console.error('Error loading table counts:', error);
  }
};

onMounted(loadTableCounts);
</script>

<style src="../assets/styles/audit-panel.css" scoped></style>
