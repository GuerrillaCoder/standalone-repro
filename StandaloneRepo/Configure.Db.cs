using StandaloneRepo.Data;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Data;
using ServiceStack.OrmLite;


[assembly: HostingStartup(typeof(StandaloneRepo.ConfigureDb))]

namespace StandaloneRepo;

// Database can be created with "dotnet run --AppTasks=migrate"   
public class ConfigureDb : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context,services) => {

            var connectionString = context.Configuration.GetConnectionString("Default");

            services.AddSingleton<IDbConnectionFactory>(new OrmLiteConnectionFactory(
                connectionString, PostgreSqlDialect.Provider));

            // $ dotnet ef migrations add CreateIdentitySchema
            // $ dotnet ef database update
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly(nameof(StandaloneRepo)));
                //options.UseSnakeCaseNamingConvention();
            });

            // Enable built-in Database Admin UI at /admin-ui/database
            services.AddPlugin(new AdminDatabaseFeature());



        });


}