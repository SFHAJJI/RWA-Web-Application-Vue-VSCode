using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RWA.Web.Application.Services.Validation
{
    public class ValidatorsFactory : IValidatorsFactory
    {
        private readonly IServiceProvider _provider;

        public ValidatorsFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IEnumerable<IValidator<Models.WorkflowStep>> GetValidatorsFor(string stepName)
        {
            // Resolve all registered generic validators for WorkflowStep
            var all = _provider.GetService<IEnumerable<IValidator<Models.WorkflowStep>>>() ?? Enumerable.Empty<IValidator<Models.WorkflowStep>>();

            // Filter by SupportedWorkflowStepAttribute on the concrete type
            var matches = new List<IValidator<Models.WorkflowStep>>();
            foreach (var v in all)
            {
                var t = v.GetType();
                var attrs = t.GetCustomAttributes(typeof(SupportedWorkflowStepAttribute), false).Cast<SupportedWorkflowStepAttribute>();
                if (attrs.Any(a => string.Equals(a.StepName, stepName, StringComparison.OrdinalIgnoreCase)))
                {
                    matches.Add(v);
                }
            }

            return matches;
        }

        public IValidator<T> GetValidator<T>()
        {
            return _provider.GetService<IValidator<T>>();
        }
    }

}
