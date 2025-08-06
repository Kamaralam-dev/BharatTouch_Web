using BharatTouch.CommonHelper;
using DataAccess.Repository;
using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BharatTouch.Controllers
{
    public class SitemapController : Controller
    {
        // GET: Sitemap
        public ActionResult Index()
        {
            var webUrl = "https://bharattouch.com/";

            var staticUrls = new List<SitemapUrlViewModel>
            {
                new SitemapUrlViewModel { Loc = webUrl, Priority = "1.00" },
                new SitemapUrlViewModel { Loc = webUrl + "Home/Login", Priority = "0.80" },
                new SitemapUrlViewModel { Loc = webUrl + "signup", Priority = "0.80" },
                new SitemapUrlViewModel { Loc = webUrl + "BulkOrder", Priority = "0.80" },
                new SitemapUrlViewModel { Loc = webUrl + "Home/BT_TermsOfServices", Priority = "0.80" },
                new SitemapUrlViewModel { Loc = webUrl + "Home/BT_PrivacyPolicy", Priority = "0.80" },
                new SitemapUrlViewModel { Loc = webUrl + "Home/BT_RefundAndShipping", Priority = "0.80" },
                new SitemapUrlViewModel { Loc = webUrl + "Home/ShippingAndDelivery_Policy", Priority = "0.80" },
                new SitemapUrlViewModel { Loc = webUrl + "home/login", Priority = "0.64" }
            };

            var blogUrls = new AdminRepository().GetAllBT_Blogs_Admin().Select(b => new SitemapUrlViewModel
            {
                Loc = webUrl + "blog/" + BharatTouch.CommonHelper.Utility.GenerateSlug(b.BlogTitle),
                LastMod = b.CreatedOn ?? b.CreatedOn,
                Priority = "0.70"
            }).ToList();

            var allUrls = staticUrls.Concat(blogUrls).ToList();

            return new XmlResult(allUrls);
        }
    }
}