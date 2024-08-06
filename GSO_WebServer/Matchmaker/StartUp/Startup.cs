using GSO_WebServerLibrary.Config;
using GSO_WebServerLibrary.Reposiotry.Define.GameDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using Matchmaker.Hubs;
using Matchmaker.Repository;
using Matchmaker.Repository.Interface;
using Matchmaker.Service;
using Matchmaker.Service.Background;
using Matchmaker.Service.Interfaces;
using System;

namespace Matchmaker.Startup
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the http client
            services.AddHttpClient("GsoWebServer", httpclient =>
            {
                httpclient.BaseAddress = new Uri("http://localhost:5000");
            });

            services.AddHttpClient("GameServerManager", httpclient =>
            {
                httpclient.BaseAddress = new Uri("http://localhost:7000");
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });

            services.AddSignalR();

            // Add services to the config
            services.Configure<DatabaseConfig>(Configuration.GetSection(nameof(DatabaseConfig)));
            //services.Configure<GoogleConfig>(Configuration.GetSection(nameof(GoogleConfig)));

            // Add services to the container.
            //services.AddTransient<IMasterDB, MasterDB>();
            services.AddTransient<IGameDB, GameDB>();

            services.AddHostedService<MatchmakerBackgroundService>();

            services.AddTransient<IMatchmakerService, MatchmakerService>();
            services.AddTransient<IGameServerManagerService, GameServerManagerService>();

            services.AddSingleton<IMatchQueue, MatchQueue>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add middleware to the container.
            //app.UseMiddleware<GsoWebServer.Middleware.CheckUserAuth>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MatchmakerHub>("/MatchmakerHub");
            });

        }
    }
}
