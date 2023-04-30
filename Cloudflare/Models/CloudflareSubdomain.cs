using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareSubdomain
    {
        public string Name {get;set;}
        public bool Proxied {get;set;}

        public int TTL {get;set;} = 60;
    }
}