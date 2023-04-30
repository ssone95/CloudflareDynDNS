using System.Reflection;
using CloudflareDynDns.Cloudflare.Services.Implementations;
using CloudflareDynDns.Cloudflare.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CloudflareDynDns
{
    class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args);
            host.UseSerilog((hostContext, services, configuration) => {
                configuration.ReadFrom.Configuration(hostContext.Configuration);
            });
            
            host.Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config => 
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddEnvironmentVariables(prefix: "ASPNETCORE_");
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    var env = context.HostingEnvironment;
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("config.json", optional: false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices(ConfigureServices)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<CloudflareDynDnsService>();
                })
                .UseConsoleLifetime();

        private static void ConfigureServices(HostBuilderContext builderContext, IServiceCollection services)
        {
            services.AddSingleton<ICloudflareApiWrapper, CloudflareApiWrapper>();
            services.AddSingleton<ICloudflareManager, CloudflareManager>();
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}
