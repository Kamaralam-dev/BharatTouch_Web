using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BharatTouch.Controllers
{
    public class BlogController : Controller
    {
        // GET: Blog
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VirtualCard()
        {
            return View();
        }

        public ActionResult VirtualCardBenefits()
        {
            return View();
        }
        public ActionResult SwitchOverToVirtualBusinessCards()
        {
            return View();
        }
    }
}