using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CloudflareDynDns.Cloudflare.Models.ResponseModels;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Responses
{
    public class ZoneDetailsResponse : BaseResponse
    {
        [JsonProperty("result")]
        public ZoneDetails Result {get;set;}
    }
}