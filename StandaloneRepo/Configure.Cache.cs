using ServiceStack.Caching;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.Text;
using System.Data;

[assembly: HostingStartup(typeof(ConfigureCache))]

namespace StandaloneRepo;

public class ConfigureCache : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder
            .ConfigureServices(services =>
            {
                services.AddSingleton<ICacheClient>(c => new OrmLiteCacheClient
                {
                    DbFactory = c.GetRequiredService<IDbConnectionFactory>()
                });

            }).Configure((context, app) =>
            {
                var cache = app.ApplicationServices.GetService<ICacheClient>();

                cache.InitSchema();
            });
    }
}



