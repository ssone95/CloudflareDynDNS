using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudflareDynDns.Cloudflare.Responses;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Models.ResponseModels
{
    public class DnsRecord
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("zone_id")]
        public string ZoneId { get; set; }

        [JsonProperty("zone_name")]
        public string ZoneName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("proxiable")]
        public bool Proxiable { get; set; }

        [JsonProperty("proxied")]
        public bool Proxied { get; set; }

        [JsonProperty("ttl")]
        public int Ttl { get; set; }

        [JsonProperty("locked")]
        public bool Locked { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }

        [JsonProperty("modified_on")]
        public DateTime ModifiedOn { get; set; }
    }

    public class Meta
    {
        [JsonProperty("auto_added")]
        public bool AutoAdded { get; set; }

        [JsonProperty("managed_by_apps")]
        public bool ManagedByApps { get; set; }

        [JsonProperty("managed_by_argo_tunnel")]
        public bool ManagedByArgoTunnel { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }
    }
    
    public class DnsRecordsResponse : BaseResponse
    {
        [JsonProperty("result")]
        public DnsRecord[] Result {get;set;}
    }
}