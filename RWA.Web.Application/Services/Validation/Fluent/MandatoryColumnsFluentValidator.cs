using FluentValidation;
using RWA.Web.Application.Models;
using System.Text.Json;
using System.Linq;

namespace RWA.Web.Application.Services.Validation.Fluent
{
    [RWA.Web.Application.Services.Validation.SupportedWorkflowStep(nameof(WorkflowStepNamesMapping.UploadInventory))]
    public class MandatoryColumnsFluentValidator : AbstractValidator<WorkflowStep>
    {
        private static readonly string[] Required = new[] {
            "Asset ID","Asset Description","Market Value","Asset Type 1","Asset Type 2","Local Currency",
            "Obligation Rate","Maturity Date","Expiration Date","Counterparty","RAF"
        };

        public MandatoryColumnsFluentValidator()
        {
            RuleFor(x => x.DataPayload).Custom((payload, ctx) =>
            {
                if (string.IsNullOrWhiteSpace(payload))
                {
                    ctx.AddFailure("DataPayload", "Payload is empty");
                    return;
                }

                try
                {
                    using var doc = JsonDocument.Parse(payload);
                    if (doc.RootElement.GetArrayLength() == 0)
                    {
                        ctx.AddFailure("DataPayload", "Uploaded file contains no rows");
                        return;
                    }

                    var first = doc.RootElement[0];
                    if (first.ValueKind != JsonValueKind.Object) return;
                    var keys = first.EnumerateObject().Select(p => p.Name).ToHashSet();
                    var missing = Required.Where(r => !keys.Contains(r)).ToArray();
                    if (missing.Length > 0)
                    {
                        ctx.AddFailure("DataPayload", "Missing required columns: " + string.Join(",", missing));
                    }
                }
                catch
                {
                    ctx.AddFailure("DataPayload", "Payload is not valid JSON");
                }
            });
        }
    }
}
