using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudflareDynDns.Cloudflare.Messages.MediatR.Responses
{
    public class BaseMediatorResponse
    {
        public DateTime Timestamp {get; protected set;} = DateTime.UtcNow;
        public bool Success {get; protected set;} = true;
    }
}