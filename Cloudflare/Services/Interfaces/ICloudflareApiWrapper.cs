using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudflareDynDns.Cloudflare.Models;
using CloudflareDynDns.Cloudflare.Responses;

namespace CloudflareDynDns.Cloudflare.Services.Interfaces
{
    public interface ICloudflareApiWrapper
    {
        Task<T> Get<T>(string endpoint, Dictionary<string, string> headers = null) where T : BaseResponse;
    }
}