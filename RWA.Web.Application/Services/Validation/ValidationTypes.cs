using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RWA.Web.Application.Services.Validation
{
    public enum ValidationStatus
    {
        Success,
        Warning,
        Error
    }

    public class ValidationMessage
    {
        public ValidationStatus Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? ErrorData { get; set; }
        public string? ValidatorName { get; set; }
    }

    public class ValidationResult
    {
        public ValidationStatus OverallStatus { get; set; } = ValidationStatus.Success;
        public List<ValidationMessage> Messages { get; } = new List<ValidationMessage>();

        public ValidationResult() { }

        public ValidationResult(List<ValidationMessage> messages)
        {
            Messages = messages;
            OverallStatus = messages.Any(m => m.Status == ValidationStatus.Error)
                ? ValidationStatus.Error
                : messages.Any(m => m.Status == ValidationStatus.Warning)
                    ? ValidationStatus.Warning
                    : ValidationStatus.Success;
        }
    }

}
