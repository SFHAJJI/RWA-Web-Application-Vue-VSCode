import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useRwaCaches = defineStore('rwaCaches', () => {
  const categories = ref<{ id: string; label: string }[]>(JSON.parse(localStorage.getItem('rwa:categories') || '[]'))
  const depositaire1 = ref<{ id: number; label: string }[]>(JSON.parse(localStorage.getItem('rwa:depo1') || '[]'))
  const depositaire2 = ref<{ id: number; label: string }[]>(JSON.parse(localStorage.getItem('rwa:depo2') || '[]'))
  const types = ref<{ id: string; label: string }[]>(JSON.parse(localStorage.getItem('rwa:types') || '[]'))

  function persist() {
    localStorage.setItem('rwa:categories', JSON.stringify(categories.value))
    localStorage.setItem('rwa:depo1', JSON.stringify(depositaire1.value))
    localStorage.setItem('rwa:depo2', JSON.stringify(depositaire2.value))
    localStorage.setItem('rwa:types', JSON.stringify(types.value))
  }

  function setCategories(arr: { id: string; label: string }[]) {
    categories.value = arr
    persist()
  }
  function mergeCategories(arr: { id: string; label: string }[]) {
    const map = new Map(categories.value.map(c => [c.id, c]))
    for (const it of arr) if (!map.has(it.id)) map.set(it.id, it)
    categories.value = Array.from(map.values())
    persist()
  }

  function mergeDepositaire1(arr: { id: number; label: string }[]) {
    const map = new Map(depositaire1.value.map(d => [d.id, d]))
    for (const it of arr) if (!map.has(it.id)) map.set(it.id, it)
    depositaire1.value = Array.from(map.values())
    persist()
  }
  function mergeDepositaire2(arr: { id: number; label: string }[]) {
    const map = new Map(depositaire2.value.map(d => [d.id, d]))
    for (const it of arr) if (!map.has(it.id)) map.set(it.id, it)
    depositaire2.value = Array.from(map.values())
    persist()
  }
  function mergeTypes(arr: { id: string; label: string }[]) {
    const map = new Map(types.value.map(t => [t.id, t]))
    for (const it of arr) if (!map.has(it.id)) map.set(it.id, it)
    types.value = Array.from(map.values())
    persist()
  }

  // Accepts the MergeCachesResult shape from the composable
  function mergeCreated(result: any) {
    if (!result) return
    if (result.categories) mergeCategories(result.categories)
    if (result.depositaire1) mergeDepositaire1(result.depositaire1)
    if (result.depositaire2) mergeDepositaire2(result.depositaire2)
    if (result.types) mergeTypes(result.types)
  }

  return { categories, depositaire1, depositaire2, types, setCategories, mergeCreated }
})
