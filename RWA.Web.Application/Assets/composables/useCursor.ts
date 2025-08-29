import { ref } from 'vue'

export function useCursor() {
  const cursor = ref<string | null>(null)
  const total = ref<number>(0)

  function reset() {
    cursor.value = null
    total.value = 0
  }

  function applyNext(next?: string, t?: number) {
    cursor.value = next ?? null
    if (typeof t === 'number') total.value = t
  }

  return { cursor, total, reset, applyNext }
}

