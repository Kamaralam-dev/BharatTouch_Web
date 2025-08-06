using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BharatTouch
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            /*Use Single View Engine

            As you know in ASP.NET MVC, we can use multiple View Engines; it can be Web Forms[Aspx] orRazor[cshtml] or you can also create your own custom engine to process the ASP.NET MVC Views.We can also configure all engines in same application.

            You need to care two things before setting up the engine for the MVC application.Firstly, Engines list should be clear before setting up new engine and only one engine should be setting up.
            */

            // Clear all existing view engines
            ViewEngines.Engines.Clear();

            // Add only Razor view engine
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
