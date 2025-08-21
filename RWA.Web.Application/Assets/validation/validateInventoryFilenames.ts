// validateInventoryFilenames.ts
// Mirror of server-side FormatInventoryFileNameFluentValidator regex.
export type ValidationResult = {
  valid: boolean;
  badNames: string[];
  parsedMatches?: string[];
  error?: string;
};

const PATTERN = /^RWA_Report_(\d{3})_(\d{2})(\d{4})/;

export function validateFilename(name: string): boolean {
  if (!name) return false;
  return PATTERN.test(name);
}

export function validateFiles(files: FileList | File[]): ValidationResult {
  const arr = Array.isArray(files) ? files : Array.from(files);
  const badNames: string[] = [];
  const parsedMatches: string[] = [];

  arr.forEach(f => {
    // support either File objects or plain filename strings
    const name = (typeof f === 'string') ? f : ((f as any)?.name ?? String(f));
    if (!validateFilename(name)) badNames.push(name);
    else {
      const m = name.match(PATTERN);
      parsedMatches.push(m ? m[0] : '');
    }
  });

  return { valid: badNames.length === 0, badNames, parsedMatches };
}

export function validatePayloadJson(payload: string): ValidationResult {
  if (!payload || !payload.trim()) return { valid: true, badNames: [] };

  try {
    const parsed = JSON.parse(payload);
    if (!Array.isArray(parsed) || parsed.length === 0) return { valid: true, badNames: [] };

    const badNames: string[] = [];
    const parsedMatches: string[] = [];

    for (const row of parsed) {
      if (row && typeof row === 'object') {
        const name = (row.__SavedFileName ?? row.__OriginalFileName ?? '').toString();
        if (!name) {
          badNames.push('<missing filename>');
        } else if (!validateFilename(name)) {
          badNames.push(name);
        } else {
          const m = name.match(PATTERN);
          parsedMatches.push(m ? m[0] : '');
        }
      }
    }

    return { valid: badNames.length === 0, badNames, parsedMatches };
  } catch (err: any) {
    return { valid: false, badNames: [], error: 'Invalid JSON payload: ' + (err?.message ?? String(err)) };
  }
}
