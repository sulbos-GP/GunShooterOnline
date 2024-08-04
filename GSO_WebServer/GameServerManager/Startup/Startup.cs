
using GameServerManager.Repository;
using GameServerManager.Repository.Interfaces;
using GameServerManager.Servicies;
using GameServerManager.Servicies.Interfaces;
using GSO_WebServerLibrary.Config;
using GSO_WebServerLibrary.Error;

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
            // Add services to the http client
            services.AddHttpClient("GSO_Matchmaker", httpclient =>
            {
                httpclient.BaseAddress = new Uri("http://localhost:5200");
            });

            services.AddHttpClient("GSO_WebServer", httpclient =>
            {
                httpclient.BaseAddress = new Uri("http://localhost:6900");
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });

            // Add services to the config
            services.Configure<DatabaseConfig>(Configuration.GetSection(nameof(DatabaseConfig)));
            services.Configure<DockerConfig>(Configuration.GetSection(nameof(DockerConfig)));

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
            ISessionService session = app.ApplicationServices.GetRequiredService<ISessionService>();
            if (WebErrorCode.None != await session.InitMatch(1))
            {
                return;
            }

        }
    }
}
