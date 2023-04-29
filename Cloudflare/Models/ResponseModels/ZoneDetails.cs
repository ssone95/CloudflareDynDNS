using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudflareDynDns.Cloudflare.Models;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models.ResponseModels
{
    public class ZoneAccount
    {
        [JsonProperty("id")]
        public string Id {get;set;}
    }
    public class ZoneDetails : BaseResponseData
    {
        public ZoneDetails() {}

        [JsonProperty("id")]
        public string Id {get;set;}
        [JsonProperty("name")]
        public string Name {get;set;}

        [JsonProperty("account")]
        public ZoneAccount Account {get;set;}

        [JsonProperty("activated_on")]
        public string ActivatedOn {get;set;}

        [JsonProperty("created_on")]
        public string CreatedOn {get;set;}

        [JsonProperty("modified_on")]
        public string ModifiedOn {get;set;}
    }
}