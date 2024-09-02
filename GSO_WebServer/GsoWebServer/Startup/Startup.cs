using GSO_WebServerLibrary.Reposiotry.Interfaces;
using GSO_WebServerLibrary.Reposiotry.Define.MasterDB;
using GSO_WebServerLibrary.Reposiotry.Define.GameDB;
using GSO_WebServerLibrary.Reposiotry.Define.MemoryDB;
using GSO_WebServerLibrary.Servicies.Google;
using GSO_WebServerLibrary.Servicies.Interfaces;
using GsoWebServer.Servicies.Interfaces;
using GsoWebServer.Servicies.Authentication;
using GsoWebServer.Servicies.DataLoad;
using GsoWebServer.Servicies.Game;
using System;
using WebCommonLibrary.Config;

namespace GsoWebServer.Startup
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

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DictionaryKeyPolicy = null;
            });

            // Add services to the config
            services.Configure<DatabaseConfig>(Configuration.GetSection(nameof(DatabaseConfig)));
            services.Configure<GoogleConfig>(Configuration.GetSection(nameof(GoogleConfig)));

            // Add services to the container.
            services.AddTransient<IMasterDB, MasterDB>();
            services.AddTransient<IGameDB, GameDB>();

            services.AddTransient<IGoogleService, GoogleService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IDataLoadService, DataLoadService>();
            services.AddTransient<IGameService, GameService>();

            services.AddSingleton<IMemoryDB, MemoryDB>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 마스터 DB가 등록이 안되어 있다면 없으면 반드시 에러
            //IMasterDB masterDB = app.ApplicationServices.GetRequiredService<IMasterDB>();
            //if(!await masterDB.LoadMasterData())
            //{
            //    return;
            //}

            // Add middleware to the container.
            //app.UseMiddleware<GsoWebServer.Middleware.VersionCheck>();
            app.UseMiddleware<GsoWebServer.Middleware.CheckUserAuth>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
