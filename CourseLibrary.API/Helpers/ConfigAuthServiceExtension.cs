using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CourseLibrary.API.Helpers
{
    public static class ConfigAuthServiceExtension
    {
        public static AppConfigBuilder ConfigAspNetIdentity(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();
            var environment = serviceProvider.GetService<IHostEnvironment>();
            var clientConnection = configuration.GetConnectionString("ClientConnection");

            services
                .AddDbContext<CourseLibraryContext>(options => options.UseSqlServer(clientConnection))
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<CourseLibraryContext>()
                .AddDefaultTokenProviders();

             

        

            services
                .ConfigureApplicationCookie(options =>
                {
                    options.Events.OnRedirectToLogin = (context =>
                    {
                        context.Response.Headers["Location"] = context.RedirectUri;
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    });
                });

            services.AddAuthorization();

            return new AppConfigBuilder
            {
                Services = services,
                Configuration = configuration,
                Environment = environment
            };
        }

        public static AppConfigBuilder ConfigIdentityServer(
            this AppConfigBuilder builder,
            Action<IIdentityServerBuilder> addOption = null)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            var clientConnection = configuration.GetConnectionString("ClientConnection");
            var identityBuilder = services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseFailureEvents = true;
                })
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(clientConnection,c => c.MigrationsAssembly("CourseLibrary.API"));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(clientConnection, c=>c.MigrationsAssembly("CourseLibrary.API"));

                    options.EnableTokenCleanup = true;
                })          
                .AddAspNetIdentity<IdentityUser>()
                .AddProfileService<ProfileService>();
            

            addOption?.Invoke(identityBuilder);
            builder.IdentityServerBuilder = identityBuilder;

            return builder;
        }

        public static AppConfigBuilder InstallCertificate(this AppConfigBuilder builder)
        {
            var environment = builder.Environment;
            var configuration = builder.Configuration;
            var identityBuilder = builder.IdentityServerBuilder;

            if (environment.IsDevelopment())
            {
                identityBuilder.AddDeveloperSigningCredential();
            }
            else
            {
                var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                X509Certificate2Collection certs;

                store.Open(OpenFlags.ReadOnly);
                certs = store.Certificates.Find(X509FindType.FindByThumbprint, configuration["WEBSITE_LOAD_CERTIFICATES"], false);
                identityBuilder.AddSigningCredential(certs[0]);
            }

            return builder;
        }

        public static AppConfigBuilder ConfigAuthentication(this AppConfigBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["OAuthAuthority"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = configuration["ApiName"];
                    options.ApiSecret = configuration["ApiSecret"];
                });

            return builder;
        }

        public static AppConfigBuilder ConfigDatasource(this AppConfigBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            services.AddDbContext<CourseLibraryContext>(options => options
            .UseSqlServer(configuration.GetConnectionString("ClientConnection"),c=>c.MigrationsAssembly("CourseLibrary.API")));

            return builder;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return services;
        }
    }
}
