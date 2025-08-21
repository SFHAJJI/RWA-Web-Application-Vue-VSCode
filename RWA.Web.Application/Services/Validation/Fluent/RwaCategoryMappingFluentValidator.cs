using FluentValidation;
using RWA.Web.Application.Models;
using System.Text.Json;

namespace RWA.Web.Application.Services.Validation.Fluent
{
    [RWA.Web.Application.Services.Validation.SupportedWorkflowStep("RWA Category Manager")]
    public class RwaCategoryMappingFluentValidator : AbstractValidator<WorkflowStep>
    {
        public RwaCategoryMappingFluentValidator()
        {
            RuleFor(x => x.DataPayload).Custom((payload, ctx) =>
            {
                if (string.IsNullOrWhiteSpace(payload)) return;
                try
                {
                    using var doc = JsonDocument.Parse(payload);
                    // If payload contains rows, check whether rows without RefCategorieRwa exist
                    if (doc.RootElement.ValueKind != JsonValueKind.Array) return;
                    foreach (var el in doc.RootElement.EnumerateArray())
                    {
                        if (el.ValueKind != JsonValueKind.Object) continue;
                        if (!el.TryGetProperty("RefCategorieRwa", out var cat) || string.IsNullOrWhiteSpace(cat.GetString()))
                        {
                            ctx.AddFailure("DataPayload", "Some rows are missing RWA mapping (RefCategorieRwa)");
                            break;
                        }
                    }
                }
                catch
                {
                    // ignore
                }
            });
        }
    }
}
