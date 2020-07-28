using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace BattleshipsWeb
{
    public class Global : HttpApplication
    {
        void Application_Start(Object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}