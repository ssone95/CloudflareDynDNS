using CloudflareDynDns.Cloudflare.Messages.MediatR.Requests;
using CloudflareDynDns.Cloudflare.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudflareDynDns.Cloudflare.Services.Implementations
{

    public class CloudflareDynDnsService : ICloudflareDynDnsService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CloudflareDynDnsService> _logger;

        private bool _isRunning;
        public CloudflareDynDnsService(IMediator mediator, ILogger<CloudflareDynDnsService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new InitializeCloudflareManagerRequest());

            if (!response.Success)
            {
                throw new Exception($"Failed to initialize Cloudflare Manager!");
            }

            // Intentionally leaving this running in the separate thread
            _isRunning = true;
            Task.Run(MainLoop);
        }

        private async Task MainLoop()
        {
            var ttlResponse = await _mediator.Send(new CloudflareConfigurationTTLRequest());
            _logger.LogInformation($"Processing Subdomains from the configuration...");
            while (_isRunning)
            {
                var result = await _mediator.Send(new UpdateDNSRecordsRequest());
                if (result.Success) 
                {
                    _logger.LogInformation($"Successfully processed DNS records update!");
                }
                else
                {
                    _logger.LogWarning($"Failure during processing DNS records update!");
                }
                await Task.Delay(ttlResponse.TTL);
                _logger.LogInformation($"Processing Subdomains from the configuration, TTL of {ttlResponse.TTL.TotalSeconds} seconds ellapsed...");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Cloudflare service is stopping...");
            _isRunning = false;
            return Task.CompletedTask;
        }
    }
}