using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Messages
{
    public class ResponseMessage
    {
        public ResponseMessage() {}
        public ResponseMessage(int code, string message, string type = null) 
        { 
            Code = code;
            Message = message;
            Type = type;
        }
        [JsonProperty("code")]
        public int Code {get;set; }
        [JsonProperty("message")]
        public string Message {get;set;}
        [JsonProperty("type")]
        public string Type {get;set;}
    }
}