import { reactive } from 'vue';

const state = reactive({ isPosting: false });

export function useLoadingStore() {
    return state;
}
