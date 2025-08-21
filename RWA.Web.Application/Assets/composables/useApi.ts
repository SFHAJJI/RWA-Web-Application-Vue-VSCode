import { useLoadingStore } from '../stores/loading';

export async function postJson(url: string, body: any) {
    const loading = useLoadingStore();
    loading.isPosting = true;
    try {
        const resp = await fetch(url, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(body)
        });
        // Expect server to return 204 NoContent for posts that publish via SignalR
        if (resp.status === 204) {
            return null;
        }
        const data = await resp.json();
        return data;
    } finally {
        // Keep spinner until SignalR snapshot arrives; consumers may still hide it on SignalR event.
        // For safety, we hide it here too so spinner doesn't hang forever if SignalR fails.
        const loadingRef = useLoadingStore();
        loadingRef.isPosting = false;
    }
}
