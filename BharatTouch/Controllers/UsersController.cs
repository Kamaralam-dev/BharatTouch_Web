using QRCoder;
using DataAccess.Models;
using DataAccess.Repository;
using BharatTouch.CommonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using DataAccess.ApiHelper;
using System.IO;
using DataAccess.ViewModels;
using System.Configuration;
using System.Drawing;
using static QRCoder.PayloadGenerator;
using System.Drawing.Imaging;
using System.Web.UI;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using SautinSoft.Document;
using System.Text.RegularExpressions;
using Razorpay.Api;
using BharatTouch.CommonHelper;
using System.Web.Hosting;
using System.Diagnostics;
using System.Net.Mail;
using BharatTouch.JwtTokens;
using System.ComponentModel.Design;
using Microsoft.Extensions.Logging;
using GroopGo.Api.WebHelper;
using Hangfire;
using Microsoft.Office.Interop.Word;

namespace BharatTouch.Controllers
{
    //[OutputCache(Duration = 300, VaryByParam = "code")]
    public class UsersController : Controller
    {
        #region Declaration
        UserRepository _userRepo = new UserRepository();
        ProfessionalRepository _professionalRepo = new ProfessionalRepository();
        EducationRepository _educationRepo = new EducationRepository();
        BlogRepository _blogRepository = new BlogRepository();
        SocialRepository _socialMediaRepo = new SocialRepository();
        YouTubeRepository _youTubeRepository = new YouTubeRepository();
        ScheduleAndMeetingRepository _scheduleRepository = new ScheduleAndMeetingRepository();
        CommonRepository _commonRepo = new CommonRepository();
        CLientTestimonialRepository _clientTestimonialRepo = new CLientTestimonialRepository();
        CertificationRepository _certificationRepo = new CertificationRepository();
        PackagesRepository _packageRepo = new PackagesRepository();
        ProfileTemplateRepository _profileTemplateRepo = new ProfileTemplateRepository();
        OrderRepository _orderRepo = new OrderRepository();
        PaymentRepository _paymentRepo = new PaymentRepository();
        NfcCardRepository _nfcRepo = new NfcCardRepository();
        string recaptchaSecretKey = ConfigValues.GoogleCaptchaSecretKey;
        string webUrl = ConfigValues.WebUrl;
        private readonly RazorpayClient _razorpayClient = new RazorpayClient(ConfigValues.RazorPayApiKey, ConfigValues.RazorPayApiSecret);
        #endregion

        #region ManageUsers   
        public ActionResult Index()
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //MailMessage mail = new MailMessage();
            //mail.From = new MailAddress("noreply@bharattouch.com");
            //mail.Subject = "This is a test message";
            //mail.Body = "If you can see this mail, your SMTP service is working";
            //mail.To.Add("kamaralamcp@gmail.com");
            //SmtpClient smtp = new SmtpClient("mdin-pp-wb3.webhostbox.net");

            //NetworkCredential credential = new NetworkCredential("noreply@bharattouch.com", "Nav0z7?71");
            //smtp.Credentials = credential;
            //smtp.Port = 25;
            //smtp.Send(mail);
            //string outMessage = "";
            //Utility.SendEmail("kamaralamcp@gmail.com", "test", "test", out outMessage, "");
            return View();
        }

