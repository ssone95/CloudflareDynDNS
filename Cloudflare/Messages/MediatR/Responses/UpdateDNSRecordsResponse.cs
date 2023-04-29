using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudflareDynDns.Cloudflare.Messages.MediatR.Responses
{
    public class UpdateDNSRecordsResponse : BaseMediatorResponse
    {
        public DateTime CompletionTimestamp {get;set;} = DateTime.UtcNow;

        public UpdateDNSRecordsResponse(bool success)
        {
            Success = success;
        }
    }
}