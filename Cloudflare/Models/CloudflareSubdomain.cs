using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareSubdomain
    {
        [JsonProperty("domain")]
        public string Domain {get;set;}
        [JsonProperty("name")]
        public string Name {get;set;}
        [JsonProperty("proxied")]
        public bool Proxied {get;set;}
    }
}