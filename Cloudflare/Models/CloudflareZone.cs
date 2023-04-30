using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudflareDynDns.Cloudflare.Models
{
    public class CloudflareZone
    {
        public string ZoneId {get;set;}
        public string Domain {get;set;}
        public CloudflareSubdomain[] Subdomains {get;set;}
    }
}