using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudflareDynDns.Cloudflare.Messages.MediatR.Responses
{
    public class InitializeCloudflareManagerResponse : BaseMediatorResponse
    {
        public InitializeCloudflareManagerResponse(bool success) 
        {
            Success = success;
        }
    }
}