using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RWA.Web.Application.Services.BddMatch
{
    public class BddMatchBackgroundService : BackgroundService
    {
        private readonly IBddMatchJobQueue _queue;
        private readonly IBddMatchService _service;
        private readonly ILogger<BddMatchBackgroundService> _logger;

        public BddMatchBackgroundService(IBddMatchJobQueue queue, IBddMatchService service, ILogger<BddMatchBackgroundService> logger)
        {
            _queue = queue;
            _service = service;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("BDD Match worker started");
            await foreach (var job in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation("Processing BDD match job {Version}", job.Version);
                    await _service.ComputeAndPersistAsync(job.Version, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing BDD match job {Version}", job.Version);
                }
            }
        }
    }
}

