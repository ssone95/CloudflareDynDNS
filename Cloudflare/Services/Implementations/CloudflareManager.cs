using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CloudflareDynDns.Cloudflare.Models;
using CloudflareDynDns.Cloudflare.Models.ResponseModels;
using CloudflareDynDns.Cloudflare.Responses;
using CloudflareDynDns.Cloudflare.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CloudflareDynDns.Cloudflare.Services.Implementations
{
    public class CloudflareManager : ICloudflareManager
    {
        private readonly HttpClient _httpClient;
        private readonly ICloudflareApiWrapper _cloudflareApi;
        private readonly ILogger<CloudflareManager> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private CloudflareConfiguration _config = null;

        private Dictionary<string, string> _headers;
        private string _publicIP = string.Empty;
        public string PublicIP => _publicIP;
        private bool _loadedConfiguration;
        private bool _canAcceptRequests;
        public bool LoadedConfiguration => _loadedConfiguration;
        public bool CanHandleRequests => _canAcceptRequests && LoadedConfiguration;

        public CloudflareManager(ILogger<CloudflareManager> logger, ICloudflareApiWrapper cloudflareApi, IHostApplicationLifetime applicationLifetime)
        {
            _httpClient = new();
            _cloudflareApi = cloudflareApi;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
        }
        public async Task LoadConfiguration(string configurationFile = "config.json")
        {
            if (_loadedConfiguration) return;

            string jsonString = await File.ReadAllTextAsync("config.json");
            if (string.IsNullOrEmpty(jsonString)) 
            {
                _logger.LogError($"Configuration could not be loaded!");
                _loadedConfiguration = false;
            }
            _config = await Task.Run(() => JsonConvert.DeserializeObject<CloudflareConfiguration>(jsonString));
            _logger.LogInformation($"Loaded Cloudflare configuration!");

            _loadedConfiguration = true;
            _canAcceptRequests = true;
        }
        public async Task<bool> VerifyCloudflareToken()
        {
            // TODO: Do validation and throw some errors here
            if (!CanHandleRequests) return false;

            var response = await _cloudflareApi.Get<VerifyTokenResponse>("user/tokens/verify", new()
            {
                { "Authorization", $"Bearer {_config.Authentication.ApiToken}"}
            });

            if (response.Success)
            {
                PrepareDefaultRequestHeaders();
            }
            return response.Success;
        }

        public async Task<bool> RefreshDNSRecords() 
        {
            var zoneExists = await CheckIfZoneExists();
            if (!zoneExists) return false;



            return true;
        }

        private async Task<bool> CheckIfZoneExists() 
        {
            if (!CanHandleRequests) return false;

            var response = await _cloudflareApi.Get<ZoneDetailsResponse>($"zones/{_config.ZoneId}", _headers);

            return response.Success;
        }

        private void PrepareDefaultRequestHeaders()
        {
            _headers = new()
            {
                { "X-Auth-Email", _config.Authentication.ApiKey.EmailAddress },
                { "Authorization", $"Bearer {_config.Authentication.ApiToken}" }
            };
        }

        public async Task<string> RefreshPublicIPAddress()
        {
            if (!CanHandleRequests) return string.Empty;

            string publicIp = "0.0.0.0";
            const string ddnsHost = "https://v4.ident.me";
            try
            {
                using (var response = await _httpClient.GetAsync(ddnsHost))
                {
                    publicIp = await GetPublicIpFromResponse(response);
                }
                _publicIP = publicIp;
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
            }
            return publicIp;
        }

        private async Task<string> GetPublicIpFromResponse(HttpResponseMessage message)
        {
            if (message.IsSuccessStatusCode)
            {
                var body = await message.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    return GetIpAddressFromString(body);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private string GetIpAddressFromString(string body)
        {
            string regexPattern = @"\d+\.\d+\.\d+\.\d+";
            var match = Regex.Match(body, regexPattern);
            if (match.Success)
            {
                return match.Value;
            }
            return string.Empty;
        }

        public int GetTTL() => _config.TTL;

        public void StopProcessingRequests()
        {
            _canAcceptRequests = false;
            _applicationLifetime.StopApplication();
        }
    }
}