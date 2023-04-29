using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudflareDynDns.Cloudflare.Messages.MediatR.Requests;
using CloudflareDynDns.Cloudflare.Messages.MediatR.Responses;
using CloudflareDynDns.Cloudflare.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CloudflareDynDns.Cloudflare.Messages.MediatR.Handlers
{
    public class CloudflareConfigurationTTlRequestHandler : IRequestHandler<CloudflareConfigurationTTLRequest, CloudflareConfigurationTTLResponse>
    {
        private readonly ILogger<CloudflareConfigurationTTlRequestHandler> _logger;

        private readonly ICloudflareManager _manager;

        public CloudflareConfigurationTTlRequestHandler(ILogger<CloudflareConfigurationTTlRequestHandler> logger, ICloudflareManager manager)
        {
            _logger = logger;
            _manager = manager;
        }
        public async Task<CloudflareConfigurationTTLResponse> Handle(CloudflareConfigurationTTLRequest request, CancellationToken cancellationToken)
        {
            return new(_manager.GetTTL());
        }
    }
}