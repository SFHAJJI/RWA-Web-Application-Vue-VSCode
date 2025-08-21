import { defineStore } from 'pinia';
import { ref } from 'vue';

export type ToastItem = { id: number; type: 'info'|'warning'|'error'; message: string };

export const useToastStore = defineStore('toast', () => {
    const queue = ref<ToastItem[]>([]);
    let nextId = 1;

    function pushToast(t: { type?: string; message?: string }) {
        const item: ToastItem = { id: nextId++, type: (t.type as any) || 'info', message: t.message || '' };
        queue.value.push(item);
        return item.id;
    }

    function removeToast(id: number) {
        queue.value = queue.value.filter(x => x.id !== id);
    }

    function clear() {
        queue.value = [];
    }

    return { queue, pushToast, removeToast, clear };
});
