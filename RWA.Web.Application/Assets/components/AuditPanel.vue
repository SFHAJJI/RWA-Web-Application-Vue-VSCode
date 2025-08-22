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
            :items-length="currentTableItemsLength"
            :loading="tableLoading"
            @update:options="loadServerItems"
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

// Table data
const tableData = reactive<Record<string, any[]>>({
  inventory: [],
  history: [],
  tethys: []
});

const tableColumns = reactive<Record<string, any[]>>({
  inventory: [],
  history: [],
  tethys: []
});

const tableItemsLength = reactive<Record<string, number>>({
  inventory: 0,
  history: 0,
  tethys: 0
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

const currentTableItemsLength = computed(() => {
  if (activeCard.value === null) return 0;
  const tableName = auditTables[activeCard.value].name;
  return tableItemsLength[tableName];
});

// Methods
const togglePanel = () => {
  if (isOpen.value) closePanel();
  else openPanel();
};

const openPanel = async () => {
  isOpen.value = true;
  await loadTableCounts();
};

const closePanel = () => {
  isOpen.value = false;
  activeCard.value = null;
};

const selectCard = async (index: number) => {
  if (activeCard.value === index) {
    activeCard.value = null;
    return;
  }
  
  activeCard.value = index;
  const table = auditTables[index];
  
  // Load columns if not already loaded, data will be loaded by the datatable's event
  if (tableColumns[table.name].length === 0) {
    await loadTableSchema(table);
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

const loadTableSchema = async (table: AuditTable) => {
  try {
    const columnsResponse = await fetch(`${table.apiEndpoint}/columns`);
    if (columnsResponse.ok) {
      const columnsData = await columnsResponse.json();
      tableColumns[table.name] = columnsData || [];
    }
  } catch (error) {
    console.error(`Error loading ${table.name} schema:`, error);
  }
};

const loadServerItems = async (options: any) => {
  if (activeCard.value === null) return;
  
  const table = auditTables[activeCard.value];
  tableLoading.value = true;

  try {
    const { page, itemsPerPage, sortBy, filters } = options;
    
    const requestBody = {
      page: page,
      pageSize: itemsPerPage,
      sortBy: sortBy.length > 0 ? sortBy[0].key : null,
      sortDesc: sortBy.length > 0 ? sortBy[0].order === 'desc' : false,
      filters: filters || {}
    };

    const response = await fetch(`${table.apiEndpoint}/data`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(requestBody)
    });

    if (response.ok) {
      const data = await response.json();
      tableData[table.name] = data.items || [];
      tableItemsLength[table.name] = data.totalItems || 0;
    }
  } catch (error) {
    console.error(`Error loading ${table.name} data:`, error);
    tableData[table.name] = [];
    tableItemsLength[table.name] = 0;
  } finally {
    tableLoading.value = false;
  }
};

// Initialize on mount
onMounted(async () => {
  await loadTableCounts();
});
</script>
<style src="../public/css/audit-panel.css" scoped></style>
