using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RWA.Web.Application.Models;
using FluentValidation;

namespace RWA.Web.Application.Services.Validation
{
    // Service dedicated to running FluentValidation validators and mapping results to the project's ValidationResult
    public class FluentValidationService
    {
        public async Task<ValidationResult> RunValidatorsAsync(WorkflowStep workflowStep, IEnumerable<IValidator<WorkflowStep>> fluentValidators)
        {
            var result = new ValidationResult();

            if (fluentValidators == null) return result;

            var validatorList = fluentValidators.ToList();

            var tasks = validatorList.Select(v => Task.Run(() =>
            {
                var ctx = new FluentValidation.ValidationContext<WorkflowStep>(workflowStep);
                var res = v.Validate(ctx);
                return (v, res);
            }));

            var results = await Task.WhenAll(tasks);

            foreach (var (validator, res) in results)
            {
                var name = validator?.GetType().Name ?? "FluentValidator";
                if (res != null && res.Errors != null)
                {
                    foreach (var f in res.Errors)
                    {
                        var msg = new ValidationMessage
                        {
                            Status = ValidationStatus.Error,
                            Message = f.ErrorMessage,
                            ValidatorName = name,
                            ErrorData = null
                        };
                        result.Messages.Add(msg);
                    }
                }
            }

            if (result.Messages.Any(m => m.Status == ValidationStatus.Error))
            {
                result.OverallStatus = ValidationStatus.Error;
            }
            else if (result.Messages.Any(m => m.Status == ValidationStatus.Warning))
            {
                result.OverallStatus = ValidationStatus.Warning;
            }

            return result;
        }
    }
}
