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
        private readonly IHostingEnvironment _env;

        private bool _isRunning;
        public CloudflareDynDnsService(IMediator mediator, ILogger<CloudflareDynDnsService> logger, IHostingEnvironment env)
        {
            _mediator = mediator;
            _logger = logger;
            _env = env;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Running in {_env.EnvironmentName} environment!");
            _logger.Log(LogLevel.None, "Testing");
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
            while (_isRunning)
            {
                var result = await _mediator.Send(new UpdateDNSRecordsRequest());
                if (result.Success) 
                {
                    _logger.Log(LogLevel.None, $"Successfully processed DNS records update!");
                }
                else
                {
                    _logger.LogWarning($"Failure during processing DNS records update!");
                }
                await Task.Delay((int)ttlResponse.TTL.TotalMilliseconds);
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