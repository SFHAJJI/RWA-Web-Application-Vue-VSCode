using System.Collections.Generic;
using FluentValidation;
using RWA.Web.Application.Models;

namespace RWA.Web.Application.Services.Validation
{
    public interface IValidatorsFactory
    {
        // Return the strongly-typed Fluent validators applicable for the given step name
        IEnumerable<IValidator<WorkflowStep>> GetValidatorsFor(string stepName);
        IValidator<T> GetValidator<T>();
    }
}
