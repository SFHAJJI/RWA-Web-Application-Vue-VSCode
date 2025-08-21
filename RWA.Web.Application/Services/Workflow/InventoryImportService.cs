using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RWA.Web.Application.Services.ExcelManagementService.Import;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Text.Json;

namespace RWA.Web.Application.Services.Workflow
{
    public class InventoryImportService : IInventoryImportService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IInventoryMapper _mapper;

        public InventoryImportService(IWebHostEnvironment env, IInventoryMapper mapper = null)
        {
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _mapper = mapper; // optional; if null, we'll fallback to a basic mapping in the controller
        }

        public async Task<InventoryImportResult> ImportAsync(string fileName, byte[] fileContents)
        {
            var parseResult = await ParseOnlyAsync(fileName, fileContents);
            if (!parseResult.Success)
            {
                return parseResult;
            }

            // Now do the actual import (database persistence)
            string parsedJson = null;
            int mappedCount = 0;
            if (_mapper != null && parseResult.ParsedTable != null)
            {
                var mapRes = await _mapper.MapAsync(parseResult.ParsedTable);
                parsedJson = mapRes.jsonRows;
                mappedCount = mapRes.mappedCount;
            }
            else
            {
                parsedJson = parseResult.ParsedRowsJson;
                mappedCount = parseResult.RowsParsed;
            }

            return new InventoryImportResult
            {
                Success = true,
                MissingColumns = parseResult.MissingColumns,
                RowsParsed = parseResult.RowsParsed,
                RowsSaved = mappedCount,
                SavedFilePath = parseResult.SavedFilePath,
                ParsedTable = parseResult.ParsedTable,
                ParsedRowsJson = parsedJson
            };
        }

        public async Task<InventoryImportResult> ParseOnlyAsync(string fileName, byte[] fileContents)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("fileName");
            if (fileContents == null || fileContents.Length == 0) throw new ArgumentException("fileContents");

            var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

            var safeName = Path.GetFileName(fileName);
            var savedPath = Path.Combine(uploadsDir, $"{Guid.NewGuid():N}_{safeName}");
            await File.WriteAllBytesAsync(savedPath, fileContents);

            var sheetNames = new List<string>();
            var tableCollection = ExcelDataContext.ReadFromExcel(savedPath, ref sheetNames);
            if (tableCollection == null || tableCollection.Count == 0)
            {
                return new InventoryImportResult { Success = false, MissingColumns = Array.Empty<string>(), RowsParsed = 0, RowsSaved = 0, SavedFilePath = savedPath };
            }

            var table = tableCollection[0];

            // If the uploaded file name contains metadata (pattern RWA_Report_{3digits}_{MMyyyy}),
            // inject helper columns into the DataTable so downstream mappers can persist them.
            try
            {
                // reuse previously computed safeName
                var m = System.Text.RegularExpressions.Regex.Match(safeName ?? string.Empty, "^RWA_Report_(\\d{3})_(\\d{2})(\\d{4})");
                if (m.Success)
                {
                    var sourceDigits = m.Groups[1].Value;
                    var mm = m.Groups[2].Value;
                    var yyyy = m.Groups[3].Value;
                    var mmYYYY = mm + yyyy;

                    // Ensure Source column exists
                    if (!table.Columns.Contains("Source")) table.Columns.Add("Source", typeof(string));
                    if (!table.Columns.Contains("DateFinContrat")) table.Columns.Add("DateFinContrat", typeof(string));

                    foreach (DataRow r in table.Rows)
                    {
                        r["Source"] = sourceDigits;
                        r["DateFinContrat"] = mmYYYY;
                    }
                }
            }
            catch
            {
                // ignore failures - this is best-effort metadata enrichment
            }

            // Parse-only mode: just create JSON without database persistence
            string parsedJson = "";
            var rows = new List<Dictionary<string, object>>();
            foreach (DataRow r in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn c in table.Columns)
                {
                    dict[c.ColumnName] = r[c] is DBNull ? null : r[c];
                }
                rows.Add(dict);
            }
            parsedJson = JsonSerializer.Serialize(rows);

            return new InventoryImportResult
            {
                Success = true,
                MissingColumns = Array.Empty<string>(),
                RowsParsed = table.Rows.Count,
                RowsSaved = 0, // No persistence in parse-only mode
                SavedFilePath = savedPath,
                ParsedTable = table,
                ParsedRowsJson = parsedJson
            };
        }
    }
}
