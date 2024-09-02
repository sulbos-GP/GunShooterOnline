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
        protected string host = "";

        public WebResource(string host) 
        { 
            this.host = host;
        }
    }
}
