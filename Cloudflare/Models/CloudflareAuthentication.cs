using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareAuthentication
    {
        public string ApiToken {get;set;}
        public CloudflareApiKey ApiKey {get;set;}
    }
}