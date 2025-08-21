namespace RWA.Web.Application.Services.Workflow.Commands
{
    /// <summary>
    /// Fire-and-forget command for saving files to wwwroot
    /// </summary>
    public class SaveFileCommand : IFileCommand
    {
        private readonly string _fileName;
        private readonly byte[] _fileBytes;
        private readonly string _wwwrootPath;
        private readonly ILogger _logger;

        public SaveFileCommand(string fileName, byte[] fileBytes, string wwwrootPath, ILogger logger)
        {
            _fileName = fileName;
            _fileBytes = fileBytes;
            _wwwrootPath = wwwrootPath;
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                var uploadsPath = Path.Combine(_wwwrootPath, "uploads");
                Directory.CreateDirectory(uploadsPath);

                var safeFileName = Path.GetFileName(_fileName);
                var filePath = Path.Combine(uploadsPath, $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{safeFileName}");

                await File.WriteAllBytesAsync(filePath, _fileBytes);
                _logger.LogInformation("File saved successfully: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save file {FileName}", _fileName);
                // Don't throw - this is fire-and-forget
            }
        }
    }
}
