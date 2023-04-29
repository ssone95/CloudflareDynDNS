using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Responses
{
    public class Meta
    {
        [JsonProperty("auto_added")]
        public bool AutoAdded { get; set; }
        [JsonProperty("source")]
        public string Source { get; set; }
    }

    public class PatchDnsZoneData
    {
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("proxied")]
        public bool Proxied { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("comment")]
        public string Comment { get; set; }
        [JsonProperty("created_on")]
        public DateTime CreatedOn { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("locked")]
        public bool Locked { get; set; }
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
        [JsonProperty("modified_on")]
        public DateTime ModifiedOn { get; set; }
        [JsonProperty("proxiable")]
        public bool Proxiable { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        [JsonProperty("ttl")]
        public int TTL { get; set; }
        [JsonProperty("zone_id")]
        public string ZoneId { get; set; }
        [JsonProperty("zone_name")]
        public string ZoneName { get; set; }
    }
    public class PatchDnsZoneResponse : BaseResponse
    {
        [JsonProperty("result")]
        public PatchDnsZoneData Result {get;set;}
    }
}