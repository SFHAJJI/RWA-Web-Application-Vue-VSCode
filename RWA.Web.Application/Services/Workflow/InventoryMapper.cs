using System;
using System.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Workflow
{
    public class InventoryMapper : IInventoryMapper
    {
    private readonly IWorkflowDbProvider _dbProvider;
    private readonly ILogger<InventoryMapper> _logger;
        private static int _callCounter = 0;

        public InventoryMapper(IWorkflowDbProvider dbProvider, ILogger<InventoryMapper> logger)
        {
            _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("🏗️  INVENTORY MAPPER CREATED - Thread {ThreadId}", Environment.CurrentManagedThreadId);
        }

        public async Task<(string jsonRows, int mappedCount)> MapAsync(DataTable table)
        {
            var callId = Interlocked.Increment(ref _callCounter);
            var threadId = Environment.CurrentManagedThreadId;
            var timestamp = DateTime.UtcNow;
            
            _logger.LogInformation("📍 INVENTORY MAPPER START - Call #{CallId} on Thread {ThreadId} at {Timestamp}", 
                callId, threadId, timestamp);
            
            // Log stack trace to see who's calling us
            var stackTrace = Environment.StackTrace;
            _logger.LogDebug("📍 CALL STACK for Call #{CallId}:\n{StackTrace}", callId, stackTrace);
            
            if (table == null) 
            {
                _logger.LogWarning("⚠️  Call #{CallId}: DataTable is null, returning empty result", callId);
                return ("[]", 0);
            }

            _logger.LogInformation("📊 Call #{CallId}: Processing {RowCount} rows from DataTable", callId, table.Rows.Count);

            var rows = new List<HecateInventaireNormalise>();
            int i = 0;
            foreach (DataRow r in table.Rows)
            {
                i++;
                var ent = new HecateInventaireNormalise();
                // column-aware mapping: tolerate various column names coming from Excel/CSV
                string? Col(string[] candidates)
                {
                    foreach (var c in candidates)
                        if (r.Table.Columns.Contains(c)) return c;
                    return null;
                }

                var colIdent = Col(new[] { "Asset ID", "Identifiant", "Identifiant Origine", "IdentifiantOrigine" });
                var colNom = Col(new[] { "Asset Description", "Nom", "Asset Description" });
                var colVm = Col(new[] { "Market Value", "ValeurDeMarche", "MarketValue" });
                var colCat1 = Col(new[] { "Asset Type 1", "Categorie1", "Category1" });
                var colCat2 = Col(new[] { "Asset Type 2", "Categorie2", "Category2" });
                var colDev = Col(new[] { "Local Currency", "DeviseDeCotation", "Devise" });
                var colTaux = Col(new[] { "Obligation Rate", "TauxObligation" });
                var colMat = Col(new[] { "Maturity Date", "DateMaturite" });
                var colExp = Col(new[] { "Expiration Date", "DateExpiration" });
                var colTiers = Col(new[] { "Counterparty", "Tiers" });
                var colRaf = Col(new[] { "RAF", "RAF", "Raf" });
                var colSource = Col(new[] { "Source", "Source" });
                var colDateFinContrat = Col(new[] { "DateFinContrat" });
                var colBoaSj = Col(new[] { "BOA_SJ" });
                var colBoaCont = Col(new[] { "BOA_Contrepartie" });
                var colBoaDef = Col(new[] { "BOA_DEFAUT" });

                ent.IdentifiantOrigine = colIdent != null && !(r[colIdent] is DBNull) ? r[colIdent].ToString() ?? string.Empty : string.Empty;
                ent.Identifiant =  string.Empty;
                ent.Nom = colNom != null && !(r[colNom] is DBNull) ? r[colNom].ToString() ?? string.Empty : string.Empty;
                if (colVm != null && double.TryParse(r[colVm]?.ToString(), out var vm)) ent.ValeurDeMarche = vm;
                ent.Categorie1 = colCat1 != null ? Convert.ToString(r[colCat1])! : null;
                ent.Categorie2 = colCat2 != null ? Convert.ToString(r[colCat2])! : null;
                ent.DeviseDeCotation = colDev != null && !(r[colDev] is DBNull) ? r[colDev].ToString() ?? string.Empty : "EUR";
                if (colTaux != null && decimal.TryParse(r[colTaux]?.ToString(), out var taux)) ent.TauxObligation = taux;
                if (colMat != null && DateOnly.TryParse(r[colMat]?.ToString(), out var dm)) ent.DateMaturite = dm;
                if (colExp != null && DateOnly.TryParse(r[colExp]?.ToString(), out var de)) ent.DateExpiration = de;
                ent.Tiers = colTiers != null && !(r[colTiers] is DBNull) ? r[colTiers].ToString() : null;
                ent.Raf = colRaf != null && !(r[colRaf] is DBNull) ? r[colRaf].ToString() : null;
                ent.BoaSj = colBoaSj != null && !(r[colBoaSj] is DBNull) ? r[colBoaSj].ToString() : null;
                ent.BoaContrepartie = colBoaCont != null && !(r[colBoaCont] is DBNull) ? r[colBoaCont].ToString() : null;
                ent.BoaDefaut = colBoaDef != null && !(r[colBoaDef] is DBNull) ? r[colBoaDef].ToString() : null;
                // set defaults for non-nullable fields in HecateInventaireNormalise
                ent.PeriodeCloture = colDateFinContrat != null && !(r[colDateFinContrat] is DBNull) ? r[colDateFinContrat].ToString() : null;
                ent.Source = colSource != null && !(r[colSource] is DBNull) ? r[colSource].ToString() : null;
                ent.NumLigne = i;
                ent.Identifiant = ent.IdentifiantOrigine ?? string.Empty;
                ent.RefTypeDepot = 0;
                ent.RefTypeResultat = 0;
                rows.Add(ent);
            }

            // Persist to DbContext inside a fresh scope so DB work does not depend on the request scope's lifetime
            if (rows.Count > 0)
            {
                _logger.LogInformation("💾 Call #{CallId} BEFORE AddRangeAsync - Thread {ThreadId}, {RowCount} rows (using new scope)", 
                    callId, threadId, rows.Count);

                try
                {
                    _logger.LogDebug("🔄 Call #{CallId}: Persisting rows via IWorkflowDbProvider...", callId);
                    var savedCount = await _dbProvider.PersistInventoryRowsAsync(rows);
                    _logger.LogInformation("✅ Call #{CallId} PersistInventoryRowsAsync SUCCESS - Thread {ThreadId}, Saved {SavedCount} entities", 
                        callId, threadId, savedCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Call #{CallId} DATABASE ERROR - Thread {ThreadId}, Exception: {ExceptionType}", 
                        callId, threadId, ex.GetType().Name);
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("⚠️  Call #{CallId}: No rows to persist (rows.Count = 0)", callId);
            }

            var json = System.Text.Json.JsonSerializer.Serialize(rows);
            
            _logger.LogInformation("🏁 INVENTORY MAPPER END - Call #{CallId} on Thread {ThreadId}, Duration: {Duration}ms", 
                callId, threadId, (DateTime.UtcNow - timestamp).TotalMilliseconds);
            
            return (json, rows.Count);
        }
    }
}
