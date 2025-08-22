<template>
  <div>
    <!-- Audit trigger button (small arrow on top left) -->
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
      <div class="audit-table-container" v-show="activeCard !== null">
        <div class="table-header">
          <h4>{{ auditTables[activeCard]?.title }}</h4>
        </div>
        
        <div class="table-content">
          <EnhancedDataTable
            :headers="currentTableColumns"
            :items="currentTableData"
            :loading="tableLoading"
          />
        </div>
      </div>
    </ResizablePanel>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';
import ResizablePanel from './audit/ResizablePanel.vue';
import EnhancedDataTable from './audit/EnhancedDataTable.vue';

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
const tableLoading = ref(false);

// Audit tables configuration
const auditTables = reactive<AuditTable[]>([
  {
    name: 'inventory',
    title: 'Inventaire Normalisé',
    description: 'Table principale du workflow - HecateInventaireNormalise',
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
  }
]);

// Table data
const tableData = reactive<Record<string, any[]>>({
  inventory: [],
  history: []
});

const tableColumns = reactive<Record<string, any[]>>({
  inventory: [],
  history: []
});

// Computed properties
const currentTableData = computed(() => {
  if (activeCard.value === null) return [];
  const tableName = auditTables[activeCard.value].name;
  return tableData[tableName];
});

const currentTableColumns = computed(() => {
  if (activeCard.value === null) return [];
  const tableName = auditTables[activeCard.value].name;
  return tableColumns[tableName];
});

// Methods
const togglePanel = () => {
  console.log('Toggling panel');
  if (isOpen.value) {
    closePanel();
  } else {
    openPanel();
  }
};

const openPanel = async () => {
  console.log('Opening panel');
  isOpen.value = true;
  // Load counts when panel opens
  await loadTableCounts();
};

const closePanel = () => {
  console.log('Closing panel');
  isOpen.value = false;
  activeCard.value = null;
};

const selectCard = async (index: number) => {
  console.log(`Card ${index} selected`);
  if (activeCard.value === index) {
    // Close if clicking on active card
    activeCard.value = null;
    return;
  }
  
  activeCard.value = index;
  const table = auditTables[index];
  
  // Load table data if not already loaded
  if (tableData[table.name].length === 0) {
    await loadTableData(table);
  }
};

const loadTableCounts = async () => {
  console.log('Loading table counts');
  try {
    for (const table of auditTables) {
      const response = await fetch(`${table.apiEndpoint}/count`);
      if (response.ok) {
        const data = await response.json();
        table.count = data.count || 0;
        console.log(`Count for ${table.name}: ${table.count}`);
      }
    }
  } catch (error) {
    console.error('Error loading table counts:', error);
  }
};

const loadTableData = async (table: AuditTable) => {
  console.log(`Loading table data for ${table.name}`);
  tableLoading.value = true;
  
  try {
    // Load columns schema
    const columnsResponse = await fetch(`${table.apiEndpoint}/columns`);
    if (columnsResponse.ok) {
      const columnsData = await columnsResponse.json();
      tableColumns[table.name] = columnsData || [];
      console.log(`Columns for ${table.name}:`, tableColumns[table.name]);
    }
    
    // Load data
    const dataResponse = await fetch(`${table.apiEndpoint}/data`);
    if (dataResponse.ok) {
      const data = await dataResponse.json();
      tableData[table.name] = data.rows || [];
      console.log(`Data for ${table.name}:`, tableData[table.name]);
    }
  } catch (error) {
    console.error(`Error loading ${table.name} data:`, error);
  } finally {
    tableLoading.value = false;
  }
};

// Initialize on mount
onMounted(async () => {
  console.log('AuditPanel mounted');
  // Pre-load counts
  await loadTableCounts();
});
</script>
<style src="../public/css/audit-panel.css" scoped></style>
