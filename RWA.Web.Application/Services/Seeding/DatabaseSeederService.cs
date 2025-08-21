using Microsoft.EntityFrameworkCore;
using RWA.Web.Application.Models;
using System.Diagnostics;
using System.Data;
using System.Collections;
using RWA.Web.Application.Services.ExcelManagementService.Import;

namespace RWA.Web.Application.Services.Seeding
{
    public class DatabaseSeederService
    {
        private readonly RwaContext _context;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly string _rwaDatoDir;

        public DatabaseSeederService(RwaContext context, ILogger logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
            _rwaDatoDir = Path.Combine(environment.ContentRootPath, "RWA Data");
        }

        public async Task SeedDatabaseAsync()
        {
            var totalStopwatch = Stopwatch.StartNew();
            _logger.LogInformation("🚀 ULTRA-FAST PARALLEL DATATABLE SEEDING INITIATED");

            try
            {
                // Quick check if already seeded
                if (await _context.CommonUsers.AnyAsync())
                {
                    _logger.LogInformation("⚡ Database already seeded - SKIPPING");
                    return;
                }

                // PHASE 1: 🔥 PARALLEL INDEPENDENT TABLE SEEDING (5 simultaneous from EquivalenceCatRWA.xlsx)
                var phase1Tasks = new Task[]
                {
                    SeedBddHistoriqueAsync(),
                    SeedHecateCategorieRwaFromEquivalenceFileAsync(),
                    SeedHecateTypeBloombergFromEquivalenceFileAsync(),
                    SeedHecateCatDepositaire1FromEquivalenceFileAsync(),
                    SeedHecateCatDepositaire2FromEquivalenceFileAsync()
                };

                _logger.LogInformation("⚡ PHASE 1: Launching 5 parallel independent DataTable seeds from EquivalenceCatRWA.xlsx...");
                await Task.WhenAll(phase1Tasks);

                // PHASE 2: 🎯 DEPENDENT TABLE (awaits Phase 1 completion)
                _logger.LogInformation("⚡ PHASE 2: Processing dependent table with FK lookups...");
                await SeedHecateEquivalenceCatRwaAsync();

                // PHASE 3: 👥 USER DATA
                await SeedUsersAsync();

                totalStopwatch.Stop();
                _logger.LogInformation($"🏆 ULTRA-FAST SEEDING COMPLETED: {totalStopwatch.ElapsedMilliseconds}ms total");
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 SEEDING FAILED: {ex.Message}");
                throw;
            }
        }

        private async Task SeedBddHistoriqueAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Templates", "HECATE", "BDDHistorique.xlsx");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"⚠️  BDD Historique file not found: {filePath}");
                    return;
                }

                // ULTRA-FAST: Direct DataTableCollection read (no row iteration)
                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var bddDt = tables["BDD"];
                
                if (bddDt?.Rows.Count > 0)
                {
                    // PARALLEL: AsParallel() for row processing + bulk insert
                    var entities = bddDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => new HecateInterneHistorique
                        {
                            Source = row.Field<string>("Source") ?? string.Empty,
                            RefCategorieRwa = row.Field<string>("Catégorie RWA") ?? string.Empty,
                            IdentifiantUniqueRetenu = row.Field<string>("Identifiant unique retenu") ?? string.Empty,
                            Raf = row.Field<string>("RAF") ?? string.Empty,
                            LibelleOrigine = row.Field<string>("Libellé d'origine") ?? string.Empty,
                            DateEcheance = TryParseDateOnly(row.Field<object>("Date d'échéance") ?? DateTime.MinValue),
                            IdentifiantOrigine = row.Field<string>("Identifiant d'origine") ?? string.Empty,
                            Bbgticker = string.Empty, // Not in worksheet columns provided
                            LibelleTypeDette = "SUBORDONNE", // Always set to 'SUBORDONNE' as requested
                            LastUpdate = DateTime.Now.ToString("dd-MM-yyyy_Hmmss")
                        })
                        .ToList();

