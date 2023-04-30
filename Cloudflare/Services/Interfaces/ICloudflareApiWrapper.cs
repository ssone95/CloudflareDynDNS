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
        Task<T> Patch<T>(string endpoint, Dictionary<string, string> properties, Dictionary<string, string> headers = null) where T : BaseResponse;
        Task<T> Post<T>(string endpoint, object dataToTransfer = null, Dictionary<string, string> headers = null) where T : BaseResponse;
        Task<T> Put<T>(string endpoint, object dataToTransfer = null, Dictionary<string, string> headers = null) where T : BaseResponse;
        Task<T> Delete<T>(string endpoint, object dataToTransfer = null, Dictionary<string, string> headers = null) where T : BaseResponse;
    }
}