using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models.ResponseModels
{
    public class UserTokenDetails : BaseResponseData
    {
        [JsonProperty("id")]
        public string Id {get;set;}
        [JsonProperty("status")]
        public string Status {get;set;}
    }
}