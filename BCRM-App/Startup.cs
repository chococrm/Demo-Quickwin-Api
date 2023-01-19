using BCRM.Common.Constants;
using BCRM.Common.Extensions;
using BCRM.Common.Helpers;
using BCRM.Common.Services;
using BCRM.Common.Services.RemoteInternal.IAM;
using BCRM.Logging;
using BCRM.Logging.Extension;
using BCRM_App.Areas.Api;
using BCRM_App.Constants;
using BCRM_App.Models.DBModel.Demoquickwin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BCRM_App
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:3000", "http://localhost:8080")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });


            // Application Insights
            services.AddApplicationInsightsTelemetry();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddNewtonsoftJson(x => x.SerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new LowerCaseNamingStrategy() })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .Add_BCRMLogging_Internal();

            // BCRM - Register App
            services.Register_BCRM_App(Configuration, Access_Mode: BCRM_Core_Const.Credentials.AccessMode.External);

            // BCRM - Client
            services.AddBCRM_Client(setup =>
            {
                setup.Set_Client_Environment(WebHostEnvironment);
                setup.Set_Request_MaxRetry(3);
                setup.UseApiEndpointHttpMessageHandler(sp => new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => { return true; }
                });
            });

            // BCRM - Common
            services.AddBCRM_Common(Configuration);

            // BCRM - CRM
            services.AddBCRM_CRM(Configuration);

            // BCRM - Config
            services.AddSingleton<IBCRM_Config_Service, BCRM_Config_Service>();

            // BCRM - Storage
            services.AddBCRM_Storage(Configuration);

            // BCRM - IAM
            services.Add_IAM_Client_Service();

            // Sql Connection
            Task<string> demoquickwinConStr = Task.Run(() => BCRM.Common.Configs.BCRM_Config.Credentials.Database.BCRM.ConnectionString(BCRM.Common.Configs.BCRM_Config.Platform.Brand_Ref));

            demoquickwinConStr.Wait();

            CCConstant.Database.ConnectionString.Core = demoquickwinConStr.Result;

            services.AddDbContext<BCRM_81_Entities>(options =>
            {
                options.UseSqlServer(CCConstant.Database.ConnectionString.Core,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                });
            }, ServiceLifetime.Transient);

            // BCRM - Logging
            BCRM_Logging_AzureAnalytics_Configs sink_Hot = BCRM_Logging_Sink.Read_Sink_Settings(Configuration, "BCRM_Logging:Sinks:bcrm-hot") as BCRM_Logging_AzureAnalytics_Configs;
            BCRM_Logging_AzureStorage_Configs sink_Cold = BCRM_Logging_Sink.Read_Sink_Settings(Configuration, "BCRM_Logging:Sinks:bcrm-cold") as BCRM_Logging_AzureStorage_Configs;

            sink_Hot.Credentials_FromKeyVault(BCRM.Common.Configs.BCRM_Config.Azure.KeyVault.Endpoint.External, BCRM_Core_Const.Azure.KeyVault.Secrets.BCRM_Apps_Log_Analytics);
            sink_Cold.Credentials_FromKeyVault(BCRM.Common.Configs.BCRM_Config.Azure.KeyVault.Endpoint.External, BCRM_Core_Const.Azure.KeyVault.Secrets.BCRM_Apps_Log_BlobStorage);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .BCRM_Enrich()
                .BCRM_Destructure()
                // Sink - Azure Analytics
                .BCRM_AZ_Analytics(sink_Hot)
                // Sink - Azure Storage
                .BCRM_AZ_Storage(sink_Cold)
                .CreateLogger();

            // Api - Versioning
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                BCRM_App_Api_RouteConfig.Config(endpoints); // Area - Api
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}