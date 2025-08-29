import axios from 'axios';

export async function fetchFailed(cursor?: string, take = 20) {
  const { data } = await axios.get('/api/workflow/tethys-status', { params: { filter:'failed', cursor, take }});
  return data as { items:any[], nextCursor?:string, total:number };
}

export async function searchTethys(query: string, cursor?: string, take = 20) {
  const { data } = await axios.get('/api/workflow/tethys/search', { params: { q: query, cursor, take }});
  return data as { items:any[], nextCursor?:string, total:number };
}

export async function fetchSuggestions(numLigne: number) {
  const { data } = await axios.get(`/api/workflow/tethys/suggestions/${numLigne}`);
  return data as any[];
}

export async function assignRaf(numLigne: number, raf: string) {
  await axios.post('/api/workflow/tethys/assign', { numLigne, raf });
}

export async function getTethysStatus(filter = "all", cursor?: string, take = 20) {
  const { data } = await axios.get('/api/workflow/tethys-status', { params: { filter, cursor, take }});
  return data as { items:any[], nextCursor?:string, total:number };
}

export async function updateTethysStatus() {
  await axios.post('/api/workflow/update-tethys-status');
}
