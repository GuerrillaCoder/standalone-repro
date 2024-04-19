using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.FluentValidation;
using ServiceStack.Html;
using System.Security.Claims;
using ServiceStack.Text;
using StandaloneRepo.ServiceModel.Users;
using StandaloneRepo.Data;

[assembly: HostingStartup(typeof(StandaloneRepo.ConfigureAuth))]

namespace StandaloneRepo;

// Add any additional metadata properties you want to store in the Users Typed Session

public interface IExternalLoginAuthInfoProvider
{
    void PopulateUser(ExternalLoginInfo info, ApplicationUser user);
}

public class ConfigureAuth : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(async services =>
        {
            services.AddSingleton<IAuthHttpGateway, AuthHttpGateway>();
            services.AddTransient<IExternalLoginAuthInfoProvider, ExternalLoginAuthInfoProvider>();

            services.AddPlugin(new AuthFeature(IdentityAuth.For<ApplicationUser>(options =>
            {
                options.SessionFactory = () => new CustomUserSession();
                options.CredentialsAuth();
                options.AdminUsersFeature(feature =>
                {
                    feature.FormLayout =
                    [
                        Input.For<ApplicationUser>(x => x.UserName, c => c.FieldsPerRow(2)),
                        Input.For<ApplicationUser>(x => x.Email, c =>
                        {
                            c.Type = Input.Types.Email;
                            c.FieldsPerRow(2);
                        }),
                        Input.For<ApplicationUser>(x => x.FirstName, c => c.FieldsPerRow(2)),
                        Input.For<ApplicationUser>(x => x.LastName, c => c.FieldsPerRow(2)),
                        Input.For<ApplicationUser>(x => x.DisplayName, c => c.FieldsPerRow(2)),
                        Input.For<ApplicationUser>(x => x.PhoneNumber, c =>
                        {
                            c.Type = Input.Types.Tel;
                            c.FieldsPerRow(2);
                        }),
                        Input.For<ApplicationUser>(x => x.Permissions, c => c.Type = Input.Types.Tag),
                        //Input.For<ApplicationUser>(x => x.Permissions, c => {
                        //    c.Type = Input.Types.Combobox;
                        //    c.AllowableEntries = new KeyValuePair<string, string>[]
                        //                        {
                        //                            new KeyValuePair<string, string>("CanViewReports", "Can View Report"),
                        //                            new KeyValuePair<string, string>("CanDeleteReports", "Can Delete Report"),
                        //                        };
                        //    c.Multiple = true;
                        //}),
                        //Input.For<ApplicationUser>(x => x.Permissions, c => {
                        //    c.Type = Input.Types.Tag;
                        //    c.ReadOnly = true;
                        //}),
                    ];
                });
            })));
        })
        ;
}
public static class SeedAuthData
{
    public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Create the "Admin" role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create the admin user if it doesn't exist
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };
            await userManager.CreateAsync(adminUser, "AdminPassword123!");
            await userManager.AddToRoleAsync(adminUser, "Admin");

            //await userManager.AddClaimAsync(adminUser, new Claim(JwtClaimTypes.Permission, "Test"));
        }
    }
}
public class ExternalLoginAuthInfoProvider(IConfiguration configuration, IAuthHttpGateway authGateway)
    : IExternalLoginAuthInfoProvider
{
    public void PopulateUser(ExternalLoginInfo info, ApplicationUser user)
    {
        var accessToken = info.AuthenticationTokens?.FirstOrDefault(x => x.Name == "access_token");
        //var accessTokenSecret = info.AuthenticationTokens?.FirstOrDefault(x => x.Name == "access_token_secret");

        if (info.LoginProvider == "Facebook")
        {
            user.FacebookUserId = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            user.DisplayName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            user.FirstName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            user.LastName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;

            if (accessToken != null)
            {
                var facebookInfo = JsonObject.Parse(authGateway.DownloadFacebookUserInfo(accessToken.Value, "picture"));
                var picture = facebookInfo.Object("picture");
                var data = picture?.Object("data");
                if (data != null)
                {
                    if (data.TryGetValue("url", out var profileUrl))
                    {
                        user.ProfileUrl = profileUrl.SanitizeOAuthUrl();
                    }
                }
            }
        }
        else if (info.LoginProvider == "Google")
        {
            user.GoogleUserId = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            user.DisplayName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            user.FirstName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            user.LastName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            user.GoogleProfilePageUrl = info.Principal?.Claims.FirstOrDefault(x => x.Type == "urn:google:profile")?.Value;

            if (accessToken != null)
            {
                var googleInfo = JsonObject.Parse(authGateway.DownloadGoogleUserInfo(accessToken.Value));
                user.ProfileUrl = googleInfo.Get("picture").SanitizeOAuthUrl();
            }
        }
        else if (info.LoginProvider == "Microsoft")
        {
            user.MicrosoftUserId = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            user.DisplayName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            user.FirstName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            user.LastName = info.Principal?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            if (accessToken != null)
            {
                user.ProfileUrl = authGateway.CreateMicrosoftPhotoUrl(accessToken.Value, "96x96");
            }
        }
    }
}

