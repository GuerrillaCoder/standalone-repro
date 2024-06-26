using ServiceStack;
using ServiceStack.Data;

[assembly: HostingStartup(typeof(StandaloneRepo.ConfigureAutoQuery))]

namespace StandaloneRepo;

public class ConfigureAutoQuery : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services =>
        {
            // Enable Audit History
            services.AddSingleton<ICrudEvents>(c =>
                new OrmLiteCrudEvents(c.GetRequiredService<IDbConnectionFactory>()));

            services.AddPlugin(new AutoQueryDataFeature());

            // For Bookings https://github.com/NetCoreApps/BookingsCrud
            services.AddPlugin(new AutoQueryFeature
            {
                MaxLimit = 1000,
                //IncludeTotal = true,
                
            });
        })
        .ConfigureAppHost(appHost =>
        {
            appHost.Resolve<ICrudEvents>().InitSchema();
        });
}
