
using GameServerManager.Repository;
using GameServerManager.Repository.Interfaces;
using GameServerManager.Servicies;
using GameServerManager.Servicies.Interfaces;
using WebCommonLibrary.Config;
using WebCommonLibrary.Enum;
using WebCommonLibrary.Error;

namespace GameServerManager.Startup
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
            // Add services to the config
            services.Configure<DatabaseConfig>(Configuration.GetSection(nameof(DatabaseConfig)));
            services.Configure<DockerConfig>(Configuration.GetSection(nameof(DockerConfig)));

            IConfigurationSection endPointConfig;
            endPointConfig = Configuration.GetSection(nameof(LocalConfig)).GetSection(nameof(EndPointConfig));
            services.Configure<EndPointConfig>(endPointConfig);

            // Add services to the http client
            EndPointConfig? endPoint = endPointConfig.Get<EndPointConfig>();
            if (endPoint == null)
            {
                return;
            }

            services.AddHttpClient("GsoWebServer", httpclient =>
            {
                httpclient.BaseAddress = new Uri(endPoint.Center);
            });

            services.AddHttpClient("Matchmaker", httpclient =>
            {
                httpclient.BaseAddress = new Uri(endPoint.Matchmaker);
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });

            // Add services to the container.
            services.AddTransient<ISessionService, SessionService>();

            services.AddSingleton<IDockerService, DockerService>();
            services.AddSingleton<ISessionMemory, SessionMemory>();

            services.AddHostedService<SessionBackgroundService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add middleware to the container.
            //app.UseMiddleware<GsoWebServer.Middleware.VersionCheck>();
            //app.UseMiddleware<GsoWebServer.Middleware.CheckUserAuth>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //초기화
            ISessionService session =app.ApplicationServices.GetRequiredService<ISessionService>();
            if (WebErrorCode.None != await session.InitMatch(1))
            {
                return;
            }

        }
    }
}
