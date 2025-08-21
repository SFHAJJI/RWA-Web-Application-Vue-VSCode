namespace RWA.Web.Application.Services.Workflow.Dtos
{
    public class TriggerUploadResultDto
    {
        public RWA.Web.Application.Models.Dtos.WorkflowStepDto[] Steps { get; set; } = new RWA.Web.Application.Models.Dtos.WorkflowStepDto[0];
        public RWA.Web.Application.Models.Dtos.ValidationMessageDto[] Validation { get; set; } = new RWA.Web.Application.Models.Dtos.ValidationMessageDto[0];
        public string? SavedFile { get; set; }
    }
}
