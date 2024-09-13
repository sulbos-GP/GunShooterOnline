using GSO_WebServerLibrary.Reposiotry.Define.GameDB;
using GSO_WebServerLibrary.Reposiotry.Define.MasterDB;
using GSO_WebServerLibrary.Reposiotry.Interfaces;
using Matchmaker.Hubs;
using Matchmaker.Repository;
using Matchmaker.Repository.Interface;
using Matchmaker.Service;
using Matchmaker.Service.Background;
using Matchmaker.Service.Interfaces;
using WebCommonLibrary.Config;
using WebCommonLibrary.Error;

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

            IConfigurationSection databaseConfig;
            IConfigurationSection endPointConfig;
#if AWS
            databaseConfig = Configuration.GetSection(nameof(AwsConfig)).GetSection(nameof(DatabaseConfig));
            endPointConfig = Configuration.GetSection(nameof(AwsConfig)).GetSection(nameof(EndPointConfig));
#else
            databaseConfig = Configuration.GetSection(nameof(LocalConfig)).GetSection(nameof(DatabaseConfig));
            endPointConfig = Configuration.GetSection(nameof(LocalConfig)).GetSection(nameof(EndPointConfig));
#endif
            // Add services to the config
            services.Configure<DatabaseConfig>(databaseConfig);
            services.Configure<EndPointConfig>(endPointConfig);

            // Add services to the http client
            EndPointConfig? endPoint = endPointConfig.Get<EndPointConfig>();
            if(endPoint == null)
            {
                return;
            }

            services.AddHttpClient("GsoWebServer", httpclient =>
            {
                httpclient.BaseAddress = new Uri(endPoint.Center);
            });

            services.AddHttpClient("GameServerManager", httpclient =>
            {
                httpclient.BaseAddress = new Uri(endPoint.GameServerManager);
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });

            services.AddSignalR();



            //services.Configure<GoogleConfig>(Configuration.GetSection(nameof(GoogleConfig)));

            // Add services to the container.
            services.AddSingleton<IMasterDB, MasterDB>();
            services.AddTransient<IGameDB, GameDB>();

            services.AddHostedService<MatchmakerBackgroundService>();

            services.AddTransient<IMatchmakerService, MatchmakerService>();
            services.AddTransient<IGameServerManagerService, GameServerManagerService>();

            services.AddSingleton<IMatchQueue, MatchQueue>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            IMatchmakerService service = app.ApplicationServices.GetRequiredService<IMatchmakerService>();
            await service.ClearMatch();

        }
    }
}
