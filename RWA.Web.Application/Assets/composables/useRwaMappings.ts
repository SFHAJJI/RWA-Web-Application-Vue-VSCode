// Lightweight helper composable: fetch get-* endpoints and post mappings.
// Use in Vue components or plain JS/TS.

export type MissingRowDto = {
  identifiant: string;
  identifiantOrigine: string;
  nom: string;
  categorie1?: string | null;
  categorie2?: string | null;
  numLigne: number;
  depositaire1Id?: number | null;
  depositaire1Created?: boolean;
  depositaire2Id?: number | null;
  depositaire2Created?: boolean;
  suggestedRefCategorieRwa?: string | null;
  confidence: number;
};

export type CategoryDto = { id: string; label: string };

export type EquivalenceMappingDto = {
  source: string;
  depositaire1Id?: number | null;
  depositaire1?: string;
  depositaire2Id?: number | null;
  depositaire2?: string | null;
  refCategorieRwaId?: string | null;
  refCategorieRwaLibelle?: string | null;
  refTypeBloombergId?: string | null;
  refTypeBloombergLibelle?: string | null;
  identifiantOrigine: string;
};

const apiBase = '/api/workflow';

export async function fetchMissingRows(): Promise<MissingRowDto[]> {
  const res = await fetch(`${apiBase}/get-missing-rows`);
  if (!res.ok) throw new Error('Failed to load missing rows');
  return await res.json();
}

export async function fetchCategories(): Promise<CategoryDto[]> {
  const res = await fetch(`${apiBase}/get-categories`);
  if (!res.ok) throw new Error('Failed to load categories');
  return await res.json();
}

// mappings: array of Mapping DTOs. Prefer to populate *_Id fields on the client (hidden),
// server accepts ids or libelles and returns created ids in the response.
export async function postApplyEquivalences(mappings: EquivalenceMappingDto[]) {
  // Use postJson helper so UI shows global posting progress and relies on SignalR for authoritative updates.
  // The server still returns ApplyMappingsResponseDto (with created ids) in case callers need it,
  // but primary state updates are delivered via SignalR.
  const { postJson } = await import('../composables/useApi');
  const res = await postJson(`${apiBase}/post-apply-equivalences`, mappings);
  return res;
}

// Merge created items from server into local caches. Returns updated categories array.
export type MergeCachesResult = {
  categories: CategoryDto[];
  depositaire1?: { id: number; label: string }[];
  depositaire2?: { id: number; label: string }[];
  types?: { id: string; label: string }[];
}

export function mergeCreatedIntoCaches(categories: CategoryDto[], serverResult: any): MergeCachesResult {
  const result: MergeCachesResult = { categories: [...categories], depositaire1: [], depositaire2: [], types: [] };
  if (!serverResult || !serverResult.equivalenceResult) return result;
  const eq = serverResult.equivalenceResult;
  const map = new Map(result.categories.map(c => [c.id, c]));
  // created categories (id, libelle)
  if (Array.isArray(eq.createdCategorie)) {
    for (const pair of eq.createdCategorie) {
      const id = pair[0];
      const lib = pair[1];
      if (!map.has(id)) map.set(id, { id, label: lib });
    }
  }
  result.categories = Array.from(map.values());

  if (Array.isArray(eq.createdDepositaire1)) {
    for (const pair of eq.createdDepositaire1) {
      const id = pair[0];
      const lib = pair[1];
      result.depositaire1!.push({ id, label: lib });
    }
  }
  if (Array.isArray(eq.createdDepositaire2)) {
    for (const pair of eq.createdDepositaire2) {
      const id = pair[0];
      const lib = pair[1];
      result.depositaire2!.push({ id, label: lib });
    }
  }
  if (Array.isArray(eq.createdTypeBloomberg)) {
    for (const pair of eq.createdTypeBloomberg) {
      const id = pair[0];
      const lib = pair[1];
      result.types!.push({ id, label: lib });
    }
  }

  return result;
}

// Helper to merge server result into Pinia store - optional convenience
export async function mergeIntoPiniaStore(serverResult: any) {
  try {
    // dynamic import to avoid forcing pinia at module load
    const { useRwaCaches } = await import('../stores/rwaCaches')
    // pinia must be installed in app; this is a helper for dev/testing
    // calling code should call this inside setup() where pinia is available
    const storeModule = useRwaCaches();
    storeModule.mergeCreated(serverResult);
    return storeModule;
  } catch (e) {
    console.warn('Failed merging into Pinia store', e);
    return null;
  }
}

// Helper: build mapping payload for a single UI row. Assumes you keep ids hidden in the UI state.
export function buildMappingPayload(row: MissingRowDto, chosenCategory: CategoryDto | null, options?: { depositaire1Id?: number; depositaire2Id?: number; typeBloombergId?: string }) : EquivalenceMappingDto {
  return {
    source: row.identifiant, // or row.source if you keep it
    depositaire1Id: options?.depositaire1Id,
    depositaire1: row.categorie1 ?? undefined,
    depositaire2Id: options?.depositaire2Id,
    depositaire2: row.categorie2 ?? undefined,
    refCategorieRwaId: chosenCategory?.id ?? undefined,
    refCategorieRwaLibelle: chosenCategory?.label ?? row.suggestedRefCategorieRwa ?? undefined,
    refTypeBloombergId: options?.typeBloombergId ?? undefined,
    refTypeBloombergLibelle: undefined,
    identifiantOrigine: row.identifiantOrigine,
  } as EquivalenceMappingDto;
}
