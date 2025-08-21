namespace RWA.Web.Application.Services.Workflow.Commands
{
    /// <summary>
    /// Command interface for file operations using Command pattern
    /// </summary>
    public interface IFileCommand
    {
        Task ExecuteAsync();
    }
}
