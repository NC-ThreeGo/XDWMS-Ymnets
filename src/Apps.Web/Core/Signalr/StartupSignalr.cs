using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Apps.Web.Core.Signalr.StartupSignalr))]

namespace Apps.Web.Core.Signalr
{
    public class StartupSignalr
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
