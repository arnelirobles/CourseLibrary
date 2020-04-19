using AutoMapper;
using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Defaults;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace CourseLibrary.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
            });

            //services.AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<CourseLibraryContext>();

             services.ConfigAspNetIdentity()
                .ConfigIdentityServer()
                .InstallCertificate()
                .ConfigAuthentication()
                .ConfigDatasource();

            services.AddControllers(setupAction => setupAction.ReturnHttpNotAcceptable = true)
                 
                 .AddNewtonsoftJson(setupAction =>
                 {
                     setupAction.SerializerSettings.ContractResolver =
                     new CamelCasePropertyNamesContractResolver();
                 })
                 .AddXmlDataContractSerializerFormatters()
                 .ConfigureApiBehaviorOptions(setupAction =>
                 {
                     setupAction.InvalidModelStateResponseFactory = context =>
                     {
                         var problemDetailsFactory = context.HttpContext
                         .RequestServices
                         .GetRequiredService<ProblemDetailsFactory>();

                         var problemDetails = problemDetailsFactory
                         .CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                         problemDetails.Detail = "See errors field for details";
                         problemDetails.Instance = context.HttpContext.Request.Path;

                         var actionExecutingContext = context as ActionExecutingContext;

                         if ((context.ModelState.ErrorCount > 0) &&
                            (actionExecutingContext?.ActionArguments.Count ==
                            context.ActionDescriptor.Parameters.Count))
                         {
                             problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                             problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                             problemDetails.Title = "One or more validations occured.";

                             return new UnprocessableEntityObjectResult(problemDetails)
                             {
                                 ContentTypes = { "application/problem+json" }
                             };
                         }

                         problemDetails.Status = StatusCodes.Status400BadRequest;
                         problemDetails.Title = "One or more errors on input occured.";
                         return new BadRequestObjectResult(problemDetails)
                         {
                             ContentTypes = { "application/problem+json" }
                         };
                     };
                 });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.RegisterServices();
            services.AddMvc();
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Migrate(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(
                    appBuilder =>
                        appBuilder.Run(
                            async context =>
                            {
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                            }
                            )
                    );
            }

            app.UseRouting();

            app.UseIdentityServer();

            //app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void Migrate(IApplicationBuilder application)
        {
            using(var scope = application.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                context.Database.Migrate();

                if (!context.Clients.Any())
                {
                   foreach(var client in IdentityDefaults.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }

                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach(var aipResource in IdentityDefaults.GetApiResources())
                    {
                        context.ApiResources.Add(aipResource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach(var idResource in IdentityDefaults.GetIdentityResources())
                    {
                        context.IdentityResources.Add(idResource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}