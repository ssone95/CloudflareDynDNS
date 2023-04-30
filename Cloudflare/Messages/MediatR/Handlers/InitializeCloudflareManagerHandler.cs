using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudflareDynDns.Cloudflare.Messages.MediatR.Requests;
using CloudflareDynDns.Cloudflare.Messages.MediatR.Responses;
using CloudflareDynDns.Cloudflare.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudflareDynDns.Cloudflare.Messages.MediatR.Handlers
{
    public class InitializeCloudflareManagerHandler : IRequestHandler<InitializeCloudflareManagerRequest, InitializeCloudflareManagerResponse>
    {
        private readonly IMediator _mediator;
        private readonly ICloudflareManager _manager;
        private readonly ILogger<InitializeCloudflareManagerHandler> _logger;
        public InitializeCloudflareManagerHandler(IMediator mediator, ICloudflareManager manager, ILogger<InitializeCloudflareManagerHandler> logger)
        {
            _mediator = mediator;
            _manager = manager;
            _logger = logger;
        }

        public async Task<InitializeCloudflareManagerResponse> Handle(InitializeCloudflareManagerRequest request, CancellationToken cancellationToken)
        {
            if (!_manager.CanHandleRequests) 
            {
                return new(false);
            }

            var publicIp = await _manager.RefreshPublicIPAddress();
            _logger.Log(LogLevel.None, $"Using '{publicIp.ipAddress}' as publicly available IP Address...");

            bool tokenValidationSucceeded = await _manager.VerifyCloudflareToken();
            _logger.Log(LogLevel.None, $"Token validated: {tokenValidationSucceeded}");

            var isEverythingOk = IsEverythingValidAtStartup(publicIp.ipAddress, tokenValidationSucceeded);
            return new(isEverythingOk);
        }

        private bool IsEverythingValidAtStartup(string publicIp, bool tokenValidated) => 
            !string.IsNullOrEmpty(publicIp) 
            && tokenValidated;
    }
}