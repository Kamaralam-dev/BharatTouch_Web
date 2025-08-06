using BharatTouch.CommonHelper;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using System.Configuration;

namespace BharatTouch.Controllers
{

    //[OutputCache(Duration = 300, VaryByParam = "code")]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            var countries = new DataAccess.Repository.CountryRepository().GetCountries("BharatTouch/Profile/Profile.cshtml")
              .Select(n => new SelectListItem
              {
                  Text = n.Country + " (" + n.NumberCode + ")",
                  Value = n.NumberCode + ";" + n.MinNumberLength + ";" + n.MaxNumberLength
              }).ToList();

            ViewBag.CountryCodes = countries;
            ViewBag.IsLoggedIn = Utility.GetCookie("UserId_auth").ToIntOrZero() != 0;
            var UserUrlCode = Utility.GetCookie("UserUrlCode").NullToString();
            var DisplayName = Utility.GetCookie("DisplayName_auth").NullToString();

            var codeOrName = UserUrlCode;
            if (!string.IsNullOrWhiteSpace(DisplayName))
            {
                codeOrName = DisplayName;
            }
            ViewBag.CodeOrName = codeOrName;
            return View();
        }

        //[OutputCache(Duration = 300, VaryByParam = "none")]
        public ActionResult LandingPage(string code)
        {

            ViewBag.Code = code;

            var countries = new DataAccess.Repository.CountryRepository().GetCountries("BharatTouch/Profile/Profile.cshtml")
                          .Select(n => new SelectListItem
                          {
                              Text = n.Country + " (" + n.NumberCode + ")",
                              Value = n.NumberCode + ";" + n.MinNumberLength + ";" + n.MaxNumberLength
                          }).ToList();

            ViewBag.CountryCodes = countries;

            List<UserPackageViewModel> packages = new DataAccess.Repository.PackagesRepository().GetAllPackages();




            return View(packages);
        }

        public ActionResult SmartSignup(string code)
        {
            ViewBag.Code = code;

            var countries = new DataAccess.Repository.CountryRepository().GetCountries("BharatTouch/Profile/Profile.cshtml")
                          .Select(n => new SelectListItem
                          {
                              Text = n.Country + " (" + n.NumberCode + ")",
                              Value = n.NumberCode + ";" + n.MinNumberLength + ";" + n.MaxNumberLength
                          }).ToList();

            ViewBag.CountryCodes = countries;

            List<UserPackageViewModel> packages = new DataAccess.Repository.PackagesRepository().GetAllPackages();




            return View(packages);
        }

        
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Pricing()
        {
            return View();
        }

        public ActionResult LoginModel()
        {
            return PartialView("_login");
        }

        public ActionResult SignupModel(int? PackageId)
        {
            var modal = new UserModel();
            //if (PackageId.HasValue)
            //{
            //    modal.PackageId = PackageId.Value;
            //}
            return PartialView("_signup", modal);
        }
        public ActionResult ForgotPassword()
        {
            return PartialView("_forgotPassword");
        }
        public ActionResult ChangeImageModel()
        {
            return PartialView("_changeImage");
        }

        public ActionResult Logout()
        {
            Utility.RemoveCookie("UserId_auth");
            Utility.RemoveCookie("UserName_auth");
            Utility.RemoveCookie("EmailId_auth");
            Utility.RemoveCookie("UserUrlCode");
            Utility.RemoveCookie("UserType_auth");
            Utility.RemoveCookie("DisplayName_auth");
            return RedirectToAction("index");
        }

        //[Route("User/Profile/{code}")]
        public ActionResult VirtualBusinessCard()
        {
            return View();
        }

        public ActionResult BT_TermsOfServices()
        {
            return View();
        }

        public ActionResult BT_FAQ()
        {
            return View();
        }

        public ActionResult BT_PrivacyPolicy()
        {
            return View();
        }

        public ActionResult BT_RefundAndShipping()
        {
            return View();
        }


        public ActionResult ShippingAndDelivery_Policy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult InsertNewsLetter(NewsLetterModel model)
        {
            try
            {
                new UserRepository().InsertNewsLetter(model, "BharatTouch/Home/InsertNewsLetter");
                return new ActionState { Success = true, Message = "Done!", Data = "News letter submitted successfully.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [Route("Blog")]
        public ActionResult BT_Blog()
        {
            var data = new AdminRepository().GetAllBT_Blogs_Admin();
            return View(data);
        }


        //[Route("Blog/{id}")]
        public ActionResult BT_BlogDetail(string slug)
        {
            var model = new BT_BlogViewModel();
            model = new AdminRepository().GetBT_BlogsBySlug(slug);

            var imagePath = ConfigValues.ImagePath.Substring(1) + "/BT_Blog/" + model.BlogId + ".png";
            var path = Server.MapPath(imagePath);
            if (System.IO.File.Exists(path))
            {
                model.BlogImage = imagePath;
            }
            return View(model);
        }

    }
}