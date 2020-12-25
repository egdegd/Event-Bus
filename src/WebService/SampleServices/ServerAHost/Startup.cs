using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using WebAPI.Core.Config;
using System.Web.Http;

[assembly: OwinStartup(typeof(WebAPI.SelfHost.Startup))]

namespace WebAPI.SelfHost
{
    public class Startup
    {
        public static HttpConfiguration HttpConfiguration { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration = new HttpConfiguration();

            WebApiConfig.Register(HttpConfiguration);

            app.UseWebApi(HttpConfiguration);
        }
    }
}
