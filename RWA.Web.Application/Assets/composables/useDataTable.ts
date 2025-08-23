import { ref, reactive, watch } from 'vue';
import { post } from '../api';

export function useDataTable(apiEndpoint: string, initialFilters: Record<string, string>) {
  const serverItems = ref<any[]>([]);
  const loading = ref(true);
  const totalItems = ref(0);
  const searchTrigger = ref('');
  const filters = reactive(initialFilters);

  let cancellationTokenSource = new AbortController();

  watch(filters, () => {
    searchTrigger.value = String(Date.now());
  }, { deep: true });

  const loadServerItems = async (options: any) => {
    loading.value = true;
    cancellationTokenSource.abort();
    cancellationTokenSource = new AbortController();

    try {
      const { page, itemsPerPage, sortBy } = options;
      const activeFilters = Object.fromEntries(
        Object.entries(filters).filter(([, value]) => value)
      );

      const requestBody = {
        page,
        pageSize: itemsPerPage,
        sortBy: sortBy.length > 0 ? sortBy[0].key : '',
        sortDesc: sortBy.length > 0 ? sortBy[0].order === 'desc' : false,
        filters: activeFilters,
      };

      const response = await post(apiEndpoint, requestBody, { signal: cancellationTokenSource.signal });
      serverItems.value = response.data.items;
      totalItems.value = response.data.totalItems;
    } catch (error: any) {
      if (error.name !== 'CanceledError') {
        console.error(`Error loading data from ${apiEndpoint}:`, error);
      }
    } finally {
      loading.value = false;
    }
  };

  return {
    serverItems,
    loading,
    totalItems,
    searchTrigger,
    filters,
    loadServerItems,
  };
}
