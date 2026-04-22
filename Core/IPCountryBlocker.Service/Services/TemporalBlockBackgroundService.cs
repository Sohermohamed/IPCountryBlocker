using IPCountryBlocker.Service.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IPCountryBlocker.Presentation
{
    public class TemporalBlockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public TemporalBlockBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var countryService = scope.ServiceProvider.GetRequiredService<CountryService>();
                    countryService.CleanupExpiredTemporalBlocks();
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
}
