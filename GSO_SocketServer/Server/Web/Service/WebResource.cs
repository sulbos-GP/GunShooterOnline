using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClientCore;

namespace Server.Web.Service
{
    public class WebResource
    {
        private readonly string host = "";
        private readonly WebServer owner;

        protected string Host
        {
            get
            {
                return host;
            }
        }

        protected WebServer Owner
        {
            get 
            { 
                return owner; 
            } 
        }

        public WebResource(WebServer owner, string host) 
        { 
            this.host = host;
            this.owner = owner;
        }
    }
}
