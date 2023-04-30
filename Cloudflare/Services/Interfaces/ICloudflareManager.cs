namespace CloudflareDynDns.Cloudflare.Services.Interfaces
{
    public interface ICloudflareManager
    {
        string PublicIP { get; }
        bool CanHandleRequests {get;}

        Task<(bool requiresDnsRefresh, string ipAddress)> RefreshPublicIPAddress();
        Task<bool> VerifyCloudflareToken();
        Task<bool> RefreshDNSRecords();
        
        int GetRefreshIntervalSeconds();
        int GetRequestTimeoutSeconds();
        void StopProcessingRequests();
    }
}