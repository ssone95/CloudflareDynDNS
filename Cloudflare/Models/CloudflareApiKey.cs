using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareApiKey
    {
        [JsonProperty("api_key")]
        public string ApiKey {get;set;}
        [JsonProperty("account_email")]
        public string EmailAddress {get;set;}
    }
}