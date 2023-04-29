using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudflareDynDns.Cloudflare.Messages.MediatR.Responses;
using MediatR;

namespace CloudflareDynDns.Cloudflare.Messages.MediatR.Requests
{
    public class CloudflareConfigurationTTLRequest : IRequest<CloudflareConfigurationTTLResponse>
    {
    }
}