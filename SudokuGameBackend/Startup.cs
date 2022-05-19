using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Invio.Extensions.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SudokuGameBackend.BLL.Extensions;
using SudokuGameBackend.BLL.Hubs;
using SudokuGameBackend.BLL.Interfaces;
using SudokuGameBackend.BLL.Services;
using KissLog;
using KissLog.AspNetCore;
using KissLog.CloudListeners.Auth;
using KissLog.CloudListeners.RequestLogsListener;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;

namespace SudokuGameBackend
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private IWebHostEnvironment CurrentEnvironment { get; set; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            if (CurrentEnvironment.IsProduction())
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\HostingSpaces\user15854\user15854.realhost-free.net\data\keys\"));

                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
                services.AddScoped<KissLog.ILogger>((context) =>
                {
                    return Logger.Factory.Get();
                });

                services.AddLogging(logging =>
                {
                    logging.AddKissLog();
                });
            }

            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                });
            }

            services.AddSingleton<IGameSessionsService, GameSessionsService>();
            services.AddSingleton<IMatchmakingService, MatchmakingService>();
            services.AddSingleton<ICacheService, CacheService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IPuzzleService, PuzzleService>();
            services.AddScoped<IStatsService, StatsService>();
            services.AddScoped<ICountriesService, CountriesService>();

            services.AddMemoryCache();
            services.AddControllers();
            services.AddSignalR();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://securetoken.google.com/sudokugameapp";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/sudokugameapp",
                        ValidateAudience = true,
                        ValidAudience = "sudokugameapp",
                        ValidateLifetime = true
                    };
                    options.AddQueryStringAuthentication();
                });

            services.AddDalDependencies(Configuration.GetConnectionString("ConnectionString"));
            services.AddAutoMapper();
            services.AddHttpClient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            if (env.IsProduction())
            {
                app.UseKissLogMiddleware(options => {
                    ConfigureKissLog(options);
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapControllers();
                endpoints.MapHub<MatchmakerHub>("/matchmaker");
                endpoints.MapHub<GameHub>("/game");
            });
        }

        private void ConfigureKissLog(IOptionsBuilder options)
        {
            // KissLog internal logs
            options.InternalLog = (message) =>
            {
                Debug.WriteLine(message);
            };

            // register logs output
            RegisterKissLogListeners(options);
        }

        private void RegisterKissLogListeners(IOptionsBuilder options)
        {
            // multiple listeners can be registered using options.Listeners.Add() method

            // register KissLog.net cloud listener
            options.Listeners.Add(new RequestLogsApiListener(new Application(
                Configuration["KissLog.OrganizationId"],
                Configuration["KissLog.ApplicationId"])
            )
            {
                ApiUrl = Configuration["KissLog.ApiUrl"]
            });
        }
    }
}
