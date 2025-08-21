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
    <div class="audit-panel" :class="{ 'open': isOpen }">
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
          <v-data-table
            v-if="currentTableData.length > 0"
            :headers="currentTableColumns"
            :items="currentTableData"
            :loading="tableLoading"
            class="audit-table"
            density="compact"
            :items-per-page="25"
          />
          <div v-else-if="tableLoading" class="table-loading">
            <v-progress-circular indeterminate color="primary"></v-progress-circular>
            <p>Chargement des données...</p>
          </div>
          <div v-else class="table-empty">
            <v-icon size="48" color="grey">mdi-database-off</v-icon>
            <p>Aucune donnée disponible</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue';

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
  const columns = tableColumns[tableName];
  // Transform API column format (text, value) to Vuetify format (title, value)
  return columns.map(col => ({
    title: col.text,
    value: col.value
  }));
});

// Methods
const togglePanel = () => {
  if (isOpen.value) {
    closePanel();
  } else {
    openPanel();
  }
};

const openPanel = async () => {
  isOpen.value = true;
  // Load counts when panel opens
  await loadTableCounts();
};

const closePanel = () => {
  isOpen.value = false;
  activeCard.value = null;
};

const selectCard = async (index: number) => {
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
  try {
    for (const table of auditTables) {
      const response = await fetch(`${table.apiEndpoint}/count`);
      if (response.ok) {
        const data = await response.json();
        table.count = data.count || 0;
      }
    }
  } catch (error) {
    console.error('Error loading table counts:', error);
  }
};

const loadTableData = async (table: AuditTable) => {
  tableLoading.value = true;
  
  try {
    // Load columns schema
    const columnsResponse = await fetch(`${table.apiEndpoint}/columns`);
    if (columnsResponse.ok) {
      const columnsData = await columnsResponse.json();
      tableColumns[table.name] = columnsData.columns || [];
    }
    
    // Load data
    const dataResponse = await fetch(`${table.apiEndpoint}/data`);
    if (dataResponse.ok) {
      const data = await dataResponse.json();
      tableData[table.name] = data.rows || [];
    }
  } catch (error) {
    console.error(`Error loading ${table.name} data:`, error);
  } finally {
    tableLoading.value = false;
  }
};

// Initialize on mount
onMounted(async () => {
  // Pre-load counts
  await loadTableCounts();
});
</script>

<style scoped>
/* Audit trigger button */
.audit-trigger {
  position: fixed;
  top: 160px; /* Moved down to align with stepper header level */
  left: 20px;
  z-index: 1000;
  width: 40px;
  height: 40px;
  background: linear-gradient(135deg, rgb(200, 176, 207), rgb(165, 127, 176));
  border-radius: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  transition: all 0.3s ease;
}

.audit-trigger:hover {
  transform: translateX(5px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.25);
}

.audit-trigger.active {
  transform: translateX(50vw);
  opacity: 0;
  pointer-events: none;
}

/* Backdrop overlay */
.audit-backdrop {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.3);
  /* Removed backdrop-filter: blur(4px); to prevent dropdown interference */
  z-index: 1001;
  opacity: 0;
  visibility: hidden;
  transition: all 0.3s ease;
}

.audit-backdrop.active {
  opacity: 1;
  visibility: visible;
}

/* Audit panel */
.audit-panel {
  position: fixed;
  top: 0;
  left: -50%;
  width: 50%;
  height: 100vh;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  border-right: 1px solid rgba(200, 176, 207, 0.3);
  z-index: 1002;
  transition: left 0.4s cubic-bezier(0.25, 0.46, 0.45, 0.94);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.audit-panel.open {
  left: 0;
}

/* Panel header */
.audit-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px;
  background: linear-gradient(135deg, rgb(200, 176, 207), rgb(165, 127, 176));
  color: white;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.audit-title {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  display: flex;
  align-items: center;
}

/* Audit cards */
.audit-cards {
  padding: 20px;
  display: flex;
  flex-direction: row; /* Changed to horizontal layout */
  gap: 12px;
  flex-wrap: wrap; /* Allow wrapping if needed */
}

.audit-card {
  background: white;
  border: 2px solid transparent;
  border-radius: 12px;
  padding: 16px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  flex: 1; /* Cards will take equal width */
  min-width: 200px; /* Minimum width for horizontal layout */
}

.audit-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  border-color: rgb(200, 176, 207);
}

.audit-card.active {
  border-color: rgb(165, 127, 176);
  background: rgba(200, 176, 207, 0.1);
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(165, 127, 176, 0.2);
}

.card-content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.card-icon {
  flex-shrink: 0;
}

.card-info {
  flex: 1;
}

.card-title {
  margin: 0 0 4px 0;
  font-size: 1.1rem;
  font-weight: 600;
  color: #2c3e50;
}

.card-description {
  margin: 0 0 8px 0;
  font-size: 0.9rem;
  color: #64748b;
  line-height: 1.4;
}

.card-stats {
  display: flex;
  gap: 8px;
}

.stat-badge {
  background: rgba(200, 176, 207, 0.2);
  color: rgb(165, 127, 176);
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 500;
}

/* Table container */
.audit-table-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  border-top: 1px solid rgba(0, 0, 0, 0.1);
  background: white;
}

.table-header {
  padding: 16px 20px;
  background: rgba(200, 176, 207, 0.1);
  border-bottom: 1px solid rgba(0, 0, 0, 0.1);
}

.table-header h4 {
  margin: 0;
  color: rgb(165, 127, 176);
  font-weight: 600;
}

.table-content {
  flex: 1;
  overflow: auto;
  padding: 20px;
}

.audit-table {
  width: 100%;
}

.table-loading,
.table-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
  color: #64748b;
  gap: 16px;
}

.table-loading p,
.table-empty p {
  margin: 0;
  font-size: 1rem;
}

/* Responsive adjustments */
@media (max-width: 768px) {
  .audit-panel {
    width: 100%;
    left: -100%;
  }
  
  .audit-trigger.active {
    transform: translateX(100vw);
  }
}
</style>
