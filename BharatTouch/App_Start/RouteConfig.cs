using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BharatTouch
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            //old working route before company change in url -- start

            //routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapMvcAttributeRoutes();

            //// Custom route for username/adhar
            //routes.MapRoute(
            //    name: "UserAdhar",
            //    url: "{code}/adhar",
            //    defaults: new { controller = "Users", action = "Adhar" }
            //);


            //routes.MapRoute(
            //       "Default", // Route name  
            //        "{controller}/{action}/{id}", // URL with parameters  
            //        new { controller = "Home", action = "index", id = UrlParameter.Optional } // Parameter defaults
            //   );

            // end old 

            //start with company
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
               name: "Sitemap",
               url: "sitemap.xml",
               defaults: new { controller = "Sitemap", action = "Index" }
           );

            routes.MapRoute(
                name: "BlogDetails",
                url: "blog/{slug}",
                defaults: new { controller = "Home", action = "BT_BlogDetail" }
            );

            // Route for adhar
            routes.MapRoute(
                name: "UserAdhar",
                url: "{code}/adhar",
                defaults: new { controller = "Users", action = "Adhar" }
            );


               routes.MapRoute(
                name: "CompanyUserAdhar",
                url: "{company}/{code}/adhar",
                defaults: new { controller = "Users", action = "Adhar" }
            );




            // Route: companyname/username
            routes.MapRoute(
                 name: "CompanyUserProfile",
                 url: "{company}/{code}",
                 defaults: new { controller = "Users", action = "Profile" },
                 constraints: new
                 {
                     company = @"^(?!InsertViewHistory|GenerateQRCode|GetScheduleOpenWeekDays|Home|Users|adhar|RefreshAccessTokenMicrosoftAsync|OAuth|FetchMoreThemeCards|CardTheme|Admin|UserIndex|company|login|edit|AddEditUser).+$",
                     code = @"^[^/]+$"
                 }
             );

            
            routes.MapRoute(
                  name: "UserEditProfile", 
                  url: "edit/{code}",      
                  defaults: new { controller = "Users", action = "EditProfile" }
              );

            routes.MapRoute(
                 name: "CompanyEditProfile",
                 url: "edit/{company}/{code}",
                 defaults: new { controller = "Users", action = "EditProfile" },
                 constraints: new
                 {
                     company = @"^(?!InsertViewHistory|GenerateQRCode|GetScheduleOpenWeekDays|Home|Users|adhar|RefreshAccessTokenMicrosoftAsync|OAuth|FetchMoreThemeCards|CardTheme|Admin|UserIndex|company|login|AddEditUser).+$",
                     code = @"^[^/]+$"
                 }
             );
           
            routes.MapRoute(
                name: "UserProfile",
                url: "{code}",
                defaults: new { controller = "Users", action = "Profile", company = UrlParameter.Optional },
                constraints: new
                {
                    code = @"^(?!InsertViewHistory|GenerateQRCode|GetScheduleOpenWeekDays|Home|Users|adhar|RefreshAccessTokenMicrosoftAsync|OAuth|FetchMoreThemeCards|CardTheme|Admin|UserIndex|company|login|AddEditUser).+$"
                }
            );

            routes.MapRoute(
                name: "UserProfilePreview",
                url: "preview_profile/{code}/{templateId}",
                defaults: new { controller = "Users", action = "ProfilePreview" }
            );

            routes.MapRoute(
                 name: "CompanyProfilePreview",
                 url: "preview_profile/{company}/{code}/{templateId}",
                 defaults: new { controller = "Users", action = "ProfilePreview" },
                 constraints: new
                 {
                     company = @"^(?!InsertViewHistory|GenerateQRCode|GetScheduleOpenWeekDays|Home|Users|adhar|RefreshAccessTokenMicrosoftAsync|OAuth|FetchMoreThemeCards|CardTheme|Admin|UserIndex|company|login|AddEditUser).+$",
                     code = @"^[^/]+$"
                 }
             );


             // Route: admin/addedituser
            routes.MapRoute(
                name: "UserProfileAdmin",
                url: "addedituser/{code}",
                defaults: new { controller = "Admin", action = "AddEditUser" }
            );

            routes.MapRoute(
                name: "CompanyaddEditUserAdmin",
                url: "addedituser/{company}/{code}",
                defaults: new { controller = "Admin", action = "AddEditUser" },
                constraints: new
                {
                    company = @"^(?!InsertViewHistory|GenerateQRCode|GetScheduleOpenWeekDays|Home|Users|adhar|RefreshAccessTokenMicrosoftAsync|OAuth|FetchMoreThemeCards|CardTheme|Admin|UserIndex|company|login|AddEditUser).+$",
                    code = @"^[^/]+$"
                }
            );


            // Default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //end new logic of company

           
        }

    }


    public class TrailingSlashRoute : Route
    {
        public TrailingSlashRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler) { }

        public TrailingSlashRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler) { }

        public TrailingSlashRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
                              IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler) { }

        public TrailingSlashRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints,
                              RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler) { }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData path = base.GetVirtualPath(requestContext, values);

            if (path != null)
                path.VirtualPath = path.VirtualPath + "/";
            return path;
        }
    }

    public static class RouteCollectionExtensions
    {
        public static void MapRouteTrailingSlash(this RouteCollection routes, string name, string url, object defaults)
        {
            routes.MapRouteTrailingSlash(name, url, defaults, null);
        }

        public static void MapRouteTrailingSlash(this RouteCollection routes, string name, string url, object defaults,
                                             object constraints)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");

            if (url == null)
                throw new ArgumentNullException("url");

            var route = new TrailingSlashRoute(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints)
            };

            if (String.IsNullOrEmpty(name))
                routes.Add(route);
            else
                routes.Add(name, route);
        }
    }
}
