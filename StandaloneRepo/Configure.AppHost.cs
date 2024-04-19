[assembly: HostingStartup(typeof(StandaloneRepo.AppHost))]

namespace StandaloneRepo;

public class AppHost() : AppHostBase("StandaloneRepo"), IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            // Configure ASP.NET Core IOC Dependencies
        });

    public override void Configure()
    {
        // Configure ServiceStack, Run custom logic after ASP.NET Core Startup
        SetConfig(new HostConfig {
        });
    }
}