                    // Single bulk insert - fastest DB operation
                    await _context.HecateInterneHistoriques.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    _logger.LogInformation($"⚡ BDD Historique: {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 BDD Historique failed: {ex.Message}");
            }
        }

        private async Task SeedHecateCategorieRwaFromEquivalenceFileAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Param", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var catDt = tables["CategorieRWA"]; // Worksheet: CategorieRWA
                
                if (catDt?.Rows.Count > 0)
                {
                    var entities = catDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => new HecateCategorieRwa
                        {
                            IdCatRwa = row.Field<string>("Code RWA") ?? string.Empty,
                            Libelle = row.Field<string>("Libelle Categorie RWA") ?? string.Empty,
                            ValeurMobiliere = row.Field<string>("Valeur Mobiliere") ?? string.Empty
                        })
                        .Where(e => !string.IsNullOrEmpty(e.IdCatRwa))
                        .ToList();

                    await _context.HecateCategorieRwas.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    _logger.LogInformation($"⚡ HecateCategorieRwa (from EquivalenceCatRWA): {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 HecateCategorieRwa (from EquivalenceCatRWA) failed: {ex.Message}");
            }
        }

        private async Task SeedHecateTypeBloombergFromEquivalenceFileAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Param", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var typeDt = tables["TypeBloomberg"]; // Worksheet: TypeBloomberg
                
                if (typeDt?.Rows.Count > 0)
                {
                    var entities = typeDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => new HecateTypeBloomberg
                        {
                            IdTypeBloomberg = row.Field<string>("Code Bloomberg") ?? string.Empty,
                            Libelle = row.Field<string>("Libelle Bloomberg") ?? string.Empty
                        })
                        .Where(e => !string.IsNullOrEmpty(e.IdTypeBloomberg))
                        .ToList();

                    await _context.HecateTypeBloombergs.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    _logger.LogInformation($"⚡ HecateTypeBloomberg (from EquivalenceCatRWA): {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 HecateTypeBloomberg (from EquivalenceCatRWA) failed: {ex.Message}");
            }
        }

        private async Task SeedHecateCatDepositaire1FromEquivalenceFileAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Param", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var eqDt = tables["EquivalenceCatRWA"]; // Worksheet: EquivalenceCatRWA, column: Code catégorie 1
                
                if (eqDt?.Rows.Count > 0)
                {
                    var entities = eqDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => row.Field<string>("Code catégorie 1"))
                        .Where(code => !string.IsNullOrEmpty(code))
                        .Distinct()
                        .Select((code, index) => new HecateCatDepositaire1
                        {
                            IdDepositaire1 = index + 1, // Auto-increment ID
                            LibelleDepositaire1 = code ?? string.Empty
                        })
                        .ToList();

                    await _context.HecateCatDepositaire1s.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    _logger.LogInformation($"⚡ HecateCatDepositaire1 (from EquivalenceCatRWA): {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 HecateCatDepositaire1 (from EquivalenceCatRWA) failed: {ex.Message}");
            }
        }

        private async Task SeedHecateCatDepositaire2FromEquivalenceFileAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Param", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var eqDt = tables["EquivalenceCatRWA"]; // Worksheet: EquivalenceCatRWA, column: Code catégorie 2
                
                if (eqDt?.Rows.Count > 0)
                {
                    var entities = eqDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => row.Field<string>("Code catégorie 2"))
                        .Where(code => !string.IsNullOrEmpty(code))
                        .Distinct()
                        .Select((code, index) => new HecateCatDepositaire2
                        {
                            IdDepositaire2 = index + 1, // Auto-increment ID
                            LibelleDepositaire2 = code ?? string.Empty
                        })
                        .ToList();

                    await _context.HecateCatDepositaire2s.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    _logger.LogInformation($"⚡ HecateCatDepositaire2 (from EquivalenceCatRWA): {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 HecateCatDepositaire2 (from EquivalenceCatRWA) failed: {ex.Message}");
            }
        }

        private async Task SeedHecateEquivalenceCatRwaAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Param", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                // OPTIMIZATION: Pre-build fast lookup dictionaries for FK validation
                var catRwaLookup = await _context.HecateCategorieRwas.ToDictionaryAsync(c => c.IdCatRwa, c => c);
                var catDepo1Lookup = await _context.HecateCatDepositaire1s.ToDictionaryAsync(c => c.LibelleDepositaire1, c => c);
                var catDepo2Lookup = await _context.HecateCatDepositaire2s.ToDictionaryAsync(c => c.LibelleDepositaire2, c => c);
                var typeBloombergLookup = await _context.HecateTypeBloombergs.ToDictionaryAsync(c => c.IdTypeBloomberg, c => c);

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var eqDt = tables["EquivalenceCatRWA"]; // Worksheet: EquivalenceCatRWA
                
                if (eqDt?.Rows.Count > 0)
                {
                    var entities = eqDt.AsEnumerable()
                        .AsParallel()
                        .Where(row => 
                        {
                            var source = row.Field<string>("Source") ?? string.Empty;
                            var catRwa = row.Field<string>("Catégorie RWA") ?? string.Empty;
                            var codeCat1 = row.Field<string>("Code catégorie 1") ?? string.Empty;
                            var codeCat2 = row.Field<string>("Code catégorie 2") ?? string.Empty;
                            var typeBloomberg = row.Field<string>("Type Bloomberg") ?? string.Empty;
                            
                            return !string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(catRwa) &&
                                   catRwaLookup.ContainsKey(catRwa) && 
                                   catDepo1Lookup.ContainsKey(codeCat1) && 
                                   catDepo2Lookup.ContainsKey(codeCat2) &&
                                   typeBloombergLookup.ContainsKey(typeBloomberg);
                        })
                        .Select(row => 
                        {
                            var source = row.Field<string>("Source") ?? string.Empty;
                            var catRwa = row.Field<string>("Catégorie RWA") ?? string.Empty;
                            var codeCat1 = row.Field<string>("Code catégorie 1") ?? string.Empty;
                            var codeCat2 = row.Field<string>("Code catégorie 2") ?? string.Empty;
                            var typeBloomberg = row.Field<string>("Type Bloomberg") ?? string.Empty;
                            
                            return new HecateEquivalenceCatRwa
                            {
                                Source = source,
                                RefCategorieRwa = catRwa,
                                RefCatDepositaire1 = catDepo1Lookup[codeCat1].IdDepositaire1,
                                RefCatDepositaire2 = catDepo2Lookup[codeCat2].IdDepositaire2,
                                RefTypeBloomberg = typeBloomberg,
                                // Navigation properties (EF will set these automatically)
                                RefCategorieRwaNavigation = catRwaLookup[catRwa],
                                RefCatDepositaire1Navigation = catDepo1Lookup[codeCat1],
                                RefCatDepositaire2Navigation = catDepo2Lookup[codeCat2],
                                RefTypeBloombergNavigation = typeBloombergLookup[typeBloomberg]
                            };
                        })
                        .ToList();

                    await _context.HecateEquivalenceCatRwas.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    _logger.LogInformation($"⚡ HecateEquivalenceCatRwa: {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 HecateEquivalenceCatRwa failed: {ex.Message}");
            }
        }

        private async Task SeedUsersAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var users = new[]
                {
                    new CommonUser { Userid = 1, Username = "admin", Login = "admin", Password = "admin123", Emailaddress = "admin@rwa.com", Isactive = true },
                    new CommonUser { Userid = 2, Username = "user1", Login = "user1", Password = "user123", Emailaddress = "user1@rwa.com", Isactive = true },
                    new CommonUser { Userid = 3, Username = "user2", Login = "user2", Password = "user123", Emailaddress = "user2@rwa.com", Isactive = true },
                    new CommonUser { Userid = 4, Username = "analyst", Login = "analyst", Password = "analyst123", Emailaddress = "analyst@rwa.com", Isactive = true },
                    new CommonUser { Userid = 5, Username = "manager", Login = "manager", Password = "manager123", Emailaddress = "manager@rwa.com", Isactive = true }
                };

                await _context.CommonUsers.AddRangeAsync(users);
                await _context.SaveChangesAsync();
                
                sw.Stop();
                _logger.LogInformation($"⚡ Users: {users.Length} records in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 Users failed: {ex.Message}");
            }
        }

        private static DateOnly? TryParseDateOnly(object? dateValue)
        {
            if (dateValue == null) return null;
            
            if (DateTime.TryParse(dateValue.ToString(), out DateTime result))
                return DateOnly.FromDateTime(result);
                
            return null;
        }

        public async Task<bool> IsSeededAsync()
        {
            return await _context.HecateCategorieRwas.AnyAsync();
        }
    }
}