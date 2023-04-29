namespace CloudflareDynDns.Cloudflare.Services.Interfaces
{
    public interface ICloudflareManager
    {
        string PublicIP { get; }
        bool LoadedConfiguration {get;}
        bool CanHandleRequests {get;}

        Task LoadConfiguration(string configurationFile = "config.json");
        Task<(bool requiresDnsRefresh, string ipAddress)> RefreshPublicIPAddress();
        Task<bool> VerifyCloudflareToken();
        Task<bool> RefreshDNSRecords();
        
        int GetTTL();
        void StopProcessingRequests();
    }
}