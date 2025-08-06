using BharatTouch.CommonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DataAccess;

namespace BharatTouch.Controllers
{
    public class GlobalController : Controller
    {
        // GET: Global
        public ActionResult Index()
        {
            return View();
        }
    }

    public class AuthenticateUser : ActionFilterAttribute    {                public override void OnActionExecuting(ActionExecutingContext context)        {            base.OnActionExecuting(context);            if (!IsAthenticatedUser())            {                context.Result = new RedirectToRouteResult(                new RouteValueDictionary                {                        {"controller", "Home"},                        {"action", "Index"}                });                return;            }

        }        public bool IsAthenticatedUser()        {            if (!string.IsNullOrWhiteSpace(Utility.GetCookie("UserId_auth")))
                return true;            else                return false;        }    }        public class AuthenticateAdmin : ActionFilterAttribute    {                public override void OnActionExecuting(ActionExecutingContext context)        {            base.OnActionExecuting(context);            if (!IsAdmin())            {                context.Result = new RedirectToRouteResult(                new RouteValueDictionary                {                        {"controller", "Home"},                        {"action", "Index"}                });                return;            }

        }        public bool IsAdmin()        {            var userType = Utility.GetCookie("UserType_auth");            if (!string.IsNullOrWhiteSpace(userType))
            {
                if (userType.NullToString() == "AD")
                    return true;
                else
                    return false;
            }
            else
                return false;        }    }

    public class AuthenticateAdmin_Admin : ActionFilterAttribute    {
        public override void OnActionExecuting(ActionExecutingContext context)        {            base.OnActionExecuting(context);            if (!IsAdmin())            {                context.Result = new RedirectToRouteResult(                new RouteValueDictionary                {                        {"controller", "Admin"},                        {"action", "Login"}                });                return;            }

        }        public bool IsAdmin()        {            var userType = Utility.GetCookie("IsAdmin_admin");            if (!string.IsNullOrWhiteSpace(userType))
            {
                if (userType.NullToString() == "True")
                    return true;
                else
                    return false;
            }
            else
                return false;        }    }
}