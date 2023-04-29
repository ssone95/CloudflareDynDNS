using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareConfiguration
    {
        [JsonProperty("authentication")]
        public CloudflareAuthentication Authentication {get;set;}
        [JsonProperty("zone_id")]
        public string ZoneId {get;set;}
        [JsonProperty("subdomains")]
        public CloudflareSubdomain[] Subdomains {get;set;}
        [JsonProperty("a")]
        public bool ARecords {get;set;}
        [JsonProperty("aaaa")]
        public bool AAAARecords {get;set;}
        [JsonProperty("purgeUnknownRecords")]
        public bool PurgeUnknownRecords {get;set;}
        [JsonProperty("ttl")]
        public int TTL {get;set;}
    }
}