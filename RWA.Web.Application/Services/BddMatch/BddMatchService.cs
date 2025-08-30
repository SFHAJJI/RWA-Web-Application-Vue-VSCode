using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RWA.Web.Application.Models;
using RWA.Web.Application.Models.Dtos;
using Microsoft.AspNetCore.SignalR;
using RWA.Web.Application.Hubs;
using RWA.Web.Application.Services.Workflow;

namespace RWA.Web.Application.Services.BddMatch
{
    public class BddMatchService : IBddMatchService
    {
        private readonly IWorkflowDbProvider _db;
        private readonly IBddMatchStore _store;
        private readonly ILogger<BddMatchService> _logger;
        private readonly IHubContext<WorkflowHub> _hub;

        public BddMatchService(IWorkflowDbProvider db, IBddMatchStore store, ILogger<BddMatchService> logger, IHubContext<WorkflowHub> hub)
        {
            _db = db;
            _store = store;
            _logger = logger;
            _hub = hub;
        }

        public async Task ComputeAndPersistAsync(string version, CancellationToken ct = default)
        {
            var all = await _db.GetAllInventaireNormaliseAsync();
            var bdd = await _db.GetAllHecateInterneHistoriqueAsync();

            // Be robust to duplicates: pick the most recent by DateEcheance (then LastUpdate) per key
            var bddByIdU = bdd
                .Where(b => !string.IsNullOrWhiteSpace(b.IdentifiantUniqueRetenu))
                .GroupBy(b => b.IdentifiantUniqueRetenu!)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.DateEcheance)
                          .ThenByDescending(x => x.LastUpdate)
                          .First()
                );

            var bddByIdO = bdd
                .Where(b => !string.IsNullOrWhiteSpace(b.IdentifiantOrigine))
                .GroupBy(b => b.IdentifiantOrigine!)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.DateEcheance)
                          .ThenByDescending(x => x.LastUpdate)
                          .First()
                );

            // Build a VM map so we can run matching even if step 2 hasn’t persisted flags yet
            var catOptions = await _db.GetCategorieRwaOptionsAsync();
            var catMap = catOptions.ToDictionary(c => c.IdCatRwa, c => c.ValeurMobiliere?.Trim() ?? string.Empty, StringComparer.OrdinalIgnoreCase);

            bool IsVm(HecateInventaireNormalise i)
            {
                if (i.AdditionalInformation?.IsValeurMobiliere == true) return true;
                if (!string.IsNullOrWhiteSpace(i.RefCategorieRwa) && catMap.TryGetValue(i.RefCategorieRwa, out var vm))
                {
                    return string.Equals(vm, "Y", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(vm, "O", StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }

            var items = all.Where(IsVm).ToList();
            _store.Init(version, items.Count);

            // Emit initial progress so the UI shows the bar immediately
            await _hub.Clients.All.SendAsync("BddMatchProgress", new { version, processed = 0, total = items.Count }, ct);

            var processed = 0;
            var batchSize = 500;
            for (int i = 0; i < items.Count; i += batchSize)
            {
                var batch = items.Skip(i).Take(batchSize).ToList();
                var resultRows = new List<BddMatchRow>(batch.Count);

                foreach (var item in batch)
                {
                    // capture inventory fields before we possibly overwrite them
                    var invIdU = item.IdentifiantUniqueRetenu;
                    var invRaf = item.Raf;
                    var info = item.AdditionalInformation ?? new AdditionalInformation();
                    BddMatchRow row;
                    if (!string.IsNullOrEmpty(item.IdentifiantOrigine) && bddByIdU.TryGetValue(item.IdentifiantOrigine, out var matchU))
                    {
                        info.AddtoBDDDto = new AddtoBDDDto { AddToBDD = false, IsMappedByIdUniqueRetenu = true };
                        if (string.IsNullOrEmpty(item.Raf)) info.RafOrigin = "BDDHistory";
                        item.Raf = matchU.Raf; // inventory now uses BDD RAF
                        item.IdentifiantUniqueRetenu = matchU.IdentifiantUniqueRetenu; // align IdU
                        item.DateFinContrat = matchU.DateEcheance;
                        row = new BddMatchRow(item.NumLigne, false, matchU.Raf, "IdUniqueRetenu", invIdU, invRaf);
                    }
                    else if (!string.IsNullOrEmpty(item.IdentifiantOrigine) && bddByIdO.TryGetValue(item.IdentifiantOrigine, out var matchO))
                    {
                        info.AddtoBDDDto = new AddtoBDDDto { AddToBDD = false, IsMappedByIdOrigine = true };
                        if (string.IsNullOrEmpty(item.Raf)) info.RafOrigin = "BDDHistory";
                        item.Raf = matchO.Raf; // inventory now uses BDD RAF
                        item.IdentifiantUniqueRetenu = matchO.IdentifiantUniqueRetenu; // align IdU
                        item.DateFinContrat = matchO.DateEcheance;
                        row = new BddMatchRow(item.NumLigne, false, matchO.Raf, "IdOrigine", invIdU, invRaf);
                    }
                    else
                    {
                        var add = !string.IsNullOrEmpty(item.Raf);
                        info.AddtoBDDDto = new AddtoBDDDto { AddToBDD = add };
                        row = new BddMatchRow(item.NumLigne, add, null, add ? "AddToBDD" : "NoMatch", invIdU, invRaf);
                    }
                    item.AdditionalInformation = info;
                    resultRows.Add(row);
                }

                await _db.UpdateInventaireNormaliseRangeAsync(batch);
                processed += batch.Count;
                _store.Append(version, resultRows, processed);
                await _hub.Clients.All.SendAsync("BddMatchProgress", new { version, processed, total = items.Count }, ct);
                if (ct.IsCancellationRequested) break;
            }
            _logger.LogInformation("BDD matching complete for version {Version}. Processed {Processed} items.", version, processed);
            await _hub.Clients.All.SendAsync("BddMatchProgress", new { version, processed, total = items.Count }, ct);
        }
    }
}