        public ActionResult DeleteUserAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteUserAccount(string UserId)
        {
            try
            {
                var isSuccess = _userRepo.AviPdfUserDeleteByUserId(UserId);
                return new ActionState { Success = isSuccess, Message = isSuccess ? "User deleted successfully." : "UserId not matched", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult VerificationAviPdfUser(string email)
        {
            var isVerified = _userRepo.AviPdfUserEmailVerification(email);
            return View(isVerified);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult PersonalInfo(UserModel model, string BirthDate1, HttpPostedFileBase PortfolioLink, HttpPostedFileBase ServicesLink, HttpPostedFileBase ResumeLink, HttpPostedFileBase MenuLink)
        {

            if (ModelState.IsValid)
            {
                var codeOrNameString = model.UrlCode.NullToString() == "" ? model.Displayname : model.UrlCode;
                try
                {
                    model.BirthDate = DateTimeFormatter.ConvertToDateTime(BirthDate1);
                    var docDirectoryPath = "/uploads/pdffiles/" + model.UserId;
                    if (PortfolioLink != null && PortfolioLink.ContentLength > 0)
                    {
                        string portfoliofileName;
                        portfoliofileName = PortfolioLink.FileName;
                        var newFileName = "Porfolio_" + codeOrNameString + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(portfoliofileName);

                        string fileExtension = Path.GetExtension(portfoliofileName).ToLower();
                        if (fileExtension != ".pdf")
                        {
                            return new ActionState { Message = "Failed!", Data = "Portfolio- Only pdf files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                        }

                        var folderPath = Server.MapPath("~" + docDirectoryPath);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        var dbPath = Path.Combine(folderPath, newFileName);
                        PortfolioLink.SaveAs(dbPath);
                        model.PortfolioLink = docDirectoryPath + "/" + newFileName;
                    }

                    if (ServicesLink != null && ServicesLink.ContentLength > 0)
                    {
                        string servicefileName;
                        servicefileName = ServicesLink.FileName;
                        string fileExtension = Path.GetExtension(servicefileName).ToLower();
                        if (fileExtension != ".pdf")
                        {
                            return new ActionState { Message = "Failed!", Data = "Services- Only pdf files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                        }
                        var newFileName = "Service_" + codeOrNameString + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(servicefileName);
                        var folderPath = Server.MapPath("~" + docDirectoryPath);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        var dbPath = Path.Combine(folderPath, newFileName);
                        ServicesLink.SaveAs(dbPath);
                        model.ServicesLink = docDirectoryPath + "/" + newFileName;
                    }

                    if (ResumeLink != null && ResumeLink.ContentLength > 0)
                    {
                        string resumefileName;
                        resumefileName = ResumeLink.FileName;
                        string fileExtension = Path.GetExtension(resumefileName).ToLower();
                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension != ".pdf" || fileExtension != ".doc" || fileExtension != ".docx")
                        {
                            var newFileName = "Resume_" + codeOrNameString + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(resumefileName);
                            var folderPath = Server.MapPath("~" + docDirectoryPath);
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);

                            var dbPath = Path.Combine(folderPath, newFileName);
                            ResumeLink.SaveAs(dbPath);
                            model.ResumeLink = docDirectoryPath + "/" + newFileName;
                        }
                        else
                        {
                            return new ActionState { Message = "Failed!", Data = "Resume- Only (pdf, jpg, jpeg, png, doc, docx) files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                        }

                    }

                    if (MenuLink != null && MenuLink.ContentLength > 0)
                    {
                        string menufileName;
                        menufileName = MenuLink.FileName;
                        string fileExtension = Path.GetExtension(menufileName).ToLower();
                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension != ".pdf" || fileExtension != ".doc" || fileExtension != ".docx")
                        {
                            var newFileName = "Menu_" + codeOrNameString + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(menufileName);
                            var folderPath = Server.MapPath("~" + docDirectoryPath);
                            if (!Directory.Exists(folderPath))
                                Directory.CreateDirectory(folderPath);

                            var dbPath = Path.Combine(folderPath, newFileName);
                            MenuLink.SaveAs(dbPath);
                            model.MenuLink = docDirectoryPath + "/" + newFileName;
                        }
                        else
                        {
                            return new ActionState { Message = "Failed!", Data = "Menu- Only (pdf, jpg, jpeg, png, doc, docx) files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                        }
                    }
                    int OutFlag;
                    _userRepo.UpdatePersonalInfo(model, out OutFlag, "BharatTouch/EditProfile/PersonalInfo");
                    if (OutFlag == 1)
                    {
                        return new ActionState { Message = "Done!", Data = "Personal info updated successfully!", Success = true, OptionalValue = model.UrlCode, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (OutFlag == 9)
                    {
                        return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new ActionState { Message = "Failed!", Data = "User not found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            else
            {
                var errorMessage = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));

                //return new ActionState { Message = "Failed!", Data = "Invalid Form!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                return new ActionState { Message = "Failed!", Data = errorMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult AdhaarUpdate(UserModel model, HttpPostedFileBase frontImg, HttpPostedFileBase BackImg)
        {
            try
            {
                model.UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                string segment = DateTime.Now.ToString("yyyyMMdd");
                if (frontImg != null && frontImg.ContentLength > 0)
                {
                    var uploadres = Utility.SaveCompressImages(frontImg, "Adhaar_Front_" + model.UserId, ConfigValues.ImagePath.Substring(1) + "/adhaar/" + model.UserId, 300);
                    if (uploadres == "invalid")
                    {
                        return new ActionState { Success = false, Message = "Failed!", Data = "Only jpg,jpeg,png files allowed.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (uploadres == "0")
                    {
                        return new ActionState { Success = false, Message = "Failed!", Data = "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    model.AdhaarFrontImgPath = uploadres;
                }
                if (BackImg != null && BackImg.ContentLength > 0)
                {
                    var uploadres = Utility.SaveCompressImages(BackImg, "Adhaar_Back_" + model.UserId, ConfigValues.ImagePath.Substring(1) + "/adhaar/" + model.UserId, 300);
                    if (uploadres == "invalid")
                    {
                        return new ActionState { Success = false, Message = "Failed!", Data = "Only jpg,jpeg,png files allowed.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (uploadres == "0")
                    {
                        return new ActionState { Success = false, Message = "Failed!", Data = "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    model.AdhaarBackImgPath = uploadres;
                }

                var OutFlag = _userRepo.AdhaarUpdate(model, "BharatTouch/EditProfile/AdhaarUpdate");
                if (OutFlag == 1)
                {
                    return new ActionState { Message = "Done!", Data = "Adhaar updated successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else if (OutFlag == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = "User not found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult ContactInfo(UserModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int OutFlag;
                    _userRepo.UpdateContactInfo(model, out OutFlag, "BharatTouch/EditProfile/ContactInfo");
                    if (OutFlag == 1)
                    {
                        return new ActionState { Message = "Done!", Data = "Contact info updated successfully!", Success = true, OptionalValue = "", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (OutFlag == 9)
                    {
                        return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new ActionState { Message = "Done!", Data = "Contact info inserted successfully!", Success = true, OptionalValue = "", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                    }
                }
                catch (Exception ex)
                {
                    return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            else
            {
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors);
                return new ActionState { Message = "Failed!", Data = "Invalid Form!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult AboutInfo(UserModel model)
        {
            try
            {
                int OutFlag;
                _userRepo.UpdateAboutInfo(model, out OutFlag, "BharatTouch/EditProfile/ContactInfo");
                if (OutFlag == 1)
                {
                    return new ActionState { Message = "Done!", Data = "About info updated sucessfully!", Success = true, OptionalValue = "", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else if (OutFlag == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Done!", Data = "About info inserted successfully!", Success = true, OptionalValue = "", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }

                //ContactData generator = new ContactData(ContactData.ContactOutputType.VCard3, model.FirstName, model.LastName, null, model.Phone,null,model.EmailId,model.BirthDate?.ToString(),null,null,null,model.City,model.Zip,model.Country,null,model.StateName);
                //string payload = generator.ToString();

                //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                //QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                //QRCode qrCode = new QRCode(qrCodeData);
                //Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20);

                //using (MemoryStream memory = new MemoryStream())
                //{
                //    using (FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + @"/Uploads/QRCodes/contact_QR_"+model.UrlCode+".png", FileMode.Create, FileAccess.ReadWrite))
                //    {
                //        qrCodeAsBitmap.Save(memory, ImageFormat.Png);
                //        byte[] bytes = memory.ToArray();
                //        fs.Write(bytes, 0, bytes.Length);
                //    }
                //}

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult SocialInfo(string LinkedIn, string Twitter, string Facebook, string Instagram, string Skype, string Youtube, string Teams, string Snapchat)
        {
            try
            {
                int userId = Utility.GetCookie("UserId_auth").ToIntOrZero();

                _socialMediaRepo.DeleteSocialMedia(userId, "BharatTouch/EditProfile/SocialInfo");

                if (!string.IsNullOrWhiteSpace(LinkedIn))
                    _socialMediaRepo.UpsertUserSocial(userId, "LinkedIn", LinkedIn, "BharatTouch/EditProfile/SocialInfo");

                if (!string.IsNullOrWhiteSpace(Twitter))
                    _socialMediaRepo.UpsertUserSocial(userId, "Twitter", Twitter, "BharatTouch/EditProfile/SocialInfo");

                if (!string.IsNullOrWhiteSpace(Facebook))
                    _socialMediaRepo.UpsertUserSocial(userId, "Facebook", Facebook, "BharatTouch/EditProfile/SocialInfo");

                if (!string.IsNullOrWhiteSpace(Instagram))
                    _socialMediaRepo.UpsertUserSocial(userId, "Instagram", Instagram, "BharatTouch/EditProfile/SocialInfo");

                if (!string.IsNullOrWhiteSpace(Skype))
                    _socialMediaRepo.UpsertUserSocial(userId, "Skype", Skype, "BharatTouch/EditProfile/SocialInfo");

                if (!string.IsNullOrWhiteSpace(Youtube))
                    _socialMediaRepo.UpsertUserSocial(userId, "Youtube", Youtube, "BharatTouch/EditProfile/SocialInfo");

                if (!string.IsNullOrWhiteSpace(Teams))
                    _socialMediaRepo.UpsertUserSocial(userId, "Teams", Teams, "BharatTouch/EditProfile/SocialInfo");

                if (!string.IsNullOrWhiteSpace(Snapchat))
                    _socialMediaRepo.UpsertUserSocial(userId, "Snapchat", Snapchat, "BharatTouch/EditProfile/SocialInfo");

                return new ActionState { Message = "Done!", Data = "Social media info updated successfully!", Success = true, OptionalValue = "", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [AuthenticateUser]
        [HttpPost]
        //[OutputCache(Duration = 60, VaryByParam = "none", Location = OutputCacheLocation.Client)]
        public ActionResult GetUsers()
        {
            int totRows = 0;
            var users = _userRepo.GetAllUsers(Utility.StartIndex(), Utility.PageSize(), Utility.SortBy(), Utility.SortDesc(), Utility.FilterText(), out totRows, "BharatTouch/Index/GetUsers");

            return Json(new { recordsFiltered = totRows, recordsTotal = totRows, data = users }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Create(int? id)
        {
            if (id == null)
                return View(new UserModel());


            UserModel user = new UserModel();
            user = _userRepo.GetUserById(id.Value, "BharatTouch/SignupModal/Create");
            if (user == null)
                return View(new UserModel());

            return View(user);

        }

        //[Route("User/EditProfile/{code}")]
        // [Route("edit/{company}/{code}")]
        [AuthenticateUser]
        [OutputCache(Duration = 20, VaryByParam = "code", Location = OutputCacheLocation.Client)]
        public ActionResult EditProfile(string code, string company = null)
        {
            var businessTypeParentList = _commonRepo.GetBusinessTypeParentList("BharatTouch/EditProfile/EditProfile");
            var defaultItem = new BusinessTypeModel { BusinessTypeId = 0, BusinessType = "Select Company Category" };
            businessTypeParentList.Insert(0, defaultItem);

            //var package = _packageRepo.GetPackageByCodeOrName(code, "BharatTouch/EditProfile/EditProfile");
            //ViewBag.package = package;

            if (code == null)
            {
                ViewBag.BusinessTypeParentList = new SelectList(businessTypeParentList, "BusinessTypeId", "BusinessType");
                return View(new UserModel());
            }

            var fullCode = "";
            if (company != null && company != "")
            {
                fullCode = company + "/" + code;
            }
            else
            {
                fullCode = code;
            }

            UserModel user = new UserModel();
            user = _userRepo.GetUserByCodeOrName(fullCode, "BharatTouch/EditProfile/EditProfile");

            var userId = Utility.GetCookie("UserId_auth").ToIntOrZero();
            if (user == null || user.UserId != userId)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.UserId = user.UserId;
            ViewBag.BusinessTypeParentList = new SelectList(businessTypeParentList, "BusinessTypeId", "BusinessType", user.CompanyTypeParentId);
            ViewBag.IsNotCompanyUser = user.CompanyId == 0;
            return View(user);

        }


        //[Route("{code}")]
        public ActionResult Profile(string code, string company = null)
        {
            var reservedWords = new[] { "company", "login", "InsertViewHistory", "GenerateQRCode", "GetScheduleOpenWeekDays", "Home", "Users", "OAuth", "RefreshAccessTokenMicrosoftAsync", "FetchMoreThemeCards", "CardTheme", "UserIndex", "Admin", "AddEditUser" };

            if (reservedWords.Contains(code, StringComparer.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(company) && reservedWords.Contains(company, StringComparer.OrdinalIgnoreCase)))
            {
                return HttpNotFound(); // or redirect to default controller/action
            }

            int totalRows = 0;
            var linkedIn = "";
            var Twitter = "";
            var Facebook = "";
            var Instagram = "";
            var Skype = "";
            var Youtube = "";
            var Teams = "";
            var Snapchat = "";
            var fullCode = "";

            var countries = new DataAccess.Repository.CountryRepository().GetCountries("Bharattouch/Profile/Profile.cshtml").Select(n => new SelectListItem
            {
                Text = n.Country + " (" + n.NumberCode + ")",
                Value = n.NumberCode + ";" + n.MinNumberLength + ";" + n.MaxNumberLength
            }).ToList();

            if (company != null && company != "")
            {
                fullCode = company + "/" + code;
                ViewBag.ProfileCode = Url.RequestContext.RouteData.Values["company"].NullToString() + "/" + Url.RequestContext.RouteData.Values["code"].NullToString();
            }
            else
            {
                fullCode = code;
                ViewBag.ProfileCode = Url.RequestContext.RouteData.Values["code"].NullToString();
            }

            ViewBag.WebUrl = ConfigurationManager.AppSettings["WebUrl"].ToString();
            ViewBag.ActionName = ControllerContext.RouteData.Values["action"].ToString();
            int loggedUserId = Utility.GetCookie("UserId_auth").ToIntOrZero();


            var profilePermissionData = new UserRepository().GetProfilePermissionDetails(fullCode);

            var profileDetail = profilePermissionData.ProfileDetails;

            if (profileDetail == null)
                return RedirectToAction("Index", "Home", new { area = "" });

            #region GeneralFlags

            ViewBag.HasBlogs = profilePermissionData.SummaryCounts.TotalBlogCount > 0;
            ViewBag.ShowCertifications = !profileDetail.HideCertification && profilePermissionData.SummaryCounts.TotalCertificationCount > 0;
            ViewBag.IsEducationListAvailable = profilePermissionData.SummaryCounts.TotalEducationCount > 0;
            ViewBag.TotalLeads = profilePermissionData.SummaryCounts.TotalLeadCount;
            ViewBag.IsExperienceAvailable = profilePermissionData.SummaryCounts.TotalProfessionalCount > 0;
            ViewBag.HasTeam = profilePermissionData.SummaryCounts.TotalTeamCount > 0;
            ViewBag.IsYoutubeListAvailable = profilePermissionData.SummaryCounts.TotalYouTubeVideo > 0;

            #endregion

            #region SocialMedia

            var socialDetails = profilePermissionData.SocialMedia;
            if (socialDetails != null && socialDetails.Count > 0)
            {
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "LinkedIn") != null)
                {
                    linkedIn = socialDetails.FirstOrDefault(x => x.SocialMedia == "LinkedIn").Url.NullToString();
                    ViewBag.LinkedIn = linkedIn;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Twitter") != null)
                {
                    Twitter = socialDetails.FirstOrDefault(x => x.SocialMedia == "Twitter").Url.NullToString();
                    ViewBag.Twitter = Twitter;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Facebook") != null)
                {
                    Facebook = socialDetails.FirstOrDefault(x => x.SocialMedia == "Facebook").Url.NullToString();
                    ViewBag.Facebook = Facebook;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Instagram") != null)
                {
                    Instagram = socialDetails.FirstOrDefault(x => x.SocialMedia == "Instagram").Url.NullToString();
                    ViewBag.Instagram = Instagram;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Skype") != null)
                {
                    Skype = socialDetails.FirstOrDefault(x => x.SocialMedia == "Skype").Url.NullToString();
                    ViewBag.Skype = Skype;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Youtube") != null)
                {
                    Youtube = socialDetails.FirstOrDefault(x => x.SocialMedia == "Youtube").Url.NullToString();
                    ViewBag.Youtube = Youtube;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Teams") != null)
                {
                    Teams = socialDetails.FirstOrDefault(x => x.SocialMedia == "Teams").Url.NullToString();
                    ViewBag.Teams = Teams;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Snapchat") != null)
                {
                    Snapchat = socialDetails.FirstOrDefault(x => x.SocialMedia == "Snapchat").Url.NullToString();
                    ViewBag.Snapchat = Snapchat;
                }
            }

            #endregion

            #region UPIDetails

            var upiDetails = profilePermissionData.UpiDetails;
            bool isPaymentQrAvailable = false;
            if (upiDetails != null)
            {
                isPaymentQrAvailable = System.IO.File.Exists(Server.MapPath("~" + upiDetails.QrImage));
                ViewBag.PayeeName = upiDetails.PayeeName;
                ViewBag.QrImage = upiDetails.QrImage;
                ViewBag.UPIID = upiDetails.UPIId;
            }

            ViewBag.IsPaymentQrAvailable = isPaymentQrAvailable;

            #endregion

            #region OpenWeekDays

            var OpenWeekDaysDetails = profilePermissionData.OpenWeekDays;
            bool isAnyOpenWeekDay = false;
            if (OpenWeekDaysDetails != null)
            {
                isAnyOpenWeekDay = (OpenWeekDaysDetails.Sun || OpenWeekDaysDetails.Mon || OpenWeekDaysDetails.Tue || OpenWeekDaysDetails.Wed || OpenWeekDaysDetails.Thu || OpenWeekDaysDetails.Fri || OpenWeekDaysDetails.Sat);
            }
            ViewBag.IsAnyWeekDayOpen = isAnyOpenWeekDay;

            #endregion

            #region TokenDetails

            var userTokenDetail = profilePermissionData.OauthTokens;
            if (userTokenDetail != null)
            {
                ViewBag.GoogleAccessToken = userTokenDetail.GoogleAccessToken != null ? userTokenDetail.GoogleAccessToken : string.Empty;
                ViewBag.MicrosoftAccessToken = userTokenDetail.MicrosoftAccessToken != null ? userTokenDetail.MicrosoftAccessToken : string.Empty;
            }

            #endregion

            #region ViewBags

            ViewBag.UserType = profileDetail.UserType.NullToString();
            ViewBag.HideGallery = profileDetail.HideGallery;
            ViewBag.HideTestimonial = profileDetail.HideTestimonial;
            ViewBag.ShowEducation = profileDetail.ShowEducation;
            ViewBag.ShowExperience = profileDetail.ShowExperience;
            ViewBag.HideYouTube = profileDetail.HideYouTube;
            ViewBag.HideBlog = profileDetail.HideBlog;
            ViewBag.HideTeam = profileDetail.HideTeam;
            ViewBag.CountryId = profileDetail.CountryId;
            ViewBag.CountryList = countries;

            ViewBag.IsProfilePictureExist = false;
            if (Utility.IsExistFile(profileDetail.ProfileImage.NullToString()))
            {
                ViewBag.ProfileImage = profileDetail.ProfileImage.NullToString();
                ViewBag.IsProfilePictureExist = true;
                ViewBag.WebLinkHttps = profileDetail.ProfileImage.NullToString();
            }
            else
            {
                ViewBag.ProfileImage = "/FormAssets/img/blank-profile-picture.jpg"; //"https://Bharattouch.com" +
                ViewBag.WebLinkHttps = "/SmartTheme/images/bharattouchfavico.ico";
            }

            ViewBag.FirstName = profileDetail.FirstName;
            ViewBag.LastName = profileDetail.LastName;
            ViewBag.FullName = profileDetail.FirstName + " " + profileDetail.LastName.NullToString();
            ViewBag.Title = profileDetail.FirstName + " " + profileDetail.LastName.NullToString();
            ViewBag.CompanyType = profileDetail.CompanyType;
            ViewBag.LoggedUserId = loggedUserId;
            ViewBag.IsProfileOwner = loggedUserId == profileDetail.UserID;
            ViewBag.ProfileOwnerId = profileDetail.UserID;
            ViewBag.ReferalCode = profileDetail.ReferalCode;
            ViewBag.ShowAbout = profileDetail.ShowAbout;
            ViewBag.ShowSkill = profileDetail.ShowSkill;
            ViewBag.IsSkillsNotAvailable = profileDetail.SkillName1.NullToString() == "" && profileDetail.SkillName2.NullToString() == "" && profileDetail.SkillName3.NullToString() == "" && profileDetail.SkillName4.NullToString() == "" && profileDetail.SkillName5.NullToString() == "" && profileDetail.SkillName6.NullToString() == "";

            DirectoryInfo directoryInfo = new DirectoryInfo(Server.MapPath("~/uploads/Portfolio/" + profileDetail.UserID));
            FileInfo[] Files = null;
            bool hasGalleryImages = false;
            try
            {
                Files = directoryInfo.GetFiles("*"); //Getting Text files
                hasGalleryImages = Files != null && Files.Length > 0;
            }
            catch (Exception ex)
            {
                hasGalleryImages = false;
            }

            var companyId = profileDetail.CompanyId;
            DirectoryInfo c = new DirectoryInfo(Server.MapPath("~/uploads/portfolio/company/" + companyId));
            FileInfo[] comFiles = null;
            bool hasComGalleryImages = false;
            try
            {
                comFiles = c.GetFiles("*");
                hasComGalleryImages = comFiles != null && comFiles.Length > 0;
            }
            catch (Exception ex)
            {
                hasComGalleryImages = false;
            }


            ViewBag.HasGalleryImages = companyId > 0 ? hasComGalleryImages : hasGalleryImages;
            ViewBag.AboutDescription = profileDetail.AboutDescription;
            ViewBag.ShowTestimonial = profilePermissionData.SummaryCounts.TotalClientTestimonialCount > 0;
            ViewBag.LoopClientTestimonial = profilePermissionData.SummaryCounts.TotalClientTestimonialCount > 2 ? "true" : "";
            ViewBag.DecodedDescriptionOG = ViewBag.DecodedDescriptionMeta = profileDetail.AboutDescription != null ? "" : HttpUtility.HtmlDecode(profileDetail.AboutDescription);
            ViewBag.CaptchaSiteKey = ConfigValues.GoogleCaptchaSiteKey;
            ViewBag.IPAddress = Request.UserHostAddress;
            ViewBag.BrowserName = Request.Browser.Browser;
            ViewBag.BrowserVersion = Request.Browser.Version;
            ViewBag.IsMobileDevice = Request.Browser.IsMobileDevice;
            ViewBag.BrowserPlatform = Request.Browser.Platform;
            ViewBag.BestFitCoverImageStyleClass = profileDetail.BestFitCoverImage.NullToString() == "" ? "contain" : profileDetail.BestFitCoverImage.NullToString();
            var whatsappWithCountryCode = profileDetail.WhatsappNumberCode.NullToString() + profileDetail.Whatsapp.NullToString();
            ViewBag.WhatsappWithCountryCode = whatsappWithCountryCode;
            ViewBag.WhatsappForLink = Regex.Replace(whatsappWithCountryCode, @"[^0-9]", "");
            var phoneNumberWithCountryCode = profileDetail.NumberCode.NullToString() + profileDetail.Phone.NullToString();
            ViewBag.PhoneNumberWithCountryCode = phoneNumberWithCountryCode;

            string modifiedWebsiteUrl = profileDetail.Website;
            if (modifiedWebsiteUrl.NullToString() != "" && !modifiedWebsiteUrl.StartsWith("http"))
            {
                ViewBag.ModifiedWebsiteUrl = "http://" + modifiedWebsiteUrl;
            }
            else
            {
                ViewBag.ModifiedWebsiteUrl = modifiedWebsiteUrl;
            }

            ViewBag.IsPortfolioFileExists = string.IsNullOrWhiteSpace(profileDetail.PortfolioLink) == false && System.IO.File.Exists(Server.MapPath(profileDetail.PortfolioLink));
            ViewBag.IsResumeFileExists = string.IsNullOrWhiteSpace(profileDetail.ResumeLink) == false && System.IO.File.Exists(Server.MapPath(profileDetail.ResumeLink));
            ViewBag.IsServicesFileExists = string.IsNullOrWhiteSpace(profileDetail.ServicesLink) == false && System.IO.File.Exists(Server.MapPath(profileDetail.ServicesLink));
            ViewBag.IsMenuLinkExists = string.IsNullOrWhiteSpace(profileDetail.MenuLink) == false && System.IO.File.Exists(Server.MapPath(profileDetail.MenuLink));

            // Used in Classic touch template
            ViewBag.NoAboutContent = profileDetail.AboutDescription.NullToString() == "" &&
             (string.IsNullOrWhiteSpace(profileDetail.PortfolioLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.PortfolioLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.ResumeLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.ResumeLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.ServicesLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.ServicesLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.MenuLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.MenuLink))) &&
             string.IsNullOrWhiteSpace(profileDetail.Phone) &&
             string.IsNullOrWhiteSpace(linkedIn) &&
             string.IsNullOrWhiteSpace(Twitter) &&
             string.IsNullOrWhiteSpace(Facebook) &&
             string.IsNullOrWhiteSpace(Instagram) &&
             string.IsNullOrWhiteSpace(Skype);

            // Used in dark and bold touch template
            ViewBag.NoAboutContentWithoutIcons = profileDetail.AboutDescription.NullToString() == "" &&
             (string.IsNullOrWhiteSpace(profileDetail.PortfolioLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.PortfolioLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.ResumeLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.ResumeLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.ServicesLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.ServicesLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.MenuLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.MenuLink)));

            #endregion

            //default template
            var profileTemplate = "Profile";



            // multi template 
            //if (profileDetail.SelectedTemplateName != null && code.ToLower() != "kamar.alam")
            //    profileTemplate = profileDetail.SelectedTemplateName.Replace(".cshtml", "");
            //else
            //profileTemplate = "UltraPerformanceProfile";

            if (profileDetail.SelectedTemplateName != null)
                profileTemplate = profileDetail.SelectedTemplateName.Replace(".cshtml", "");

            ViewBag.IsAboutSectionWithIcons = profileTemplate == "Profile";

            return View(profileTemplate, profileDetail);
            //return View("Profile", profileDetail);

        }

        //[Route("preview_profile/{code}/{templateId}")]
        //public ActionResult ProfilePreview(string code, int templateId)
        //{
        //    var profileTemplate = "Profile";
        //    ProfileDetail profile = new ProfileDetail();
        //    profile = _userRepo.GetProfileDetail(code, "BharatTouch/Profile/Profile");
        //    if (profile == null)
        //        return RedirectToAction("Index", "Home", new { area = "" });

        //    var package = _packageRepo.GetPackageByUserId(profile.UserID, "BharatTouch/Profile/Profile");
        //    ViewBag.package = package;
        //    ViewBag.AboutDescription = profile.AboutDescription;

        //    var template = _profileTemplateRepo.GetTemplateById(templateId, "BharatTouch/Preview_Profile/ProfilePreview");

        //    // multi template 
        //    if (template != null)
        //        profileTemplate = template.TemplateFile.Replace(".cshtml", "");

        //    return View(profileTemplate, profile);

        //}




        // [Route("preview_profile/{code}/{templateId}")]
        public ActionResult ProfilePreview(string code, int templateId, string company = null)
        {
            var linkedIn = "";
            var Twitter = "";
            var Facebook = "";
            var Instagram = "";
            var Skype = "";
            var Youtube = "";
            var Teams = "";
            var Snapchat = "";
            var fullCode = "";

            //  ViewBag.ProfileCode = Url.RequestContext.RouteData.Values["code"].NullToString();//codeorname
            ViewBag.WebUrl = ConfigurationManager.AppSettings["WebUrl"].ToString();
            ViewBag.ActionName = ControllerContext.RouteData.Values["action"].ToString();
            int loggedUserId = Utility.GetCookie("UserId_auth").ToIntOrZero();

            var countries = new DataAccess.Repository.CountryRepository().GetCountries("Bharattouch/Profile/Profile.cshtml").Select(n => new SelectListItem
            {
                Text = n.Country + " (" + n.NumberCode + ")",
                Value = n.NumberCode + ";" + n.MinNumberLength + ";" + n.MaxNumberLength
            }).ToList();

            if (company != null && company != "")
            {
                fullCode = company + "/" + code;
                ViewBag.ProfileCode = Url.RequestContext.RouteData.Values["company"].NullToString() + "/" + Url.RequestContext.RouteData.Values["code"].NullToString();
            }
            else
            {
                fullCode = code;
                ViewBag.ProfileCode = Url.RequestContext.RouteData.Values["code"].NullToString();
            }

            var profilePermissionData = new UserRepository().GetProfilePermissionDetails(fullCode);

            var profileDetail = profilePermissionData.ProfileDetails;

            #region GeneralFlags

            ViewBag.HasBlogs = profilePermissionData.SummaryCounts.TotalBlogCount > 0;
            ViewBag.ShowCertifications = !profileDetail.HideCertification && profilePermissionData.SummaryCounts.TotalCertificationCount > 0;
            ViewBag.IsEducationListAvailable = profilePermissionData.SummaryCounts.TotalEducationCount > 0;
            ViewBag.TotalLeads = profilePermissionData.SummaryCounts.TotalLeadCount;
            ViewBag.IsExperienceAvailable = profilePermissionData.SummaryCounts.TotalProfessionalCount > 0;
            ViewBag.HasTeam = profilePermissionData.SummaryCounts.TotalTeamCount > 0;
            ViewBag.IsYoutubeListAvailable = profilePermissionData.SummaryCounts.TotalYouTubeVideo > 0;

            #endregion

            #region SocialMedia

            var socialDetails = profilePermissionData.SocialMedia;
            if (socialDetails != null && socialDetails.Count > 0)
            {
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "LinkedIn") != null)
                {
                    linkedIn = socialDetails.FirstOrDefault(x => x.SocialMedia == "LinkedIn").Url.NullToString();
                    ViewBag.LinkedIn = linkedIn;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Twitter") != null)
                {
                    Twitter = socialDetails.FirstOrDefault(x => x.SocialMedia == "Twitter").Url.NullToString();
                    ViewBag.Twitter = Twitter;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Facebook") != null)
                {
                    Facebook = socialDetails.FirstOrDefault(x => x.SocialMedia == "Facebook").Url.NullToString();
                    ViewBag.Facebook = Facebook;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Instagram") != null)
                {
                    Instagram = socialDetails.FirstOrDefault(x => x.SocialMedia == "Instagram").Url.NullToString();
                    ViewBag.Instagram = Instagram;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Skype") != null)
                {
                    Skype = socialDetails.FirstOrDefault(x => x.SocialMedia == "Skype").Url.NullToString();
                    ViewBag.Skype = Skype;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Youtube") != null)
                {
                    Youtube = socialDetails.FirstOrDefault(x => x.SocialMedia == "Youtube").Url.NullToString();
                    ViewBag.Youtube = Youtube;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Teams") != null)
                {
                    Teams = socialDetails.FirstOrDefault(x => x.SocialMedia == "Teams").Url.NullToString();
                    ViewBag.Teams = Teams;
                }
                if (socialDetails.FirstOrDefault(x => x.SocialMedia == "Snapchat") != null)
                {
                    Snapchat = socialDetails.FirstOrDefault(x => x.SocialMedia == "Snapchat").Url.NullToString();
                    ViewBag.Snapchat = Snapchat;
                }
            }

            #endregion

            #region UPIDetails

            var upiDetails = profilePermissionData.UpiDetails;
            bool isPaymentQrAvailable = false;
            if (upiDetails != null)
            {
                isPaymentQrAvailable = System.IO.File.Exists(Server.MapPath("~" + upiDetails.QrImage));
                ViewBag.PayeeName = upiDetails.PayeeName;
                ViewBag.QrImage = upiDetails.QrImage;
                ViewBag.UPIID = upiDetails.UPIId;
            }

            ViewBag.IsPaymentQrAvailable = isPaymentQrAvailable;

            #endregion

            #region OpenWeekDays

            var OpenWeekDaysDetails = profilePermissionData.OpenWeekDays;
            bool isAnyOpenWeekDay = false;
            if (OpenWeekDaysDetails != null)
            {
                isAnyOpenWeekDay = (OpenWeekDaysDetails.Sun || OpenWeekDaysDetails.Mon || OpenWeekDaysDetails.Tue || OpenWeekDaysDetails.Wed || OpenWeekDaysDetails.Thu || OpenWeekDaysDetails.Fri || OpenWeekDaysDetails.Sat);
            }
            ViewBag.IsAnyWeekDayOpen = isAnyOpenWeekDay;

            #endregion

            #region TokenDetails

            var userTokenDetail = profilePermissionData.OauthTokens;
            if (userTokenDetail != null)
            {
                ViewBag.GoogleAccessToken = userTokenDetail.GoogleAccessToken != null ? userTokenDetail.GoogleAccessToken : string.Empty;
                ViewBag.MicrosoftAccessToken = userTokenDetail.MicrosoftAccessToken != null ? userTokenDetail.MicrosoftAccessToken : string.Empty;
            }

            #endregion

            #region ViewBags

            ViewBag.UserType = profileDetail.UserType.NullToString();
            ViewBag.HideGallery = profileDetail.HideGallery;
            ViewBag.HideTestimonial = profileDetail.HideTestimonial;
            ViewBag.ShowEducation = profileDetail.ShowEducation;
            ViewBag.ShowExperience = profileDetail.ShowExperience;
            ViewBag.HideYouTube = profileDetail.HideYouTube;
            ViewBag.HideBlog = profileDetail.HideBlog;
            ViewBag.HideTeam = profileDetail.HideTeam;
            ViewBag.CountryId = profileDetail.CountryId;
            ViewBag.CountryList = countries;

            ViewBag.IsProfilePictureExist = false;
            if (Utility.IsExistFile(profileDetail.ProfileImage.NullToString()))
            {
                ViewBag.ProfileImage = profileDetail.ProfileImage.NullToString();
                ViewBag.IsProfilePictureExist = true;
                ViewBag.WebLinkHttps = profileDetail.ProfileImage.NullToString();
            }
            else
            {
                ViewBag.ProfileImage = "/FormAssets/img/blank-profile-picture.jpg"; //"https://Bharattouch.com" +
                ViewBag.WebLinkHttps = "/SmartTheme/images/bharattouchfavico.ico";
            }

            ViewBag.FirstName = profileDetail.FirstName;
            ViewBag.LastName = profileDetail.LastName;
            ViewBag.FullName = profileDetail.FirstName + " " + profileDetail.LastName.NullToString();
            ViewBag.Title = profileDetail.FirstName + " " + profileDetail.LastName.NullToString();
            ViewBag.CompanyType = profileDetail.CompanyType;
            ViewBag.LoggedUserId = loggedUserId;
            ViewBag.IsProfileOwner = loggedUserId == profileDetail.UserID;
            ViewBag.ProfileOwnerId = profileDetail.UserID;
            ViewBag.ReferalCode = profileDetail.ReferalCode;
            ViewBag.ShowAbout = profileDetail.ShowAbout;
            ViewBag.ShowSkill = profileDetail.ShowSkill;
            ViewBag.IsSkillsNotAvailable = profileDetail.SkillName1.NullToString() == "" && profileDetail.SkillName2.NullToString() == "" && profileDetail.SkillName3.NullToString() == "" && profileDetail.SkillName4.NullToString() == "" && profileDetail.SkillName5.NullToString() == "" && profileDetail.SkillName6.NullToString() == "";

            DirectoryInfo directoryInfo = new DirectoryInfo(Server.MapPath("~/uploads/Portfolio/" + profileDetail.UserID));
            FileInfo[] Files = null;
            bool hasGalleryImages = false;
            try
            {
                Files = directoryInfo.GetFiles("*"); //Getting Text files
                hasGalleryImages = Files != null && Files.Length > 0;
            }
            catch (Exception ex)
            {
                hasGalleryImages = false;
            }


            var companyId = profileDetail.CompanyId;
            DirectoryInfo c = new DirectoryInfo(Server.MapPath("~/uploads/portfolio/company/" + companyId));
            FileInfo[] comFiles = null;
            bool hasComGalleryImages = false;
            try
            {
                comFiles = c.GetFiles("*");
                hasComGalleryImages = comFiles != null && comFiles.Length > 0;
            }
            catch (Exception ex)
            {
                hasComGalleryImages = false;
            }


            ViewBag.HasGalleryImages = companyId > 0 ? hasComGalleryImages : hasGalleryImages;
            ViewBag.AboutDescription = profileDetail.AboutDescription;
            ViewBag.ShowTestimonial = profilePermissionData.SummaryCounts.TotalClientTestimonialCount > 0;
            ViewBag.LoopClientTestimonial = profilePermissionData.SummaryCounts.TotalClientTestimonialCount > 2 ? "true" : "";
            ViewBag.DecodedDescriptionOG = ViewBag.DecodedDescriptionMeta = profileDetail.AboutDescription != null ? "" : HttpUtility.HtmlDecode(profileDetail.AboutDescription);
            ViewBag.CaptchaSiteKey = ConfigValues.GoogleCaptchaSiteKey;
            ViewBag.IPAddress = Request.UserHostAddress;
            ViewBag.BrowserName = Request.Browser.Browser;
            ViewBag.BrowserVersion = Request.Browser.Version;
            ViewBag.IsMobileDevice = Request.Browser.IsMobileDevice;
            ViewBag.BrowserPlatform = Request.Browser.Platform;
            ViewBag.BestFitCoverImageStyleClass = profileDetail.BestFitCoverImage.NullToString() == "" ? "contain" : profileDetail.BestFitCoverImage.NullToString();
            var whatsappWithCountryCode = profileDetail.WhatsappNumberCode.NullToString() + profileDetail.Whatsapp.NullToString();
            ViewBag.WhatsappWithCountryCode = whatsappWithCountryCode;
            ViewBag.WhatsappForLink = Regex.Replace(whatsappWithCountryCode, @"[^0-9]", "");

            string modifiedWebsiteUrl = profileDetail.Website;
            if (modifiedWebsiteUrl.NullToString() != "" && !modifiedWebsiteUrl.StartsWith("http"))
            {
                ViewBag.ModifiedWebsiteUrl = "http://" + modifiedWebsiteUrl;
            }
            else
            {
                ViewBag.ModifiedWebsiteUrl = modifiedWebsiteUrl;
            }
            ViewBag.IsPortfolioFileExists = string.IsNullOrWhiteSpace(profileDetail.PortfolioLink) == false && System.IO.File.Exists(Server.MapPath(profileDetail.PortfolioLink));
            ViewBag.IsResumeFileExists = string.IsNullOrWhiteSpace(profileDetail.ResumeLink) == false && System.IO.File.Exists(Server.MapPath(profileDetail.ResumeLink));
            ViewBag.IsServicesFileExists = string.IsNullOrWhiteSpace(profileDetail.ServicesLink) == false && System.IO.File.Exists(Server.MapPath(profileDetail.ServicesLink));
            ViewBag.IsMenuLinkExists = string.IsNullOrWhiteSpace(profileDetail.MenuLink) == false && System.IO.File.Exists(Server.MapPath(profileDetail.MenuLink));

            // Used in Classic touch template
            ViewBag.NoAboutContent = profileDetail.AboutDescription.NullToString() == "" &&
             (string.IsNullOrWhiteSpace(profileDetail.PortfolioLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.PortfolioLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.ResumeLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.ResumeLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.ServicesLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.ServicesLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.MenuLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.MenuLink))) &&
             string.IsNullOrWhiteSpace(profileDetail.Phone) &&
             string.IsNullOrWhiteSpace(linkedIn) &&
             string.IsNullOrWhiteSpace(Twitter) &&
             string.IsNullOrWhiteSpace(Facebook) &&
             string.IsNullOrWhiteSpace(Instagram) &&
             string.IsNullOrWhiteSpace(Skype);

            // Used in dark and bold touch template
            ViewBag.NoAboutContentWithoutIcons = profileDetail.AboutDescription.NullToString() == "" &&
             (string.IsNullOrWhiteSpace(profileDetail.PortfolioLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.PortfolioLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.ResumeLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.ResumeLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.ServicesLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.ServicesLink))) &&
             (string.IsNullOrWhiteSpace(profileDetail.MenuLink) || !System.IO.File.Exists(Server.MapPath(profileDetail.MenuLink)));

            #endregion

            //default template
            var profileTemplate = "Profile";

            if (profileDetail == null)
                return RedirectToAction("Index", "Home", new { area = "" });

            // multi template 

            var template = _profileTemplateRepo.GetTemplateById(templateId, "BharatTouch/Preview_Profile/ProfilePreview");

            // multi template 
            if (template != null)
                profileTemplate = template.TemplateFile.Replace(".cshtml", "");

            ViewBag.IsAboutSectionWithIcons = profileTemplate == "Profile";

            return View(profileTemplate, profileDetail);

        }

        [HttpPost]
        public async Task<ActionResult> Create(UserModel model)
        {
            //return new ActionState { Message = "Verification email sent to your email address please verify to activate the account.", Data = new { UserId = 4394, OrderId = 2226 }, Success = true, OptionalValue = 2243.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            //int loggedUser = Utility.GetCookie("UserId_auth").ToIntOrZero();
            int newUserId = 0;
            int newOrderId = 0;
            if (ModelState.IsValid)
            {

                try
                {
                    var recaptchaResponse = Request.Form["grecaptchaResponse"];
                    bool iscaptchaValid = await IsReCaptchaValid(recaptchaResponse);
                    if (!iscaptchaValid)
                    {
                        return new ActionState { Message = "Captcha failed.", Data = null, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                    if (model.UserType == null || model.UserType.NullToString() == "")
                    {
                        model.UserType = "IN";
                    }

                    UserModel user = new UserModel();
                    if (_userRepo.UpsertUser_V2(model, out newUserId, out newOrderId, "BharatTouch/SignupModal/Create") == 1)
                    {
                        return new ActionState { Message = "Email already exists!", Data = null, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                    if (model.UserId == 0)
                    {

                        //var actionMailResult = VerificationEmailAsync(model.EmailId, model.FirstName + " " + model.LastName.NullToString(), model.Password, model.Displayname, "Signup");
                        //var stopwatch = new Stopwatch();
                        //stopwatch.Start();
                        //QueueVerificationEmailAsync("alam@rnaura.com", "kamar alam", "123456", "Kamar alam", "SignupQue");
                        //stopwatch.Stop();
                        //var timeTakenAsync= "Async method took:" +stopwatch.ElapsedMilliseconds +"ms";
                        //stopwatch.Restart();
                        //var mailResultTest = VerificationEmail("alam@rnaura.com", "kamar alam", "123456", "Kamar alam", "Signup");
                        //stopwatch.Stop();
                        //var timeTaken = "sync method took:" + stopwatch.ElapsedMilliseconds + "ms";

                        //return new ActionState { Message = "Verification email sent to your email address please verify to activate the account.", Data = new { UserId = newUserId, OrderId = newOrderId }, Success = true, OptionalValue = newUserId.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                        //var mailResult = VerificationEmail(model.EmailId, model.FirstName + " " + model.LastName.NullToString(), model.Password, model.Displayname, "Signup");

                        try
                        {
                          var res=  await QueueVerificationEmailAsync(model.EmailId, model.FirstName + " " + model.LastName.NullToString(), model.Password, model.Displayname, "Signup");
                        }
                        catch (Exception ex)
                        {
                        }

                       
                        try
                        {
                         var a= await SignUpEmailToAdmin(model.FirstName + " " + model.LastName.NullToString(), model.EmailId, model.UserType, model.Displayname, newUserId, "Signup Modal Home");
                        }
                        catch (Exception ex)
                        {
                        }


                        //Send Notification to admin on device
                        var notificationUsers = new AdminRepository().GetAllDeviceToken();

                        var notifyTitle = "";
                        var notifyMessage = "";

                        //FirebaseToken is device token
                        List<string> fcmTokens = new List<string>();
                        foreach (var not in notificationUsers)
                        {
                            if (!string.IsNullOrWhiteSpace(not.FirebaseToken))
                            {
                                fcmTokens.Add(not.FirebaseToken);
                                notifyTitle = "BharatTouch - New Signup.";
                                notifyMessage = $"New signup: {model.FirstName + " " + model.LastName.NullToString()} ({model.EmailId}) has joined BharatTouch.";

                            }
                        }

                        if (fcmTokens.Count != 0)
                        {
                            var notificationResponse = PushNotification.Send(fcmTokens, notifyTitle, notifyMessage);
                        }

                        return new ActionState { Message = "A verification email will be sent to your email address. Please verify it to activate your account.", Data = new { UserId = newUserId, OrderId = newOrderId }, Success = true, OptionalValue = newUserId.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

                    }

                    return new ActionState { Message = "Profile detail updated successfully.", Data = new { UserId = newUserId, OrderId = newOrderId, urlCode = model.UrlCode.ToString() }, Success = true, OptionalValue = model.UrlCode.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return new ActionState { Message = ex.Message, Data = ex, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

            }
            else
            {
                var errorMessage = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                //var errorMessage = ModelState.Values.SelectMany(v => v.Errors);
                return new ActionState { Message = "Failed!", Data = "Invalid Form!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }



        [Route("starter.instructions")]
        public ActionResult StarterInstructions()
        {
            return View();
        }

        public ActionResult CheckValidReferalCode(string ReferredByCode)
        {
            try
            {
                var model = _userRepo.CheckValidReferalCode(ReferredByCode, "BharatTouch/Home/CheckValidReferalCode");
                if (model == null)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = "Referral code is invalid", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Success = true, Message = "Done!", Data = "Valid Error code", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenNfcCardCartWizard(WizardViewModel w, string actionName = "")
        {
            try
            {
                //var modal = _orderRepo.GetOrderByUserId(w.UserId, actionName);
                //if (modal == null)
                //{
                //    return PartialView("_nfcCardCartWizard", w);
                //}
                //w.PackageId = modal.PackageId;
                return PartialView("_nfcCardCartWizard", w);
            }
            catch (Exception ex)
            {
                return PartialView("_nfcCardCartWizard", w);
            }
        }

        [HttpPost]
        public ActionResult UpsertOrder(OrderModel model, string actionName = "")
        {
            try
            {
                _orderRepo.UpsertOrder_v2(model, actionName);
                var user = _userRepo.GetUserById(model.UserId);
                //var emailResult = NfcCardPrintDetailsEmail(model.UserId);
                return new ActionState { Success = true, Message = "Done!", Data = "Order Placed successfully.", OptionalValue = model.UserId.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenOrderModal(int UserId, string actionName = "")
        {
            try
            {
                var modal = _orderRepo.GetOrderByUserId(UserId, actionName);
                if (modal == null)
                {
                    modal = new OrderViewModel();
                }
                return PartialView("_orderUpsertFormModal", modal);
            }
            catch (Exception ex)
            {
                return PartialView("_orderUpsertFormModal", new OrderViewModel());
            }
        }

        public ActionResult OpenSelectNfcCardModal(int UserId, string actionName = "")
        {
            try
            {
                var modal = _nfcRepo.GetNfcCardColorByUserId(UserId, actionName);
                if (modal == null)
                {
                    modal = new NfcCardColorViewModel();
                    modal.UserId = UserId;
                }
                return PartialView("_NfcCardSelectForm", modal);
            }
            catch (Exception ex)
            {
                return PartialView("_NfcCardSelectForm", new NfcCardColorViewModel());
            }
        }

        [HttpPost]
        public ActionResult selectNfcCardColor(NfcCardColorViewModel model, string actionName = "")
        {
            try
            {
                int outFlag = 0;
                model.CardFinishId = 1;         // need to update this
                var result = _nfcRepo.UpdateSelectedNfcCardColor_V2(model, out outFlag, actionName);
                if (!result)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = (outFlag == 9 ? "Server error, please try again later." : "Some error occurred."), Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState { Success = true, Message = "Done!", Data = "Color updated successfully.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenNfcCardDetialsFormModal(int UserId, string actionName = "")
        {
            try
            {
                var modal = _nfcRepo.GetNfcCardColorByUserId(UserId, actionName);
                if (modal == null)
                {
                    modal = new NfcCardColorViewModel();
                    modal.UserId = UserId;
                }
                return PartialView("_NfcCardDetailForm", modal);
            }
            catch (Exception ex)
            {
                return PartialView("_NfcCardDetailForm", new NfcCardColorViewModel());
            }
        }

        [Route("NFC/Preview/{OrderId}")]
        public ActionResult NfcCardPreview(int OrderId)
        {
            try
            {
                var modal = _nfcRepo.GetNfcCardPreviewDetailsByOrderId(OrderId, "BharatTouch/NfcPreviewPage/NfcCardPreview");

                return View(modal);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult UpdateUserNfcDetails(NfcCardColorViewModel model, string actionName = "")
        {
            try
            {
                int outFlag = 0;
                var result = _nfcRepo.UpdateUsersNfcCardDetails_v2(model, out outFlag, actionName);
                if (!result)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = (outFlag == 9 ? "Server error, please try again later." : "Some error occurred."), Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState { Success = true, Message = "Done!", Data = "Nfc card details updated successfully.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenOrderPaymentFormModal(int UserId, string actionName = "")
        {
            try
            {
                var modal = _orderRepo.GetOrderByUserId(UserId, actionName);
                if (modal == null)
                {
                    modal = new OrderViewModel();
                }
                return PartialView("_orderPaymentForm", modal);
            }
            catch (Exception ex)
            {
                return PartialView("_orderPaymentForm", new OrderViewModel());
            }
        }

        [HttpPost]
        public ActionResult CheckDiscountCouponValid(string Code)
        {
            try
            {
                var modal = _paymentRepo.CheckValidDiscountCoupon(Code);
                if (modal == null)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = "Coupon is Invalid or Expired.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Success = true, Message = "Done!", Data = modal, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult AddOrderPayment(PaymentModel model, Boolean WantMetalCard, Boolean fromLogin = false, string actionName = "")
        {
            try
            {
                if (Request.Files.Count == 0)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = "Please provide payment screenshot.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                if (Request.Files[0].ContentLength == 0)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = "Please provide payment screenshot.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                var file = Request.Files[0];
                string folderPath = Path.Combine(Server.MapPath(ConfigValues.ImagePath), "Paymentscreenshot").ToLower();
                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (string.IsNullOrWhiteSpace(fileExtension))
                {
                    char[] splitImg = { '/' };
                    string[] getExtention = file.ContentType.Split(splitImg);
                    fileExtension = "." + getExtention[1];
                }
                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                {

                    model.TransactionId = null;
                    int InvoiceId = 0;
                    var result = _paymentRepo.UpsertOrderPayment(model, WantMetalCard, out InvoiceId, actionName);
                    if (result == 0)
                    {
                        return new ActionState { Success = false, Message = "Failed!", Data = "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    string path = Path.Combine(folderPath, result + ".jpg");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    file.SaveAs(path);

                    if (fromLogin)
                    {
                        var user = _userRepo.GetUserById(model.UserId);
                        if (user == null)
                        {
                            return new ActionState { Success = false, Message = "Failed!", Data = "Your screenshot is Submitted, some error occurred, please login again.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                        }
                        Utility.SetCookie("UserId_auth", user.UserId.ToString());
                        Utility.SetCookie("UserName_auth", user.FirstName.NullToString() + " " + user.LastName.NullToString());
                        Utility.SetCookie("EmailId_auth", user.EmailId);
                        Utility.SetCookie("UserUrlCode", user.UrlCode.NullToString());
                        Utility.SetCookie("UserType_auth", user.UserType.NullToString());
                        Utility.SetCookie("DisplayName_auth", user.Displayname.NullToString());
                        var codeOrName = user.UrlCode;
                        if (!string.IsNullOrWhiteSpace(user.Displayname))
                        {
                            codeOrName = user.Displayname.NullToString();
                        }
                        return new ActionState { Success = true, Message = "Done!", Data = "Order Placed successfully.", OptionalValue = codeOrName, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                    }

                    return new ActionState { Success = true, Message = "Done!", Data = "Order Placed successfully.", OptionalValue = result.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Success = false, Message = "Failed!", Data = "Only jpg, jpeg, and png file format allowed.", OptionalValue = model.UserId.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        [HttpPost]
        public ActionResult UpdateUserType(UserModel model)
        {
            try
            {
                var result = _userRepo.UpdateUserType(model, "BharatTouch/EditProfile/UpdateUserType");
                if (result == 9)
                    return new ActionState { Success = false, Message = "Failed!", Data = "Server error.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                else if (result == 0)
                    return new ActionState { Success = false, Message = "Failed!", Data = "User not found.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                else
                    return new ActionState { Success = true, Message = "Done!", Data = "Operation successfull.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _userRepo.DeleteUser(id, "BharatTouch/Index/Delete");
                return new ActionState { Message = "Done!", Data = "User deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }


        [HttpPost]
        public ActionResult UpsertUserProfileAndCoverImages(UserModel user, HttpPostedFileBase profileImageFile, HttpPostedFileBase coverImageFile)
        {
            try
            {
                if (profileImageFile != null && profileImageFile.ContentLength > 0)
                {
                    var res = Utility.SaveCompressImages(profileImageFile, user.UserId.NullToString(), ConfigValues.ImagePath.Substring(1) + "/profile");
                    if (res == "invalid")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only jpg, jpeg, png files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (res == "0")
                    {
                        return new ActionState { Message = "Failed!", Data = "Some error occurred while uploading profile image", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                    user.ProfileImage = res;
                }

                if (coverImageFile != null && coverImageFile.ContentLength > 0)
                {

                    var res = Utility.SaveCompressImages(coverImageFile, user.UserId.NullToString(), ConfigValues.ImagePath.Substring(1) + "/coverImage");
                    if (res == "invalid")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only jpg, jpeg, png files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (res == "0")
                    {
                        return new ActionState { Message = "Failed!", Data = "Some error occurred while uploading cover image", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                    user.CoverImage = res;
                }

                var result = _userRepo.UpsertUserProfileAndCoverImage(user, "BharatTouch/EditProfile/RemoveCoverImage");
                if (result)
                {
                    return new ActionState { Message = "Done!", Data = "Cover image updated successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Message = "Failed!", Data = "Some error occurred", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenCoverImagePropertyPreviewModal(int UserId)
        {
            var user = _userRepo.GetUserById(UserId, "BharatTouch/EditProfile/OpenCoverImagePropertyPreviewModal");
            if (user == null)
            {
                user = new UserModel() { UserId = UserId };
            }

            return PartialView("_CoverImageStyleModal", user);
        }

        [HttpPost]
        public ActionResult UpdateCoverImageProperty(UserModel user)
        {
            try
            {
                var result = _userRepo.UpdateCoverImageProperty(user, "BharatTouch/EditProfile/UpdateCoverImageProperty");
                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occured!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Message = "Done!", Data = "Cover image best-fit successfully updated.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult RemoveCoverImage(int UserId, string path)
        {
            try
            {
                var result = _userRepo.RemoveUserCoverImage(UserId, "BharatTouch/EditProfile/RemoveCoverImage");
                if (result)
                {
                    Utility.RemoveFile(path);
                }
                return new ActionState { Message = "Done!", Data = "Cover image removed successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }
        #endregion

        #region Authentication  

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Authenticate(UserModel model)
        {
            try
            {
                var user = _userRepo.AuthenticateUser(model, "BharatTouch/loginModal/authenticate");
                if (user != null)
                {
                    //if(user.CheckUserPaymentStatus == 2)
                    //{
                    //    return new ActionState { Message = "Info!", Data = "Your payment is pending, please complete your payment.", Success = false, Type = ActionState.InfoType, OptionalValue = user.UserId + ";" + user.CheckUserPaymentStatus.ToString() }.ToActionResult(HttpStatusCode.OK);
                    //}

                    //if(user.CheckUserPaymentStatus == 1)
                    //{
                    //    return new ActionState { Message = "Info!", Data = "Please fill your shipping address and do payments.", Success = false, Type = ActionState.InfoType, OptionalValue = user.UserId + ";" + user.CheckUserPaymentStatus.ToString() }.ToActionResult(HttpStatusCode.OK);
                    //}

                    Utility.SetCookie("UserId_auth", user.UserId.ToString());
                    Utility.SetCookie("UserName_auth", user.FirstName.NullToString() + " " + user.LastName.NullToString());
                    Utility.SetCookie("EmailId_auth", user.EmailId);
                    Utility.SetCookie("UserUrlCode", user.UrlCode.NullToString());
                    Utility.SetCookie("UserType_auth", user.UserType.NullToString());
                    Utility.SetCookie("DisplayName_auth", user.Displayname.NullToString());
                    Utility.SetCookie("CompanyId_auth", user.CompanyId.NullToString());
                    var codeOrName = user.UrlCode;
                    if (!string.IsNullOrWhiteSpace(user.Displayname))
                    {
                        codeOrName = user.Displayname.NullToString();
                    }

                    return new ActionState { Message = "Congratulations!", Data = "Login successfully.", Success = true, Type = ActionState.SuccessType, OptionalValue = codeOrName }.ToActionResult(HttpStatusCode.OK);
                }
                else
                    return new ActionState { Message = "Failed!", Data = "Email or password is wrong or account not activated.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult PdfUsers()
        {
            return View();
        }

        public ActionResult GetAllPdfUsers()
        {
            int totRows = 0;
            var users = _userRepo.GetAllAviPdfUsers(Utility.StartIndex(), Utility.PageSize(), Utility.SortBy(), Utility.SortDesc(), Utility.FilterText(), out totRows);

            return Json(new { recordsFiltered = totRows, recordsTotal = totRows, data = users }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ForgotUserPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string emailid)
        {
            try
            {
                var user = new UserRepository().GetUserByEmail(emailid);

                if (user == null)
                {
                    return new ActionState
                    {
                        Success = false,
                        Data = "Email not found.",
                        Type = ActionState.ErrorType
                    }.ToActionResult(HttpStatusCode.OK);
                }

                var jwtToken = TokenManager.GenerateJWTAuthetication(user);
                var tokenClaim = TokenManager.ValidateToken(jwtToken);

                if (string.IsNullOrEmpty(tokenClaim.UserId.NullToString()) || tokenClaim.UserId != user.UserId)
                {
                    return new ActionState { Message = "Failed!", Data = "Unauthorized forgot password attempt.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var result = ForgetPasswordEmail(emailid, jwtToken, "Users/ForgotPassword");
                if (result.Success)
                {
                    return new ActionState { Message = "Congratulations!", Data = "Reset link sent to your email address.", Success = true, Type = ActionState.SuccessType, OptionalValue = "" }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Message = "Failed!", Data = "Sorry Email service is not working. try again later.", Success = false, Type = ActionState.ErrorType, OptionalValue = "" }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult ChangeUserPassword(string token)
        {
            AdminModel model = new AdminModel();
            if (string.IsNullOrEmpty(token))
            {
                return View("Error :- Token is missing.");
            }

            var tokenClaims = TokenManager.ValidateToken(token);

            if (tokenClaims == null)
            {
                return View("Error :- Invalid token.");
            }

            model.UserId = tokenClaims.UserId;
            model.EmailId = tokenClaims.EmailId;
            model.AuthToken = token;
            return View(model);
        }

        public ActionState ForgetPasswordEmail(string toEmail, string jwtToken, string pageName = "")
        {

            string outMessage;
            string body = "Dear User,<br/>";
            body = body + "Below is the reset password link for Bharat Touch.";
            body = body + "<br/><br/>";
            body += "<a href='" + ConfigurationManager.AppSettings["WebUrl"].ToString() + "Users/ChangeUserPassword?token=" + jwtToken + "' style='text-decoration:underline;'><b>Click here to reset password</b></a>";
            body = body + "<br/><br/>";
            body = body + "Regards<br/>";
            body = body + "Bharat Touch Team";
            body = body + "<br/> this email is generated by " + pageName + " page from Bharat Touch.";
            var result = Utility.SendEmail(toEmail, "Bharat Touch - Password recovery email.", body, out outMessage);
            return new ActionState { Success = result, Message = outMessage };
        }


        public ActionResult Signup()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Utility.RemoveCookie("UserId_auth");
            Utility.RemoveCookie("UserName_auth");
            Utility.RemoveCookie("EmailId_auth");
            Utility.RemoveCookie("UserUrlCode");
            Utility.RemoveCookie("UserType_auth");
            Utility.RemoveCookie("DisplayName_auth");
            Utility.RemoveCookie("CompanyId_auth");
            return RedirectToAction("Login");
        }

        public ActionResult LockScreen()
        {
            Utility.RemoveCookie("UserId_auth");
            return RedirectToAction("Login");
        }

        public ActionResult CheckEmailAvailability(string email)
        {
            if (ConfigValues.IsApiEnable)
            {
                var dict = new Dictionary<string, string>();
                dict.Add("email", email.NullToString());
                var data = RestApiHelper.CallApi(ConfigValues.ApiUrl, "User/CheckEmailAvailability", RestSharp.Method.GET, dict);
                if (data.IsSuccess)
                    return new ActionState { Message = "Email already exists!", Data = "1", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                else
                    return new ActionState { Message = "", Data = "0", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

            }
            else
            {
                if (_userRepo.IsExistEmail(email, "BharatTouch/SignupModal/CheckEmailAvailability"))
                {
                    return new ActionState { Message = "Email already exists!", Data = "1", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                    return new ActionState { Message = "", Data = "0", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        public ActionResult CheckDisplayNameAvailability(string name)
        {

            if (_userRepo.IsExistDisplayName(name, "BharatTouch/SignupModal/CheckDisplayNameAvailability"))
            {
                return new ActionState { Message = "Display Name already exists!", Data = "1", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            else
                return new ActionState { Message = "", Data = "0", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);


        }
        public ActionResult DownloadFilesFromPath(string fileName)
        {
            try
            {
                string path = Server.MapPath(fileName);
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "application/octet-stream", System.IO.Path.GetFileName(path));
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "File does not exists!", Data = "Failed!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #endregion

        #region readOnly partialViews
        public ActionResult ReadOnlyPortfolioImages(int id)
        {
            string cacheKey = $"portfolio-images-{id}";
            var cachedData = HttpContext.Cache[cacheKey] as Tuple<int, FileInfo[]>;
            if (cachedData == null)
            {
                DirectoryInfo d = new DirectoryInfo(Server.MapPath("~/uploads/Portfolio/" + id));
                FileInfo[] Files = null;
                try
                {
                    Files = d.GetFiles("*");
                }
                catch (Exception ex) { }
                cachedData = Tuple.Create(id, Files);
                DoCache(cacheKey, cachedData);
            }
            return PartialView("_portfolioImagesReadOnly", cachedData);
        }

        public ActionResult ReadOnlyTeamImages(int id)
        {
            string cacheKey = $"team-images-{id}";
            var teamList = HttpContext.Cache[cacheKey] as List<TeamViewModel>;

            if (teamList == null)
            {
                teamList = _userRepo.GetTeam(id, "BharatTouch/Profile/ReadOnlyTeamImages");
                DoCache(cacheKey, teamList);
            }
            return PartialView("_teamImagesReadonly", teamList);
        }


        public ActionResult ReadOnlyblogList(int id)
        {
            string cacheKey = $"blog-readonly-{id}";
            var blogList = HttpContext.Cache[cacheKey] as List<BlogModel>;
            if (blogList == null)
            {
                blogList = _blogRepository.GetAllBlogs(id, "BharatTouch/EditProfile/ReadOnlyblogList").OrderByDescending(x => x.CreatedDate).ToList();
                DoCache(cacheKey, blogList);
            }
            return PartialView("_blogListReadOnly", blogList);
        }

        public ActionResult ReadOnlyEducation(int id)
        {
            string cacheKey = $"education-readonly-{id}";
            var educationList = HttpContext.Cache[cacheKey] as List<UserEducationModel>;
            if (educationList == null)
            {
                educationList = _educationRepo.GetAllUserEducations(id, "BharatTouch/Profile/ReadOnlyEducation").OrderByDescending(x => x.StartDate).ToList();
                DoCache(cacheKey, educationList);
            }
            return PartialView("_educationReadOnly", educationList);
        }

        public ActionResult ReadOnlyProfessional(int id)
        {
            string cacheKey = $"professional-readonly-{id}";
            var professionalList = HttpContext.Cache[cacheKey] as List<UserProfessionalModel>;
            if (professionalList == null)
            {
                professionalList = _professionalRepo.GetAllUserProfessionals(id, "BharatTouch/EditProfile/ReadOnlyProfessional").OrderByDescending(x => x.StartDate).ToList();
                DoCache(cacheKey, professionalList);
            }
            return PartialView("_professionalReadOnly", professionalList);
        }

        public ActionResult ReadOnlyYoutube(int id)
        {
            string cacheKey = $"youtube-readonly-{id}";
            var YouTubeList = HttpContext.Cache[cacheKey] as List<YouTubeModel>;
            if (YouTubeList == null)
            {
                YouTubeList = _youTubeRepository.GetAllYouTubes(id, "BharatTouch/EditProfile/ReadOnlyYoutube").OrderByDescending(x => x.CreatedDate).ToList();
                DoCache(cacheKey, YouTubeList);
            }
            return PartialView("_youtubeListReadOnly", YouTubeList);
        }

        public ActionResult UserLeadsList(int id)
        {
            return PartialView("_userLeads", id);
        }

        #endregion

        [HttpGet]
        public ActionResult DeleteFileFromPath(string type, string fileName)
        {
            try
            {
                int loggedUser = Utility.GetCookie("UserId_auth").ToIntOrZero();

                string filepath = Server.MapPath(fileName);
                FileInfo file = new FileInfo(filepath);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
                }
                string fileExtension = Path.GetExtension(Path.GetFileName(fileName)).ToLower();
                if (fileExtension != ".pdf")
                {
                    string directoryPath = Server.MapPath("~" + Path.GetDirectoryName(fileName));
                    string pdfFilepath = Path.Combine(directoryPath, Path.ChangeExtension(Path.GetFileName(fileName), ".pdf"));
                    FileInfo pdfFile = new FileInfo(pdfFilepath);
                    if (pdfFile.Exists)
                    {
                        pdfFile.Delete();
                    }
                }

                _userRepo.DeletePersonalFile(loggedUser, type, "BharatTouch/EditProfile/DeleteFileFromPath");
                return new ActionState { Message = "Done!", Data = "File deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult SaveTempProfileImage()
        {
            try
            {
                var filePath = string.Empty;
                if (Request.Files.Count > 0)
                {
                    filePath = Utility.SaveImage();
                    if (filePath == "0")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only jpg,jpeg,png files are allowed", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                }
                return new ActionState { Message = "Done!", Data = filePath, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult SaveUserFile()
        {
            try
            {
                string UserId = Utility.GetCookie("UserId_auth").ToString();
                var user = _userRepo.GetUserById(UserId.ToIntOrZero(), "BharatTouch/EditProfile/SaveUserFile");
                if (user == null)
                {
                    return new ActionState { Message = "Failed!", Data = "User not found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var filePath = string.Empty;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    filePath = Utility.SaveCompressImages(file, UserId, (ConfigValues.ImagePath.Substring(1) + "/profile").ToLower(), 300);
                    if (filePath == "invalid")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only jpg,jpeg,png files are allowed", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (filePath == "0")
                    {
                        return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                }
                user.ProfileImage = filePath;
                var isSuccess = _userRepo.UpsertUserProfileAndCoverImage(user, "BharatTouch/EditProfile/SaveUserFile");

                return new ActionState
                {
                    Message = isSuccess ? "Done!" : "Failed!",
                    Data = isSuccess ? "Profile picture uploaded successfully." : "Some error occured.",
                    Success = isSuccess,
                    Type = isSuccess ? ActionState.SuccessType : ActionState.ErrorType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult UpdatePassword(int userId, string password)
        {
            try
            {
                _userRepo.UpdatePassword(userId, password, "BharatTouch/_/UpdatePassword");
                return new ActionState { Message = "Done!", Data = "Password changed successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        [HttpGet]
        public ActionResult Verification(string email)
        {
            bool isVerify = _userRepo.IsVerify(email, "BharatTouch/Verification/Verification");
            //try
            //{                
            //    return new ActionState { Message = "Done!", Data = "Successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            //}
            //catch (Exception ex)
            //{
            //    return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            //}
            return View(isVerify);
        }

        [HttpPost]
        public async Task<ActionResult> SendContactEmail(LeadsViewModel model, string personalEmail, string grecaptchaResponse, string pageName = "")
        {
            try
            {
                var isvarified = await IsReCaptchaValid(grecaptchaResponse);
                if (!isvarified)
                {
                    return new ActionState { Message = "Failed!", Data = "captcha failed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var isSuccess = _userRepo.InsertLead(model, pageName);
                if (!isSuccess)
                {
                    return new ActionState { Message = "Failed!", Data = "Server error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                string adminEmails = "";
                var adminUsers = _userRepo.GetAdminUsers(pageName);
                if (adminUsers.Count > 0)
                {
                    adminEmails = string.Join(",", adminUsers.Select(x => x.EmailId));
                }
                var domain = Request.Url.Host;
                var body = LeadsEmailTemplate(model, "User", "Profile");
                string outMessage;
                var result = Utility.SendEmail(personalEmail, "New lead from " + domain, body, out outMessage, "", adminEmails);
                if (result == false)
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occurred while sending email.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(UserModel user)
        {
            try
            {
                var result = _userRepo.ChangePassword(user, "Bharattouch/EditProfile/ChangePassword");
                if (result == 1)
                {
                    Utility.RemoveCookie("UserId_auth");
                    Utility.RemoveCookie("UserName_auth");
                    Utility.RemoveCookie("EmailId_auth");
                    Utility.RemoveCookie("UserUrlCode");
                    Utility.RemoveCookie("UserType_auth");
                    Utility.RemoveCookie("DisplayName_auth");
                    return new ActionState { Success = true, Message = "Done!", Data = "Password changed successfully, please login again.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else if (result == 9)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = "Server error.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = "User Not found", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionState VerificationEmail(string toEmail, string username, string password, string displayNameOrCode, string pageName = "")
        {
            string outMessage;
            bool result = false;
            var activationLink = ConfigurationManager.AppSettings["WebUrl"].ToString() + "users/verification?email=" + toEmail;
            var profileLink = ConfigurationManager.AppSettings["WebUrl"].ToString() + "/" + displayNameOrCode;

            string body = SignupTemplate(username, activationLink, toEmail, password, profileLink, pageName);

            result = Utility.SendEmail(toEmail, "Bharat Touch - Email verification.", body, out outMessage);
            return new ActionState { Success = result, Message = outMessage };
        }

        public async Task<ActionState> VerificationEmailAsync(string toEmail, string username, string password, string displayNameOrCode, string adminEmails, string pageName = "")
        {
            //toEmail = "kamaralamcp@gmail.com";
            //adminEmails = "alam@rnaura.com";

            var outMessage = "";
            var activationLink = ConfigurationManager.AppSettings["WebUrl"].ToString() + "users/verification?email=" + toEmail;
            var profileLink = ConfigurationManager.AppSettings["WebUrl"].ToString() + "/" + displayNameOrCode;

            string body = SignupTemplate(username, activationLink, toEmail, password, profileLink, pageName);

            System.Threading.Thread.Sleep(2000);

            var isSuccess = Utility.SendEmail(toEmail, "Bharat Touch - Email verification.", body,out outMessage,adminEmails);

            //var isSuccess = await Utility.SendEmailAsync(toEmail, "Bharat Touch - Email verification.", body, "", adminEmails);

            return new ActionState { Success = isSuccess, Message = isSuccess ? "Email Send" : "Error in sending email" };
        }

        public async Task<bool> QueueVerificationEmailAsync(string toEmail, string username, string password, string displayName, string pageName = "")
        {

            var adminUsers = _userRepo.GetAdminUsers();
            string adminEmails = string.Empty;
            if (adminUsers != null && adminUsers.Count > 0)
            {
                adminEmails = string.Join(",", adminUsers.Select(x => x.EmailId));
            }

            //HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            //{
            try
            {
                BackgroundJob.Enqueue(() => VerificationEmailAsync(toEmail, username, password, displayName, adminEmails, pageName));


                //var result = await VerificationEmailAsync(toEmail, username, password, displayName, adminEmails, pageName);

                //if (!result.Success)
                //{
                //    // Log business-level failure (e.g., SMTP error)
                //    Trace.TraceError($"[VerificationEmailAsync] Failed to send to {toEmail}. Reason: {result.Message}");
                //}
                //else
                //{
                //    Trace.TraceInformation($"[VerificationEmailAsync] Successfully sent verification email to {toEmail}.");
                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
                // Catch all other exceptions
                //Trace.TraceError($"[VerificationEmailAsync] Unexpected error for {toEmail}. Message: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
            //});
        }


        public async Task<int> SignUpEmailToAdmin(string name, string email, string userType, string displayName, int newUserId, string pageName = "")
        {
            string outMessage;
            int resss = 1;
            var adminUsers = _userRepo.GetAdminUsers();
            var userTypeFullName = "";
            switch (userType)
            {
                case "IN":
                    userTypeFullName = "Individual / Freelancer";
                    break;
                case "BO":
                    userTypeFullName = "Business Owner";
                    break;
                case "SP":
                    userTypeFullName = "Service Provider";
                    break;
                case "HR":
                    userTypeFullName = "Hotel / Restaurant";
                    break;
            }
            if (adminUsers.Count > 0)
            {
                foreach (var adminUser in adminUsers)
                {
                    string body = string.Empty;
                    var root = AppDomain.CurrentDomain.BaseDirectory;
                    using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/AdminSignUpEmail.html"))
                    {
                        string readFile = reader.ReadToEnd();
                        string StrContent = string.Empty;
                        StrContent = readFile;
                        //Assing the field values in the template
                        StrContent = StrContent.Replace("{AdminFullName}", adminUser.FirstName.NullToString() + " " + adminUser.LastName.NullToString());
                        StrContent = StrContent.Replace("{Name}", name);
                        StrContent = StrContent.Replace("{Email}", email);
                        StrContent = StrContent.Replace("{UserType}", userTypeFullName);
                        StrContent = StrContent.Replace("{DisplayName}", displayName);
                        StrContent = StrContent.Replace("{CardPreviewLink}", ConfigValues.WebUrl + "NFC/Preview/" + newUserId);
                        StrContent = StrContent.Replace("{ProfileUrl}", ConfigValues.WebUrl + displayName);
                        body = StrContent.ToString();

                        // HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                        //{
                        try
                        {
                            System.Threading.Thread.Sleep(2000);

                            //BackgroundJob.Enqueue(() => 
                            Utility.SendEmail(adminUser.EmailId, "BharatTouch - New Signup.", body, out outMessage, "", "", "", 0);
                            //);
                            
                            //var isSuccess = Utility.SendEmail(adminUser.EmailId, "BharatTouch - New Signup.", body,out outMessage);

                            // commente for test
                            //var isSuccess = await Utility.SendEmailAsync(adminUser.EmailId, "BharatTouch - New Signup.", body);
                            //adminUser.EmailId
                            resss = 1;

                        }
                        catch (Exception ex)
                        {
                            // Catch all other exceptions
                            resss = 2;
                            // 
                        }
                        // });

                        //Utility.SendEmail(adminUser.EmailId, "BharatTouch - New Signup.", body, out outMessage);
                    }
                }
            }

            resss = 0;

            return resss;
        }

        public bool PackageChangeRequestEmail(int UserId, int ChangeToPlanId)
        {
            string outMessage;
            var user = _userRepo.GetUserById(UserId);

            if (user == null)
            {
                return false;
            }

            var package = _packageRepo.GetPackageById(ChangeToPlanId);

            var adminUsers = _userRepo.GetAdminUsers();
            if (adminUsers.Count > 0)
            {
                foreach (var adminUser in adminUsers)
                {
                    string body = string.Empty;
                    var root = AppDomain.CurrentDomain.BaseDirectory;
                    using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/UpdatePackageRequestEmail.html"))
                    {
                        string readFile = reader.ReadToEnd();
                        string StrContent = string.Empty;
                        StrContent = readFile;
                        //Assing the field values in the template
                        StrContent = StrContent.Replace("{AdminFullName}", adminUser.FirstName.NullToString() + " " + adminUser.LastName.NullToString());
                        StrContent = StrContent.Replace("{FullName}", user.FirstName.NullToString() + " " + user.LastName.NullToString());
                        StrContent = StrContent.Replace("{Email}", user.EmailId);
                        StrContent = StrContent.Replace("{PhoneNo}", user.NumberCode + user.Phone);
                        StrContent = StrContent.Replace("{CurrentPlanPrice}", user.PackageCost.ToString());
                        StrContent = StrContent.Replace("{ChangeToPlanName}", package.PackageName);
                        StrContent = StrContent.Replace("{ChangeToPlanPrice}", package.PackageCost.ToString());
                        StrContent = StrContent.Replace("{DisplayName}", user.Displayname);
                        StrContent = StrContent.Replace("{ProfileUrl}", ConfigValues.WebUrl + user.Displayname);
                        body = StrContent.ToString();
                        Utility.SendEmail(adminUser.EmailId, "BharatTouch - Change Plan Request.", body, out outMessage);
                    }
                }
                return true;
            }
            return false;
        }


        public async Task<bool> NfcCardPrintDetailsEmail(int OrderId, string actionName = "")
        {

            try
            {
                var model = _nfcRepo.GetNfcCardPreviewDetailsByOrderId(OrderId, actionName);

                if (model == null)
                    return false;

                string outMessage;
                var adminUsers = _userRepo.GetAdminUsers();
                if (adminUsers.Count > 0)
                {
                    string body = string.Empty;
                    var root = AppDomain.CurrentDomain.BaseDirectory;
                    using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/NfcCardPrintDetailEmail.html"))
                    {
                        string readFile = reader.ReadToEnd();
                        string StrContent = string.Empty;
                        StrContent = readFile;
                        StrContent = StrContent.Replace("{RequestedBy}", model.FirstName.NullToString() + " " + model.LastName.NullToString());
                        StrContent = StrContent.Replace("{ProfileUrl}", ConfigValues.WebUrl + "NFC/Preview/" + OrderId);
                        body = StrContent.ToString();
                    }
                    foreach (var adminUser in adminUsers)
                    {
                        //HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                        //{
                        try
                        {
                            // var result = await VerificationEmailAsync(toEmail, username, password, displayName, pageName);
                            var isSuccess = await Utility.SendEmailAsync(adminUser.EmailId, "BharatTouch - NFC Card Print Details..", body);
                        }
                        catch (Exception ex)
                        {
                            // Catch all other exceptions                                
                        }
                        //});
                    }

                    //Send Notification to admin on device
                    var notificationUsers = new AdminRepository().GetAllDeviceToken();
                    if (notificationUsers.Count > 0)
                    {
                        var notifyTitle = "";
                        var notifyMessage = "";

                        //FirebaseToken is device token
                        List<string> fcmTokens = new List<string>();
                        foreach (var not in notificationUsers)
                        {
                            if (!string.IsNullOrWhiteSpace(not.FirebaseToken))
                            {
                                fcmTokens.Add(not.FirebaseToken);
                                notifyTitle = "BharatTouch - NFC Card Print Details..";
                                notifyMessage = $"BharatTouch - {model.FirstName} ({model.EmailId}) has registered for NFC card printing.";

                            }
                        }

                        if (fcmTokens.Count != 0)
                        {
                            var notificationResponse = PushNotification.Send(fcmTokens, notifyTitle, notifyMessage);
                        }
                    }


                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //public static void ConvertHtmlToPdf(string html, string outputPath)
        //{
        //    //PdfSharp.Pdf.PdfDocument pdf = PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
        //    //pdf.Save(outputPath);


        //    var doc = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = {
        //            PaperSize = DinkToPdf.PaperKind.A4,
        //            Orientation = DinkToPdf.Orientation.Portrait,
        //            Out = outputPath
        //        },
        //        Objects = {
        //                new ObjectSettings() {
        //                    HtmlContent = html,
        //                    WebSettings = { DefaultEncoding = "utf-8" }
        //                }
        //            }
        //    };
        //    DinkToPdf.Contracts.IConverter converter = new DinkToPdf.SynchronizedConverter(new DinkToPdf.PdfTools());
        //    converter.Convert(doc);
        //}

        #region Professional
        public ActionResult OpenProfessionalModel(int? id)
        {
            UserProfessionalModel model = new UserProfessionalModel();
            if (id != null)
                model = _professionalRepo.GetUserProfessionalById(id.Value);

            //if (model == null)
            //return PartialView("_createProfessional", new UserProfessionalModel());

            return PartialView("_createProfessional", model);
        }

        public ActionResult BindProfessional(int userId)
        {
            return PartialView("_professional", userId);
        }

        [HttpPost]
        public ActionResult SaveProfessional(UserProfessionalModel model)
        {
            try
            {
                int newProfessionalId;
                model.UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                int result = _professionalRepo.UpsertUserProfessional(model, out newProfessionalId, "BharatTouch/EditProfile/SaveProfessional");
                if (result == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                if (newProfessionalId > 0)
                {
                    return new ActionState { Message = "Done!", Data = "Experience details updated successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }
        #endregion

        #region Education
        public ActionResult OpenEducationModel(int? id)
        {
            UserEducationModel model = new UserEducationModel();
            if (id != null)
                model = _educationRepo.GetUserEducationById(id.Value, "BharatTouch/EditProfile/OpenEducationModel");

            return PartialView("_createEducation", model);
        }

        public ActionResult BindEducation(int userId)
        {
            return PartialView("_education", userId);
        }

        [HttpPost]
        public ActionResult SaveEducation(UserEducationModel model)
        {
            try
            {
                int newEducationId;
                model.UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                var result = _educationRepo.UpsertUserEducation(model, out newEducationId, "BharatTouch/EditProfile/SaveEducation");

                if (result == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                if (newEducationId > 0)
                {
                    return new ActionState { Message = "Done!", Data = "Education details updated successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        public ActionResult GenerateQRCde(string urlCode)
        {
            try
            {
                var qrPath = "~/uploads/QRCodes/QR_" + urlCode + ".png";
                var WebUrl = ConfigurationManager.AppSettings["WebUrl"].ToString();

                ProfileDetail profile = new ProfileDetail();
                profile = _userRepo.GetProfileDetail(urlCode, "BharatTouch/Profile/GenerateQRCde");

                if (profile != null)
                {
                    var nickname = profile.FirstName + " " + profile.LastName.NullToString();

                    ContactData generator = new ContactData(ContactData.ContactOutputType.VCard3,
                        profile.FirstName,
                        profile.LastName,
                        null,
                        null,
                        profile.Phone,
                        null,
                        profile.EmailId,
                        profile.BirthDate,
                        WebUrl + urlCode,
                        profile.Address1,
                        null,
                        profile.City,
                        profile.Zip,
                        profile.Country,
                        profile.Tagline,
                        profile.StateName);

                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(generator.ToString(), QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeAsBitmap = qrCode.GetGraphic(20);

                    if (!Directory.Exists(Server.MapPath("~/uploads/QRCodes")))
                        Directory.CreateDirectory(Server.MapPath("~/uploads/QRCodes"));

                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(Server.MapPath(qrPath), FileMode.Create, FileAccess.ReadWrite))
                        {
                            qrCodeAsBitmap.Save(memory, ImageFormat.Png);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                return new ActionState { Message = "Done!", Data = "Qr code generated!", Success = true, OptionalValue = "/uploads/QRCodes/QR_" + urlCode + ".png", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Testimonial

        public ActionResult GetClientTestimonialBy_UserId(int UserId, string actionName = "")
        {
            try
            {
                int totalRows = 0;
                var list = _clientTestimonialRepo.GetClientTestimonialBy_UserId(UserId, out totalRows, actionName);
                return new ActionState { Success = true, Message = "Users Testimonial", Data = list, OptionalValue = totalRows.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult BindClientTestimonials(int userId)
        {
            return PartialView("_clientTestimonial", userId);
        }

        public ActionResult GetClientTestimonialBy_Id(int clientId, string actionName = "")
        {
            try
            {
                var list = _clientTestimonialRepo.GetClientTestimonialBy_Id(clientId, actionName);
                return new ActionState { Success = true, Message = "Testimonial fetched", Data = list, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult DeleteClientTestimonial(int clientId, string actionName = "")
        {
            try
            {
                _clientTestimonialRepo.DeleteTestimonialBy_Id(clientId, actionName);
                return new ActionState { Success = true, Message = "Client Deleted successfully", Data = null, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult UpsertClientTestimonial(ClientTestimonialModel model, HttpPostedFileBase CLientImagePath)
        {
            try
            {
                if (CLientImagePath != null && CLientImagePath.ContentLength > 0)
                {
                    var fileName = model.Client_Id == 0 ? DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + model.UserId : model.Client_Id.ToString();
                    var uploadres = Utility.SaveCompressImages(CLientImagePath, fileName, ConfigValues.ImagePath.Substring(1) + "/ClientTestimonial/" + model.UserId, 200);
                    if (uploadres == "invalid")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only png,jpg,jpeg are allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (uploadres == "0")
                    {
                        return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    model.PicOfClient = uploadres;
                }
                var result = _clientTestimonialRepo.UpsertClientTestimonial(model, "BharatTouch/EditProfile/UpsertClientTestimonial");
                if (result == 0)
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Message = "Done!", Data = "Client " + (result == model.Client_Id ? "updated" : "saved") + " successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #endregion

        #region User Certification
        public ActionResult OpenUserCertificationModel(int? id)
        {
            UserCertificationModel model = new UserCertificationModel();
            if (id != null)
            {
                model = _certificationRepo.GetUserCertificationBy_Id(id.Value, "BharatTouch/EditProfile/OpenUserCertificationModel");
            }

            return PartialView("_createUserCertification", model != null ? model : new UserCertificationModel());
        }

        public ActionResult GetUserCertificationBy_UserId(int UserId, string actionName = "")
        {
            try
            {
                int totalRows = 0;
                var list = _certificationRepo.GetUserCertificationBy_UserId(UserId, out totalRows, actionName);
                return new ActionState { Success = true, Message = "Users Certification", Data = list, OptionalValue = totalRows.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult BindUserCertifications(int userId)
        {
            return PartialView("_userCertifications", userId);
        }

        public ActionResult GetUserCertificationsBy_Id(int certificationId, string actionName = "")
        {
            try
            {
                var list = _certificationRepo.GetUserCertificationBy_Id(certificationId, actionName);
                return new ActionState { Success = true, Message = "User Certificate fetched", Data = list, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult DeleteUserCertification(int certificationId, string actionName = "")
        {
            try
            {
                _certificationRepo.DeleteUserCertificationBy_Id(certificationId, actionName);
                return new ActionState { Success = true, Message = "Done!", Data = "Certificate Deleted successfully", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult UpsertUserCertification(UserCertificationModel model, HttpPostedFileBase CertificateFilePath)
        {
            try
            {
                model.UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                if (CertificateFilePath != null && CertificateFilePath.ContentLength > 0)
                {
                    string certificateFileName;
                    certificateFileName = CertificateFilePath.FileName;
                    //var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(certificateFileName);
                    var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + model.UserId + Path.GetExtension(certificateFileName);
                    if (model.CertificationId != 0)
                    {
                        newFileName = model.CertificationId.NullToString() + "_" + model.UserId + Path.GetExtension(certificateFileName);
                    }

                    string fileExtension = Path.GetExtension(certificateFileName).ToLower();
                    if (fileExtension != ".png" && fileExtension != ".jpg" & fileExtension != ".jpeg")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only png,jpg,jpeg are allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    var dbSavePath = "/uploads/userCertification/" + model.UserId + "/";
                    var folderPath = Server.MapPath("~" + dbSavePath);
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var dbPath = Path.Combine(folderPath, newFileName);
                    CertificateFilePath.SaveAs(dbPath);
                    model.CertificateFile = dbSavePath + newFileName;
                }
                var result = _certificationRepo.UpsertUserCertification(model, "BharatTouch/EditProfile/UpsertUserCertification");
                if (result == 0)
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Message = "Done!", Data = "Certificate/Training " + (result == model.CertificationId ? "updated" : "saved") + " successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }
        #endregion

        [HttpGet]
        public ActionResult showhideprofilesection(string type)
        {
            try
            {

                var UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                int OutFlag;
                _userRepo.Showhideprofilesection(UserId, type, out OutFlag, "BharatTouch/EditProfile/showhideprofilesection");
                if (OutFlag == 1)
                {
                    return new ActionState { Message = "Done!", Data = "Show/hide info updated successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else if (OutFlag == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = "User not found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

            }

        }

        public ActionResult OpenServiceCatalogModel()
        {
            return PartialView("_pdfViewer");
        }

        [HttpPost]
        public ActionResult PortfolioImages(HttpPostedFileBase[] PortfolioImage, int userId)
        {
            bool filesValid = true;
            try
            {
                if (PortfolioImage.Count() == 0)
                {
                    return new ActionState { Message = "Failed!", Data = "File not found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                foreach (HttpPostedFileBase file in PortfolioImage)
                {
                    if (file != null)
                    {
                        string portfoliofileName;
                        portfoliofileName = file.FileName;
                        string fileExtension = Path.GetExtension(portfoliofileName).ToLower();

                        if (fileExtension != ".png" && fileExtension != ".jpg" & fileExtension != ".jpeg")
                        {
                            filesValid = false;
                        }
                    }
                    if (file == null && PortfolioImage.Count() == 1)
                    {
                        return new ActionState { Message = "Failed!", Data = "Please choose image first.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                }

                if (!filesValid)
                {
                    return new ActionState { Message = "Failed!", Data = "Only png,jpg,jpeg are allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                foreach (HttpPostedFileBase file in PortfolioImage)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        var result = Utility.SaveCompressImagesWithoutSegment(file, "/uploads/portfolio/" + userId, 400);
                    }
                }
                return new ActionState { Message = "Done!", Data = "Images uploaded successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult BindPortfolioImages(int userId)
        {
            return PartialView("_portfolioImages", userId);
        }

        public ActionResult DeletePortfolioImage(string path)
        {
            try
            {
                FileInfo fi = new FileInfo(Server.MapPath(path));
                if (fi.Exists)
                {
                    fi.Delete();
                    //Utility.RemoveFile(thumbnailPath);
                    return new ActionState { Message = "Done!", Data = "Image deleted successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = "File not found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult BindTeamImages(int userId)
        {
            return PartialView("_teamImages", userId);
        }

        [HttpPost]
        public ActionResult InsertTeam(int userId, string name, string designation, HttpPostedFileBase TeamImagePath)
        {
            try
            {
                if (TeamImagePath != null && TeamImagePath.ContentLength > 0)
                {

                    var uploadRes = Utility.SaveCompressImages(TeamImagePath, DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + userId, ConfigValues.ImagePath.Substring(1) + "/teams/" + userId, 200);
                    if (uploadRes == "invalid")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only png,jpg,jpeg are allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (uploadRes == "0")
                    {
                        return new ActionState { Message = "Failed!", Data = "Some Error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                    var modal = new TeamViewModel();
                    modal.Name = name;
                    modal.UserId = userId;
                    modal.Designation = designation;
                    modal.PicturePath = uploadRes;

                    _userRepo.InsertTeam(modal, "BharatTouch/EditProfile/InsertTeam");
                    return new ActionState { Message = "Done!", Data = "Team save successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = "File not found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult DeleteTeam(int TeamId, string path)
        {
            try
            {
                var result = _userRepo.DeleteTeam(TeamId, "BharatTouch/EditProfile/DeleteTeam");

                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occurred!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                Utility.RemoveFile(path);
                return new ActionState { Message = "Done!", Data = "Team deleted successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SendContactEmailHome(LeadsViewModel model, string grecaptchaResponse, string pageName = "")
        {
            try
            {
                var isvarified = await IsReCaptchaValid(grecaptchaResponse);
                if (!isvarified)
                {
                    return new ActionState { Message = "Failed!", Data = "Captcha failed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var isSuccess = _userRepo.InsertLead(model, pageName);
                if (!isSuccess)
                {
                    return new ActionState { Message = "Failed!", Data = "Server error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }


                var adminUsers = _userRepo.GetAdminUsers();
                int successSent = 0;
                if (adminUsers.Count > 0)
                {
                    foreach (var user in adminUsers)
                    {
                        string outMessage;
                        var fullName = user.FirstName.NullToString() + " " + user.LastName.NullToString();
                        var body = LeadsEmailTemplate(model, "Admin, " + fullName, "Home");
                        //var result = Utility.SendEmail(user.EmailId, "New lead from Bharat Touch", body, out outMessage);
                        //if (result)
                        //{
                        //    successSent++;
                        //}
                        //HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                        //{
                        try
                        {
                            await Utility.SendEmailAsync(user.EmailId, "New lead from Bharat Touch", body);
                        }
                        catch (Exception ex)
                        {
                            // Catch all other exceptions                                
                        }
                        //});
                    }
                }
                //if (successSent == 0)
                //{
                //    return new ActionState { Message = "Failed!", Data = "Some error occurred while sending email.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                //}
                return new ActionState { Message = "Done!", Data = "Email will sent soon.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        public ActionResult DeleteEducation(int id)
        {
            try
            {
                _educationRepo.DeleteUserEducation(id, "BharatTouch/EditProfile/DeleteEducation");
                return new ActionState { Message = "Done!", Data = "Education detail deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }


        public ActionResult DeleteExpereince(int id)
        {
            try
            {
                var result = _professionalRepo.DeleteUserProfessional(id, "BharatTouch/EditProfile/DeleteExpereince");
                if (result)
                {
                    return new ActionState { Message = "Done!", Data = "Experience detail deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = "Something went wrong.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        public ActionResult InsertViewHistory(DeviceInfoModel model, string displayName)
        {
            try
            {
                _userRepo.InsertViewHistory_v1(model, displayName, "BharatTouch/Profile/InsertViewHistory");
                return new ActionState { Message = "Done!", Data = "View history saved.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public string SignupTemplate(string userName, string activationLink, string emailID, string password, string profileLink, string pageName = "")
        {
            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/Signup.html"))
            {
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("{FullName}", userName);
                StrContent = StrContent.Replace("{ActivationLink}", activationLink);
                StrContent = StrContent.Replace("{EmailID}", emailID);
                StrContent = StrContent.Replace("{Password}", password);
                StrContent = StrContent.Replace("{ProfileLink}", profileLink);
                StrContent = StrContent.Replace("{pageName}", pageName);
                body = StrContent.ToString();
            }

            return body;
        }

        public ActionResult EmailModel(string emails)
        {
            return PartialView("_sendUserEmail", emails);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SendEmailToUsers(string EmailSubject, string emailBody, string EmailIds)
        {
            try
            {
                string outMessage = "";
                string body = emailBody;
                EmailIds = EmailIds.TrimEnd(',');

                //var toEMail = EmailIds.Split(',')[0].NullToString();
                //var bcc = "";
                //for (int i = 1; i < EmailIds.Split(',').Length; i++)
                //{
                //    bcc = EmailIds.Split(',')[i].NullToString() + ",";
                //}

                //Utility.SendEmail(EmailIds.Split(',')[0], EmailSubject, emailBodyText, out outMessage,"",bcc);
                Utility.SendEmail("", EmailSubject, emailBody, out outMessage, "", EmailIds);
                //body = body + "<br/><br/>";
                //body = body + "Regards<br/>Avi eCard Team";


                return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenBlogModel(int? id)
        {
            BlogModel model = new BlogModel();
            if (id != null)
                model = _blogRepository.GetBlogById(id.Value, "BharatTouch/EditProfile/OpenBlogModel");

            return PartialView("_createBlog", model);
        }

        public ActionResult BindBlog(int userId)
        {
            return PartialView("_blogList", userId);
        }

        [HttpPost]
        public ActionResult SaveBlog(BlogModel model)
        {
            try
            {
                model.UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                var result = _blogRepository.UpsertBlog(model, "BharatTouch/EditProfile/SaveBlog");

                if (result == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                if (model.BlogId > 0)
                {
                    return new ActionState { Message = "Done!", Data = "Blog detail updated successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else if (model.BlogId == 0)
                {
                    return new ActionState { Message = "Done!", Data = "Blog detail inserted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                    return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult DeleteBlog(int id)
        {
            try
            {
                _blogRepository.DeleteBlog(id, "BharatTouch/EditProfile/DeleteBlog");
                return new ActionState { Message = "Done!", Data = "Blog detail deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        #region YouTube
        public ActionResult OpenYouTubeModel(int? id)
        {
            YouTubeModel model = new YouTubeModel();
            if (id != null)
                model = _youTubeRepository.GetYouTubeById(id.Value, "BharatTouch/EditProfile/OpenYouTubeModel");

            return PartialView("_youtubeCreate", model);
        }

        public ActionResult BindYouTube(int userId)
        {
            return PartialView("_youtubeList", userId);
        }

        [HttpPost]
        public ActionResult SaveYouTube(YouTubeModel model)
        {
            try
            {
                model.UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                var result = _youTubeRepository.UpsertYouTube(model, "BharatTouch/EditProfile/SaveYouTube");
                if (result == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                if (model.YouTubeId > 0)
                {
                    return new ActionState { Message = "Done!", Data = "Youtube detail updated successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else if (model.YouTubeId == 0)
                {
                    return new ActionState { Message = "Done!", Data = "Youtube detail inserted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                    return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult DeleteYouTube(int id)
        {
            try
            {
                var result = _youTubeRepository.DeleteYouTube(id, "BharatTouch/EditProfile/DeleteYouTube");
                if (result)
                {
                    return new ActionState { Message = "Done!", Data = "Youtube detail deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }
        #endregion

        public ActionResult FeatureNotification()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult FeatureNotification(string EmailSubject, string emailDescription, string emailBody)
        {
            try
            {
                int totalRows = 0;
                var userList = _userRepo.GetAllUsers(out totalRows, "BharatTouch/Index/FeatureNotification");
                string EmailIds = String.Empty;
                if (userList == null)
                {
                    return new ActionState { Message = "Failed!", Data = "Someting went wrong.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                ActionState actionState = new ActionState() { Message = "Done!", Success = true, Type = ActionState.SuccessType };
                string body = FeatureUpdateEmailBody("", EmailSubject, emailDescription, emailBody);

                foreach (var model in userList)
                {
                    //string body = FeatureUpdateEmailBody(model.FirstName, EmailSubject, emailDescription, emailBody);
                    string outMessage = "";
                    Utility.SendEmail("", EmailSubject, body, out outMessage, "", model.EmailId);
                    actionState.Data = outMessage;
                }

                return actionState.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult ConvertWordToPdf(string path)
        {
            try
            {
                string directoryName = Path.GetDirectoryName(path);
                string directoryPath = Server.MapPath("~" + directoryName);
                string pdfFilePath = Path.Combine(directoryPath, Path.ChangeExtension(Path.GetFileName(path), ".pdf"));
                string relativePdfPath = pdfFilePath.Replace(Server.MapPath("~/"), "/");
                if (System.IO.File.Exists(pdfFilePath))
                {
                    return new ActionState { Message = "PDF already exists.", Data = relativePdfPath, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                string serverPath = Server.MapPath("~" + path);
                //var tempPath = Utility.CopyFileToTempFolder(serverPath);
                //if (tempPath == "")
                //{
                //    return new ActionState { Message = "Failed!", Data = "file not found or corrupted", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                //}
                //Utility.ConvertWordToSpecifiedFormat(tempPath, pdfFilePath, Word.WdSaveFormat.wdFormatPDF);

                //List<string> pdfFilePaths = SplitDocument(serverPath, 3);
                //MergePdfs(pdfFilePaths, pdfFilePath);

                //Soutin Soft code
                //DocumentCore.SetLicense("03/05/25hzuwgY0JCmN4PpMrR7cUevzB0C+aYGnW4A");
                DocumentCore document = DocumentCore.Load(serverPath);
                document.Save(pdfFilePath, new PdfSaveOptions());

                return new ActionState { Message = "Success!", Data = relativePdfPath, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #region Schedule & meetings

        public ActionResult MeetingRequestModelForProfile(int UserId, DateTime? date)
        {
            LeadsViewModel obj = new LeadsViewModel();
            obj.UserId = UserId;
            obj.Date = date;
            obj.LeadTypeId = 1;

            return PartialView("_meetingRequestForm", obj);
        }

        [HttpPost]
        async public Task<ActionResult> SendMeetingRequest(LeadsViewModel model, string grecaptchaResponse)
        {
            try
            {
                var isvarified = await IsReCaptchaValid(grecaptchaResponse);

                if (!isvarified)
                {
                    return new ActionState { Message = "Failed!", Data = "Captcha failed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var userModel = _userRepo.GetUserById(model.UserId, "BharatTouch/Profile/SendMeetingRequest");
                if (userModel == null)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var isSuccess = _userRepo.InsertLead(model, "BharatTouch/Profile/SendMeetingRequest");
                if (!isSuccess)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var emailBody = MeetingRequestEmailTemplate(model, $"{userModel.FirstName} {userModel.LastName.NullToString()}", "ProfilePage");
                string outMessage = string.Empty;
                Utility.SendEmail(userModel.EmailId, "Meeting Request!", emailBody, out outMessage);

                return new ActionState
                {
                    Message = "Success!",
                    Data = "Meeting request successfully sent!.",
                    Success = true,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetAllScheduleOpenDays(int UserId)
        {
            try
            {
                int totalRows = 0;
                var model = _scheduleRepository.GetAllScheduleOpenDays(UserId, out totalRows, "BharatTouch/EditProfile/GetAllScheduleOpenDays");

                return new ActionState() { Message = "Done!", Data = model, Success = true, Type = ActionState.SuccessType, OptionalValue = totalRows.ToString() }.ToActionResult(HttpStatusCode.OK); ;
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult BindScheduleOpenDayList(int UserId)
        {
            return PartialView("_scheduleOpenDayList", UserId);
        }

        public ActionResult DeleteScheduleOpenDay(int dayId)
        {
            try
            {
                var isSuccess = _scheduleRepository.DeleteScheduleOpenDayById(dayId, "BharatTouch/EditProfile/DeleteScheduleOpenDay");
                if (!isSuccess)
                {
                    return new ActionState { Message = "Failed!", Data = "Server error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState { Message = "Success!", Data = "Day successfully deleted.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult UpsertScheduleOpenDay(ScheduleOpenDayModel model)
        {
            try
            {
                bool result = _scheduleRepository.UpsertScheduleOpenDays(model, "BharatTouch/EditProfile/UpsertScheduleOpenDay");
                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }


                return new ActionState
                {
                    Message = "Success!",
                    Data = "Schedule day successfully upserted.",
                    Success = true,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult UpsertScheduleOpenWeekDays(ScheduleOpenWeekDayModel model)
        {
            try
            {
                var result = _scheduleRepository.UpsertScheduleOpenWeekDays(model, "BharatTouch/EditProfile/UpsertScheduleOpenWeekDays");
                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState
                {
                    Message = "Success!",
                    Data = "Schedule Week day status updated.",
                    Success = true,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetScheduleOpenWeekDays(int UserId, string actionName)
        {
            try
            {
                var model = _scheduleRepository.GetScheduleOpenWeekDays(UserId, actionName);
                if (model == null)
                {
                    return new ActionState { Message = "Failed!", Data = "Open Week days Not found", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Message = "Success!", Data = model, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }
        #endregion

        [HttpPost]
        public ActionResult UploadPaymentDetails(UpiDetailsModel model)
        {
            if (Request.Files.Count > 0)
            {
                var paymentQr = Request.Files[0];

                if (paymentQr != null && paymentQr.ContentLength > 0)
                {
                    var res = Utility.SaveCompressImages(paymentQr, model.UserId.NullToString(), ConfigValues.ImagePath.Substring(1) + "/paymentsQr", 400);
                    if (res == "invalid")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only jpg, jpeg, png files allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }
                    else if (res == "0")
                    {
                        return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                    model.QrImage = res;
                }
            }
            var isSuccess = _userRepo.UpsertUpiDetails(model, "BharatTouch/EditProfile/UploadPaymentQrCode");
            if (!isSuccess)
            {
                return new ActionState { Message = "Failed!", Data = "Server Error", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

            return new ActionState { Message = "Success!", Data = "Payment details uploaded successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
        }

        [HttpGet]
        public ActionResult DeletePaymentDetails(string fileName)
        {
            try
            {
                var UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                bool isSuccess = _userRepo.DeleteUserUpiDetailByUserId(UserId, "BharatTouch/EditProfile/DeletePaymentDetails");
                if (!isSuccess)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                Utility.RemoveFile(fileName);
                return new ActionState { Message = "Done!", Data = "Payment details deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult UserLeads(int UserId)
        {
            int loggerId = Utility.GetCookie("UserId_auth").ToIntOrZero();
            if (loggerId != UserId)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(UserId);
        }

        [HttpPost]
        public ActionResult BindLeadList(int UserId)
        {
            int totRows = 0;
            var leads = _userRepo.FetchLeadsByUserId(UserId, Utility.StartIndex(), Utility.PageSize(), Utility.SortBy(), Utility.SortDesc(), Utility.FilterText(), out totRows, "BharatTouch/Profile/BindLeadList");

            return Json(new { recordsFiltered = totRows, recordsTotal = totRows, data = leads }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult BindCompanyTypeByParent(int id)
        {
            try
            {
                var list = _commonRepo.GetBusinessTypeListBtParentId(id, "BharatTouch/EditProfile/BindCompanyTypeByParent");
                return new ActionState { Success = true, Message = "List", Data = list, OptionalValue = list.Count.ToString(), Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        [HttpPost]
        public ActionResult DeleteLeadById(int LeadId)
        {
            try
            {
                bool success = _userRepo.DeleteLeadById(LeadId, "BharatTouch/Profile/DeleteLeadById");
                if (!success)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = "Server error.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState { Success = true, Message = "Failed!", Data = "Lead deleted successfully.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        public void DoCache(string cacheKey, object value)
        {
            HttpContext.Cache.Insert(cacheKey, value, null, DateTime.Now.AddMinutes(60), TimeSpan.Zero);
        }

        public void BustCache(string cacheKey)
        {
            HttpContext.Cache.Remove(cacheKey);
            //var url = Url.Action("ReadOnlyTeamImages", "Users", new {id = id});
            //HttpResponse.RemoveOutputCacheItem(url);
        }

        private async Task<bool> IsReCaptchaValid(string gRecaptchaResponse)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecretKey}&response={gRecaptchaResponse}", null);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                return result.success == "true";
            }
        }

        public string FeatureUpdateEmailBody(string firstName, string EmailSubject, string emailDescription, string emailBodyText)
        {
            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/FeatureUpdate.html"))
            {
                var emailBody = System.Web.HttpUtility.HtmlDecode(emailBodyText);
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("{webUrl}", webUrl);
                StrContent = StrContent.Replace("{FirstName}", firstName);
                StrContent = StrContent.Replace("{Subject}", EmailSubject);
                StrContent = StrContent.Replace("{Description}", emailDescription);
                StrContent = StrContent.Replace("{content}", emailBody);
                body = StrContent.ToString();
            }
            return body;
        }

        public string MeetingRequestEmailTemplate(LeadsViewModel model, string FullName, string pageName = "")
        {
            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/MeetingRequest.html"))
            {
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("{FullName}", FullName);
                StrContent = StrContent.Replace("{RequestedBy}", model.Name);
                StrContent = StrContent.Replace("{RequestedByEmail}", model.Email);
                StrContent = StrContent.Replace("{RequestedByPhoneNo}", model.PhoneNo);
                StrContent = StrContent.Replace("{MeetingDate}", model.Date.ToString().Substring(0, 10));
                StrContent = StrContent.Replace("{Reason}", model.Message);
                StrContent = StrContent.Replace("{pageName}", pageName);
                body = StrContent.ToString();
            }

            return body;
        }

        public string LeadsEmailTemplate(LeadsViewModel model, string FullName, string pageName = "")
        {
            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/Leads.html"))
            {
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("{FullName}", FullName);
                StrContent = StrContent.Replace("{RequestedBy}", model.Name);
                StrContent = StrContent.Replace("{RequestedByEmail}", model.Email);
                StrContent = StrContent.Replace("{RequestedByPhoneNo}", model.PhoneNo);
                StrContent = StrContent.Replace("{Message}", model.Message);
                StrContent = StrContent.Replace("{pageName}", pageName);
                body = StrContent.ToString();
            }

            return body;
        }


        public ActionResult DeleteFile(string filePath)
        {
            try
            {
                var result = Utility.RemoveFile(filePath);
                if (!result)
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occured!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Message = "Done!", Data = "File deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult ChangeUserProfileTemplate(UserModel user)
        {
            try
            {
                user.UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
                var result = _userRepo.ChangeProfileTemplate(user, "BharatTouch/EditProfile/ChangeUserProfileTemplate");
                if (result == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Server error!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                if (result == 1)
                {
                    return new ActionState { Message = "Done!", Data = "Profile template updated successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState { Message = "Failed!", Data = "User not found!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        //Url Rewriting 

        public ActionResult Adhar(string code, string company = null)
        {
            ProfileDetail profile = new ProfileDetail();

            var fullCode = "";

            if (company != null && company != "")
            {
                fullCode = company + "/" + code;
            }
            else
            {
                fullCode = code;

            }

            profile = _userRepo.GetProfileDetail(fullCode, "BharatTouch/Profile/Adhar");

            // var huggingFaceClient = new HuggingFaceTextGenerator();
            //string generatedText = await huggingFaceClient.FetchHuggingFaceResponse("Once upon a time");

            //if (profile == null)
            //    return RedirectToAction("Index", "Home", new { area = "" });

            if (profile == null)
            {
                //return View(new ProfileDetail());
                return Redirect("/" + code);
            }
            //var package = _packageRepo.GetPackageByCodeOrName(code, "BharatTouch/Profile/Adhar");
            //if (package == null || package.AllowAdhaarCard == false)
            //{
            //    return Redirect("/" + code);
            //}


            return View(profile);
        }

        [HttpPost]
        public ActionResult UpdatePackage(UserPackageViewModel package, string actionName = "")
        {
            try
            {
                package.LoggedUserId = Utility.GetCookie("UserId_auth").ToIntOrZero();

                var success = PackageChangeRequestEmail(package.UserId, package.PackageId);
                if (success)
                {
                    return new ActionState { Message = "Done!", Data = "Plan change request successfully sent.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                //var result = _userRepo.UpdateUserPackage_v1(package, actionName);
                //if (result == 1)
                //{
                //    return new ActionState { Message = "Done!", Data = "Package updated successfully", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                //}
                //else if (result == 9)
                //{
                //    return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                //}

                return new ActionState { Message = "Failed!", Data = "Some error occurred, please try again later.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult ImageCompressor()
        {
            return View();
        }

        public bool CompressImageinKbs(string filePath, string savePath, long targetSizeKB = 200, long initialQuality = 70L)
        {
            try
            {
                var fullPath = Server.MapPath(filePath);
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(fullPath))
                {
                    ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders()
                        .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

                    if (jpgEncoder == null) return false;

                    EncoderParameters encoderParams = new EncoderParameters(1);
                    long quality = initialQuality;

                    // Convert KB to Bytes
                    long targetSizeBytes = targetSizeKB * 1024;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Save the image with initial quality
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                        image.Save(ms, jpgEncoder, encoderParams);

                        // Reduce quality iteratively if file is too large
                        while (ms.Length > targetSizeBytes && quality > 10)
                        {
                            ms.SetLength(0); // Clear MemoryStream
                            quality -= 10; // Reduce quality
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                            image.Save(ms, jpgEncoder, encoderParams);
                        }

                        // If still too large, resize the image
                        if (ms.Length > targetSizeBytes)
                        {
                            int newWidth = (int)(image.Width * 0.8);
                            int newHeight = (int)(image.Height * 0.8);

                            using (Bitmap resizedImage = new Bitmap(image, newWidth, newHeight))
                            {
                                ms.SetLength(0); // Clear MemoryStream
                                resizedImage.Save(ms, jpgEncoder, encoderParams);
                            }
                        }

                        var finalSavePath = Server.MapPath(savePath);
                        // Save final compressed image
                        System.IO.File.WriteAllBytes(finalSavePath, ms.ToArray());
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public string CheckAndCreateDirectory(string path, string replace, string replaceWith)
        {
            try
            {
                string relativePath = path.TrimStart('~');
                var compressedFilePath = Regex.Replace(path, @"[\\/]", "/").Replace(replace, replaceWith);

                var savePath = Server.MapPath("~" + compressedFilePath);

                var saveDirectory = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(saveDirectory))
                {
                    Directory.CreateDirectory(saveDirectory);
                }

                return compressedFilePath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private string saveComppressImage(string filePath)
        {
            var image = filePath;
            bool result = false;
            if (Utility.IsExistFile(filePath))
            {
                image = CheckAndCreateDirectory(filePath, "/uploads/", "/uploads/Compressed/");
                result = CompressImageinKbs(filePath, image, 200);
            }
            return result ? image : filePath;
        }

        [HttpPost]
        public ActionResult ReplaceImages(int UserId)
        {
            try
            {
                string actionName = "Bharattouch/Test/Index/ReplaceImages";
                string message = "";
                var model = _userRepo.GetUserImages(UserId, actionName);
                if (model.user != null)
                {
                    model.user.ProfileImage = saveComppressImage(model.user.ProfileImage);
                    model.user.CoverImage = saveComppressImage(model.user.CoverImage);
                    model.user.AdhaarFrontImgPath = saveComppressImage(model.user.AdhaarFrontImgPath);
                    model.user.AdhaarBackImgPath = saveComppressImage(model.user.AdhaarBackImgPath);
                    var result = _userRepo.UpdateUserImages(model.user, actionName);
                    if (result)
                    {
                        message += "User Images updated.;";
                    }
                    else
                    {
                        message += "User Image Update failed.;";
                    }
                }
                if (model.teams != null && model.teams.Count > 0)
                {
                    foreach (var team in model.teams)
                    {
                        var path = saveComppressImage(team.PicturePath);
                        var result = _userRepo.UpdateUserImagesByType(team.UserId, team.TeamId, path, "UT", actionName);
                        if (result)
                        {
                            message += "teamImage Images (" + team.TeamId + ") updated.;";
                        }
                        else
                        {
                            message += "teamImage Images (" + team.TeamId + ") update failed.;";
                        }
                    }
                }
                if (model.clientTestimonials != null && model.clientTestimonials.Count > 0)
                {
                    foreach (var item in model.clientTestimonials)
                    {
                        var path = saveComppressImage(item.PicOfClient);
                        var result = _userRepo.UpdateUserImagesByType(item.UserId, item.Client_Id, path, "CT", actionName);
                        if (result)
                        {
                            message += "Client Testimonial Images (" + item.Client_Id + ") updated.;";
                        }
                        else
                        {
                            message += "teamImage Images (" + item.Client_Id + ") update failed.;";
                        }
                    }
                }

                if (model.upiDetail != null)
                {
                    var path = saveComppressImage(model.upiDetail.QrImage);
                    var result = _userRepo.UpdateUserImagesByType(model.upiDetail.UserId, model.upiDetail.Id, path, "UP", actionName);
                    if (result)
                    {
                        message += "UPI QR Image (" + model.upiDetail.Id + ") updated.;";
                    }
                    else
                    {
                        message += "UPI QR Image (" + model.upiDetail.Id + ") update failed.;";
                    }
                }

                return new ActionState { Message = "Done!", Data = message, Success = model != null, Type = model != null ? ActionState.SuccessType : ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #region RazorPay Payment Gateway

        [Route("signup")]
        public ActionResult SmartWizard(string code)
        {
            ViewBag.Code = code;
            var countriesList = new CountryRepository().GetCountries("BharatTouch/Signup/SmartWizard");
            var countries = countriesList.Select(n => new SelectListItem()
            {
                Text = n.Country + " (" + n.NumberCode + ")",
                Value = n.CountryId.ToString() + ";" + n.MinNumberLength + ";" + n.MaxNumberLength
            });

            ViewBag.Countries = countries;
            return View();
        }

        [HttpPost]
        public ActionResult UserAllWizardDetails(WizardCompleteDetailsViewModel model, string actionName = "")
        {
            try
            {
                int selectCardOutFlag = 0;
                int cardDetailsOutFlag = 0;

                var cardModel = new NfcCardColorViewModel();
                cardModel.UserId = model.BasicData.UserId;
                cardModel.OrderId = model.BasicData.OrderId;
                cardModel.CardColorId = model.BasicData.CardColorId;

                // 
                cardModel.CardFinishId = model.CardStyle.CardFinishId;
                cardModel.IncludeMetalCard = model.CardStyle.IncludeMetalCard;

                cardModel.CardType = model.CardStyle.CardType;
                cardModel.PackageCost = model.CardStyle.Price;

                cardModel.NfcCardLine1 = model.CardOptions.NfcCardLine1;
                cardModel.NfcCardLine2 = model.CardOptions.NfcCardLine2;
                cardModel.NfcCardLine3 = model.CardOptions.NfcCardLine3;

                var selectCardResult = _nfcRepo.UpdateSelectedNfcCardColor_V3(cardModel, out selectCardOutFlag, actionName);
                var detailsResult = _nfcRepo.UpdateUsersNfcCardDetails_v2(cardModel, out cardDetailsOutFlag, actionName);

                var orderModel = new OrderModel();
                orderModel.UserId = model.BasicData.UserId;
                orderModel.OrderId = model.BasicData.OrderId;
                // shipping Details
                orderModel.ShippingAddress1 = model.Address.Shipping.ShippingAddress1;
                orderModel.ShippingAddress2 = model.Address.Shipping.ShippingAddress2;
                orderModel.ShippingCity = model.Address.Shipping.ShippingCity;
                orderModel.ShippingState = model.Address.Shipping.ShippingState;
                orderModel.ShippingCountry = model.Address.Shipping.ShippingCountry;
                orderModel.ShippingZip = model.Address.Shipping.ShippingZip;
                orderModel.IsSelfPick = model.Address.Shipping.IsSelfPick;

                // billing details
                orderModel.BillingAddress1 = model.Address.Billing.BillingAddress1;
                orderModel.BillingAddress2 = model.Address.Billing.BillingAddress2;
                orderModel.BillingCity = model.Address.Billing.BillingCity;
                orderModel.BillingState = model.Address.Billing.BillingState;
                orderModel.BillingCountry = model.Address.Billing.BillingCountry;
                orderModel.BillingZip = model.Address.Billing.BillingZip;

                var addressResult = _orderRepo.UpsertOrder_v2(orderModel, actionName);

                if (!selectCardResult || !detailsResult || !addressResult)
                {
                    return new ActionState { Message = "Some error occurred!", Data = null, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                int paymentDetailsOutFlag = 0;
                var paymentDetails = _orderRepo.GetOrderAmountDetialsForFinalPayment(model.BasicData.OrderId, model.BasicData.UserId, out paymentDetailsOutFlag, actionName);

                if (paymentDetails != null)
                    return new ActionState { Success = true, Message = "All details are updated successfully, next step is to do payment.", Data = paymentDetails, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                else
                    return new ActionState { Success = false, Message = "Some error occurred! while fetching payment details. try again later.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = ex.Message, Data = null, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult CreateNewCardOrder(int UserId)
        {
            try
            {
                var newOrderId = _orderRepo.CreateNewCardOrder(UserId, "BharatTouch/EditProfile/CreateNewCardOrder");
                return new ActionState
                {
                    Success = newOrderId != 0,
                    Message = newOrderId == 0 ? "Some error occurred while creating order" : "Order created successfully",
                    Data = newOrderId,
                    Type = newOrderId == 0 ? ActionState.ErrorType : ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult CreateRazorpayOrder(PaymentModel model)
        {
            try
            {
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", model.PaymentAmount * 100); // Amount should be in paise and int as datatype
                //options.Add("amount", 1); // Amount should be in paise and int as datatype
                options.Add("currency", "INR");
                options.Add("receipt", $"nfcorder_{Guid.NewGuid().ToString("N").Substring(0, 30)}"); //generate unique receipt id under 40 characters
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Order order = _razorpayClient.Order.Create(options);
                var status = order["status"].ToString();
                var orderDate = Convert.ToInt64(order["created_at"]);

                var payload = new OrderModel();
                payload.OrderId = model.OrderId;
                payload.UserId = model.OrderId;
                DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(orderDate).DateTime;

                // If you want it in local time
                DateTime localTime = dateTime.ToLocalTime();

                payload.RazorpayOrderId = order["id"].ToString();
                payload.RazorpayOrderDate = localTime;

                var res = _orderRepo.UpdateOrderWithRazorPayDetails(payload, "BharatTouch/CreateRazorPayOrder");
                if (res != 0)
                    return new ActionState() { Success = false, Message = "Server Error", Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                else
                    return new ActionState() { Success = true, Message = "Order created.", Data = new { RazorOrder = order.Attributes, RazorOorderId = payload.RazorpayOrderId }, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Razorpay.Api.Errors.BadRequestError ex)
            {
                // Razorpay BadRequestError (validation issues, bad input)
                return new ActionState
                {
                    Success = false,
                    Message = "Invalid request to Razorpay.",
                    Data = ex.ErrorCode ?? ex.Message,
                    Type = ActionState.ErrorType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Razorpay.Api.Errors.ServerError ex)
            {
                // Razorpay ServerError (5xx errors from Razorpay)
                return new ActionState
                {
                    Success = false,
                    Message = "Razorpay server error.",
                    Data = ex.ErrorCode ?? ex.Message,
                    Type = ActionState.ErrorType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred, please try again later.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        [HttpPost]
        public async Task<ActionResult> VerifyAndSaveNfcOrderPayment(PaymentModel model, bool WantMetalCard, RazorPaySignatureModel razorModel)
        {
            try
            {
                //var orderEmailResult = OrderSuccessEmails(model.OrderId);
                //return new ActionState { Success = true, Message = "Done!", Data = "Order placed successfully.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

                var result = Utility.VerifyRazorPayPayment(razorModel);
                if (!result.Success)
                {
                    return result.ToActionResult(HttpStatusCode.OK);
                }
                //model.TransactionId = razorModel.razorpay_payment_id;
                //model.PaymentStatus = "Verified";
                model.RazorPayTransactionId = razorModel.razorpay_payment_id;
                var paymentDetailsRes = new RazorPayManager().FetchPaymentDetailsById(razorModel.razorpay_payment_id);
                var details = paymentDetailsRes.Data as dynamic;
                if (paymentDetailsRes.Success)
                {
                    model.RazorPayPaymentMethod = details["method"].ToString();
                    model.RazorPayPaymentStatus = details["status"].ToString();
                    //string email = details["email"].ToString();
                    //string contact = details["contact"].ToString();
                    //string amount = details["amount"].ToString();
                }
                else
                {
                    model.RazorPayPaymentStatus = paymentDetailsRes.Message;
                    //var code = details["code"].ToString();
                    //var description = details["description"].ToString();
                    //var source = details["source"].ToString();
                    //var reason = details["reason"].ToString();
                    //var error = details["error"].ToString();
                }
                model.RazorPayJson = JsonConvert.SerializeObject(details); //details.NullToString();
                int InvoiceId = 0;
                var paymentResult = _paymentRepo.UpsertOrderPayment_v7(model, out InvoiceId);
                if (paymentResult == 0)
                {
                    var userDetails = _userRepo.GetUserById(model.UserId);
                    var adminUsers = _userRepo.GetAdminUsers();
                    if (adminUsers.Count > 0)
                    {
                        int successCount = 0;
                        foreach (var adminUser in adminUsers)
                        {
                            string outMessage = string.Empty;
                            string body = string.Empty;
                            var root = AppDomain.CurrentDomain.BaseDirectory;
                            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/RazorPayDbFailedEmail.html"))
                            {
                                string readFile = reader.ReadToEnd();
                                string StrContent = string.Empty;
                                StrContent = readFile;
                                //Assing the field values in the template
                                StrContent = StrContent.Replace("{FullName}", adminUser.FirstName.NullToString() + " " + adminUser.LastName.NullToString());
                                StrContent = StrContent.Replace("{RequestedBy}", userDetails.FirstName.NullToString() + " " + userDetails.LastName.NullToString());
                                StrContent = StrContent.Replace("{Email}", userDetails.EmailId);
                                StrContent = StrContent.Replace("{PhoneNo}", userDetails.NumberCode + userDetails.Phone);
                                StrContent = StrContent.Replace("{PaymentTransactionId}", razorModel.razorpay_payment_id);
                                StrContent = StrContent.Replace("{PaidAmount}", "Rs. " + model.PaymentAmount.ToString());
                                StrContent = StrContent.Replace("{Date}", DateTime.Now.ToString());
                                StrContent = StrContent.Replace("{UserId}", model.UserId.ToString());
                                StrContent = StrContent.Replace("{OrderId}", model.OrderId.ToString());
                                StrContent = StrContent.Replace("{WantMetalCard}", WantMetalCard.ToString());
                                StrContent = StrContent.Replace("{DisplayName}", userDetails.Displayname);
                                StrContent = StrContent.Replace("{ProfileUrl}", ConfigValues.WebUrl + userDetails.Displayname);
                                body = StrContent.ToString();
                                //var success = Utility.SendEmail(adminUser.EmailId, "BharatTouch - User Payment Save Failed!.", body, out outMessage);
                                //if (success)
                                //{
                                //    successCount++;
                                //}


                                //HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                                //{
                                try
                                {
                                    var isSuccess = await Utility.SendEmailAsync(adminUser.EmailId, "BharatTouch - User Payment Save Failed!.", body);
                                }
                                catch (Exception ex)
                                {
                                    // Catch all other exceptions                                
                                }
                                //});
                            }
                        }

                        //Send Notification to admin on device
                        var notificationUsers = new AdminRepository().GetAllDeviceToken();
                        if (notificationUsers.Count > 0)
                        {
                            var notifyTitle = "";
                            var notifyMessage = "";

                            //FirebaseToken is device token
                            List<string> fcmTokens = new List<string>();
                            foreach (var not in notificationUsers)
                            {
                                if (!string.IsNullOrWhiteSpace(not.FirebaseToken))
                                {
                                    fcmTokens.Add(not.FirebaseToken);
                                    notifyTitle = "BharatTouch - User Payment Save Failed!.";
                                    notifyMessage = $"Payment failed for user {userDetails.FirstName} ({userDetails.EmailId}) on Order #{model.OrderId}. Action required.";

                                }
                            }

                            if (fcmTokens.Count != 0)
                            {
                                var notificationResponse = PushNotification.Send(fcmTokens, notifyTitle, notifyMessage);
                            }
                        }

                        return new ActionState { Success = false, Message = "Failed!", Data = "Some error occurred while fetching your payment details, our team will contact you regards this soon.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                        //if (successCount > 0)
                        //{
                        //    return new ActionState { Success = false, Message = "Failed!", Data = "Some error occurred while fetching your payment details, our team will contact you regards this soon.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                        //}
                        //else
                        //{
                        //    return new ActionState { Success = false, Message = "Failed!", Data = "Some error occurred, please contact support team.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                        //}
                    }

                    return new ActionState { Success = false, Message = "Failed!", Data = "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {

                    var emailResult = NfcCardPrintDetailsEmail(model.OrderId);
                    var orderEmailResult = OrderSuccessEmails(model.OrderId);
                }

                //var InvoiceRes = InvoiceEmailToUser(InvoiceId, false);
                return new ActionState { Success = true, Message = "Done!", Data = "Order placed successfully.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public async Task<bool> OrderSuccessEmails(int OrderId)
        {

            try
            {
                var root = AppDomain.CurrentDomain.BaseDirectory;
                var model = _nfcRepo.GetNfcCardPreviewDetailsByOrderId(OrderId);

                if (model == null)
                    return false;


                // User Email
                string userEmailbody = string.Empty;
                using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/UserSuccessOrderEmail.html"))
                {
                    string readFile = reader.ReadToEnd();
                    string StrContent = string.Empty;
                    StrContent = readFile;
                    StrContent = StrContent.Replace("[UserFirstName]", model.FirstName.NullToString());
                    StrContent = StrContent.Replace("[OrderNumber]", model.OrderNo);
                    StrContent = StrContent.Replace("[Date]", DateTimeFormatter.ConvertToString(model.CreatedOn.Value));
                    StrContent = StrContent.Replace("[Amount]", model.PaymentAmount.NullToString());
                    userEmailbody = StrContent.ToString();
                }


                string orderItems = "<li>Pvc NFC Card – Qty: 1</li>";

                if (model.IncludeMetalCard)
                {
                    orderItems += "<li>Metal NFC Card – Qty: 1</li>";
                }

                string adminEmailBody = string.Empty;
                using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/SuccessOrderAdminEmail.html"))
                {
                    string readFile = reader.ReadToEnd();
                    string StrContent = string.Empty;
                    StrContent = readFile;
                    StrContent = StrContent.Replace("[OrderID]", model.OrderId.NullToString());
                    StrContent = StrContent.Replace("[OrderDateAndTime]", DateTimeFormatter.ConvertToString(model.CreatedOn.Value));
                    StrContent = StrContent.Replace("[CustomerFullName]", model.FirstName.NullToString() + " " + model.LastName.NullToString());
                    StrContent = StrContent.Replace("[CustomerEmail]", model.EmailId.NullToString());
                    StrContent = StrContent.Replace("[CustomerPhoneNumber]", model.NumberCode.NullToString() + model.Phone.NullToString());
                    StrContent = StrContent.Replace("[Amount]", model.PaymentAmount.NullToString());
                    StrContent = StrContent.Replace("[OrderItems]", orderItems);
                    adminEmailBody = StrContent.ToString();
                }

                //HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                //{
                try
                {
                    var isSuccess = await Utility.SendEmailAsync(model.EmailId, "Your Bharat Touch Order Has Been Successfully Placed!", userEmailbody);
                }
                catch (Exception ex)
                {
                    // Catch all other exceptions
                    Trace.TraceError($"[VerificationEmailAsync] Unexpected error for {OrderId}. Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                }
                //});

                // Admin Email
                var adminUsers = _userRepo.GetAdminUsers();
                if (adminUsers.Count > 0)
                {
                    foreach (var adminUser in adminUsers)
                    {
                        //HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                        //{
                        try
                        {
                            //var result = await VerificationEmailAsync(toEmail, username, password, displayName, pageName);
                            await Utility.SendEmailAsync(adminUser.EmailId, "New Order Placed on Bharat Touch – Order #" + OrderId, adminEmailBody);
                        }
                        catch (Exception ex)
                        {
                            // Catch all other exceptions                                
                        }
                        //});
                    }

                    //Send Notification to admin on device
                    var notificationUsers = new AdminRepository().GetAllDeviceToken();
                    if (notificationUsers.Count > 0)
                    {
                        var notifyTitle = "";
                        var notifyMessage = "";

                        //FirebaseToken is device token
                        List<string> fcmTokens = new List<string>();
                        foreach (var not in notificationUsers)
                        {
                            if (!string.IsNullOrWhiteSpace(not.FirebaseToken))
                            {
                                fcmTokens.Add(not.FirebaseToken);
                                notifyTitle = "New Order Placed on Bharat Touch – Order #" + OrderId;
                                notifyMessage = $"A new NFC card order (Order #{OrderId}) has been placed. Please review the details and proceed with processing.";

                            }
                        }

                        if (fcmTokens.Count != 0)
                        {
                            var notificationResponse = PushNotification.Send(fcmTokens, notifyTitle, notifyMessage);
                        }
                    }

                    return true;

                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ActionResult OrderPlacedView()
        {
            return View();
        }

        public ActionResult RazorPayTestPage()
        {
            decimal amount = 100; // Example amount

            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", amount); // Amount in paise
            options.Add("currency", "INR");
            options.Add("receipt", $"order_{Guid.NewGuid().ToString("N").Substring(0, 30)}"); //generate unique receipt id

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Order order = _razorpayClient.Order.Create(options);
                return View(new PaymentOrderResponse { Amount = amount, OrderId = order["id"] });

            }
            catch (Exception ex)
            {
                //handle exception
                return View(new PaymentOrderResponse { Error = ex.Message });


            }
        }

        [HttpPost]
        public ActionResult VerifyPayment(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>
        {
           { "razorpay_order_id", razorpay_order_id },
           { "razorpay_payment_id", razorpay_payment_id },
            { "razorpay_signature", razorpay_signature }
        };


            try
            {
                string expectedSignature = Utility.GenerateSHA256Hash(razorpay_order_id + "|" + razorpay_payment_id, ConfigValues.RazorPayApiSecret);

                if (expectedSignature != razorpay_signature)
                {
                    //Signature Verified
                    return Json(new { success = false, error = "Invalid Signature" });

                }

                return Json(new { success = true });

            }
            catch (Exception ex)
            {

                return Json(new { success = false, error = ex.Message });

            }
        }

        #endregion

        #region BulkOrder


        [Route("bulkorder")]
        public ActionResult BulkOrder()
        {
            BulkOrderModel model = new BulkOrderModel();
            model.MinOrder = 10;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveUpdateBulkOrder(BulkOrderModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var recaptchaResponse = Request.Form["grecaptchaResponse"];
                    bool iscaptchaValid = await IsReCaptchaValid(recaptchaResponse);
                    if (!iscaptchaValid)
                    {
                        return new ActionState { Message = "Captcha failed.", Data = null, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                    int outFlag;
                    string outMessage;
                    var result = _userRepo.UpsertBulkOrder(model, out outFlag, out outMessage);

                    return new ActionState { Message = outFlag == 0 ? "Success" : "Failed", Data = outMessage, Success = true, OptionalValue = null, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                catch (Exception ex)
                {
                    return new ActionState { Message = ex.Message, Data = ex, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

            }
            else
            {
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors);
                return new ActionState { Message = "Failed!", Data = "Invalid Form!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #endregion
    }
    public class PaymentOrderResponse
    {
        public Decimal Amount { get; set; }
        public string OrderId { get; set; }
        public string Error { get; set; }
        public bool success { get; set; }
    }


}



