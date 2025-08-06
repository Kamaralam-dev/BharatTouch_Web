using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BharatTouch.Controllers
{
    public class PdfViewerController : Controller
    {

        public ActionResult Index(string pdfpath)
        {
            ViewBag.pdfpath = pdfpath;
            return View();
        }
    }
}