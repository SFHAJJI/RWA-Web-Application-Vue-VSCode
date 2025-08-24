using RWA.Web.Application.Models.Dtos;
using RWA.Web.Application.Services.Workflow.Dtos;
using RWA.Web.Application.Services.Validation;
using RWA.Web.Application.Services.Workflow;
using RWA.Web.Application.Hubs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using RWA.Web.Application.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace RWA.Web.Application.Services.Workflow
{
    public class WorkflowStateActions : IWorkflowStateActions
    {
        private readonly IInventoryImportService _importService;
        private readonly IValidatorsFactory _validatorsFactory;
        private readonly FluentValidationService _validationService;
        private readonly IWebHostEnvironment _env;
        private readonly IWorkflowDbProvider _dbProvider;
        private readonly IHubContext<WorkflowHub> _hubContext;
        private readonly WorkflowStatusMappingOptions _statusOptions;
        private UploadTriggerCallbackDelegate? _uploadTriggerCallback;
        private ValidationTriggerCallbackDelegate? _validationTriggerCallback;
        private ErrorTriggerCallbackDelegate? _errorTriggerCallback;
        private NextStepTriggerCallbackDelegate? _nextStepTriggerCallback;


        public WorkflowStateActions(
            IInventoryImportService importService,
            IValidatorsFactory validatorsFactory,
            FluentValidationService validationService,
            IWebHostEnvironment env,
            IWorkflowDbProvider dbProvider,
            IHubContext<WorkflowHub> hubContext,
            IOptions<WorkflowStatusMappingOptions> statusOptions)
        {
            _importService = importService ?? throw new ArgumentNullException(nameof(importService));
            _validatorsFactory = validatorsFactory ?? throw new ArgumentNullException(nameof(validatorsFactory));
            _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
            _env = env ?? throw new ArgumentNullException(nameof(env));
            _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _statusOptions = statusOptions?.Value ?? throw new ArgumentNullException(nameof(statusOptions));
        }

        public void SetTriggerCallbacks(
            UploadTriggerCallbackDelegate uploadCallback,
            ValidationTriggerCallbackDelegate validationCallback,
            ErrorTriggerCallbackDelegate errorCallback,
            NextStepTriggerCallbackDelegate nextStepCallback)
        {
            _uploadTriggerCallback = uploadCallback;
            _validationTriggerCallback = validationCallback;
            _errorTriggerCallback = errorCallback;
            _nextStepTriggerCallback = nextStepCallback;
        }

        /// <summary>
        /// Executes workflow actions safely with consistent error handling and logging
        /// </summary>
        private async Task ExecuteSafelyAsync(string actionName, string stepName, Func<Task> businessLogic, object? context = null)
        {
            try
            {
                Console.WriteLine($"[WorkflowStateActions] Executing {actionName} for step '{stepName}'");
                await businessLogic();
                Console.WriteLine($"[WorkflowStateActions] Successfully completed {actionName} for step '{stepName}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WorkflowStateActions] UNEXPECTED ERROR in {actionName} for step '{stepName}': {ex.Message}");
                Console.WriteLine($"[WorkflowStateActions] Stack trace: {ex.StackTrace}");

                // Trigger UnexpectedError to handle system failures
                if (_errorTriggerCallback != null)
                {
                    var errorContext = new UnexpectedErrorContext
                    {
                        ErrorMessage = $"System error in {actionName}: {ex.Message}",
                        Exception = ex,
                        StepName = stepName,
                        Details = context?.ToString()
                    };

                    await _errorTriggerCallback(Trigger.UnexpectedError, errorContext);
                }
                else
                {
                    Console.WriteLine($"[WorkflowStateActions] WARNING: No trigger callback available for UnexpectedError in {actionName}");
                }
            }
        }

        /// <summary>
        /// Updates workflow step status and saves changes to database
        /// </summary>
        private async Task UpdateStepStatusAsync(string stepName, string status)
        {
            try
            {
                var step = await _dbProvider.GetStepByNameAsync(stepName);
                if (step != null)
                {
                    step.Status = status;
                    step.UpdatedAt = DateTime.UtcNow;
                    await _dbProvider.SaveChangesAsync();
                    Console.WriteLine($"[WorkflowStateActions] Updated step '{stepName}' to status '{status}'");
                }
                else
                {
                    Console.WriteLine($"[WorkflowStateActions] WARNING: Step '{stepName}' not found for status update");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WorkflowStateActions] ERROR updating step status: {ex.Message}");
            }
        }

        /// <summary>
        /// Sends toast notification to UI
        /// </summary>
        private async Task SendToastNotificationAsync(string level, string message, string? actionLabel = null, string? actionToken = null)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveToast", new { level, message, actionLabel, actionToken });
                Console.WriteLine($"[WorkflowStateActions] Sent toast notification: {level} - {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WorkflowStateActions] ERROR sending toast notification: {ex.Message}");
            }
        }

        /// <summary>
        /// Notifies UI about workflow steps being updated (clean approach - just send steps)
        /// </summary>
        private async Task NotifyWorkflowStepsUpdatedAsync()
        {
            try
            {
                // Get fresh data from database - no caching issues since cache was removed
                var steps = await _dbProvider.GetStepsSnapshotAsync();
                await _hubContext.Clients.All.SendAsync("ReceiveWorkflowUpdate", steps);
                Console.WriteLine($"[WorkflowStateActions] Notified UI about workflow steps update");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WorkflowStateActions] ERROR notifying workflow steps update: {ex.Message}");
            }
        }

        /// <summary>
        /// Stores validation messages in the workflow step's DataPayload
        /// </summary>
        private async Task StoreValidationMessagesAsync(string stepName, List<ValidationMessage> validationMessages)
        {
            try
            {
                // Convert ValidationMessage to ValidationMessageDto
                var validationDtos = validationMessages.Select(vm => new Models.Dtos.ValidationMessageDto
                {
                    Status = vm.Status.ToString(),
                    Message = vm.Message,
                    ErrorData = vm.ErrorData,
                    ValidatorName = vm.ValidatorName,
                    FileName = ExtractFileNameFromErrorData(vm.ErrorData)
                }).ToList();

                // Serialize to JSON
                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(validationDtos);

                // Update the workflow step with the validation messages
                var step = await _dbProvider.GetStepByNameAsync(stepName);
                if (step != null)
                {
                    step.DataPayload = jsonPayload;
                    step.UpdatedAt = DateTime.UtcNow;
                    await _dbProvider.SaveChangesAsync();
                    Console.WriteLine($"[WorkflowStateActions] Stored {validationDtos.Count} validation messages for step '{stepName}'");
                }
                else
                {
                    Console.WriteLine($"[WorkflowStateActions] WARNING: Step '{stepName}' not found for validation messages storage");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WorkflowStateActions] ERROR storing validation messages for step '{stepName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Extracts filename from ErrorData object
        /// </summary>
        private string? ExtractFileNameFromErrorData(object? errorData)
        {
            if (errorData == null) return null;

            try
            {
                // Try to get filename from common properties
                var errorDataType = errorData.GetType();
                var fileNameProp = errorDataType.GetProperty("FileName") ?? errorDataType.GetProperty("SavedFilePath");

                if (fileNameProp != null)
                {
                    var value = fileNameProp.GetValue(errorData)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        return Path.GetFileName(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WorkflowStateActions] Warning: Failed to extract filename from ErrorData: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Clears validation messages from the workflow step's DataPayload
        /// </summary>
        private async Task ClearValidationMessagesAsync(string stepName)
        {
            try
            {
                var step = await _dbProvider.GetStepByNameAsync(stepName);
                if (step != null)
                {
                    step.DataPayload = string.Empty;
                    step.UpdatedAt = DateTime.UtcNow;
                    await _dbProvider.SaveChangesAsync();
                    Console.WriteLine($"[WorkflowStateActions] Cleared validation messages for step '{stepName}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WorkflowStateActions] ERROR clearing validation messages for step '{stepName}': {ex.Message}");
            }
        }






        // State entry actions
        
        public async Task OnRWACategoryManagerEntryAsync()
        {
            await ExecuteSafelyAsync(nameof(OnRWACategoryManagerEntryAsync), "RWACategoryManager", async () =>
            {
                Console.WriteLine($"[OnRWACategoryManagerEntryAsync] Entering RWA Category Manager step");

                // Process category mapping for all uploaded inventory rows
                await ProcessCategoryMappingAsync();
            });
        }

        /// <summary>
        /// Process RWA Category mapping on RWA step entry:
        /// 1. Query all uploaded HecateInventaireNormalise rows
        /// 2. Try to map each row using HecateEquivalenceCatRwa table
        /// 3. Save successful mappings to RefCategorieRwa field
        /// 4. Collect failed mappings for UI display
        /// 5. If no failed mappings, transition to next step; otherwise show UI
        /// </summary>
        private async Task ProcessCategoryMappingAsync()
        {
            try
            {
                Console.WriteLine($"[ProcessCategoryMappingAsync] Starting RWA category mapping process");

                // Use the DbProvider method to process mappings and get results
                var result = await _dbProvider.ProcessRwaCategoryMappingAsync();

                await HandleCategoryMappingResultAsync(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProcessCategoryMappingAsync] ERROR: {ex.Message}");
                await _errorTriggerCallback!(Trigger.UnexpectedError, new UnexpectedErrorContext 
                { 
                    ErrorMessage = $"Failed to process category mapping: {ex.Message}"
                });
            }
        }

        private async Task HandleCategoryMappingResultAsync(RWA.Web.Application.Models.Dtos.RwaCategoryManagerPayloadDto result)
        {
            if (result.MissingMappingRows.Count == 0)
            {
                // All mappings complete - mark step as successfully finished and transition to next step
                Console.WriteLine($"[HandleCategoryMappingResultAsync] All mappings complete, setting step to SuccessStatus and transitioning to next step");
                await _dbProvider.UpdateStepStatusAndDataAsync("RWA Category Manager", _statusOptions.SuccessStatus, string.Empty);
                
                // Notify UI of the successful completion
                await SendToastNotificationAsync("success", "All RWA category mappings applied successfully.");

                // Transition to next step
                if (_nextStepTriggerCallback != null)
                {
                    await _nextStepTriggerCallback(Trigger.GoToBDDManager);
                }
            }
            else
            {
                // Still have missing mappings - update UI with remaining rows
                Console.WriteLine($"[HandleCategoryMappingResultAsync] {result.MissingMappingRows.Count} mappings still missing, updating UI");
                
                var payloadJson = JsonSerializer.Serialize(result);
                
                await _dbProvider.UpdateStepStatusAndDataAsync("RWA Category Manager", _statusOptions.ErrorStatus, payloadJson);
                
                // Notify UI via SignalR
                await SendToastNotificationAsync("error", $"{result.MissingMappingRows.Count} RWA category mappings still require attention.");
                await NotifyWorkflowStepsUpdatedAsync();
            }
        }

        private async Task ProcessBddMatchingAsync()
        {
            var previousStep = await _dbProvider.GetStepByNameAsync("RWA Category Manager");
            var enrichedItems = JsonSerializer.Deserialize<List<EnrichedInventaireDto>>(previousStep.DataPayload);
            var allItems = await _dbProvider.GetAllInventaireNormaliseAsync();
            var allItemsDict = allItems.ToDictionary(i => i.NumLigne, i => i);

            var successfulMatches = new List<HecateInventaireNormalise>();
            var failedMatches = new List<HecateInventaireNormalise>();

            foreach (var enrichedItem in enrichedItems)
            {
                var item = allItemsDict[enrichedItem.NumLigne];
                if (enrichedItem.IsValeurMobiliere)
                {
                    item.IdentifiantUniqueRetenu = item.Identifiant;
                }
                else
                {
                    item.IdentifiantUniqueRetenu = $"{item.Source}{item.RefCategorieRwa}{item.PeriodeCloture.Substring(0, 2)}{item.PeriodeCloture.Substring(4, 2)}";
                }

                var match = await _dbProvider.FindMatchInHistoriqueAsync(h => h.IdentifiantOrigine == item.Identifiant);
                if (match == null)
                {
                    match = await _dbProvider.FindMatchInHistoriqueAsync(h => h.IdentifiantUniqueRetenu == item.IdentifiantUniqueRetenu || (item.IdentifiantUniqueRetenu != null && h.IdentifiantUniqueRetenu.Substring(2) == item.IdentifiantUniqueRetenu));
                }
                if (match == null)
                {
                    match = await _dbProvider.FindMatchInHistoriqueAsync(h => h.LibelleOrigine == item.Nom);
                }

                if (match != null)
                {
                    item.Rafenrichi = match.Raf;
                    item.DateFinContrat = match.DateEcheance;
                    successfulMatches.Add(item);
                }
                else
                {
                    failedMatches.Add(item);
                }
            }

            var payload = new
            {
                SuccessfulMatches = successfulMatches,
                FailedMatches = failedMatches
            };

            var payloadJson = JsonSerializer.Serialize(payload);
            await _dbProvider.UpdateStepStatusAndDataAsync("BDD Manager", _statusOptions.CurrentStatus, payloadJson);
            await NotifyWorkflowStepsUpdatedAsync();
        }

        public async Task OnBDDManagerEntryAsync()
        {
            await ExecuteSafelyAsync(nameof(OnBDDManagerEntryAsync), "BDDManager", async () =>
            {
                Console.WriteLine($"[OnBDDManagerEntryAsync] Entering BDD Manager step");

                // Process BDD matching for all inventory rows
                await ProcessBddMatchingAsync();
            });
        }
        // State exit actions


        public async Task OnRWACategoryManagerExitAsync(string stepName)
        {
            await ExecuteSafelyAsync(nameof(OnRWACategoryManagerExitAsync), stepName, async () =>
            {
                var allItems = await _dbProvider.GetAllInventaireNormaliseAsync();
                var categories = await _dbProvider.GetCategorieRwaOptionsAsync();
                var categoriesDict = categories.ToDictionary(c => c.IdCatRwa, c => c);

                var enrichedItems = allItems.Select(item => new EnrichedInventaireDto
                {
                    NumLigne = item.NumLigne,
                    IsValeurMobiliere = categoriesDict.ContainsKey(item.RefCategorieRwa) && (categoriesDict[item.RefCategorieRwa]?.ValeurMobiliere?.ToUpper() == "Y" || categoriesDict[item.RefCategorieRwa]?.ValeurMobiliere?.ToUpper() == "O"),
                    Libelle = categoriesDict.ContainsKey(item.RefCategorieRwa) ? categoriesDict[item.RefCategorieRwa].Libelle : string.Empty
                }).ToList();

                var payloadJson = JsonSerializer.Serialize(enrichedItems);
                await _dbProvider.UpdateStepStatusAndDataAsync(stepName, _statusOptions.SuccessStatus, payloadJson);
            });
        }

        public async Task OnBDDManagerExitAsync(string stepName)
        {
            await Task.CompletedTask;
        }

        public async Task OnRafManagerExitAsync(string stepName)
        {
            await Task.CompletedTask;
        }

        public async Task OnEnrichiExportExitAsync(string stepName)
        {
            await Task.CompletedTask;
        }

        // Internal transition actions with parameters

        /// <summary>
        /// A helper class to hold the result of processing a single file.
        /// </summary>
        private class FileProcessingResult
        {
            public string FileName { get; set; }
            public InventoryImportResult ParseResult { get; set; }
            public List<ValidationMessage> ValidationMessages { get; set; }
        }

        /// <summary>
        /// Processes and validates a single uploaded file.
        /// </summary>
        private async Task<FileProcessingResult> ProcessAndValidateFileAsync((string FileName, byte[] Content) file, IEnumerable<IValidator<WorkflowStep>> validators)
        {
            Console.WriteLine($"[ProcessAndValidateFileAsync] Processing file: {file.FileName}");

            // 1. Parse the file
            var parseResult = await _importService.ParseOnlyAsync(file.FileName, file.Content);
            parseResult.FileName = file.FileName; // Ensure filename is set in result
            Console.WriteLine($"[ProcessAndValidateFileAsync] Parse successful for {file.FileName}: {parseResult.RowsParsed} rows parsed");

            // 2. Create WorkflowStep for validation
            var workflowStep = new Models.WorkflowStep
            {
                StepName = "UploadInventory",
                Status = _statusOptions.CurrentStatus,
                DataPayload = parseResult.ParsedRowsJson
            };

            // 3. Run validation
            var validationResult = await _validationService.RunValidatorsAsync(workflowStep, validators);

            // 4. Enrich validation messages with FileName
            foreach (var message in validationResult.Messages)
            {
                if (message.ErrorData == null)
                {
                    message.ErrorData = new { FileName = file.FileName, SavedFilePath = file.FileName };
                }
            }

            return new FileProcessingResult
            {
                FileName = file.FileName,
                ParseResult = parseResult,
                ValidationMessages = validationResult.Messages.ToList()
            };
        }

        /// <summary>
        /// Triggers the appropriate validation callback based on the aggregated validation result.
        /// </summary>
        private async Task TriggerValidationCallbackAsync(ValidationResult aggregateValidationResult, List<InventoryImportResult> parsedFiles)
        {
            Console.WriteLine($"[TriggerValidationCallbackAsync] Aggregate validation completed with status: {aggregateValidationResult.OverallStatus}");

            var context = new ValidationResultContext
            {
                StepName = "UploadInventory",
                ValidationResult = aggregateValidationResult,
                ParsedFiles = parsedFiles
            };

            if (_validationTriggerCallback != null)
            {
                var trigger = aggregateValidationResult.OverallStatus switch
                {
                    ValidationStatus.Success => Trigger.ValidationSuccess,
                    ValidationStatus.Warning => Trigger.ValidationWarning,
                    ValidationStatus.Error => Trigger.ValidationError,
                    _ => Trigger.ValidationError // Default case for unknown statuses
                };

                Console.WriteLine($"[TriggerValidationCallbackAsync] Triggering '{trigger}'");
                await _validationTriggerCallback(trigger, context);
            }
            else
            {
                Console.WriteLine($"[TriggerValidationCallbackAsync] WARNING: _validationTriggerCallback is null.");
            }
        }

        public async Task OnTriggerUploadAsync(List<(string FileName, byte[] Content)> files)
        {
            await ExecuteSafelyAsync(nameof(OnTriggerUploadAsync), "UploadInventory", async () =>
            {
                Console.WriteLine($"[OnTriggerUploadAsync] Processing upload: {files.Count} files - [{string.Join(", ", files.Select(f => $"{f.FileName} ({f.Content.Length} bytes)"))}]");

                var fileProcessingResults = new List<FileProcessingResult>();
                var validators = _validatorsFactory.GetValidatorsFor("Upload inventory");
                foreach (var file in files)
                {
                    var result = await ProcessAndValidateFileAsync(file, validators);
                    fileProcessingResults.Add(result);
                }

                var parsedFiles = fileProcessingResults.Select(r => r.ParseResult).ToList();

                // Aggregate validation results from all files
                var allValidationMessages = fileProcessingResults.SelectMany(r => r.ValidationMessages).ToList();
                var aggregateValidationResult = new ValidationResult(allValidationMessages);

                // Trigger the appropriate response based on the aggregated result
                await TriggerValidationCallbackAsync(aggregateValidationResult, parsedFiles);

            }, new { fileCount = files.Count, fileNames = string.Join(", ", files.Select(f => f.FileName)) });
        }

        public async Task OnUploadPendingAsync(AggregateUploadResultContext aggregateContext)
        {
            await ExecuteSafelyAsync(nameof(OnUploadPendingAsync), "UploadInventory", async () =>
            {
                var importTasks = aggregateContext.Results.Select(async context =>
                {
                    var importResult = await _importService.ImportAsync(context.ImportResult);
                    context.ImportResult = importResult;
                    if (!importResult.Success)
                    {
                        context.ErrorMessage = "Database import failed";
                    }
                    return context;
                }).ToList();

                await Task.WhenAll(importTasks);

                if (aggregateContext.IsSuccess)
                {
                    if (_uploadTriggerCallback != null)
                    {
                        await _uploadTriggerCallback(Trigger.UploadSuccess, aggregateContext);
                    }
                }
                else
                {
                    if (_uploadTriggerCallback != null)
                    {
                        await _uploadTriggerCallback(Trigger.UploadFailed, aggregateContext);
                    }
                }
            }, aggregateContext);
        }

        public async Task OnUploadSuccessAsync(AggregateUploadResultContext context)
        {
            await ExecuteSafelyAsync(nameof(OnUploadSuccessAsync), "UploadInventory", async () =>
            {
                Console.WriteLine($"[OnUploadSuccessAsync] Upload completed successfully for all files.");

                // ATOMIC UPDATE: Update current step to SuccessStatus
                await _dbProvider.UpdateStepStatusAndDataAsync("Upload inventory", _statusOptions.SuccessStatus, string.Empty);
                Console.WriteLine($"[OnUploadSuccessAsync] Set step status to SuccessStatus using atomic update");

                // Success notification for all successful files
                var fileList = string.Join("\n", context.Results.Where(r => r.IsSuccess).Select(r => r.FileName));
                if (!string.IsNullOrEmpty(fileList))
                {
                    await SendToastNotificationAsync("success", $"Successfully uploaded files:\n{fileList}");
                }

                // Fire NextUploadToRWACatManager trigger to transition to next state
                if (_nextStepTriggerCallback != null)
                {
                    Console.WriteLine($"[OnUploadSuccessAsync] Firing NextUploadToRWACatManager trigger");
                    await _nextStepTriggerCallback(Trigger.NextUploadToRWACatManager);
                }
            }, context);
        }

        public async Task OnUploadFailedAsync(AggregateUploadResultContext context)
        {
            await ExecuteSafelyAsync(nameof(OnUploadFailedAsync), "UploadInventory", async () =>
            {
                // Update step status to ErrorStatus
                await UpdateStepStatusAsync("Upload inventory", _statusOptions.ErrorStatus);

                // Error notification for each failed file
                foreach (var result in context.Results.Where(r => !r.IsSuccess))
                {
                    var errorMessage = !string.IsNullOrEmpty(result.ErrorMessage)
                        ? $"Upload failed for {result.FileName}: {result.ErrorMessage}"
                        : $"Upload failed for {result.FileName}";
                    await SendToastNotificationAsync("error", errorMessage, "Retry", "retry_upload");
                }
            }, context);
        }

        public async Task OnApplyRwaMappingsAsync(List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto> mappings)
        {
            await ExecuteSafelyAsync(nameof(OnApplyRwaMappingsAsync), "RWACategoryManager", async () =>
            {
                Console.WriteLine($"[OnApplyRwaMappingsAsync] Applying {mappings.Count} RWA mappings");

                try
                {
                    // Use DbProvider to apply the mappings
                    var appliedCount = await _dbProvider.ApplyRwaMappingsAsync(mappings);
                    Console.WriteLine($"[OnApplyRwaMappingsAsync] Queued {appliedCount} mappings for background processing.");

                    // After applying mappings, re-process to check if any mappings are still missing
                    var result = await _dbProvider.ProcessRwaCategoryMappingAsync();
                    
                    await HandleCategoryMappingResultAsync(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[OnApplyRwaMappingsAsync] ERROR: {ex.Message}");
                    await _errorTriggerCallback!(Trigger.UnexpectedError, new UnexpectedErrorContext 
                    { 
                        ErrorMessage = $"Failed to apply RWA mappings: {ex.Message}"
                    });
                }
            });
        }

        public async Task OnApplyEquivalenceMappingsAsync(List<RWA.Web.Application.Models.Dtos.EquivalenceMappingDto> mappings)
        {
            await Task.CompletedTask;
        }

        public async Task OnValidationSuccessAsync(ValidationResultContext context)
        {
            await ExecuteSafelyAsync(nameof(OnValidationSuccessAsync), context.StepName, async () =>
            {
                Console.WriteLine($"[OnValidationSuccessAsync] Validation successful, triggering UploadPending");

                // ATOMIC CLEAR: Clear any previous validation messages atomically
                Console.WriteLine($"[OnValidationSuccessAsync] Clearing validation messages atomically");
                await _dbProvider.UpdateStepStatusAndDataAsync("Upload inventory", _statusOptions.CurrentStatus, string.Empty);
                Console.WriteLine($"[OnValidationSuccessAsync] Validation messages cleared successfully");

                // Trigger UploadPending for all files to start database import
                if (context.ParsedFiles != null && _uploadTriggerCallback != null)
                {
                    var aggregateContext = new AggregateUploadResultContext
                    {
                        Results = context.ParsedFiles.Select(file => new UploadResultContext
                        {
                            FileName = file.FileName,
                            ImportResult = file,
                            ValidationResult = context.ValidationResult
                        }).ToList()
                    };
                    await _uploadTriggerCallback(Trigger.UploadPending, aggregateContext);
                }
                else
                {
                    Console.WriteLine($"[OnValidationSuccessAsync] WARNING: ParsedFiles is null or _uploadTriggerCallback is null. Cannot proceed with upload.");
                }
            }, context);
        }

        public async Task OnValidationWarningAsync(ValidationResultContext context)
        {
            await ExecuteSafelyAsync(nameof(OnValidationWarningAsync), context.StepName, async () =>
            {
                Console.WriteLine($"[OnValidationWarningAsync] Validation has warnings, but proceeding to UploadPending");

                // Convert validation messages to DTOs and serialize
                var validationMessages = context.ValidationResult.Messages.ToList();
                var validationDtos = validationMessages.Select(vm => new Models.Dtos.ValidationMessageDto
                {
                    Status = vm.Status.ToString(),
                    Message = vm.Message,
                    ErrorData = vm.ErrorData,
                    ValidatorName = vm.ValidatorName,
                    FileName = ExtractFileNameFromErrorData(vm.ErrorData)
                }).ToList();

                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(validationDtos);

                // ATOMIC UPDATE: Use the new method that handles load-modify-save in one context
                Console.WriteLine($"[OnValidationWarningAsync] Updating step atomically - Status: CurrentWarningStatus, ValidationMessages: {validationDtos.Count}");
                await _dbProvider.UpdateStepStatusAndDataAsync("Upload inventory", _statusOptions.CurrentWarningStatus, jsonPayload);
                Console.WriteLine($"[OnValidationWarningAsync] Step updated successfully");

                // Notify UI with updated steps (clean approach)
                await NotifyWorkflowStepsUpdatedAsync();

                // Trigger UploadPending for each file to start database import
                if (context.ParsedFiles != null && _uploadTriggerCallback != null)
                {
                    var aggregateContext = new AggregateUploadResultContext
                    {
                        Results = context.ParsedFiles.Select(file => new UploadResultContext
                        {
                            FileName = file.FileName,
                            ImportResult = file,
                            ValidationResult = context.ValidationResult
                        }).ToList()
                    };
                    await _uploadTriggerCallback(Trigger.UploadPending, aggregateContext);
                }
                else
                {
                    Console.WriteLine($"[OnValidationWarningAsync] WARNING: ParsedFiles is null or _uploadTriggerCallback is null. Cannot proceed with upload.");
                }
            }, context);
        }

        public async Task OnValidationErrorAsync(ValidationResultContext context)
        {
            await ExecuteSafelyAsync(nameof(OnValidationErrorAsync), context.StepName, async () =>
            {
                Console.WriteLine($"[OnValidationErrorAsync] Validation failed with errors - NOT proceeding to database import");
                Console.WriteLine($"[OnValidationErrorAsync] Found {context.ValidationResult.Messages.Count} validation messages");

                // Convert validation messages to DTOs and serialize
                var validationMessages = context.ValidationResult.Messages.ToList();
                var validationDtos = validationMessages.Select(vm => new Models.Dtos.ValidationMessageDto
                {
                    Status = vm.Status.ToString(),
                    Message = vm.Message,
                    ErrorData = vm.ErrorData,
                    ValidatorName = vm.ValidatorName,
                    FileName = ExtractFileNameFromErrorData(vm.ErrorData)
                }).ToList();

                var jsonPayload = System.Text.Json.JsonSerializer.Serialize(validationDtos);

                // ATOMIC UPDATE: Use the new method that handles load-modify-save in one context
                Console.WriteLine($"[OnValidationErrorAsync] Updating step atomically - Status: ErrorStatus, ValidationMessages: {validationDtos.Count}");
                await _dbProvider.UpdateStepStatusAndDataAsync("Upload inventory", _statusOptions.ErrorStatus, jsonPayload);
                Console.WriteLine($"[OnValidationErrorAsync] Step updated successfully");

                // Notify UI with updated steps (clean approach)
                await NotifyWorkflowStepsUpdatedAsync();

                // DO NOT proceed to import when there are validation errors
                await Task.CompletedTask;
            }, context);
        }

        public async Task OnUnexpectedErrorAsync(UnexpectedErrorContext context)
        {
            await ExecuteSafelyAsync(nameof(OnUnexpectedErrorAsync), context.StepName, async () =>
            {
                Console.WriteLine($"[OnUnexpectedErrorAsync] System error occurred: {context.ErrorMessage}");

                // Update step status to ErrorStatus 
                await UpdateStepStatusAsync(context.StepName, _statusOptions.ErrorStatus);

                // Log system error details
                Console.WriteLine($"[OnUnexpectedErrorAsync] Exception: {context.Exception?.GetType().Name}: {context.Exception?.Message}");
                if (!string.IsNullOrEmpty(context.Details))
                {
                    Console.WriteLine($"[OnUnexpectedErrorAsync] Details: {context.Details}");
                }

                // Notify UI about system error
                await SendToastNotificationAsync("error", $"System error: {context.ErrorMessage}", "Retry", "retry_upload");

                await Task.CompletedTask;
            }, context);
        }

        public async Task OnForceNextFallbackAsync(ForceNextContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnTransitionedAsync(TransitionDto transitionDto)
        {
            if (!(transitionDto.Trigger == "LetsGo"))
            {
                await ExecuteSafelyAsync(nameof(OnTransitionedAsync), transitionDto.Destination, async () =>
                {
                    Console.WriteLine($"[OnTransitionedAsync] State transition: {transitionDto.Source} -> {transitionDto.Destination} via trigger {transitionDto.Trigger}");

                    // Generic step progression logic based on state transition
                    await HandleStepProgressionAsync(transitionDto);

                    // Always notify UI about workflow steps changes
                    await NotifyWorkflowStepsUpdatedAsync();
                }, transitionDto);
            }

        }

        /// <summary>
        /// Generic method to handle step progression based on state transitions
        /// </summary>
        private async Task HandleStepProgressionAsync(TransitionDto transitionDto)
        {
            try
            {
                // Map destination state to step name
                var nextStepName = GetStepNameFromState(transitionDto.Destination);

                if (!string.IsNullOrEmpty(nextStepName))
                {
                    Console.WriteLine($"[HandleStepProgressionAsync] Setting next step '{nextStepName}' to CurrentStatus");

                    // ATOMIC UPDATE: Use the atomic method to ensure database consistency
                    await _dbProvider.UpdateStepStatusAndDataAsync(nextStepName, _statusOptions.CurrentStatus, string.Empty);
                    Console.WriteLine($"[HandleStepProgressionAsync] Successfully set '{nextStepName}' to CurrentStatus using atomic update");
                }
                else
                {
                    Console.WriteLine($"[HandleStepProgressionAsync] No step mapping found for destination state '{transitionDto.Destination}'");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HandleStepProgressionAsync] ERROR handling step progression: {ex.Message}");
            }
        }

        /// <summary>
        /// Maps state machine states to workflow step names
        /// </summary>
        private string? GetStepNameFromState(string stateName)
        {
            return stateName switch
            {
                "UploadInventoryFiles" => "Upload inventory",
                "RWACategoryManager" => "RWA Category Manager",
                "BDDManager" => "BDD Manager",
                "RafManager" => "RAF Manager",
                "EnrichiExport" => "Fichier Enrichie Generation",
                _ => null
            };
        }

        // Helper methods for orchestrator delegation
        public async Task<IEnumerable<RWA.Web.Application.Models.Dtos.WorkflowStepDto>> GetWorkflowStepsSnapshotAsync()
        {
            // Ensure database is seeded with default workflow steps
            await _dbProvider.SeedDefaultWorkflowIfEmptyAsync();

            // Get workflow steps from database
            var steps = await _dbProvider.GetStepsSnapshotAsync();

            // Convert to DTOs
            return steps.Select(s => new RWA.Web.Application.Models.Dtos.WorkflowStepDto
            {
                Id = s.Id,
                StepName = s.StepName,
                Status = s.Status,
                DataPayload = s.DataPayload
            });
        }

        public Task<IEnumerable<object>> GetCategoriesForDropdownAsync()
        {
            return Task.FromResult(new List<object>().AsEnumerable());
        }

        public Task<List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>> GetEquivalenceCandidatesForMissingRowsAsync()
        {
            return Task.FromResult(new List<RWA.Web.Application.Models.Dtos.EquivalenceCandidateDto>());
        }

        public Task<List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>> GetMissingRowsWithSuggestionsAsync()
        {
            return Task.FromResult(new List<RWA.Web.Application.Models.Dtos.RwaMappingRowDto>());
        }

        public async Task<List<HecateInventaireNormalise>> GetInventaireNormaliseByNumLignes(List<int> numLignes)
        {
            return await _dbProvider.GetInventaireNormaliseByNumLignes(numLignes);
        }

        public async Task<bool> IsResetCompleteAsync()
        {
            try
            {
                Console.WriteLine("Starting workflow reset...");
                
                // 1. Clear HecateInventaireNormalise table
                await _dbProvider.ClearInventoryTableAsync();
                
                // 2. Initialize workflow steps and clear their payloads
                await _dbProvider.InitializeWorkflowStepsAsync();
                
                // 3. Get updated steps and send to UI via SignalR
                var steps = await _dbProvider.GetWorkflowStepsAsync();
                await _hubContext.Clients.All.SendAsync("WorkflowStepsUpdated", steps);

                await SendToastNotificationAsync("success", "Reset finished");
                
                Console.WriteLine("Workflow reset completed successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during workflow reset: {ex.Message}");
                return false; // This will prevent the state transition if reset fails
            }
        }
    }
}
