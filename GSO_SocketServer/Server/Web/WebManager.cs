using Server.Docker;
using Server.Web.Service;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClientCore;

namespace Server.Web
{

    public enum EWebServer
    {
        None,
        GameServerManager,
        Lobby,
    }

    public class WebServer : WebClientService
    {

        #region Resource
        private ServerManagerResource serverManagerResource = null;
        private LobbyResource lobbyResource = null;

        public ServerManagerResource ServerManager
        {
            get
            {
                return this.serverManagerResource;
            }
        }

        public LobbyResource Lobby
        {
            get
            {
                return this.lobbyResource;
            }
        }


        #endregion

        public WebServer()
        {

        }

        public void initializeServiceAndResource()
        {
            ConfigureServices();
            ConfigureResource();
        }

        public void ConfigureServices()
        {
            AddHttpClientUri(EWebServer.GameServerManager.ToString(), $"http://{DockerUtil.GetHostIP()}:7000");
            AddHttpClientUri(EWebServer.Lobby.ToString(), $"http://{DockerUtil.GetHostIP()}:5000");
        }

        public void ConfigureResource()
        {
            serverManagerResource = new ServerManagerResource(this, EWebServer.GameServerManager.ToString());
            lobbyResource = new LobbyResource(this, EWebServer.Lobby.ToString());
        }

    }
}
