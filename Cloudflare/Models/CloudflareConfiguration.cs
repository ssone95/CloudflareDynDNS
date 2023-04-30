using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareConfiguration
    {
        public CloudflareAuthentication Authentication {get;set;}

        public CloudflareZone Zone {get;set;}

        public int RefreshTimeSeconds {get;set;} = 60;
        public int RequestTimeoutSeconds {get;set;} = 45;
    }
}