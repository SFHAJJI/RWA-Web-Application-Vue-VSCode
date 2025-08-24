using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RWA.Web.Application.Models;

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
                // Create a fresh scope so we use a DbContext instance with a lifetime tied to this operation
                using var scope = _scopeFactory.CreateScope();
                var scopedDb = scope.ServiceProvider.GetRequiredService<RwaContext>();

                var scopedId = scopedDb.GetHashCode();
                _logger.LogInformation("💾 PersistInventoryRowsAsync START - Using scoped DbContext {DbContextId}, Rows: {RowCount}", scopedId, rows is System.Collections.ICollection c ? c.Count : -1);

                scopedDb.HecateInventaireNormalises.AddRange(rows);
                var saved = await scopedDb.SaveChangesAsync();

                _logger.LogInformation("✅ PersistInventoryRowsAsync SUCCESS - Scoped DbContext {DbContextId}, Saved {SavedCount}", scopedId, saved);
                return saved;
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
                return await WithDbAsync(async db => await db.WorkflowSteps.FirstOrDefaultAsync(s => s.StepName == stepName));
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
                    var cur = await db.WorkflowSteps.FirstOrDefaultAsync(s => s.Status == _statusOptions.CurrentStatus);
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

        public async Task SeedDefaultWorkflowIfEmptyAsync()
        {
            try
            {
                await WithDbAsync(async db =>
                {
                    var existing = await db.WorkflowSteps.ToListAsync();
                    if (existing.Any()) return;

                    var steps = new[] {
                        new WorkflowStep { StepName = "Upload inventory", Status = _statusOptions.CurrentStatus, DataPayload = "{}" },
                        new WorkflowStep { StepName = "RWA Category Manager", Status = _statusOptions.PendingStatus, DataPayload = "{}" },
                        new WorkflowStep { StepName = "BDD Manager", Status = _statusOptions.PendingStatus, DataPayload = "{}" },
                        new WorkflowStep { StepName = "RAF Manager", Status = _statusOptions.PendingStatus, DataPayload = "{}" },
                        new WorkflowStep { StepName = "Fichier Enrichie Generation", Status = _statusOptions.PendingStatus, DataPayload = "{}" }
                    };

                    db.WorkflowSteps.AddRange(steps);
                    await db.SaveChangesAsync();
                });
            }
            catch (ObjectDisposedException)
            {
                // Context disposed, ignore
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
                    var step = await db.WorkflowSteps.FirstOrDefaultAsync(s => s.StepName == stepName);
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
                                .FirstOrDefaultAsync(c => c.Libelle == row.Categorie1);

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

        private async Task<long> GetOrCreateCatDepositaire1Async(RwaContext db, string libelle)
        {
            var cat = await db.HecateCatDepositaire1s.FirstOrDefaultAsync(c => c.LibelleDepositaire1 == libelle);
            if (cat == null)
            {
                cat = new HecateCatDepositaire1 { LibelleDepositaire1 = libelle };
                db.HecateCatDepositaire1s.Add(cat);
                await db.SaveChangesAsync();
            }
            return cat.IdDepositaire1;
        }

        private async Task<long> GetOrCreateCatDepositaire2Async(RwaContext db, string libelle)
        {
            var cat = await db.HecateCatDepositaire2s.FirstOrDefaultAsync(c => c.LibelleDepositaire2 == libelle);
            if (cat == null)
            {
                cat = new HecateCatDepositaire2 { LibelleDepositaire2 = libelle };
                db.HecateCatDepositaire2s.Add(cat);
                await db.SaveChangesAsync();
            }
            return cat.IdDepositaire2;
        }

        public async Task<int> ApplyRwaMappingsAsync(System.Collections.Generic.List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> mappings)
        {
            // Fire-and-forget the creation of HecateEquivalenceCatRwa entities
            _ = Task.Run(() =>
            {
                Parallel.ForEach(mappings, async mapping =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<RwaContext>();

                    try
                    {
                        var cat1Id = await GetOrCreateCatDepositaire1Async(db, mapping.Cat1);
                        var cat2Id = await GetOrCreateCatDepositaire2Async(db, mapping.Cat2 ?? string.Empty);

                        var equivalence = new HecateEquivalenceCatRwa
                        {
                            Source = mapping.Source,
                            RefCatDepositaire1 = cat1Id,
                            RefCatDepositaire2 = cat2Id,
                            RefCategorieRwa = mapping.CategorieRwaId,
                            RefTypeBloomberg = mapping.TypeBloombergId
                        };

                        db.HecateEquivalenceCatRwas.Add(equivalence);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing RWA mapping for source {Source}", mapping.Source);
                    }
                });
            });

            // Return the number of mappings queued for processing
            return await Task.FromResult(mappings.Count);
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
                    var current = await db.WorkflowSteps.FirstOrDefaultAsync(s => s.Status == _statusOptions.CurrentStatus);
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
        
        public async Task<RWA.Web.Application.Models.Dtos.RwaCategoryManagerPayloadDto> ProcessRwaCategoryMappingAsync()
        {
            return await WithDbAsync(async db =>
            {
                // 1. Get all uploaded inventory rows that don't have RefCategorieRwa set
                var inventoryRows = await db.HecateInventaireNormalises
                    .Where(h => string.IsNullOrEmpty(h.RefCategorieRwa))
                    .ToListAsync();

                // 2. Get all equivalence mappings with their navigation properties
                var equivalenceMappings = await db.HecateEquivalenceCatRwas
                    .Include(e => e.RefCatDepositaire1Navigation)
                    .Include(e => e.RefCatDepositaire2Navigation)
                    .AsNoTracking()
                    .ToListAsync();

                // Create a lookup for faster mapping using the Libelle properties
                var mappingLookup = equivalenceMappings
                    .ToLookup(em => (
                        em.Source,
                        em.RefCatDepositaire1Navigation?.LibelleDepositaire1 ?? string.Empty,
                        em.RefCatDepositaire2Navigation?.LibelleDepositaire2 ?? string.Empty
                    ));

                var successfulMappings = new List<HecateInventaireNormalise>();
                var failedMappings = new List<HecateInventaireNormalise>();

                // 3. Process mappings in parallel
                Parallel.ForEach(inventoryRows, inventoryRow =>
                {
                    var key = (inventoryRow.Source, inventoryRow.Categorie1, inventoryRow.Categorie2);
                    var mapping = mappingLookup[key].FirstOrDefault();

                    if (mapping != null)
                    {
                        inventoryRow.RefCategorieRwa = mapping.RefCategorieRwa;
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
                        Libelle = c.Libelle 
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
                    step.Status = step.StepName == "Upload Inventory Files" ? "current" : "pending";
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
    }
}
