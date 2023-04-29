using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudflareDynDns.Cloudflare.Messages.MediatR.Responses
{
    public class CloudflareConfigurationTTLResponse : BaseMediatorResponse
    {
        public TimeSpan TTL {get;}
        public CloudflareConfigurationTTLResponse(int ttl) 
        {
            TTL = TimeSpan.FromSeconds(ttl);
        }
    }
}