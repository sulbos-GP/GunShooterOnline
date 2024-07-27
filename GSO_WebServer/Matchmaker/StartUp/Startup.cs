using Matchmaker.Hubs;
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
                httpclient.BaseAddress = new Uri("http://localhost:5200");
            });

            services.AddHttpClient("GameServerManager", httpclient =>
            {
                httpclient.BaseAddress = new Uri("http://localhost:6900");
            });

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });

            services.AddSignalR();

            // Add services to the config
            //services.Configure<DbConfig>(Configuration.GetSection(nameof(DbConfig)));
            //services.Configure<GoogleConfig>(Configuration.GetSection(nameof(GoogleConfig)));

            // Add services to the container.
            //services.AddTransient<IMasterDB, MasterDB>();
            //services.AddTransient<IGameDB, GameDB>();
            //
            //services.AddTransient<IGoogleService, GoogleService>();
            //services.AddTransient<IAuthenticationService, AuthenticationService>();
            //services.AddTransient<IDataLoadService, DataLoadService>();
            //services.AddTransient<IGameService, GameService>();
            //
            //services.AddSingleton<IMemoryDB, MemoryDB>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                endpoints.MapHub<MatchmakerHub>("/NotificationHub");
            });

        }
    }
}
