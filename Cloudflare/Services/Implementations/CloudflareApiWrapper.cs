using System.Net.Http.Headers;
using System.Text.Json;
using CloudflareDynDns.Cloudflare.Messages;
using CloudflareDynDns.Cloudflare.Models;
using CloudflareDynDns.Cloudflare.Responses;
using CloudflareDynDns.Cloudflare.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Services.Implementations
{
    public class CloudflareApiWrapper : ICloudflareApiWrapper
    {
        private readonly HttpClient _client;
        private readonly ILogger<CloudflareApiWrapper> _logger;
        private const string _apiBaseAddress = "https://api.cloudflare.com/client/v4/";
        public CloudflareApiWrapper(ILogger<CloudflareApiWrapper> logger)
        {
            _client = new()
            {
                Timeout = TimeSpan.FromSeconds(30),
                BaseAddress = new Uri(_apiBaseAddress)
            };
            _logger = logger;
        }

        SemaphoreSlim _semaphore = new(1, 1);
        public async Task<T> Get<T>(string endpoint, Dictionary<string, string> headers = null) where T : BaseResponse
        {
            T response;
            try
            {
                await _semaphore.WaitAsync();
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var httpResponse = await _client.SendAsync(request);
                if ((int)httpResponse.StatusCode >= 200 && (int)httpResponse.StatusCode < 500)
                {
                    var jsonContent = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<T>(jsonContent);
                }
                else
                {
                    response = CreateErrorResponse<T>($"Cloudflare API is currently not available. StatusCode: {httpResponse.StatusCode}");
                }
                _semaphore.Release();
            }
            catch (Exception ex)
            {
                _semaphore.Release();
                _logger.LogError($"{ex}");
                response = CreateErrorResponse<T>($"Cloudflare API is currently not available. Error: {ex}");
            }
            return response;
        }

        public async Task<T> Patch<T>(string endpoint, Dictionary<string, string> properties, Dictionary<string, string> headers = null) where T : BaseResponse
        {
            T response;
            try
            {
                await _semaphore.WaitAsync();
                var request = new HttpRequestMessage(HttpMethod.Patch, endpoint);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string jsonRequestContent = JsonConvert.SerializeObject(properties);
                request.Content = new StringContent(jsonRequestContent, new MediaTypeHeaderValue("application/json"));
                
                var httpResponse = await _client.SendAsync(request);
                if ((int)httpResponse.StatusCode >= 200 && (int)httpResponse.StatusCode < 500)
                {
                    var jsonContent = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<T>(jsonContent);
                }
                else
                {
                    response = CreateErrorResponse<T>($"Cloudflare API is currently not available. StatusCode: {httpResponse.StatusCode}");
                }
                _semaphore.Release();
            }
            catch (Exception ex)
            {
                _semaphore.Release();
                _logger.LogError($"{ex}");
                response = CreateErrorResponse<T>($"Cloudflare API is currently not available. Error: {ex}");
            }
            return response;
        }

        private T CreateErrorResponse<T>(string message) where T : BaseResponse
        {
            BaseResponse response = new();
            response.Success = false;
            response.Errors = new List<ResponseMessage>() 
            {
                new(code: -1, message: message)
            };
            return (T)response;
        }
    }
}