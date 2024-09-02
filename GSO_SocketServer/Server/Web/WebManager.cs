using Server.Docker;
using Server.Web.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClientCore;

namespace Server.Web
{
    public class WebManager
    {
        private static WebManager instance;
        private WebClientService webService = new WebClientService();

        #region Resource
        private ServerManagerResource serverManagerResource = new ServerManagerResource();
        #endregion

        public static WebManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WebManager();
                }
                return instance;
            }
        }

        public WebClientService WebClient
        {
            get
            {
                return webService;
            }
        }

        public void ConfigureServices()
        {
            webService.AddHttpClientUri("GameServerManager", $"http://{DockerUtil.GetHostIP()}:7000");
        }

    }
}
