using System.Web.Http;

namespace SmartcatPlugin.API
{
    public class RegisterRoutesBase
    {
        protected static void MapRouteWithSession(HttpConfiguration configuration, string routeName,
            string routePath, string controller, string action)
        {
            var routes = configuration.Routes;
            routes.MapHttpRoute(routeName, routePath, new
            {
                controller = controller,
                action = action
            });

            var route = System.Web.Routing.RouteTable.Routes[routeName] as System.Web.Routing.Route;
            route.RouteHandler = new SessionRouteHandler();
        }
    }
}