//.ConfigureAppHost(appHost =>
//{
//    var appSettings = appHost.AppSettings;
//    appHost.Plugins.Add(new AuthFeature(() => new CustomUserSession(),
//    [
//        new JwtAuthProvider(appSettings) {
//            AuthKeyBase64 = appSettings.GetString("AuthKeyBase64") ?? "cARl12kvS/Ra4moVBIaVsrWwTpXYuZ0mZf/gNLUhDW5=",
//        },
//        new CredentialsAuthProvider(appSettings),     /* Sign In with Username / Password credentials */
//        new FacebookAuthProvider(appSettings),        /* Create App https://developers.facebook.com/apps */
//        new GoogleAuthProvider(appSettings),          /* Create App https://console.developers.google.com/apis/credentials */
//        new MicrosoftGraphAuthProvider(appSettings) /* Create App https://apps.dev.microsoft.com */
//    ])
//    {
//        IncludeDefaultLogin = false
//    });
//    // Removing unused UserName in Admin Users UI 
//    appHost.Plugins.Add(new AdminUsersFeature {

//        // Show custom fields in Search Results
//        QueryUserAuthProperties =
//        [
//            nameof(AppUser.Id),
//            nameof(AppUser.Email),
//            nameof(AppUser.DisplayName),
//            nameof(AppUser.Department),
//            nameof(AppUser.CreatedDate),
//            nameof(AppUser.LastLoginDate)
//        ],
//        QueryMediaRules =
//        [
//            MediaRules.ExtraSmall.Show<AppUser>(x => new { x.Id, x.Email, x.DisplayName }),
//            MediaRules.Small.Show<AppUser>(x => x.Department)
//        ],
//        // Add Custom Fields to Create/Edit User Forms
//        FormLayout =
//        [
//            Input.For<AppUser>(x => x.Email),
//            Input.For<AppUser>(x => x.DisplayName),
//            Input.For<AppUser>(x => x.Company),
//            Input.For<AppUser>(x => x.Department, c => c.FieldsPerRow(2)),
//            Input.For<AppUser>(x => x.PhoneNumber, c =>
//            {
//                c.Type = Input.Types.Tel;
//                c.FieldsPerRow(2);
//            }),
//            Input.For<AppUser>(x => x.Nickname, c =>
//            {
//                c.Help = "Public alias (3-12 lower alpha numeric chars)";
//                c.Pattern = "^[a-z][a-z0-9_.-]{3,12}$";
//                //c.Required = true;
//            }),
//            Input.For<AppUser>(x => x.ProfileUrl, c => c.Type = Input.Types.Url),
//            Input.For<AppUser>(x => x.IsArchived), Input.For<AppUser>(x => x.ArchivedDate)
//        ]
//    });
//});
