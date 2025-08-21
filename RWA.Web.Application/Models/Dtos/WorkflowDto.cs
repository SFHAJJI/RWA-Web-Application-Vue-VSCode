using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RWA.Web.Application.Models.Dtos
{
    public class ValidationMessageDto
    {
        public string Status { get; set; }
        public string? Message { get; set; }
    public object? ErrorData { get; set; }
    public string? ValidatorName { get; set; }
    public string? FileName { get; set; }
    }

    public class WorkflowStepDto
    {
        public int Id { get; set; }
        public string StepName { get; set; }
        public string Status { get; set; }
        public string? DataPayload { get; set; }
        public List<ValidationMessageDto>? ValidationMessages { get; set; }
    }

    public class WorkflowStatusDto
    {
        public List<WorkflowStepDto> Steps { get; set; } = new List<WorkflowStepDto>();
    }
}
