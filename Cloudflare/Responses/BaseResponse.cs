using System.Text.Json.Serialization;
using CloudflareDynDns.Cloudflare.Messages;
using CloudflareDynDns.Cloudflare.Models;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Responses
{
    public class BaseResponse
    {
        public BaseResponse() {}
        public BaseResponse(bool success, ICollection<ResponseMessage> errors = null, ICollection<ResponseMessage> messages = null)
        {
            Success = success;
            if (errors != null)
            {
                Errors = new List<ResponseMessage>(errors);
            }
            if (messages != null)
            {
                Messages = new List<ResponseMessage>(messages);
            }
        }
        [JsonProperty("errors")]
        public ICollection<ResponseMessage> Errors {get; set;}
        [JsonProperty("messages")]
        public ICollection<ResponseMessage> Messages {get; set;}
        [JsonProperty("success")]
        public bool Success {get;set;} = true;

    }
}