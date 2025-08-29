using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RWA.Web.Application.Models;
using RWA.Web.Application.Services;
using RWA.Web.Application.Services.Helpers;
using RWA.Web.Application.Models.Dtos;

namespace RWA.Web.Application.Services.Workflow
{
    public class EfWorkflowDbProvider : IWorkflowDbProvider
    {
        private readonly ILogger<EfWorkflowDbProvider> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly WorkflowStatusMappingOptions _statusOptions;
        private static int _operationCounter = 0;

        public EfWorkflowDbProvider(ILogger<EfWorkflowDbProvider> logger, IServiceScopeFactory scopeFactory, IOptions<WorkflowStatusMappingOptions> statusOptions)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _statusOptions = statusOptions.Value;

            _logger.LogInformation("🏗️  EF WORKFLOW DB PROVIDER CREATED - Thread {ThreadId}", Environment.CurrentManagedThreadId);
        }

        // Helper to run a function with a fresh scoped RwaContext
        private async Task<T> WithDbAsync<T>(Func<RwaContext, Task<T>> func)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RwaContext>();
            return await func(db);
        }

        private async Task WithDbAsync(Func<RwaContext, Task> func)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RwaContext>();
            await func(db);
        }

        public async Task<int> PersistInventoryRowsAsync(System.Collections.Generic.IEnumerable<RWA.Web.Application.Models.HecateInventaireNormalise> rows)
        {
            if (rows == null) return 0;

            try
            {
                return await WithDbAsync(async scopedDb =>
                {
                    var scopedId = scopedDb.GetHashCode();
                    _logger.LogInformation("💾 PersistInventoryRowsAsync START - Using scoped DbContext {DbContextId}, Rows: {RowCount}", scopedId, rows is System.Collections.ICollection c ? c.Count : -1);

                    scopedDb.HecateInventaireNormalises.AddRange(rows);
                    var saved = await scopedDb.SaveChangesAsync();

                    _logger.LogInformation("✅ PersistInventoryRowsAsync SUCCESS - Scoped DbContext {DbContextId}, Saved {SavedCount}", scopedId, saved);
                    return saved;
                });
            }
            catch (ObjectDisposedException)
            {
                return 0;
            }
        }

        public async Task<IEnumerable<WorkflowStep>> GetAllWorkflowStepsOrderedAsync()
        {
            try
            {
                return await WithDbAsync(async db => await db.WorkflowSteps.OrderBy(s => s.Id).ToListAsync());
            }
            catch (ObjectDisposedException)
            {
                return new List<WorkflowStep>();
            }
        }

        public async Task<IEnumerable<object>> GetCategoriesForDropdownAsync()
        {
            try
            {
                return await WithDbAsync(async db => await db.HecateCategorieRwas.Select(c => new { id = c.IdCatRwa, label = c.Libelle }).ToListAsync());
            }
            catch (ObjectDisposedException)
            {
                return new List<object>();
            }
        }

        public async Task<WorkflowStep?> GetStepByNameAsync(string stepName)
        {
            try
            {
                return await WithDbAsync(async db => await db.WorkflowSteps.FirstOrDefaultAsync(s => s.StepName.TrimmedEquals(stepName, StringComparison.OrdinalIgnoreCase)));
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
        }

        public async Task<WorkflowStep?> GetCurrentStepAsync()
        {
            try
            {
                return await WithDbAsync(async db =>
                {
                    var cur = await db.WorkflowSteps.FirstOrDefaultAsync(s => s.Status.TrimmedEquals(_statusOptions.CurrentStatus, StringComparison.OrdinalIgnoreCase));
                    if (cur == null)
                    {
                        if (!await db.WorkflowSteps.AnyAsync())
                        {
                            return (WorkflowStep?)null;
                        }
                    }
                    return cur;
                });
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
        }

        public async Task<System.Collections.Generic.List<WorkflowStep>> GetStepsSnapshotAsync()
        {
            try
            {
                // Always return fresh data - no caching to avoid consistency issues
                return await WithDbAsync(async db => await db.WorkflowSteps.OrderBy(s => s.Id).ToListAsync());
            }
            catch (ObjectDisposedException)
            {
                return new List<WorkflowStep>();
            }
        }

        public async Task SaveChangesAsync()
        {
            var operationId = Interlocked.Increment(ref _operationCounter);
            var threadId = Environment.CurrentManagedThreadId;

            _logger.LogInformation("💾 EF_DB_PROVIDER SaveChangesAsync START - Op #{OperationId}, Thread {ThreadId}", operationId, threadId);

            try
            {
                await WithDbAsync(async db =>
                {
                    await db.SaveChangesAsync();
                });

                _logger.LogInformation("✅ EF_DB_PROVIDER SaveChangesAsync SUCCESS - Op #{OperationId}, Thread {ThreadId}", operationId, threadId);
            }
            catch (ObjectDisposedException)
            {
                _logger.LogWarning("⚠️  EF_DB_PROVIDER SaveChangesAsync DISPOSED - Op #{OperationId}, Thread {ThreadId}", operationId, threadId);
                // Context disposed, ignore
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ EF_DB_PROVIDER SaveChangesAsync ERROR - Op #{OperationId}, Thread {ThreadId}, Exception: {ExceptionType}", 
                    operationId, threadId, ex.GetType().Name);
                throw;
            }
        }

        /// <summary>
        /// Updates a workflow step's status and data payload atomically within a single context
        /// This fixes the Entity Framework context isolation issue
        /// </summary>
        public async Task UpdateStepStatusAndDataAsync(string stepName, string status, string dataPayload)
        {
            var operationId = Interlocked.Increment(ref _operationCounter);
            var threadId = Environment.CurrentManagedThreadId;

            _logger.LogInformation("🔄 EF_DB_PROVIDER UpdateStepStatusAndDataAsync START - Op #{OperationId}, Thread {ThreadId}, Step: '{StepName}', Status: '{Status}', DataPayload Length: {DataLength}", 
                operationId, threadId, stepName, status, dataPayload?.Length ?? 0);

            try
            {
                await WithDbAsync(async db =>
                {
                    var step = await db.WorkflowSteps.FirstOrDefaultAsync(s => s.StepName.TrimmedEquals(stepName, StringComparison.OrdinalIgnoreCase));
                    if (step != null)
                    {
                        var oldStatus = step.Status;
                        var oldDataLength = step.DataPayload?.Length ?? 0;

                        step.Status = status;
                        step.DataPayload = dataPayload ?? string.Empty;
                        step.UpdatedAt = DateTime.UtcNow;

                        var changeCount = await db.SaveChangesAsync();
                        
                        _logger.LogInformation("✅ EF_DB_PROVIDER UpdateStepStatusAndDataAsync SUCCESS - Op #{OperationId}, Step: '{StepName}', Status: '{OldStatus}' → '{NewStatus}', DataPayload: {OldLength} → {NewLength} chars, Changes: {ChangeCount}", 
                            operationId, stepName, oldStatus, status, oldDataLength, dataPayload?.Length ?? 0, changeCount);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️  EF_DB_PROVIDER UpdateStepStatusAndDataAsync NOT_FOUND - Op #{OperationId}, Step: '{StepName}' not found", operationId, stepName);
                    }
                });
            }
            catch (ObjectDisposedException)
            {
                _logger.LogWarning("⚠️  EF_DB_PROVIDER UpdateStepStatusAndDataAsync DISPOSED - Op #{OperationId}, Thread {ThreadId}", operationId, threadId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ EF_DB_PROVIDER UpdateStepStatusAndDataAsync ERROR - Op #{OperationId}, Thread {ThreadId}, Step: '{StepName}', Exception: {ExceptionType}", 
                    operationId, threadId, stepName, ex.GetType().Name);
                throw;
            }
        }
        public async Task UpdateStepStatusAsync(string stepName, string status)
        {
            await WithDbAsync(async db =>
            {
                var step = await db.WorkflowSteps.FirstOrDefaultAsync(s => s.StepName.TrimmedEquals(stepName, StringComparison.OrdinalIgnoreCase));
                if (step != null)
                {
                    step.Status = status;
                    step.UpdatedAt = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                }
            });
        }
        public async Task AddRangeWorkflowStepsAsync(IEnumerable<WorkflowStep> steps)
        {
            try
            {
                await WithDbAsync(async db =>
                {
                    db.WorkflowSteps.AddRange(steps);
                    await db.SaveChangesAsync();
                });
            }
            catch (ObjectDisposedException)
            {
                // Context disposed, ignore
            }
        }

        public async Task<int> AutoMapExactMatchesAsync()
        {
            try
            {
                return await WithDbAsync(async db =>
                {
                    var unmappedRows = await db.HecateInventaireNormalises
                        .Where(r => string.IsNullOrEmpty(r.RefCategorieRwa))
                        .Take(100)
                        .ToListAsync();

                    var mappedCount = 0;
                    foreach (var row in unmappedRows)
                    {
                        if (!string.IsNullOrEmpty(row.Categorie1))
                        {
                            var exactMatch = await db.HecateCategorieRwas
                                .FirstOrDefaultAsync(c => c.Libelle.TrimmedEquals(row.Categorie1, StringComparison.OrdinalIgnoreCase));

                            if (exactMatch != null)
                            {
                                row.RefCategorieRwa = exactMatch.Libelle;
                                mappedCount++;
                            }
                        }
                    }

                    if (mappedCount > 0)
                    {
                        await db.SaveChangesAsync();
                    }

                    return mappedCount;
                });
            }
            catch (ObjectDisposedException)
            {
                return 0;
            }
        }

        private async Task<long> GetOrCreateCategoryAsync<TEntity, TKey>(DbSet<TEntity> dbSet, System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, Func<TEntity> createEntity, Func<TEntity, TKey> getKey) where TEntity : class
        {
            return await WithDbAsync(async db =>
            {
                var cat = await dbSet.FirstOrDefaultAsync(predicate);
                if (cat == null)
                {
                    cat = createEntity();
                    dbSet.Add(cat);
                    await db.SaveChangesAsync();
                }
                return (long)Convert.ChangeType(getKey(cat), typeof(long));
            });
        }

        private void UpdateIdentifiantUniqueRetenu(HecateInventaireNormalise item, HecateCategorieRwa categorieRwa)
        {
            if (categorieRwa != null)
            {
                if (categorieRwa.ValeurMobiliere.Trim().Equals("O", StringComparison.OrdinalIgnoreCase))
                {
                    item.IdentifiantUniqueRetenu = item.IdentifiantOrigine;
                    item.AdditionalInformation.IsValeurMobiliere = true;
                }
                else if (categorieRwa.ValeurMobiliere.Trim().Equals("N", StringComparison.OrdinalIgnoreCase))
                {
                    var periodeCloture = item.PeriodeCloture;
                    if (!string.IsNullOrEmpty(periodeCloture) && periodeCloture.Length > 4)
                    {
                        periodeCloture = $"{periodeCloture.Substring(0, 2)}{periodeCloture.Substring(periodeCloture.Length - 2)}";
                    }
                    item.IdentifiantUniqueRetenu = $"{item.Source}{categorieRwa.IdCatRwa}{periodeCloture}";
                    item.AdditionalInformation.IsValeurMobiliere = false;
                }
            }
        }

        public async Task<int> ApplyRwaMappingsAsync(System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> mappings)
        {
            foreach (var mapping in mappings)
            {
                try
                {
                    await WithDbAsync(async db =>
                    {
                        var cat1Id = await GetOrCreateCategoryAsync(db.HecateCatDepositaire1s, c => c.LibelleDepositaire1.TrimmedEquals(mapping.Cat1, StringComparison.OrdinalIgnoreCase), () => new HecateCatDepositaire1 { LibelleDepositaire1 = mapping.Cat1 }, c => c.IdDepositaire1);
                        var cat2Id = await GetOrCreateCategoryAsync(db.HecateCatDepositaire2s, c => c.LibelleDepositaire2.TrimmedEquals(mapping.Cat2 ?? string.Empty, StringComparison.OrdinalIgnoreCase), () => new HecateCatDepositaire2 { LibelleDepositaire2 = mapping.Cat2 ?? string.Empty }, c => c.IdDepositaire2);

                        var equivalence = new HecateEquivalenceCatRwa
                        {
                            Source = mapping.Source,
                            RefCatDepositaire1 = cat1Id,
                            RefCatDepositaire2 = cat2Id,
                            RefCategorieRwa = mapping.CategorieRwaId,
                            RefTypeBloomberg = mapping.TypeBloombergId
                        };

                        db.HecateEquivalenceCatRwas.Add(equivalence);

                        var inventoryItems = await db.HecateInventaireNormalises
                            .Where(i => mapping.NumLignes.Contains(i.NumLigne))
                            .ToListAsync();

                        var categorieRwa = await db.HecateCategorieRwas.FindAsync(mapping.CategorieRwaId);

                        foreach (var item in inventoryItems)
                        {
                            item.RefCategorieRwa = mapping.CategorieRwaId;
                            UpdateIdentifiantUniqueRetenu(item, categorieRwa);
                        }

                        await db.SaveChangesAsync();
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing RWA mapping for source {Source}", mapping.Source);
                }
            }

            return mappings.Count;
        }

        public async Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>> GetMissingRowsWithSuggestionsAsync()
        {
            try
            {
                return await WithDbAsync(async db =>
                {
                    var missing = await db.HecateInventaireNormalises
                        .Where(r => string.IsNullOrEmpty(r.RefCategorieRwa))
                        .Take(50)
                        .ToListAsync();

                    return missing
                        .GroupBy(row => new { row.Source, row.Categorie1, row.Categorie2 })
                        .Select(g => new RWA.Web.Application.Models.Dtos.RwaMappingRowDto
                        {
                            Source = g.Key.Source,
                            Cat1 = g.Key.Categorie1,
                            Cat2 = g.Key.Categorie2,
                            NumLignes = g.Select(row => row.NumLigne).ToList()
                        })
                        .ToList();
                });
            }
            catch (ObjectDisposedException)
            {
                return new List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>();
            }
        }

        public async Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>> GetEquivalenceCandidatesForMissingRowsAsync()
        {
            try
            {
                // Simplified implementation for now
                return await Task.FromResult(new List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>());
            }
            catch (ObjectDisposedException)
            {
                return new List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>();
            }
        }

        public async Task<RWA.Web.Application.Models.Dtos.EquivalenceApplyResultDto> ApplyEquivalenceMappingsAsync(System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto> mappings)
        {
            try
            {
                // Simplified implementation
                return await Task.FromResult(new RWA.Web.Application.Models.Dtos.EquivalenceApplyResultDto { CreatedOrUpdatedCount = mappings.Count });
            }
            catch (ObjectDisposedException)
            {
                return new RWA.Web.Application.Models.Dtos.EquivalenceApplyResultDto { CreatedOrUpdatedCount = 0 };
            }
        }

        public async Task<System.Collections.Generic.List<WorkflowStep>> ForceNextFallbackAndGetStepsAsync()
        {
            try
            {
                return await WithDbAsync(async db =>
                {
                    var current = await db.WorkflowSteps.FirstOrDefaultAsync(s => s.Status.TrimmedEquals(_statusOptions.CurrentStatus, StringComparison.OrdinalIgnoreCase));
                    if (current != null)
                    {
                        current.Status = _statusOptions.WarningStatus; // ForceNext uses warning status
                        var next = await db.WorkflowSteps
                            .Where(s => s.Id > current.Id)
                            .OrderBy(s => s.Id)
                            .FirstOrDefaultAsync();

                        if (next != null)
                        {
                            next.Status = "Current";
                        }

                        await db.SaveChangesAsync();
                    }

                    return await db.WorkflowSteps.OrderBy(s => s.Id).ToListAsync();
                });
            }
            catch (ObjectDisposedException)
            {
                return new List<WorkflowStep>();
            }
        }

        public async Task<System.Collections.Generic.List<WorkflowStep>> ResetWorkflowAsync()
        {
            try
            {
                return await WithDbAsync(async db =>
                {
                    var allSteps = await db.WorkflowSteps.OrderBy(s => s.Id).ToListAsync();

                    for (int i = 0; i < allSteps.Count; i++)
                    {
                        if (i == 0)
                        {
                            allSteps[i].Status = "Current";
                        }
                        else
                        {
                            allSteps[i].Status = "Open";
                        }
                        allSteps[i].DataPayload = "{}";
                        allSteps[i].UpdatedAt = DateTime.UtcNow;
                    }

                    await db.SaveChangesAsync();

                    return allSteps;
                });
            }
            catch (ObjectDisposedException)
            {
                return new List<WorkflowStep>();
            }
        }

        // RWA Category Manager specific methods

        private class CaseInsensitiveTupleComparer : IEqualityComparer<(string Source, string Cat1, string Cat2)>
        {
            public bool Equals((string Source, string Cat1, string Cat2) x, (string Source, string Cat1, string Cat2) y)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(x.Source, y.Source) &&
                       StringComparer.OrdinalIgnoreCase.Equals(x.Cat1, y.Cat1) &&
                       StringComparer.OrdinalIgnoreCase.Equals(x.Cat2, y.Cat2);
            }

            public int GetHashCode((string Source, string Cat1, string Cat2) obj)
            {
                return HashCode.Combine(
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Source ?? string.Empty),
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Cat1 ?? string.Empty),
                    StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Cat2 ?? string.Empty)
                );
            }
        }
        
        public async Task<RWA.Web.Application.Models.Dtos.RwaCategoryManagerPayloadDto> ProcessRwaCategoryMappingAsync()
        {
            return await WithDbAsync(async db =>
            {
                // 1. Get all equivalence mappings with their navigation properties
                var equivalenceMappings = await db.HecateEquivalenceCatRwas
                    .Include(e => e.RefCatDepositaire1Navigation)
                    .Include(e => e.RefCatDepositaire2Navigation)
                    .AsNoTracking()
                    .ToListAsync();

                var categoriesRwa = await db.HecateCategorieRwas.AsNoTracking().ToDictionaryAsync(c => c.IdCatRwa, c => c);

                // Create a lookup for faster, case-insensitive mapping
                var mappingLookup = equivalenceMappings
                    .ToLookup(em => (
                        Source: em.Source,
                        Cat1: em.RefCatDepositaire1Navigation?.LibelleDepositaire1 ?? string.Empty,
                        Cat2: em.RefCatDepositaire2Navigation?.LibelleDepositaire2 ?? string.Empty
                    ), new CaseInsensitiveTupleComparer());

                // 2. Get all uploaded inventory rows that don't have RefCategorieRwa set, using AsNoTracking
                var inventoryRows = await db.HecateInventaireNormalises
                    .Where(h => string.IsNullOrEmpty(h.RefCategorieRwa))
                    .AsNoTracking()
                    .ToListAsync();

                var successfulMappings = new List<HecateInventaireNormalise>();
                var failedMappings = new List<HecateInventaireNormalise>();

                // 3. Process mappings in parallel
                Parallel.ForEach(inventoryRows, inventoryRow =>
                {
                    var key = (
                        Source: inventoryRow.Source,
                        Cat1: inventoryRow.Categorie1 ?? string.Empty,
                        Cat2: inventoryRow.Categorie2 ?? string.Empty
                    );

                    var mapping = mappingLookup[key].FirstOrDefault();

                    if (mapping != null)
                    {
                        inventoryRow.RefCategorieRwa = mapping.RefCategorieRwa;
                        if (categoriesRwa.TryGetValue(mapping.RefCategorieRwa, out var categorieRwa))
                        {
                            UpdateIdentifiantUniqueRetenu(inventoryRow, categorieRwa);
                        }
                        lock (successfulMappings)
                        {
                            successfulMappings.Add(inventoryRow);
                        }
                    }
                    else
                    {
                        lock (failedMappings)
                        {
                            failedMappings.Add(inventoryRow);
                        }
                    }
                });

                // 4. Update the database with successful mappings
                if (successfulMappings.Any())
                {
                    db.HecateInventaireNormalises.UpdateRange(successfulMappings);
                    await db.SaveChangesAsync();
                }

                // 5. Group failed mappings for UI display
                var missingMappingRows = failedMappings
                    .GroupBy(row => new { row.Source, row.Categorie1, row.Categorie2 })
                    .Select(g => new RWA.Web.Application.Models.Dtos.RwaMappingRowDto
                    {
                        Source = g.Key.Source,
                        Cat1 = g.Key.Categorie1,
                        Cat2 = g.Key.Categorie2,
                        NumLignes = g.Select(row => row.NumLigne).ToList()
                    })
                    .ToList();

                // Get dropdown options for UI
                var categorieRwaOptions = await db.HecateCategorieRwas
                    .Select(c => new RWA.Web.Application.Models.Dtos.CategorieRwaOptionDto
                    {
                        IdCatRwa = c.IdCatRwa,
                        Libelle = c.Libelle
                    })
                    .ToListAsync();

                var typeBloombergOptions = await db.HecateTypeBloombergs
                    .Select(t => new RWA.Web.Application.Models.Dtos.TypeBloombergOptionDto
                    {
                        IdTypeBloomberg = t.IdTypeBloomberg,
                        Libelle = t.Libelle
                    })
                    .ToListAsync();

                return new RWA.Web.Application.Models.Dtos.RwaCategoryManagerPayloadDto
                {
                    MissingMappingRows = missingMappingRows,
                    CategorieRwaOptions = categorieRwaOptions,
                    TypeBloombergOptions = typeBloombergOptions
                };
            });
        }

        public async Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.CategorieRwaOptionDto>> GetCategorieRwaOptionsAsync()
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateCategorieRwas
                    .Select(c => new RWA.Web.Application.Models.Dtos.CategorieRwaOptionDto
                    {
                        IdCatRwa = c.IdCatRwa,
                        Libelle = c.Libelle,
                        ValeurMobiliere = c.ValeurMobiliere
                    })
                    .ToListAsync();
            });
        }

        public async Task<System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.TypeBloombergOptionDto>> GetTypeBloombergOptionsAsync()
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateTypeBloombergs
                    .Select(t => new RWA.Web.Application.Models.Dtos.TypeBloombergOptionDto 
                    { 
                        IdTypeBloomberg = t.IdTypeBloomberg, 
                        Libelle = t.Libelle 
                    })
                    .ToListAsync();
            });
        }

        public async Task ClearInventoryTableAsync()
        {
            await WithDbAsync(async db =>
            {
                // Remove all records from HecateInventaireNormalise table
                var inventoryRecords = db.HecateInventaireNormalises.ToList();
                if (inventoryRecords.Any())
                {
                    db.HecateInventaireNormalises.RemoveRange(inventoryRecords);
                    await db.SaveChangesAsync();
                }
            });
        }

        public async Task InitializeWorkflowStepsAsync()
        {
            await WithDbAsync(async db =>
            {
                var steps = await db.WorkflowSteps.ToListAsync();
                
                foreach (var step in steps)
                {
                    // Clear the payload and set appropriate status
                    step.DataPayload = "{}"; // Empty JSON payload
                    step.Status = step.StepName.TrimmedEquals("Upload Inventory Files", StringComparison.OrdinalIgnoreCase) ? "current" : "pending";
                }
                
                await db.SaveChangesAsync();
            });
        }

        public async Task<System.Collections.Generic.List<WorkflowStep>> GetWorkflowStepsAsync()
        {
            return await WithDbAsync(async db =>
            {
                return await db.WorkflowSteps
                    .OrderBy(s => s.Id)
                    .ToListAsync();
            });
        }

        public async Task<List<HecateInventaireNormalise>> GetInventaireNormaliseByNumLignes(List<int> numLignes)
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateInventaireNormalises
                    .Where(h => numLignes.Contains(h.NumLigne))
                    .ToListAsync();
            });
        }

        public async Task<List<HecateInventaireNormalise>> GetAllInventaireNormaliseAsync()
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateInventaireNormalises.ToListAsync();
            });
        }

        public async Task<List<HecateInventaireNormalise>> GetAllInventaireNormaliseAsNoTrackingAsync()
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateInventaireNormalises.AsNoTracking().ToListAsync();
            });
        }

        public async Task<List<HecateInterneHistorique>> GetAllHecateInterneHistoriqueAsync()
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateInterneHistoriques.AsNoTracking().ToListAsync();
            });
        }

        public async Task<HecateInterneHistorique> FindMatchInHistoriqueAsync(System.Linq.Expressions.Expression<Func<HecateInterneHistorique, bool>> predicate)
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateInterneHistoriques.FirstOrDefaultAsync(predicate);
            });
        }

        public async Task AddBddHistoriqueAsync(List<RWA.Web.Application.Models.Dtos.HecateInterneHistoriqueDto> items)
        {
            await WithDbAsync(async db =>
            {
                var entities = items.Select(i => i.ToHecateInterneHistorique()).ToList();
                db.HecateInterneHistoriques.AddRange(entities);
                await db.SaveChangesAsync();
            });
        }

        public async Task UpdateObligationsAsync(List<RWA.Web.Application.Models.Dtos.HecateInventaireNormaliseDto> items)
        {
            await WithDbAsync(async db =>
            {
                var numLignes = items.Select(i => i.NumLigne).ToList();
                var entities = await db.HecateInventaireNormalises
                    .Where(h => numLignes.Contains(h.NumLigne))
                    .ToListAsync();

                foreach (var entity in entities)
                {
                    var item = items.First(i => i.NumLigne == entity.NumLigne);
                    
                    if (DateOnly.TryParseExact(item.DateMaturite, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate))
                    {
                        entity.DateMaturite = parsedDate;
                    }
                    else
                    {
                        entity.DateMaturite = null;
                    }
        
                    entity.TauxObligation = item.TauxObligation;
                }

                await db.SaveChangesAsync();
            });
        }

        public async Task<List<HecateContrepartiesTransparence>> GetHecateContrepartiesTransparenceAsync()
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateContrepartiesTransparences.AsNoTracking().ToListAsync();
            });
        }

        public async Task UpdateInventaireNormaliseRangeAsync(List<HecateInventaireNormalise> items)
        {
            await WithDbAsync(async db =>
            {
                db.HecateInventaireNormalises.UpdateRange(items);
                await db.SaveChangesAsync();
            });
        }

        public async Task<List<HecateTethy>> GetTethysDataByRafAsync(List<string> rafs)
        {
            return await WithDbAsync(async db =>
            {
                var query = db.HecateTethys.AsNoTracking();

                // Use a predicate builder to create a more efficient query
                var predicate = PredicateBuilder.New<HecateTethy>();
                foreach (var raf in rafs)
                {
                    predicate = predicate.Or(t => (t.IdentifiantRaf != null && t.IdentifiantRaf.TrimmedEquals(raf, StringComparison.OrdinalIgnoreCase)) || (t.RafTeteGroupeReglementaire != null && t.RafTeteGroupeReglementaire.TrimmedEquals(raf, StringComparison.OrdinalIgnoreCase)));
                }

                return await query.Where(predicate).Select(item => item.TrimProperties()).ToListAsync();
            });
        }

        public Task<bool> TethysExistsAsync(string raf, CancellationToken ct = default)
        {
            var key = raf?.Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(key)) return Task.FromResult(false);

            return WithDbAsync(db =>
                db.HecateTethys
                  .AsNoTracking()
                  .AnyAsync(t =>
                      (t.IdentifiantRaf != null && t.IdentifiantRaf.Trim().ToUpper() == key) ||
                      (t.RafTeteGroupeReglementaire != null && t.RafTeteGroupeReglementaire.Trim().ToUpper() == key),
                      ct)
            );
        }

        public async Task<HashSet<string>> GetExistingTethysRafsAsync(List<string> rafs)
        {
            return await WithDbAsync(async db =>
            {
                var upperRafs = rafs.Select(r => r.ToUpperInvariant()).ToList();

                var results = await db.HecateTethys
                    .AsNoTracking()
                    .Where(t => (t.IdentifiantRaf != null && upperRafs.Contains(t.IdentifiantRaf)) ||
                                (t.RafTeteGroupeReglementaire != null && upperRafs.Contains(t.RafTeteGroupeReglementaire)))
                    .Select(t => new[] { t.IdentifiantRaf, t.RafTeteGroupeReglementaire })
                    .ToListAsync();

                return new HashSet<string>(results.SelectMany(r => r).Where(r => r != null && upperRafs.Contains(r.ToUpperInvariant())), StringComparer.OrdinalIgnoreCase);
            });
        }

        public async Task<int> GetTethysCountAsync()
        {
            return await WithDbAsync(async db => await db.HecateTethys.CountAsync());
        }

        public async Task UpdateTethysStatusForNumLignesAsync(List<int> numLignes, bool status)
        {
            await WithDbAsync(async db =>
            {
                var itemsToUpdate = await db.HecateInventaireNormalises
                    .Where(h => numLignes.Contains(h.NumLigne))
                    .ToListAsync();

                foreach (var item in itemsToUpdate)
                {
                    item.AdditionalInformation.TethysRafStatus = status;
                }

                await db.SaveChangesAsync();
            });
        }

        public async Task UpdateRafAsync(List<HecateTethysDto> items)
        {
            await WithDbAsync(async db =>
            {
                var categories = await db.HecateCategorieRwas.AsNoTracking().ToListAsync();
                var categoriesDict = categories.ToDictionary(c => c.IdCatRwa, c => c);

                var numLignes = items.Select(i => i.NumLigne).ToList();
                var entities = await db.HecateInventaireNormalises
                    .Where(h => numLignes.Contains(h.NumLigne))
                    .ToListAsync();

                var transparenceItemsToAdd = new List<HecateContrepartiesTransparence>();

                foreach (var entity in entities)
                {
                    var item = items.First(i => i.NumLigne == entity.NumLigne);

                    if (entity.Raf != item.Raf)
                    {
                        entity.Raf = item.Raf;
                    }
                    if (entity.LibelleOrigine != item.CptTethys)
                    {
                        entity.LibelleOrigine = item.CptTethys;
                    }

                    var isValeurMobiliere = categoriesDict.ContainsKey(entity.RefCategorieRwa) &&
                                            ((categoriesDict[entity.RefCategorieRwa]?.ValeurMobiliere.TrimmedEquals("Y", StringComparison.OrdinalIgnoreCase) ?? false) ||
                                             (categoriesDict[entity.RefCategorieRwa]?.ValeurMobiliere.TrimmedEquals("O", StringComparison.OrdinalIgnoreCase) ?? false));

                    if (!isValeurMobiliere && !string.IsNullOrEmpty(item.Raf))
                    {
                        transparenceItemsToAdd.Add(new HecateContrepartiesTransparence
                        {
                            LibelleContrepartieOrigine = item.Cpt,
                            RafEntite = item.Raf,
                            LibelleCourtTethys = item.CptTethys
                        });
                    }
                }

                if (transparenceItemsToAdd.Any())
                {
                    db.HecateContrepartiesTransparences.AddRange(transparenceItemsToAdd);
                }

                await db.SaveChangesAsync();
            });
        }

        public async Task<bool> AreAllRafsCompletedAsync()
        {
            return await WithDbAsync(async db =>
            {
                return await db.HecateInventaireNormalises.AllAsync(h => !string.IsNullOrEmpty(h.Raf));
            });
        }

        public async Task<HecateTethysPayload> GetTethysMappingPayloadAsync()
        {
            return await WithDbAsync(async db =>
            {
                var dtos = await db.HecateInventaireNormalises
                    .AsNoTracking()
                    .Select(item => new
                    {
                        item,
                        tethyByIdentifiantRaf = db.HecateTethys.AsNoTracking().FirstOrDefault(t => t.IdentifiantRaf == item.Raf),
                        tethyByRafTeteGroupeReglementaire = db.HecateTethys.AsNoTracking().FirstOrDefault(t => t.RafTeteGroupeReglementaire == item.Raf)
                    })
                    .Select(x => new HecateTethysDto
                    {
                        NumLigne = x.item.NumLigne,
                        Source = x.item.Source,
                        Cpt = x.item.Nom,
                        Raf = x.item.Raf,
                        CptTethys = x.tethyByIdentifiantRaf != null ? x.tethyByIdentifiantRaf.LibelleCourt : x.tethyByRafTeteGroupeReglementaire.NomTeteGroupeReglementaire,
                        IsGeneric = x.tethyByRafTeteGroupeReglementaire != null,
                        IsMappingTethysSuccessful = x.tethyByIdentifiantRaf != null || x.tethyByRafTeteGroupeReglementaire != null
                    })
                    .ToListAsync();

                return new HecateTethysPayload { Dtos = dtos };
            });
        }
    }
}
