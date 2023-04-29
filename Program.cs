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
            CreateHostBuilder(args).Build().Run();
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
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: false);
                    config.AddEnvironmentVariables();
                })
                .UseSerilog((ctx, lc) => lc.WriteTo.Console())
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
