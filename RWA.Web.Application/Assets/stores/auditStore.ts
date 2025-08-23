import { defineStore } from 'pinia';
import axios from 'axios';

export const useAuditStore = defineStore('audit', {
  state: () => ({
    historyColumns: [] as any[],
    tethysColumns: [] as any[],
    inventaireColumns: [] as any[],
  }),
  actions: {
    async fetchHistoryColumns() {
      if (this.historyColumns.length === 0) {
        try {
          const response = await axios.get('/api/audit/history/columns');
          this.historyColumns = response.data;
        } catch (error) {
          console.error('Error fetching history columns:', error);
        }
      }
    },
    async fetchTethysColumns() {
      if (this.tethysColumns.length === 0) {
        try {
          const response = await axios.get('/api/audit/tethys/columns');
          this.tethysColumns = response.data;
        } catch (error) {
          console.error('Error fetching tethys columns:', error);
        }
      }
    },
    async fetchInventaireColumns() {
      if (this.inventaireColumns.length === 0) {
        try {
          const response = await axios.get('/api/audit/inventory/columns');
          this.inventaireColumns = response.data;
        } catch (error) {
          console.error('Error fetching inventaire columns:', error);
        }
      }
    }
  },
});
