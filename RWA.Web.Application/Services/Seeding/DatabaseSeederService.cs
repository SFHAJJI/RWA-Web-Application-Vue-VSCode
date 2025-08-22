using Microsoft.EntityFrameworkCore;
using RWA.Web.Application.Models;
using System.Diagnostics;
using System.Data;
using System.Collections;
using Microsoft.Extensions.Options;
using RWA.Web.Application.Services.ExcelManagementService.Import;
using System.Text.RegularExpressions;

namespace RWA.Web.Application.Services.Seeding
{
    public class DatabaseSeederService
    {
        private readonly RwaContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ExcelColumnMappings _columnMappings;
        private readonly string _rwaDatoDir;

        public DatabaseSeederService(RwaContext context, IWebHostEnvironment environment, IOptions<ExcelColumnMappings> columnMappings)
        {
            _context = context;
            _environment = environment;
            _columnMappings = columnMappings.Value;
            _rwaDatoDir = Path.Combine(environment.ContentRootPath, "RWA Data", "Seeding");
        }

        public async Task SeedDatabaseAsync()
        {
            var totalStopwatch = Stopwatch.StartNew();
            Console.WriteLine("🚀 ULTRA-FAST PARALLEL DATATABLE SEEDING INITIATED");

            try
            {
                // Quick check if already seeded
                if (await _context.CommonUsers.AnyAsync())
                {
                    Console.WriteLine("⚡ Database already seeded - SKIPPING");
                    return;
                }

                // PHASE 1: 🔥 PARALLEL INDEPENDENT TABLE SEEDING (5 simultaneous from EquivalenceCatRWA.xlsx)
                var phase1Tasks = new Task[]
                {
                    SeedBddHistoriqueAsync(),
                    SeedHecateCategorieRwaFromEquivalenceFileAsync(),
                    SeedHecateTypeBloombergFromEquivalenceFileAsync(),
                    SeedHecateCatDepositaire1FromEquivalenceFileAsync(),
                    SeedHecateCatDepositaire2FromEquivalenceFileAsync(),
                    SeedTethysAsync()
                };

                Console.WriteLine("⚡ PHASE 1: Launching 5 parallel independent DataTable seeds from EquivalenceCatRWA.xlsx...");
                await Task.WhenAll(phase1Tasks);

                // PHASE 2: 🎯 DEPENDENT TABLE (awaits Phase 1 completion)
                Console.WriteLine("⚡ PHASE 2: Processing dependent table with FK lookups...");
                await SeedHecateEquivalenceCatRwaAsync();

                // PHASE 3: 👥 USER DATA
                await SeedUsersAsync();

                totalStopwatch.Stop();
                Console.WriteLine($"🏆 ULTRA-FAST SEEDING COMPLETED: {totalStopwatch.ElapsedMilliseconds}ms total");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 SEEDING FAILED: {ex.Message}");
                throw;
            }
        }

