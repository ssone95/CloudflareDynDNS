using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareAuthentication
    {
        [JsonProperty("api_token")]
        public string ApiToken {get;set;}
        [JsonProperty("api_key")]
        public CloudflareApiKey ApiKey {get;set;}
    }
}