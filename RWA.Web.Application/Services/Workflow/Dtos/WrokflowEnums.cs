using System;
using Mono.TextTemplating;

namespace RWA.Web.Application.Services.Workflow.Dtos
{
    
        public enum Trigger
        {
            LetsGo, // Initial bootstrap trigger
            UploadInventoryFiles,
            UploadSuccess,
            UploadFailed,
            UploadPending,
            ValidationSuccess,
            ValidationWarning,
            ValidationError,
            UnexpectedError, // For system errors (DB issues, step not found, etc.)
            ForceNextFallback,
            ReValidate,
            Next,
            NextUploadToRWACatManager,
            NextRWACatManagerToBDDManager,
            NextBDDManagerToRafManager,
            NextRafManagerToEnrichiExport,
            Previous,
            GoToBDDManager,
            GoToUploadInventoryFiles,
            Reset,
            ApplyRwaMappings,
            ApplyEquivalenceMappings,
            ForceNext
        }

        public enum State
        {
            InitialState,
            UploadInventoryFiles,
            RWACategoryManager,
            BDDManager,
            RafManager,
            EnrichiExport
        }
}
