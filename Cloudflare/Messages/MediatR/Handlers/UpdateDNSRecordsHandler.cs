using CloudflareDynDns.Cloudflare.Messages.MediatR.Requests;
using CloudflareDynDns.Cloudflare.Messages.MediatR.Responses;
using CloudflareDynDns.Cloudflare.Services.Interfaces;
using MediatR;

namespace CloudflareDynDns.Cloudflare.Messages.MediatR.Handlers
{
    public class RefreshDNSMessageHandler : IRequestHandler<UpdateDNSRecordsRequest, UpdateDNSRecordsResponse>
    {
        private readonly ICloudflareManager _manager;
        private readonly IMediator _mediator;

        public RefreshDNSMessageHandler(ICloudflareManager manager, IMediator mediator)
        {
            _manager = manager;
            _mediator = mediator;
        }

        public async Task<UpdateDNSRecordsResponse> Handle(UpdateDNSRecordsRequest request, CancellationToken cancellationToken)
        {
            var result = await _manager.RefreshDNSRecords();
            return new(result);
        }
    }
}