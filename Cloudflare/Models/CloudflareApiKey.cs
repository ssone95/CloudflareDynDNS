using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareApiKey
    {
        public string ApiKey {get;set;}
        public string EmailAddress {get;set;}
    }
}