        private async Task SeedBddHistoriqueAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Config", "BDDHistorique.xlsx");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️  BDD Historique file not found: {filePath}");
                    return;
                }

                // ULTRA-FAST: Direct DataTableCollection read (no row iteration)
                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var bddDt = tables["BDD"];
                
                if (bddDt?.Rows.Count > 0)
                {
                    var columnMap = CreateColumnMap(bddDt, _columnMappings.BDDHistorique);
                    // PARALLEL: AsParallel() for row processing + bulk insert
                    var entities = bddDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => new HecateInterneHistorique
                        {
                            Source = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.Source)]) ?? string.Empty,
                            RefCategorieRwa = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.CategorieRWA)]) ?? string.Empty,
                            IdentifiantUniqueRetenu = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.IdentifiantUniqueRetenu)]) ?? string.Empty,
                            Raf = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.RAF)]) ?? string.Empty,
                            LibelleOrigine = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.LibelleOrigine)]) ?? string.Empty,
                            DateEcheance = TryParseDateOnly(row.Field<object>(columnMap[nameof(_columnMappings.BDDHistorique.DateEcheance)]) ?? null),
                            IdentifiantOrigine = row.Field<string>(columnMap[nameof(_columnMappings.BDDHistorique.IdentifiantOrigine)]) ?? string.Empty,
                            Bbgticker = string.Empty, // Not in worksheet columns provided
                            LibelleTypeDette = "SUBORDONNE", // Always set to 'SUBORDONNE' as requested
                            LastUpdate = DateTime.Now.ToString("dd-MM-yyyy_Hmmss")
                        })
                        .ToList();

                    // Single bulk insert - fastest DB operation
                    await _context.HecateInterneHistoriques.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    Console.WriteLine($"⚡ BDD Historique: {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 BDD Historique failed: {ex.Message}");
            }
        }

        private async Task SeedHecateCategorieRwaFromEquivalenceFileAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Config", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var catDt = tables["CategorieRWA"]; // Worksheet: CategorieRWA
                
                if (catDt?.Rows.Count > 0)
                {
                    var columnMap = CreateColumnMap(catDt, _columnMappings.EquivalenceCatRWA.CategorieRWA);
                    var entities = catDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => new HecateCategorieRwa
                        {
                            IdCatRwa = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.CategorieRWA.CodeRWA)]) ?? string.Empty,
                            Libelle = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.CategorieRWA.LibelleCategorieRWA)]) ?? string.Empty,
                            ValeurMobiliere = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.CategorieRWA.ValeurMobiliere)]) ?? string.Empty,
                        })
                        .Where(e => !string.IsNullOrEmpty(e.IdCatRwa))
                        .ToList();

                    await _context.HecateCategorieRwas.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    Console.WriteLine($"⚡ HecateCategorieRwa (from EquivalenceCatRWA): {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 HecateCategorieRwa (from EquivalenceCatRWA) failed: {ex.Message}");
            }
        }

        private async Task SeedHecateTypeBloombergFromEquivalenceFileAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Config", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var typeDt = tables["TypeBloomberg"]; // Worksheet: TypeBloomberg
                
                if (typeDt?.Rows.Count > 0)
                {
                    var columnMap = CreateColumnMap(typeDt, _columnMappings.EquivalenceCatRWA.TypeBloomberg);
                    var entities = typeDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => new HecateTypeBloomberg
                        {
                            IdTypeBloomberg = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.TypeBloomberg.CodeBloomberg)]) ?? string.Empty,
                            Libelle = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.TypeBloomberg.LibelleBloomberg)]) ?? string.Empty
                        })
                        .Where(e => !string.IsNullOrEmpty(e.IdTypeBloomberg))
                        .ToList();

                    await _context.HecateTypeBloombergs.AddRangeAsync(entities);
                    await _context.SaveChangesAsync();
                    
                    sw.Stop();
                    Console.WriteLine($"⚡ HecateTypeBloomberg (from EquivalenceCatRWA): {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 HecateTypeBloomberg (from EquivalenceCatRWA) failed: {ex.Message}");
            }
        }

        private async Task SeedHecateCatDepositaire1FromEquivalenceFileAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Config", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var eqDt = tables["EquivalenceCatRWA"]; // Worksheet: EquivalenceCatRWA, column: Code catégorie 1
                
                if (eqDt?.Rows.Count > 0)
                {
                    var columnMap = CreateColumnMap(eqDt, _columnMappings.EquivalenceCatRWA.EquivalenceCatRWA);
                    var entities = eqDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.CodeDepositaire1)]))
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
                    Console.WriteLine($"⚡ HecateCatDepositaire1 (from EquivalenceCatRWA): {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 HecateCatDepositaire1 (from EquivalenceCatRWA) failed: {ex.Message}");
            }
        }

        private async Task SeedHecateCatDepositaire2FromEquivalenceFileAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Config", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️  EquivalenceCatRWA file not found: {filePath}");
                    return;
                }

                var sheetNames = new List<string>();
                DataTableCollection tables = ExcelDataContext.ReadFromExcel(filePath, ref sheetNames);
                var eqDt = tables["EquivalenceCatRWA"]; // Worksheet: EquivalenceCatRWA, column: Code catégorie 2
                
                if (eqDt?.Rows.Count > 0)
                {
                    var columnMap = CreateColumnMap(eqDt, _columnMappings.EquivalenceCatRWA.EquivalenceCatRWA);
                    var entities = eqDt.AsEnumerable()
                        .AsParallel()
                        .Select(row => row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.CodeDepositaire2)]))
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
                    Console.WriteLine($"⚡ HecateCatDepositaire2 (from EquivalenceCatRWA): {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 HecateCatDepositaire2 (from EquivalenceCatRWA) failed: {ex.Message}");
            }
        }

        private async Task SeedHecateEquivalenceCatRwaAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_rwaDatoDir, "Config", "EquivalenceCatRWA.xlsx");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️  EquivalenceCatRWA file not found: {filePath}");
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
                    var columnMap = CreateColumnMap(eqDt, _columnMappings.EquivalenceCatRWA.EquivalenceCatRWA);
                    var entities = eqDt.AsEnumerable()
                        .AsParallel()
                        .Where(row =>
                        {
                            var source = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.Source)]) ?? string.Empty;
                            var catRwa = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.CategorieRWA)]) ?? string.Empty;
                            var codeCat1 = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.CodeDepositaire1)]) ?? string.Empty;
                            var codeCat2 = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.CodeDepositaire2)]) ?? string.Empty;
                            var typeBloomberg = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.TypeBloomberg)]) ?? string.Empty;

                            return !string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(catRwa) &&
                                   catRwaLookup.ContainsKey(catRwa) &&
                                   catDepo1Lookup.ContainsKey(codeCat1) &&
                                   catDepo2Lookup.ContainsKey(codeCat2) &&
                                   typeBloombergLookup.ContainsKey(typeBloomberg);
                        })
                        .Select(row =>
                        {
                            var source = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.Source)]) ?? string.Empty;
                            var catRwa = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.CategorieRWA)]) ?? string.Empty;
                            var codeCat1 = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.CodeDepositaire1)]) ?? string.Empty;
                            var codeCat2 = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.CodeDepositaire2)]) ?? string.Empty;
                            var typeBloomberg = row.Field<string>(columnMap[nameof(_columnMappings.EquivalenceCatRWA.EquivalenceCatRWA.TypeBloomberg)]) ?? string.Empty;

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
                    Console.WriteLine($"⚡ HecateEquivalenceCatRwa: {entities.Count} records in {sw.ElapsedMilliseconds}ms");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 HecateEquivalenceCatRwa failed: {ex.Message}");
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
                Console.WriteLine($"⚡ Users: {users.Length} records in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Users failed: {ex.Message}");
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

        private Dictionary<string, string> CreateColumnMap<T>(DataTable dt, T mapping)
        {
            var map = new Dictionary<string, string>();
            var properties = typeof(T).GetProperties();
            var dtColumns = dt.Columns.Cast<DataColumn>().ToList();

            foreach (var prop in properties)
            {
                var expectedName = (string)prop.GetValue(mapping)!;
                var normalizedExpectedName = NormalizeString(expectedName);
                
                var matchedColumn = dtColumns.FirstOrDefault(c => NormalizeString(c.ColumnName) == normalizedExpectedName);

                if (matchedColumn != null)
                {
                    map[prop.Name] = matchedColumn.ColumnName;
                }
                else
                {
                    Console.WriteLine($"⚠️  Column mapping not found for '{expectedName}'");
                }
            }
            return map;
        }

        private string NormalizeString(string input)
        {
            // Remove all non-alphanumeric characters and convert to lower case
            return Regex.Replace(input, "[^a-zA-Z0-9]", "").ToLower();
        }

        private async Task SeedTethysAsync()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "RWA Data", "Seeding", "Tethys", "Extract_BOA_14112024.txt");
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"⚠️  Tethys file not found: {filePath}");
                    return;
                }

                var batch = new List<HecateTethy>();
                var batchSize = 10000; // Process in batches of 10,000
                var processedKeys = new HashSet<(string, string, string)>(); // Track composite keys

                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var fields = line.Split(';');
                        if (fields.Length >= 19)
                        {
                            var identifiantRaf = fields[0];
                            var codeIsin = fields[9];
                            var codeCusip = fields[12];

                            if (string.IsNullOrEmpty(identifiantRaf) || string.IsNullOrEmpty(codeIsin) || string.IsNullOrEmpty(codeCusip))
                            {
                                continue; // Skip if any part of the composite key is null/empty
                            }

                            var compositeKey = (identifiantRaf, codeIsin, codeCusip);
                            if (!processedKeys.Add(compositeKey))
                            {
                                Console.WriteLine($"[DUPLICATE DETECTED] Found duplicate Tethys key: (IdentifiantRaf: {identifiantRaf}, CodeIsin: {codeIsin}, CodeCusip: {codeCusip})");
                                continue; // Skip already processed key
                            }

                            batch.Add(new HecateTethy
                            {
                                IdentifiantRaf = identifiantRaf,
                                LibelleCourt = fields[1],
                                RaisonSociale = fields[2],
                                PaysDeResidence = fields[3],
                                PaysDeNationalite = fields[4],
                                NumeroEtNomDeRue = fields[5],
                                Ville = fields[6],
                                CategorieTethys = fields[7],
                                NafNace = fields[8],
                                CodeIsin = codeIsin,
                                SegmentDeRisque = fields[10],
                                SegmentationBpce = fields[11],
                                CodeCusip = codeCusip,
                                RafTeteGroupeReglementaire = fields[13],
                                NomTeteGroupeReglementaire = fields[14],
                                DateNotationInterne = fields[15],
                                CodeNotation = fields[16],
                                CodeConso = fields[17],
                                CodeApparentement = fields[18]
                            });

                            if (batch.Count >= batchSize)
                            {
                                await _context.HecateTethys.AddRangeAsync(batch);
                                await _context.SaveChangesAsync();
                                batch.Clear();
                            }
                        }
                    }
                }

                // Save any remaining records
                if (batch.Any())
                {
                    await _context.HecateTethys.AddRangeAsync(batch);
                    await _context.SaveChangesAsync();
                }

                sw.Stop();
                Console.WriteLine($"⚡ Tethys: {_context.HecateTethys.Count()} records in {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Tethys failed: {ex.Message}");
            }
        }
    }
}
