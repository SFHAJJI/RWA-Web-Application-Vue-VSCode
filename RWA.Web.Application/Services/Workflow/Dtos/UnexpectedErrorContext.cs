namespace RWA.Web.Application.Services.Workflow.Dtos
{
    /// <summary>
    /// Context for system/unexpected errors (DB issues, step not found, etc.)
    /// Different from ValidationError which is for user input validation errors
    /// </summary>
    public class UnexpectedErrorContext
    {
        public string StepName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string? Details { get; set; }
        public Exception? Exception { get; set; }
    }
}
