using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace _2FA.Services
{
    // Placeholder HostedService; extend to generate notifications per rules
    public class NotificationGenerationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationGenerationService> _logger;

        public NotificationGenerationService(IServiceProvider serviceProvider, ILogger<NotificationGenerationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // TODO: Resolve INotificationGenerationService implementation and run rules.
                    _logger.LogDebug("NotificationGenerationService tick");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running notification generation");
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
