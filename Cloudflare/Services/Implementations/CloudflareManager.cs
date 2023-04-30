using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CloudflareDynDns.Cloudflare.Models;
using CloudflareDynDns.Cloudflare.Models.ResponseModels;
using CloudflareDynDns.Cloudflare.Responses;
using CloudflareDynDns.Cloudflare.Services.Interfaces;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        private Dictionary<string, string> _headers;
        private string _publicIP = string.Empty;
        public string PublicIP => _publicIP;

        public bool CanHandleRequests { get; private set; }

        private CloudflareConfiguration _config => _configuration.Get<CloudflareConfiguration>();

        public CloudflareManager(ILogger<CloudflareManager> logger, ICloudflareApiWrapper cloudflareApi, IHostApplicationLifetime applicationLifetime, 
            IConfiguration configuration)
        {
            _httpClient = new();
            _cloudflareApi = cloudflareApi;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _configuration = configuration;

            CanHandleRequests = true;
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
            _logger.Log(LogLevel.None, $"Processing DNS records from the configuration...");
            var refreshResult = await RefreshPublicIPAddress();

            var zoneExists = await CheckIfZoneExistsAsync(_config.Zone.ZoneId);
            if (!zoneExists) return false;

            var existingDnsRecords = await GetExistingDnsRecordsAsync(_config.Zone.ZoneId);
            if (existingDnsRecords?.Success != true) return false;

            foreach (var configDnsRecord in _config.Zone.Subdomains)
            {
                await ProcessDnsRecordRefresh(existingDnsRecords, configDnsRecord, refreshResult.requiresDnsRefresh);
            }

            return true;
        }

        private async Task ProcessDnsRecordRefresh(DnsRecordsResponse existingDnsRecords, CloudflareSubdomain configDnsRecord, bool ipAddressChanged)
        {
            var existingDnsRecord = existingDnsRecords.Result.FirstOrDefault(x => x.Name.ToLower() == configDnsRecord.Name.ToLower());
            if (existingDnsRecord != null)
            {
                _logger.LogInformation($"Existing record: {configDnsRecord.Name}");
                await UpdateExistingDnsRecord(existingDnsRecord, configDnsRecord, ipAddressChanged);
            }
            else
            {
                _logger.LogInformation($"New record: {configDnsRecord.Name}");
                await CreateNewDnsRecord(configDnsRecord);
            }
        }

        private async Task CreateNewDnsRecord(CloudflareSubdomain configDnsRecord)
        {
            _logger.LogInformation($"Creating a new Subdomain record {configDnsRecord.Name} with public ip: {PublicIP}");

            var response = await _cloudflareApi.Post<PatchDnsZoneResponse>($"zones/{_config.Zone.ZoneId}/dns_records", new {
                content = PublicIP,
                name = configDnsRecord.Name,
                proxied = configDnsRecord.Proxied,
                type = "A",
                comment = $"CloudflareDynDNS .NET tool rocks!",
                ttl = configDnsRecord.TTL
            }, _headers);

            if (response.Success)
            {
                _logger.LogInformation($"Successfully created {configDnsRecord.Name} with ip address {PublicIP}!");
            }
            else
            {
                _logger.LogError($"Failure during creation of new DNS entry {configDnsRecord.Name}, will retry after {_config.RefreshTimeSeconds} seconds!");
            }
        }

        private async Task UpdateExistingDnsRecord(DnsRecord existingDnsRecord, CloudflareSubdomain configDnsRecord, bool ipAddressChanged)
        {
            var now = DateTime.UtcNow;
            var shouldBeUpdatedOn = existingDnsRecord.ModifiedOn.Add(TimeSpan.FromSeconds(configDnsRecord.TTL));

            if (shouldBeUpdatedOn < now || existingDnsRecord.Content != PublicIP || ipAddressChanged)
            {
                _logger.LogInformation($"Updating Subdomain {configDnsRecord.Name} - previous update on {existingDnsRecord.ModifiedOn}, " +
                    $"public ip - old: {existingDnsRecord.Content}, new: {PublicIP}");
                
                var response = await _cloudflareApi.Patch<PatchDnsZoneResponse>($"zones/{_config.Zone.ZoneId}/dns_records/{existingDnsRecord.Id}", new() {
                    { "content", PublicIP },
                    { "name", existingDnsRecord.Name },
                    { "type", existingDnsRecord.Type },
                    { "comment", $"CloudflareDynDNS .NET tool rocks!" },
                    { "ttl", configDnsRecord.TTL.ToString() }
                }, _headers);
                if (response.Success)
                {
                    _logger.LogInformation($"Successfully updated {configDnsRecord.Name} with new ip address!");
                }
                else
                {
                    _logger.LogError($"Failure during updating bound ip for {configDnsRecord.Name}, will retry after {_config.RefreshTimeSeconds} seconds!");
                }
            }
            else
            {
                _logger.LogInformation($"Subdomain {configDnsRecord.Name} was updated recently ({existingDnsRecord.ModifiedOn}) and public ip didn't change, skipping...");
            }
        }

        private async Task<bool> CheckIfZoneExistsAsync(string zoneId) 
        {
            if (!CanHandleRequests) return false;

            var response = await _cloudflareApi.Get<ZoneDetailsResponse>($"zones/{zoneId}", _headers);

            return response.Success;
        }

        private async Task<DnsRecordsResponse> GetExistingDnsRecordsAsync(string zoneId) 
        {
            if (!CanHandleRequests) return null;
            return await _cloudflareApi.Get<DnsRecordsResponse>($"zones/{zoneId}/dns_records", _headers);
        }

        private void PrepareDefaultRequestHeaders()
        {
            _headers = new()
            {
                { "X-Auth-Email", _config.Authentication.ApiKey.EmailAddress },
                { "Authorization", $"Bearer {_config.Authentication.ApiToken}" }
            };
        }

        public async Task<(bool requiresDnsRefresh, string ipAddress)> RefreshPublicIPAddress()
        {
            if (!CanHandleRequests) return (false, string.Empty);

            string publicIp = "0.0.0.0";
            const string ddnsHost = "https://v4.ident.me";
            try
            {
                _logger.LogInformation($"Checking current public IP...");
                string prevPublicIp = PublicIP;
                using (var response = await _httpClient.GetAsync(ddnsHost))
                {
                    publicIp = await GetPublicIpFromResponse(response);
                }
                _publicIP = publicIp;

                bool arePreviousAndNewIpEqual = prevPublicIp != PublicIP;
                if (arePreviousAndNewIpEqual) 
                {
                    _logger.Log(LogLevel.None, $"Public IP changed (old: {prevPublicIp}, new: {PublicIP}), refreshing DNS configuration...");
                }
                return (arePreviousAndNewIpEqual, PublicIP);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}");
                return (false, publicIp);
            }
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

        public int GetRefreshIntervalSeconds() => _config.RefreshTimeSeconds;
        public int GetRequestTimeoutSeconds() => _config.RequestTimeoutSeconds;

        public void StopProcessingRequests()
        {
            CanHandleRequests = false;
            _applicationLifetime.StopApplication();
        }
    }
}