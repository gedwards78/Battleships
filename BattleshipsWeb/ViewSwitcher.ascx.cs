using Microsoft.AspNet.FriendlyUrls.Resolvers;
using System;
using System.Web;
using System.Web.Routing;

namespace BattleshipsWeb
{
    public partial class ViewSwitcher : System.Web.UI.UserControl
    {
        protected String CurrentView { get; private set; }

        protected String AlternateView { get; private set; }

        protected String SwitchUrl { get; private set; }

        protected void Page_Load(Object sender, EventArgs e)
        {
            // Determine current view
            var isMobile = WebFormsFriendlyUrlResolver.IsMobileView(new HttpContextWrapper(this.Context));
            this.CurrentView = isMobile ? "Mobile" : "Desktop";

            // Determine alternate view
            this.AlternateView = isMobile ? "Desktop" : "Mobile";

            // Create switch URL from the route, e.g. ~/__FriendlyUrls_SwitchView/Mobile?ReturnUrl=/Page
            var switchViewRouteName = "AspNet.FriendlyUrls.SwitchView";
            var switchViewRoute = RouteTable.Routes[switchViewRouteName];
            if (switchViewRoute == null)
            {
                // Friendly URLs is not enabled or the name of the switch view route is out of sync
                this.Visible = false;
                return;
            }
            var url = this.GetRouteUrl(switchViewRouteName, new { view = this.AlternateView, __FriendlyUrls_SwitchViews = true });
            url += "?ReturnUrl=" + HttpUtility.UrlEncode(this.Request.RawUrl);
            this.SwitchUrl = url;
        }
    }
}