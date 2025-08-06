using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI;
using BharatTouch.CommonHelper;
using DataAccess;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.ViewModels;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.html.head;
using Microsoft.Office.Interop.Word;
using BharatTouch.CommonHelper;
using BharatTouch.Controllers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using WebGrease.Css.Ast;
using Razorpay.Api;
using PdfSharp.Charting;
using System.Windows.Interop;
using System.Web.Hosting;
using System.Diagnostics;
using SautinSoft.Document.Drawing;

namespace BharatTouch.Controllers
{

    public class AdminController : Controller
    {

        #region Declaration
        AdminRepository _adminRepo = new AdminRepository();
        CountryRepository _countryRepo = new CountryRepository();
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
        PaymentRepository _paymentRepo = new PaymentRepository();
        string recaptchaSecretKey = ConfigValues.GoogleCaptchaSecretKey;
        string webUrl = ConfigValues.WebUrl;
        #endregion

        #region All View Page

        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        [AuthenticateAdmin_Admin]
        public ActionResult UserIndex()
        {
            return View();
        }

        [AuthenticateAdmin_Admin]
        public ActionResult PackageIndex()
        {
            return View();
        }

        [AuthenticateAdmin_Admin]
        public ActionResult CountryIndex()
        {
            return View();
        }

        public ActionResult OrderIndex()
        {
            return View();
        }

        public ActionResult BulkOrderIndex()
        {
            return View();
        }

        public ActionResult CompanyIndex()
        {
            return View();
        }

        public ActionResult BlogIndex()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Utility.RemoveCookie("UserId_admin");
            Utility.RemoveCookie("UserName_admin");
            Utility.RemoveCookie("EmailId_admin");
            Utility.RemoveCookie("UserUrlCode_admin");
            Utility.RemoveCookie("UserType_admin");
            Utility.RemoveCookie("DisplayName_admin");
            Utility.RemoveCookie("IsAdmin_admin");
            return RedirectToAction("Login");
        }

        [AuthenticateAdmin_Admin]
        public ActionResult BusinessTypeIndex()
        {
            var businessTypeParentList = _commonRepo.GetBusinessTypeParentList("BharatTouch/BusinessType/BusinessTypeIndex");

            businessTypeParentList = businessTypeParentList.Where(x => x.BusinessTypeId != 36).ToList(); //exclude "others" field

            var defaultItem = new BusinessTypeModel { BusinessTypeId = 0, BusinessType = "Select Company Category" };

            businessTypeParentList.Insert(0, defaultItem);

            ViewBag.BusinessTypeParentList = new SelectList(businessTypeParentList, "BusinessTypeId", "BusinessType");

            return View();
        }

        public ActionResult TextGenerator()
        {
            return View();
        }

        public ActionResult LeadIndex()
        {
            return View();
        }
                
        public ActionResult LeadCommunicationDetail(int leadId)
        {
            var model = new List<LeadCommunicationViewModel>();

            model = _adminRepo.GetLeadCommunicationByLeadId_Admin(leadId);

            var allUsers = _userRepo.GetAdminUsers();
            ViewBag.Users = allUsers.Select(u => new SelectListItem
            {
                Value = u.UserId.ToString(),
                Text = (u.FirstName + " " + (u.LastName ?? "")).Trim(),
                //Selected = (model.CommunicatedBy == u.UserId)
            }).ToList();

            var communicationTypes = _adminRepo.GetLeadCommunicationType_Admin();
            ViewBag.CommunicationTypes = communicationTypes.Select(c => new SelectListItem
            {
                Value = c.CommunicationTypeId.ToString(),
                Text = c.CommunicationType,
                //Selected = (model.CommunicationTypeId == c.CommunicationTypeId)
            }).ToList();

            ViewBag.FollowUpCommunicationTypes = communicationTypes.Select(c => new SelectListItem
            {
                Value = c.CommunicationTypeId.ToString(),
                Text = c.CommunicationType,
               // Selected = (model.FollowUpCommunicationTypeId == c.CommunicationTypeId)
            }).ToList();

            return View(model);
        }

        #endregion

        #region User

        [HttpPost]
        public ActionResult AuthenticateAdmin(AdminModel model)
        {
            try
            {
                var user = _adminRepo.Authenticate_Admin(model, "BharatTouch/loginModal/authenticate");
                if (user != null)
                {
                    Utility.SetCookie("UserId_admin", user.UserId.ToString());
                    Utility.SetCookie("UserName_admin", user.FirstName.NullToString() + " " + user.LastName.NullToString());
                    Utility.SetCookie("EmailId_admin", user.EmailId);
                    Utility.SetCookie("UserUrlCode_admin", user.UrlCode.NullToString());
                    Utility.SetCookie("UserType_admin", user.UserType.NullToString());
                    Utility.SetCookie("DisplayName_admin", user.Displayname.NullToString());
                    Utility.SetCookie("IsAdmin_admin", user.IsAdmin.NullToString());
                    var codeOrName = user.UrlCode;
                    if (!string.IsNullOrWhiteSpace(user.Displayname))
                    {
                        codeOrName = user.Displayname.NullToString();
                    }
                    Utility.SetCookie("UserUrlCodeDisplayName_admin", codeOrName);
                    return new ActionState { Message = "Congratulations!", Data = "Login successfully.", Success = true, Type = ActionState.SuccessType, OptionalValue = user.UserType.NullToString() }.ToActionResult(HttpStatusCode.OK);
                }
                else
                    return new ActionState { Message = "Failed!", Data = "Email or password is wrong or only admin have access to login.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult GetUsers()
        {
            int totRows = 0;
            var users = _adminRepo.GetAllUsers_Admin(Utility.StartIndex(), Utility.PageSize(), Utility.SortBy(), Utility.SortDesc(), Utility.FilterText(), out totRows, "BharatTouch/Index/GetUsers");

            return Json(new { recordsFiltered = totRows, recordsTotal = totRows, data = users }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete_User(int id)
        {
            try
            {
                _adminRepo.DeleteUser_Admin(id, "BharatTouch/Index/Delete");
                return new ActionState { Message = "Done!", Data = "User deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        [HttpPost]
        public ActionResult GetAllUserHistory(int userId)
        {
            var users = _adminRepo.GetAllUserHistory_Admin(userId);

            return Json(new { recordsFiltered = users.Count, recordsTotal = users.Count, data = users }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetUserReferredByCode(string referalCode)
        {
            var users = _adminRepo.UserReferredByCode(referalCode);

            return Json(new { recordsFiltered = users.Count, recordsTotal = users.Count, data = users }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Edit Profile Func

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
                        return new ActionState { Message = "Failed!", Data = "Only jpg,jpeg,png,bmp and gif files are allowed", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
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

        public ActionResult BindProfessional(int userId)
        {
            return PartialView("_professional", userId);
        }

        public ActionResult OpenProfessionalModel(int? id, int? userid)
        {
            UserProfessionalModel model = new UserProfessionalModel();
            model.UserId = userid.Value;
            if (id != null)
                model = _professionalRepo.GetUserProfessionalById(id.Value);

            //if (model == null)
            //return PartialView("_createProfessional", new UserProfessionalModel());

            return PartialView("_createProfessional", model);
        }

        [HttpPost]
        public ActionResult SaveProfessional(UserProfessionalModel model)
        {
            try
            {
                int newProfessionalId;
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

        public void BustCache(string cacheKey)
        {
            HttpContext.Cache.Remove(cacheKey);
            //var url = Url.Action("ReadOnlyTeamImages", "Users", new {id = id});
            //HttpResponse.RemoveOutputCacheItem(url);
        }

        public ActionResult BindEducation(int userId)
        {
            return PartialView("_education", userId);
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
                var result = _clientTestimonialRepo.UpsertClientTestimonial(model, "BharatTouch/EditProfile/InsertTeam");
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

        public ActionResult OpenEducationModel(int? id, int? userid)
        {
            UserEducationModel model = new UserEducationModel();
            model.UserId = userid.Value;
            if (id != null)
                model = _educationRepo.GetUserEducationById(id.Value, "BharatTouch/EditProfile/OpenEducationModel");

            return PartialView("_createEducation", model);
        }

        [HttpPost]
        public ActionResult SaveEducation(UserEducationModel model)
        {
            try
            {
                int newEducationId;
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

        public ActionResult BindPortfolioImages(int userId)
        {
            return PartialView("_portfolioImages", userId);
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
                        string portfoliofileName;
                        portfoliofileName = file.FileName;
                        var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(portfoliofileName);

                        string fileExtension = Path.GetExtension(portfoliofileName).ToLower();

                        var folderPath = Server.MapPath("~/uploads/Portfolio/" + userId);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        var dbPath = Path.Combine(folderPath, newFileName);
                        file.SaveAs(dbPath);
                    }
                }
                return new ActionState { Message = "Done!", Data = "Images uploaded successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult DeletePortfolioImage(string path, string thumbnailPath)
        {
            try
            {
                FileInfo fi = new FileInfo(Server.MapPath(path));
                if (fi.Exists)
                {
                    fi.Delete();
                    Utility.RemoveFile(thumbnailPath);
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

        public ActionResult DeleteTeam(int TeamId, string path)
        {
            try
            {
                _userRepo.DeleteTeam(TeamId, "BharatTouch/EditProfile/DeleteTeam");

                FileInfo fi = new FileInfo(Server.MapPath(path));
                if (fi.Exists)
                {
                    fi.Delete();
                }
                return new ActionState { Message = "Done!", Data = "Team deleted successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

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


        [HttpPost]
        public ActionResult SaveBlog(BlogModel model)
        {
            try
            {
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

        public ActionResult BindBlog(int userId)
        {
            return PartialView("_blogList", userId);
        }

        public ActionResult OpenBlogModel(int? id)
        {
            BlogModel model = new BlogModel();
            if (id != null)
                model = _blogRepository.GetBlogById(id.Value, "BharatTouch/EditProfile/OpenBlogModel");

            return PartialView("_createBlog", model);
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

        public ActionResult BindYouTube(int userId)
        {
            return PartialView("_youtubeList", userId);
        }

        public ActionResult OpenYouTubeModel(int? id, int? userId)
        {
            YouTubeModel model = new YouTubeModel();
            model.UserId = userId.Value;
            if (id != null)
                model = _youTubeRepository.GetYouTubeById(id.Value, "BharatTouch/EditProfile/OpenYouTubeModel");

            return PartialView("_youtubeCreate", model);
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

        public ActionResult BindScheduleOpenDayList(int UserId)
        {
            return PartialView("_scheduleOpenDayList", UserId);
        }

        [HttpPost]
        public ActionResult DeletePaymentDetails(string fileName, int UserId)
        {
            try
            {
                bool isSuccess = _userRepo.DeleteUserUpiDetailByUserId(UserId, "BharatTouch/EditProfile/DeletePaymentDetails");
                if (!isSuccess)
                {
                    return new ActionState { Message = "Failed!", Data = "Server Error", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                string filepath = Server.MapPath(fileName);
                FileInfo file = new FileInfo(filepath);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
                }
                return new ActionState { Message = "Done!", Data = "Payment details deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
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

        public ActionResult OpenCoverImagePropertyPreviewModal(int UserId)
        {
            var user = _userRepo.GetUserById(UserId, "BharatTouch/EditProfile/OpenCoverImagePropertyPreviewModal");
            if (user == null)
            {
                user = new UserModel() { UserId = UserId };
            }

            return PartialView("_CoverImageStyleModal", user);
        }

        public ActionResult BindClientTestimonials(int userId)
        {
            return PartialView("_clientTestimonial", userId);
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

        public ActionResult OpenUserCertificationModel(int? id, int? userid)
        {
            UserCertificationModel model = new UserCertificationModel();
            model.UserId = userid.Value;
            if (id != null)
            {
                model = _certificationRepo.GetUserCertificationBy_Id(id.Value, "BharatTouch/EditProfile/OpenUserCertificationModel");
            }

            return PartialView("_createUserCertification", model);
        }

        public ActionResult BindUserCertifications(int userId)
        {
            return PartialView("_userCertifications", userId);
        }

        [HttpPost]
        public ActionResult DeleteUserCertification(int certificationId, string actionName = "")
        {
            try
            {
                _certificationRepo.DeleteUserCertificationBy_Id(certificationId, actionName);
                return new ActionState { Success = true, Message = "User Certificate Deleted successfully", Data = null, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = ex.Message, Data = null, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
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

        [HttpPost]
        public ActionResult SaveYouTube(YouTubeModel model)
        {
            try
            {
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

        [HttpPost]
        public ActionResult AdhaarUpdate(UserModel model, HttpPostedFileBase frontImg, HttpPostedFileBase BackImg)
        {
            try
            {
                //model.UserId = Utility.GetCookie("UserId_auth").ToIntOrZero();
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

        [HttpGet]
        public ActionResult showhideprofilesection(string type, int UserId)
        {
            try
            {
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

        public ActionResult UserLeadsList(int id)
        {
            return PartialView("_userLeads", id);
        }

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

        [HttpPost]
        public ActionResult ChangePassword(UserModel user)
        {
            try
            {
                var result = _userRepo.ChangePassword(user, "Bharattouch/EditProfile/ChangePassword");
                if (result == 1)
                {
                    return new ActionState { Success = true, Message = "Done!", Data = "Password changed successfully.", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
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

        [HttpPost]
        public ActionResult ChangeUserProfileTemplate(UserModel user)
        {
            try
            {
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


        [HttpPost]
        public ActionResult SaveUserFile(int UserId)
        {
            try
            {
                var user = _userRepo.GetUserById(UserId, "BharatTouch/EditProfile/SaveUserFile");
                if (user == null)
                {
                    return new ActionState { Message = "Failed!", Data = "User not found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                var filePath = string.Empty;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    filePath = Utility.SaveCompressImages(file, UserId.NullToString(), (ConfigValues.ImagePath.Substring(1) + "/profile").ToLower(), 300);
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

        //[Route("addedituser/{code}")]
        [AuthenticateAdmin_Admin]
        [OutputCache(Duration = 20, VaryByParam = "code", Location = OutputCacheLocation.Client)]
        public ActionResult AddEditUser(string code,string company = null)
        {
            ViewBag.IsAddEditUserAction = true;
            var businessTypeParentList = _commonRepo.GetBusinessTypeParentList("BharatTouch/AddEditUser/AddEditUser");
            var defaultItem = new BusinessTypeModel { BusinessTypeId = 0, BusinessType = "Select Company Category" };
            businessTypeParentList.Insert(0, defaultItem);

           // var package = _packageRepo.GetPackageByCodeOrName(code, "BharatTouch/EditProfile/EditProfile");
          //  ViewBag.package = package;

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

            //var userId = Utility.GetCookie("UserId_Admin").ToIntOrZero();
            //if (user == null || user.UserId != userId)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            ViewBag.UserProfilePic = user.ProfileImage.NullToString() != "" ? user.ProfileImage.NullToString() : "/FormAssets/img/blank-profile-picture.jpg";
            ViewBag.BusinessTypeParentList = new SelectList(businessTypeParentList, "BusinessTypeId", "BusinessType", user.CompanyTypeParentId);
            return View(user);

        }

        //[Route("addedituser/{code}")]
        //[AuthenticateAdmin_Admin]
        //[OutputCache(Duration = 20, VaryByParam = "code", Location = OutputCacheLocation.Client)]
        //public ActionResult AddEditUser(string code)
        //{
        //    ViewBag.IsAddEditUserAction = true;
        //    var businessTypeParentList = _commonRepo.GetBusinessTypeParentList("BharatTouch/AddEditUser/AddEditUser");
        //    var defaultItem = new BusinessTypeModel { BusinessTypeId = 0, BusinessType = "Select Company Category" };
        //    businessTypeParentList.Insert(0, defaultItem);

        //    var package = _packageRepo.GetPackageByCodeOrName(code, "BharatTouch/EditProfile/EditProfile");
        //    ViewBag.package = package;

        //    if (code == null)
        //    {
        //        ViewBag.BusinessTypeParentList = new SelectList(businessTypeParentList, "BusinessTypeId", "BusinessType");
        //        return View(new UserModel());
        //    }

        //    UserModel user = new UserModel();
        //    user = _userRepo.GetUserByCodeOrName(code, "BharatTouch/EditProfile/EditProfile");

        //    //var userId = Utility.GetCookie("UserId_Admin").ToIntOrZero();
        //    //if (user == null || user.UserId != userId)
        //    //{
        //    //    return RedirectToAction("Index", "Home");
        //    //}
        //    ViewBag.UserProfilePic = user.ProfileImage.NullToString() != "" ? user.ProfileImage.NullToString() : "/FormAssets/img/blank-profile-picture.jpg";
        //    ViewBag.BusinessTypeParentList = new SelectList(businessTypeParentList, "BusinessTypeId", "BusinessType", user.CompanyTypeParentId);
        //    return View(user);

        //}

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
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors);
                return new ActionState { Message = "Failed!", Data = "Invalid Form!", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        [HttpPost]
        public ActionResult UpdateUserStatus(UserModel user)
        {
            try
            {
                var result = _userRepo.UpdateActiveStatus(user, "BharatTouch/Admin/UserIndex/UpdateUserStatus");
                var message = "";
                if (result == 0)
                    message = "User Not Found.";
                else if (result == 1)
                    message = "Active status updated successfully.";
                else
                    message = "Server Error.";

                return new ActionState
                {
                    Success = result == 1,
                    Message = result == 1 ? "Done!" : "Failed!",
                    Data = message,
                    OptionalValue = result.ToString(),
                    Type = result == 1 ? ActionState.SuccessType : ActionState.ErrorType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
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

        [HttpPost]
        public ActionResult UpsertUserCertification(UserCertificationModel model, HttpPostedFileBase CertificateFilePath)
        {
            try
            {
                if (CertificateFilePath != null && CertificateFilePath.ContentLength > 0)
                {
                    string certificateFileName;
                    certificateFileName = CertificateFilePath.FileName;
                    var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(certificateFileName);

                    string fileExtension = Path.GetExtension(certificateFileName).ToLower();
                    if (fileExtension != ".png" && fileExtension != ".jpg" & fileExtension != ".jpeg" && fileExtension != ".pdf")
                    {
                        return new ActionState { Message = "Failed!", Data = "Only png,jpg,jpeg,pdf are allowed.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
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

        [HttpPost]
        public ActionResult SocialInfo(string LinkedIn, string Twitter, string Facebook, string Instagram, string Skype, string Youtube)
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

                return new ActionState { Message = "Done!", Data = "Social media info updated successfully!", Success = true, OptionalValue = "", Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }


        #endregion

        #region Business type

        [HttpPost]
        public ActionResult GetBusinessType()
        {
            var businessTypes = _commonRepo.GetBusinessTypeList();

            return Json(new { recordsFiltered = businessTypes.Count, recordsTotal = businessTypes.Count, data = businessTypes }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteBusinessTypeById(int id)
        {
            try
            {
                int outFlag;
                string outMessage;

                _commonRepo.SoftDeleteBusinessTypeById(id, Utility.GetCookie("UserId_admin").ToIntOrZero(), out outFlag, out outMessage);

                if (outFlag == 0)
                {
                    return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        [HttpPost]
        public ActionResult AddEditBusinessType(int? id, int flag, int? businessCategoryId)
        {
            var model = new BusinessTypeViewModel();

            var businessTypeParentList = _commonRepo.GetBusinessTypeParentList("BharatTouch/BusinessType/BusinessTypeIndex");

            businessTypeParentList = businessTypeParentList.Where(x => x.BusinessTypeId != 36).ToList(); //exclude "others" field

            var defaultItem = new BusinessTypeModel { BusinessTypeId = 0, BusinessType = "Select Company Category" };

            businessTypeParentList.Insert(0, defaultItem);

            ViewBag.BusinessTypeParentList = new SelectList(businessTypeParentList, "BusinessTypeId", "BusinessType");


            if (id.HasValue)
            {
                model = _commonRepo.GetBusinessTypeById(id.Value);
                ViewBag.BusinessTypeParentList = new SelectList(businessTypeParentList, "BusinessTypeId", "BusinessType", model.ParentId);
            }
            else
            {
                if (flag == 0)
                    model.ParentId = businessCategoryId.Value;
            }

            if (flag == 0)
            {
                return PartialView("_addEditBusinessType", model);

            }
            else
            {
                return PartialView("_addEditParentBusinessTypeCategory", model);
            }
        }

        [HttpPost]
        public async Task<ActionResult> SaveOrUpdateBusinessType(BusinessTypeModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                _commonRepo.UpsertBusinessType(model, out outFlag, out outMessage);

                if (outFlag == 0)
                {
                    return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }

            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Packages

        [HttpPost]
        public ActionResult GetAllPackages()
        {
            var package = _packageRepo.GetAllPackages();

            return Json(new { recordsFiltered = package.Count, recordsTotal = package.Count, data = package }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeletePackage(int id)
        {
            try
            {
                int outFlag;
                string outMessage;
                _packageRepo.DeletePackage(id, Utility.GetCookie("UserId_admin").ToIntOrZero(), out outFlag, out outMessage);

                if (outFlag == 0)
                {
                    return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        [HttpPost]
        public ActionResult AddEditPackage(int? id)
        {
            var model = new UserPackageViewModel();

            if (id.HasValue)
            {
                model = _packageRepo.GetPackageById(id.Value);
            }

            return PartialView("_addEditPackage", model);

        }

        [HttpPost]
        public async Task<ActionResult> SaveOrUpdatePackage(UserPackageViewModel model)
        {
            try
            {
                int outPackageId;
                int outFlag;
                string outMessage;
                model.CreatedBy = Utility.GetCookie("UserId_admin").ToIntOrZero();
                _packageRepo.UpsertPackage(model, out outFlag, out outMessage, out outPackageId);

                if (outFlag == 0)
                {
                    return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult GetUserHavePackage(int packageId)
        {
            var user = _packageRepo.GetUserHavePackage(packageId);

            return Json(new { recordsFiltered = user.Count, recordsTotal = user.Count, data = user }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdatePackage(UserPackageViewModel package, string actionName = "")
        {
            try
            {
                package.LoggedUserId = Utility.GetCookie("UserId_admin").ToIntOrZero();
                var result = _userRepo.UpdateUserPackage_v1(package, actionName);
                if (result == 1)
                {
                    return new ActionState { Message = "Done!", Data = "Package updated successfully", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else if (result == 9)
                {
                    return new ActionState { Message = "Failed!", Data = "Some error occurred.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                return new ActionState { Message = "Failed!", Data = "No User Found.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult OpenPackageModal(UserPackageViewModel modal)
        {
            var package = _packageRepo.GetPackageByUserId(modal.UserId);
            if (package == null)
            {
                package = modal;
            }
            package.LoggedUserId = Utility.GetCookie("UserId_admin").ToIntOrZero();
            return PartialView("_upgradePackage", package);
        }

        [HttpPost]
        public ActionResult GetPackageHistoryByUserId(int UserId)
        {
            var history = _packageRepo.GetUserPackagehistoryByUserId(UserId);
            return Json(new { recordsFiltered = history.Count, recordsTotal = history.Count, data = history }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Country

        [HttpPost]
        public ActionResult GetAllCountry()
        {
            var country = _countryRepo.GetCountries("BharatTouch/CountryIndex/GetAllCountry");

            return Json(new { recordsFiltered = country.Count, recordsTotal = country.Count, data = country }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteCountry(int id)
        {
            try
            {
                int outFlag;
                string outMessage;
                _countryRepo.SoftDeleteCountry(id, Utility.GetCookie("UserId_admin").ToIntOrZero(), out outFlag, out outMessage);
                if (outFlag == 0)
                {
                    return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        [HttpPost]
        public ActionResult AddEditCountry(int? id)
        {
            var model = new CountryModel();

            if (id.HasValue)
            {
                model = _countryRepo.GetCountryById(id.Value);
            }

            return PartialView("_addEditCountry", model);

        }

        [HttpPost]
        public async Task<ActionResult> SaveOrUpdateCountry(CountryModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                model.CreatedBy = Utility.GetCookie("UserId_admin").ToIntOrZero();
                _countryRepo.UpsertCountry(model, out outFlag, out outMessage);

                if (outFlag == 0)
                {
                    return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #endregion

        #region DiscountCoupon

        [AuthenticateAdmin_Admin]
        public ActionResult DiscountCouponIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetDiscountCouponList()
        {
            var pagination = new PaginationModel();
            pagination.Page = Utility.PageNumber();
            pagination.Size = Utility.PageSize();
            pagination.SortBy = Utility.SortBy();
            pagination.SortOrder = Utility.SortDesc();
            pagination.SearchText = Utility.FilterText();
            var list = _paymentRepo.DiscountCouponFetchAll(pagination, "BharatTouch/DiscountCouponIndex/GetDiscountCouponList");

            return Json(new { recordsFiltered = list.Count, recordsTotal = list.Count, data = list }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDiscountCouponById(int id)
        {
            try
            {
                var result = _paymentRepo.DeleteDiscountCoupon(id, "BharatTouch/DiscountCouponIndex/DeleteDiscountCouponById");

                return new ActionState()
                {
                    Success = result == 1,
                    Message = result == 1 ? "Operation successfull." : result == 0 ? "Discount coupon not found." : "Server error.",
                    Data = null,
                    Type = result == 1 ? ActionState.SuccessType : ActionState.ErrorType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult AddEditDiscountCouponModal(int? id)
        {
            var model = new DiscountCouponModel();

            if (!id.HasValue)
            {
                return PartialView("_addEditDiscountCoupon", model);
            }

            var discountCouponModel = _paymentRepo.GetDiscountCouponById(id.Value, "BharatTouch/DiscountCouponIndex/AddEditDiscountCouponModal");

            if (discountCouponModel == null)
            {
                return PartialView("_addEditDiscountCoupon", model);
            }

            return PartialView("_addEditDiscountCoupon", discountCouponModel);
        }

        [HttpPost]
        public ActionResult SaveOrUpdateDiscountCoupon(DiscountCouponModel model)
        {
            try
            {
                if (model.CreatedBy == 0)
                {
                    model.CreatedBy = Utility.GetCookie("UserId_admin").ToIntOrZero();
                }
                var result = _paymentRepo.UpsertDiscountCoupon(model);
                if (result == 0)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState()
                {
                    Success = true,
                    Message = "Success!",
                    Data = "Discount coupon " + (result == model.DiscountCouponId ? "updated" : "added") + " successfully.",
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Payment
        public ActionResult OpenPaymentDetailsModal(int? UserId)
        {
            var model = new PaymentRepository().GetPaymentByUserId(UserId.Value);
            if (model == null)
            {
                model = new PaymentModel();
            }

            return PartialView("_paymentDetails", model);
        }
        #endregion

        #region Invoice

        private static string InvoiceItemRowGenerator(int index, string desc, decimal cost, decimal discount, decimal total)
        {
            string str = "<tr " + (Utility.IsOdd(index) ? "style='background:#fafafa;'" : "") + ">";
            str += "<td style='padding:12px 10px;'>" + index + "</td>";
            str += "<td style='padding:12px 10px;'>" + desc + "</td>";
            str += "<td style='padding:12px 10px; text-align:right;'>₹" + cost + "</td>";
            str += "<td style='padding:12px 10px; text-align:right;'>₹" + discount + "</td>";
            str += "<td style='padding:12px 10px; text-align:right;'>₹" + total + "</td>";
            str += "</tr>";
            return str;
        }

        [HttpPost]
        public ActionResult GenerateInvoice(int InvoiceId)
        {
            try
            {
                var model = _paymentRepo.FetchInvoiceById(InvoiceId);

                if (model == null)
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Invoice not found.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                string body = string.Empty;
                var root = AppDomain.CurrentDomain.BaseDirectory;
                string rowsHtml = "";
                var referalDiscount = 0m;
                var packageName = model.PackageName;
                if (model.IsValidReferral)
                {
                    referalDiscount = Math.Round(model.PackageCost * 0.05m, 2);
                    packageName = packageName + " (with 5% Referal Discount.)";
                }
                var finalPackageCost = Math.Round(model.PackageCost - referalDiscount, 2);
                int rowIndex = 1;
                rowsHtml += InvoiceItemRowGenerator(rowIndex, packageName, model.PackageCost, referalDiscount, finalPackageCost);
                if (model.DiscountCoupon.NullToString() != "")
                {
                    var discount = 0m;
                    if (model.PercentageOff != 0m)
                    {
                        discount = Math.Round(finalPackageCost * (model.PercentageOff / 100m), 2);
                    }
                    if (model.AmountOff != 0m)
                    {
                        discount = model.AmountOff;
                    }
                    rowIndex++;
                    rowsHtml += InvoiceItemRowGenerator(rowIndex, "Coupon (Discount on Plan)", 0, Math.Round(discount, 2), 0);
                }
                if (model.WantMetalCard)
                {
                    rowIndex++;
                    rowsHtml += InvoiceItemRowGenerator(rowIndex, "Metal Card", 2499, 0, 2499);
                }
                using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/Template_Invoice.html"))
                {
                    string readFile = reader.ReadToEnd();
                    string StrContent = string.Empty;
                    StrContent = readFile;
                    StrContent = StrContent.Replace("{{CustomerName}}", model.FullName);
                    StrContent = StrContent.Replace("{{CustomerAddress}}", model.BillingAddress1);
                    StrContent = StrContent.Replace("{{CustomerCity}}", model.BillingCity);
                    StrContent = StrContent.Replace("{{CustomerSate}}", model.BillingState);
                    StrContent = StrContent.Replace("{{CustomerCountry}}", model.BillingCountry);
                    StrContent = StrContent.Replace("{{CustomerZip}}", model.BillingZip);
                    StrContent = StrContent.Replace("{{InvoiceDate}}", DateTimeFormatter.ConvertToString(model.InvoiceDate, DateTimeFormatter.DefaultStyle));
                    StrContent = StrContent.Replace("{{InvoiceNumber}}", model.InvoiceNumber);
                    StrContent = StrContent.Replace("{{InvoiceItems}}", rowsHtml);
                    StrContent = StrContent.Replace("{{Subtotal}}", model.Subtotal.ToString("N2"));
                    StrContent = StrContent.Replace("{{Tax}}", model.TaxAmount.ToString("N2"));
                    StrContent = StrContent.Replace("{{ShippingFee}}", model.ShippingAmount.ToString("N2"));
                    StrContent = StrContent.Replace("{{Total}}", model.PaymentAmount.ToString("N2"));
                    body = StrContent.ToString();
                }

                return new ActionState() { Success = true, Message = "Operation successfully.", Data = body, OptionalValue = model.InvoiceNumber, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult SaveInvoicePdf(int InvoiceId)
        {
            var model = _paymentRepo.FetchInvoiceById(InvoiceId);

            if (model == null)
                return new ActionState() { Success = false, Message = "Failed!", Data = "Invoice not found.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

            var folderPath = Server.MapPath("~/GeneratedInvoices");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, "Invoice_" + model.InvoiceNumber + ".pdf");
            var pdfPath = "/GeneratedInvoices/Invoice_" + model.InvoiceNumber + ".pdf";
            if (Utility.IsExistFile("~" + pdfPath))
                return new ActionState() { Success = true, Message = "Operation successfully.", Data = pdfPath, OptionalValue = model.InvoiceNumber, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

            if (HttpContext.Request.Files.Count == 0)
                return new ActionState() { Success = false, Message = "Failed!", Data = "Pdf file not found.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

            var dbPath = Utility.SaveFile("/GeneratedInvoices", "Invoice_" + model.InvoiceNumber + ".pdf");
            if (string.IsNullOrEmpty(dbPath))
                return new ActionState() { Success = false, Message = "Failed!", Data = "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

            return new ActionState() { Success = true, Message = "Operation successfully.", Data = dbPath, OptionalValue = model.InvoiceNumber, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult SendInvoiceEmailToUser(int InvoiceId)
        {
            try
            {
                var model = _paymentRepo.FetchInvoiceById(InvoiceId);

                if (model == null)
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Invoice not found or some error occurred. please, try again later.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                var folderPath = Server.MapPath("~/GeneratedInvoices");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, "Invoice_" + model.InvoiceNumber + ".pdf");
                var pdfPath = "/GeneratedInvoices/Invoice_" + model.InvoiceNumber + ".pdf";

                if (!Utility.IsExistFile("~" + pdfPath))
                {
                    if (HttpContext.Request.Files.Count == 0)
                        return new ActionState() { Success = false, Message = "Failed!", Data = "Pdf file not found.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                    var dbPath = Utility.SaveFile("/GeneratedInvoices", "Invoice_" + model.InvoiceNumber + ".pdf");
                    if (string.IsNullOrEmpty(dbPath))
                        return new ActionState() { Success = false, Message = "Failed!", Data = "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }

                string outMessage;
                string body = string.Empty;
                var root = AppDomain.CurrentDomain.BaseDirectory;
                var html = "";
                using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/UserInvoiceEmail.html"))
                {
                    string readFile = reader.ReadToEnd();
                    string StrContent = string.Empty;
                    StrContent = readFile;
                    StrContent = StrContent.Replace("{FullName}", model.FullName);
                    html = StrContent.ToString();
                }
                var adminUsers = _userRepo.GetAdminUsers();
                var emails = ConfigValues.MailBcc;
                if (adminUsers.Count > 0)
                    emails = string.Join(",", adminUsers.Select(x => x.EmailId));

                var success = Utility.SendEmail(model.EmailId, "BharatTouch - Invoice.", html, out outMessage, "", emails, filePath);
                return new ActionState()
                {
                    Success = success,
                    Message = success ? "Success!" : "Failed!",
                    Data = outMessage,
                    Type = success ? ActionState.SuccessType : ActionState.ErrorType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }
        #endregion

        #region Ai

        [HttpPost]
        async public Task<ActionResult> AiTextGenerator(string inputText)
        {
            try
            {
                var huggingFaceClient = new HuggingFaceTextGenerator();
                string generatedText = await huggingFaceClient.FetchHuggingFaceResponse(inputText);
                if (generatedText == null)
                {
                    return new ActionState { Message = "Failed!", Data = "Failed to generate text.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState { Message = "Done!", Data = generatedText, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }
        #endregion

        #region Order

        [HttpPost]
        public ActionResult GetOrders()
        {
            var orders = _adminRepo.GetAllOrder_Admin();

            return Json(new { recordsFiltered = orders.Count, recordsTotal = orders.Count, data = orders }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveOrUpdateOrderPrintingStatus(int? orderId)
        {
            try
            {
                string msg;
                string outMessage;
                var result = _adminRepo.OrderPrinting_Admin(orderId.Value, "OrderInPrinting");
                if (result == false)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    var model = new OrderShippingModel();
                    model = _adminRepo.GetIsShippedById_Admin(orderId.Value);
                    var cardType = model.IncludeMetalCard ? "Metal" : "Plastic";
                    string body = orderInPrintingTemplate(model.UserFirstName, model.OrderNo, cardType, model.OrderDate.Split(' ')[0]);

                    var adminUsers = _userRepo.GetAdminUsers();
                    var emails = ConfigValues.MailBcc;
                    if (adminUsers.Count > 0)
                        emails = string.Join(",", adminUsers.Select(x => x.EmailId));

                    var emailRes = Utility.SendEmail(model.UserEmailId, "🖨️ Your Bharat Touch Card Is Now in Printing!", body, out outMessage, "", emails);

                    if (emailRes)
                    {
                        msg = "Record Updated successfully" + ", " + outMessage;
                    }
                    else
                    {
                        msg = "Record Updated successfully";
                    }

                    return new ActionState()
                    {
                        Success = true,
                        Message = "Success!",
                        Data = msg,
                        Type = ActionState.SuccessType
                    }.ToActionResult(HttpStatusCode.OK);
                }

            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult AddEditOrderShipped(int? orderId, int flag)
        {
            var model = new OrderShippingModel();

            var orders = _adminRepo.GetAllOrderShppingCompany_Admin("OrderShippingCompany");

            ViewBag.ShippingData = new SelectList(orders, "shippingcompanyid", "companyname");

            model.Shippingdate = DateTime.Now;
            model.OrderId = orderId.Value;

            if (flag == 1)
            {
                model = _adminRepo.GetIsShippedById_Admin(orderId.Value);
                if (model.Shippingdate == null)
                {
                    model.Shippingdate = DateTime.Now;
                }
            }

            return PartialView("_addEditOrderShipped", model);
        }

        [HttpPost]
        public ActionResult SaveOrUpdateOrderShipping(OrderShippingModel model)
        {
            try
            {
                string msg;
                string outMessage;

                var result = _adminRepo.SaveOrUpdateOrderShipping_Admin(model, "OrderShipped");
                if (result == false)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Some error occurred while order shipping.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    var orderModel = new OrderShippingModel();
                    orderModel = _adminRepo.GetIsShippedById_Admin(model.OrderId);

                    string body = orderIsShippingTemplate(orderModel.UserFirstName, orderModel.OrderNo, orderModel.CourierCompany, orderModel.Trackingnumber, orderModel.CourierCompanyUrl);

                    var adminUsers = _userRepo.GetAdminUsers();
                    var emails = ConfigValues.MailBcc;
                    if (adminUsers.Count > 0)
                        emails = string.Join(",", adminUsers.Select(x => x.EmailId));

                    var emailRes = Utility.SendEmail(orderModel.UserEmailId, "🚚 Your Bharat Touch Card Has Been Shipped!!", body, out outMessage, "", emails);

                    if (emailRes)
                    {
                        msg = "Order is Shipped successfully" + ", " + outMessage;
                    }
                    else
                    {
                        msg = "Order is Shipped successfully";
                    }
                    return new ActionState()
                    {
                        Success = true,
                        Message = "Success!",
                        Data = msg,
                        Type = ActionState.SuccessType
                    }.ToActionResult(HttpStatusCode.OK);
                }


            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public string orderInPrintingTemplate(string userFirstName, string orderNo, string cardType, string orderDate)
        {
            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/OrderInPrinting.html"))
            {
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("{userFirstName}", userFirstName);
                StrContent = StrContent.Replace("{orderNo}", orderNo);
                StrContent = StrContent.Replace("{cardType}", cardType);
                StrContent = StrContent.Replace("{orderDate}", orderDate);
                body = StrContent.ToString();
            }

            return body;
        }

        public string orderIsShippingTemplate(string userFirstName, string orderNo, string courierName, string trackingNumber, string trackingUrl)
        {
            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/OrderIsShipping.html"))
            {
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("{userFirstName}", userFirstName);
                StrContent = StrContent.Replace("{orderNo}", orderNo);
                StrContent = StrContent.Replace("{courierName}", courierName);
                StrContent = StrContent.Replace("{trackingNumber}", trackingNumber);
                StrContent = StrContent.Replace("{trackingUrl}", trackingUrl);
                body = StrContent.ToString();
            }

            return body;
        }



        #endregion

        [HttpPost]
        public ActionResult AddEditUserByAdmin(int? id)
        {
            var model = new UserByAdminModel();

            var cardColor = _adminRepo.GetAllNfcCardColor_Admin("NFCCardColor");
            var cardFinishColor = _adminRepo.GetAllNfcCardFinishColor_Admin("NFCCardColorFinish");

            var pagination = new PaginationModel();
            pagination.Page = 0;
            pagination.Size = 100;
            pagination.SortBy = "DiscountCouponId";
            pagination.SortOrder = "asc";
            pagination.SearchText = "";
            var list = _paymentRepo.DiscountCouponFetchAll(pagination, "BharatTouch/DiscountCouponIndex/GetDiscountCouponList");

            var modifiedList = list.Select(x => new
            {
                DiscountCouponText = $"{x.CouponName + "(" + x.Code + ")"}",
                DiscountCouponIdText = $"{x.DiscountCouponId + "," + x.PercentageOff + "," + x.AmountOff}"
            }).ToList();

            ViewBag.DiscountCouponList = new SelectList(modifiedList, "DiscountCouponIdText", "DiscountCouponText");

            ViewBag.NFCCardColor = new SelectList(cardColor, "cardcolorid", "color");
            ViewBag.NFCCardFinishColor = new SelectList(cardFinishColor, "cardfinishid", "finish");


            //if (id.HasValue)
            //{

            //}

            return PartialView("_addEditUserByAdmin", model);
        }

        [HttpPost]
        public ActionResult SaveOrUpdateUserByAdmin(UserByAdminModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                model.CreatedBy = Utility.GetCookie("UserId_admin").ToIntOrZero();
                if (model.DiscountCouponIdText != null)
                {
                    model.DiscountCouponIdText = model.DiscountCouponIdText.Split(',')[0];
                }
                var result = _adminRepo.SaveOrUpdateUserByAdmin_Admin(model, out outFlag, out outMessage, "UserUpsert");
                if (result == false)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = outMessage, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState()
                {
                    Success = true,
                    Message = "Success!",
                    Data = outMessage,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #region Bulk Order

        [HttpPost]
        public ActionResult GetAllBulkOrder()
        {
            var orders = _adminRepo.GetAllBulkOrder_Admin("getAllBulkOrder");

            return Json(new { recordsFiltered = orders.Count, recordsTotal = orders.Count, data = orders }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Company

        [HttpPost]
        public ActionResult GetAllCompany()
        {

            int totalRows;
            var data = _adminRepo.GetAllCompany_Admin(out totalRows);

            return Json(new { recordsFiltered = totalRows, recordsTotal = totalRows, data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteCompany(int id)
        {
            try
            {
                int outFlag;
                string outMessage;
                _adminRepo.CompanyDelete_Admin(id, out outFlag, out outMessage);

                if (outFlag == 0)
                {
                    return new ActionState { Message = outMessage, Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = outMessage, Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        [HttpPost]
        public ActionResult AddEditCompany(int? id)
        {
            var model = new CompanyViewModel();

            if (id.HasValue)
            {
                model = _adminRepo.GetCompanyById_Admin(id.Value);
            }

            return PartialView("_addEditCompany", model);

        }

        public ActionResult CheckUserDisplayNameAvailability(string name)
        {

            if (new CompanyRepository().IsExistCompanyUserDisplayName(name, "BharatTouch/admin/CheckUserDisplayNameAvailability"))
            {
                return new ActionState { Message = "Display Name already exists!", Data = "1", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            else
                return new ActionState { Message = "", Data = "0", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

        }

        // [ValidateInput(false)]
        [HttpPost]
        public async Task<ActionResult> SaveOrUpdateCompany(CompanyModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                model.CreatedBy = Utility.GetCookie("UserId_admin").ToIntOrZero();
                var loggedName = Utility.GetCookie("UserName_admin").NullToString();
                var loggedEmail = Utility.GetCookie("EmailId_admin").NullToString();
                _adminRepo.SaveOrUpdateCompany_Admin(model, out outFlag, out outMessage);

                if (outFlag == 0)
                {
                    if (model.CompanyId == 0)
                    {
                        var userLink = "";
                        userLink = ConfigurationManager.AppSettings["WebUrl"].NullToString();
                        var userDisplayName = model.CompanyDisplayName + "/" + model.AdminDisplayName;
                        var companyAdminNameAndEmail = loggedName + "/" + loggedEmail;
                        userLink = userLink + userDisplayName;
                        QueueVerificationEmailAsync(model.AdminEmail, model.AdminFirstname + " " + model.AdminLastname.NullToString(), "Welcome*123", "CompanyAdminUpsert");
                        CompanyAdminEmailToSuperAdmin(model.AdminFirstname + " " + model.AdminLastname.NullToString(), model.AdminEmail, userDisplayName, model.CompanyName, userLink, companyAdminNameAndEmail, "Admin/SaveOrUpdateCompany");
                    }

                    return new ActionState { Message = "Done!", Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = "Failed!", Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public void QueueVerificationEmailAsync(string toEmail, string username, string password, string pageName = "")
        {
            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            {
                try
                {
                    var result = await VerificationEmailAsync(toEmail, username, password, pageName);

                }
                catch (Exception ex)
                {
                    // Catch all other exceptions
                    Trace.TraceError($"[VerificationEmailAsync] Unexpected error for {toEmail}. Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                }
            });
        }

        public async Task<ActionState> VerificationEmailAsync(string toEmail, string username, string password, string pageName = "")
        {

            var activationLink = ConfigurationManager.AppSettings["WebUrl"].ToString() + "admin/adminverification?email=" + toEmail;

            string body = CompanyAdminVerificationEmail(username, activationLink, toEmail, password, pageName);

            var isSuccess = await Utility.SendEmailAsync(toEmail, "Welcome to BharatTouch – Your Company Account Activation Details", body);

            return new ActionState { Success = isSuccess, Message = isSuccess ? "Email Send" : "Error in sending email" };
        }

        public string CompanyAdminVerificationEmail(string userName, string activationLink, string emailID, string password, string pageName = "")
        {
            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/CompanyAdminVerificationEmail.html"))
            {
                string readFile = reader.ReadToEnd();
                string StrContent = string.Empty;
                StrContent = readFile;
                //Assing the field values in the template
                StrContent = StrContent.Replace("{FullName}", userName);
                StrContent = StrContent.Replace("{ActivationLink}", activationLink);
                StrContent = StrContent.Replace("{EmailID}", emailID);
                StrContent = StrContent.Replace("{Password}", password);
                StrContent = StrContent.Replace("{pageName}", pageName);
                body = StrContent.ToString();
            }

            return body;
        }

        public void CompanyAdminEmailToSuperAdmin(string name, string email, string displayName, string companyName, string userProfileLink, string companyAdminNameAndEmail, string pageName = "")
        {
            string outMessage;

            var adminUsers = _userRepo.GetAdminUsers();

            if (adminUsers.Count > 0)
            {
                foreach (var adminUser in adminUsers)
                {
                    string body = string.Empty;
                    var root = AppDomain.CurrentDomain.BaseDirectory;
                    using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/CompanyAdminEmail.html"))
                    {
                        string readFile = reader.ReadToEnd();
                        string StrContent = string.Empty;
                        StrContent = readFile;
                        //Assing the field values in the template
                        StrContent = StrContent.Replace("{DisplayName}", displayName);
                        StrContent = StrContent.Replace("{Name}", name);
                        StrContent = StrContent.Replace("{Email}", email);
                        StrContent = StrContent.Replace("{companyName}", companyName);
                        StrContent = StrContent.Replace("{userProfileLink}", userProfileLink);
                        StrContent = StrContent.Replace("{companyAdminNameAndEmail}", companyAdminNameAndEmail);
                        StrContent = StrContent.Replace("{dateTime}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        StrContent = StrContent.Replace("{pageName}", pageName);
                        body = StrContent.ToString();

                        HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                        {
                            try
                            {
                                var isSuccess = await Utility.SendEmailAsync(adminUser.EmailId, "New User Account Created by Company Admin on BharatTouch", body);
                            }
                            catch (Exception ex)
                            {
                                // Catch all other exceptions                                
                            }
                        });

                    }
                }
            }
        }


        [HttpGet]
        public ActionResult adminverification(string email)
        {
            bool isVerify = _userRepo.IsVerify(email, "BharatTouch/Admin/Verification");
            return View(isVerify);
        }
        #endregion

        #region Blog

        public ActionResult GetAllBlog()
        {
            var data = _adminRepo.GetAllBT_Blogs_Admin();

            return Json(new { recordsFiltered = data.Count(), recordsTotal = data.Count(), data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteBT_Blog(int id)
        {
            try
            {
                int outFlag;
                string outMessage;
                _adminRepo.BlogDelete_Admin(id, out outFlag, out outMessage);

                if (outFlag == 0)
                {
                    var path = ConfigValues.ImagePath.Substring(1) + "/BT_Blog/" + id + ".png";
                    var physicalPath = Server.MapPath(path);
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }

                    return new ActionState { Message = outMessage, Data = outMessage, Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
                }
                else
                {
                    return new ActionState { Message = outMessage, Data = outMessage, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);

                }
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        [HttpPost]
        public ActionResult AddEditBTBlog(int? id)
        {
            var model = new BT_BlogViewModel();

            if (id.HasValue)
            {
                model = _adminRepo.GetBT_BlogsById_Admin(id.Value);

                var imagePath = ConfigValues.ImagePath.Substring(1) + "/BT_Blog/" + model.BlogId + ".png";
                var path = Server.MapPath(imagePath);
                if (System.IO.File.Exists(path))
                {
                    model.BlogImage = imagePath;
                }

            }

            return PartialView("_addEditBT_Blog", model);

        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveOrUpdateBT_Blog()
        {
            try
            {
                var request = HttpContext.Request;

                var model = new BT_BlogModel
                {
                    BlogId = request.Form["BlogId"].ToIntOrZero(),
                    BlogTitle = request.Form["BlogTitle"],
                    BlogDescription = request.Form["BlogDescription"],
                    BlogKeywords = request.Form["BlogKeywords"],
                    IsActive = request.Form["IsActive"].ToBoolean(),
                    BlogTagLine = request.Form["BlogTagLine"],
                    CreatedBy = Utility.GetCookie("UserId_admin").ToIntOrZero()
                };

                int outBlogId;
                int outFlag;
                string outMessage;

                var file = request.Files["BlogImage"];

                if (file != null && file.ContentLength > 0)
                {
                    var fileExtension = Path.GetExtension(file.FileName)?.ToLower();

                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                    {
                        return new ActionState
                        {
                            Message = "Only JPG, JPEG, and PNG files are allowed.",
                            Data = "",
                            Success = false,
                            Type = ActionState.ErrorType
                        }.ToActionResult(HttpStatusCode.BadRequest);
                    }
                }


                _adminRepo.SaveOrUpdateBT_Blog_Admin(model, out outFlag, out outMessage, out outBlogId);

                if (outFlag == 0)
                {
                    if (outBlogId > 0)
                    {

                        if (file != null && file.ContentLength > 0)
                        {

                            Utility.SaveCompressImages(file, outBlogId.ToString(), ConfigValues.ImagePath.Substring(1) + "/BT_Blog/", 200);


                            return new ActionState
                            {
                                Message = "Done!",
                                Data = outMessage,
                                Success = true,
                                Type = ActionState.SuccessType
                            }.ToActionResult(HttpStatusCode.OK);
                        }

                    }

                }

                return new ActionState
                {
                    Message = "Done!",
                    Data = outMessage,
                    Success = true,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ActionState
                {
                    Message = "Failed!",
                    Data = ex.Message,
                    Success = false,
                    Type = ActionState.ErrorType
                }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public JsonResult DeleteBlogImage(int blogId)
        {
            try
            {
                var imagePath = ConfigValues.ImagePath.Substring(1) + "/BT_Blog/" + blogId + ".png";
                if (string.IsNullOrEmpty(imagePath))
                {
                    return Json(new { Success = false, Message = "No image specified.", Type = "error" });
                }

                var physicalPath = Server.MapPath(imagePath);

                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                    return Json(new { Success = true, Message = "Image deleted successfully.", Type = "success" });
                }
                else
                {
                    return Json(new { Success = false, Message = "Image file not found.", Type = "warning" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error deleting image: " + ex.Message, Type = "error" });
            }
        }

        #endregion

        #region Lead

        [HttpPost]
        public ActionResult GetOrderDetail(int orderId)
        {
            var model = new OrderAdminViewModel();
            if (orderId > 0)
                model = _adminRepo.GetOrderByOrderId(orderId);

            return PartialView("_orderDetails", model);
        }

        [HttpPost]
        public ActionResult SaveLeadConvertFromOrder(int orderId)
        {
            try
            {
                int outFlag;
                string outMessage;

                var userId = Utility.GetCookie("UserId_admin").ToIntOrZero();

                _adminRepo.Lead_ConvertFromOrder(orderId, userId, out outFlag, out outMessage);
                return new ActionState()
                {
                    Success = outFlag == 0,
                    Message = outFlag == 0 ? "Success" : "Error",
                    Data = outMessage,
                    Type = outFlag == 0 ? ActionState.SuccessType : ActionState.ErrorType
                }.ToActionResult(HttpStatusCode.OK);

            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult GetAllLeadsAdmin()
        {
            var dt = _adminRepo.GetAllLeads_Admin("BharatTouch/Admin/GetAllLeadsAdmin");

            return Json(new { recordsFiltered = dt.Count(), recordsTotal = dt.Count(), data = dt }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteLeadsAdmin(int id)
        {
            try
            {
                int outFlag; string outMessage;
                _adminRepo.DeleteLeads_Admin(id, out outFlag, out outMessage);
                return new ActionState { Message = outMessage, Success = outFlag == 0, Type = outFlag == 0 ? ActionState.SuccessType : ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        [HttpPost]
        public ActionResult AddEditLeadsAdmin(int? id)
        {
            var model = new LeadsAdminViewModel();

            ViewBag.LeadStatus = _adminRepo.GetAllLeadsStatus_Admin();
            //var allAdminUsers = _userRepo.GetAdminUsers();
            int totRows;
            var users = _adminRepo.GetAllUsers_Admin(0, 1000000, "", "", "", out totRows, "BharatTouch/Admin/AddEditLeadsAdmin");
            var filteredAdminUsers = users
        .Where(u => u.UserType == "SA" || u.UserType == "LA")
        .ToList();
            ViewBag.AdminUsers = filteredAdminUsers;

            if (id.HasValue)
            {
                model = _adminRepo.GetLeadById_Admin(id.Value);
            }
            else
            {
                model.AssignedTo = Utility.GetCookie("UserId_Admin").ToIntOrZero();
            }

            return PartialView("_addEditLeads", model);

        }

        [HttpPost]
        public ActionResult SaveOrUpdateLeads(LeadAdminModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                model.CreatedBy = Utility.GetCookie("UserId_admin").ToIntOrZero();

                var result = _adminRepo.Lead_CreateManual(model, out outFlag, out outMessage);
                if (result == false)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = outMessage, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState()
                {
                    Success = true,
                    Message = "Success!",
                    Data = outMessage,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        //[HttpPost]
        //public ActionResult AddEditLeadsCommunicationAdmin(int? id)
        //{
        //    var model = new LeadCommunicationModel();
        //    model.LeadId = id.Value;

        //    int totRows;
        //    var CommunicatedBy = _adminRepo.GetAllUsers_Admin(0, 10000, "", "", "", out totRows, "BharatTouch/Admin/AddEditLeadsCommunicationAdmin");

        //    var userSelectList = CommunicatedBy
        //        .Select(u => new SelectListItem
        //        {
        //            Value = u.UserId.ToString(),
        //            Text = (u.FirstName + " " + (u.LastName ?? "")).Trim()
        //        }).ToList();

        //    ViewBag.Users = userSelectList;


        //    var communicationTypes = _adminRepo.GetLeadCommunicationType_Admin();
        //    ViewBag.CommunicationTypes = new SelectList(communicationTypes, "CommunicationTypeId", "CommunicationType");

        //    if (id.HasValue)
        //    {
        //        model = _adminRepo.GetLeadCommunicationByLeadId_Admin(id.Value);
        //        model.LeadId = id.Value;
        //    }

        //    return PartialView("_addEditLeadsCommunication", model);
        //}

        //[HttpPost]
        //public ActionResult AddEditLeadsCommunicationAdmin(int leadId)
        //{
        //    var model = new LeadCommunicationModel();

        //    if (leadId > 0)
        //    {
        //        var dt = _adminRepo.GetLeadCommunicationByLeadId_Admin(leadId);
        //        if (dt != null)
        //        {
        //            model = dt;
        //        }
        //    }

        //    model.LeadId = leadId;

        //    int totRows;
        //    var allUsers = _adminRepo.GetAllUsers_Admin(0, 10000, "", "", "", out totRows, "Admin/AddEditLeadsCommunication");

        //    var communicationTypes = _adminRepo.GetLeadCommunicationType_Admin();

        //    ViewBag.Users = allUsers.Select(u => new SelectListItem
        //    {
        //        Value = u.UserId.ToString(),
        //        Text = (u.FirstName + " " + (u.LastName ?? "")).Trim(),
        //        Selected = (model.CommunicatedBy == u.UserId)
        //    }).ToList();

        //    ViewBag.CommunicationTypes = communicationTypes.Select(c => new SelectListItem
        //    {
        //        Value = c.CommunicationTypeId.ToString(),
        //        Text = c.CommunicationType,
        //        Selected = (model.CommunicationTypeId == c.CommunicationTypeId)
        //    }).ToList();

        //    ViewBag.FollowUpCommunicationTypes = communicationTypes.Select(c => new SelectListItem
        //    {
        //        Value = c.CommunicationTypeId.ToString(),
        //        Text = c.CommunicationType,
        //        Selected = (model.FollowUpCommunicationTypeId == c.CommunicationTypeId)
        //    }).ToList();

        //    return PartialView("_addEditLeadsCommunication", model);
        //}

        [HttpPost]
        public ActionResult SaveOrUpdateLeadsCommunication(LeadCommunicationModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                model.CreatedBy = Utility.GetCookie("UserId_admin").ToIntOrZero();

                var result = _adminRepo.LeadCommunication_Upsert(model, out outFlag, out outMessage);
                if (result == false)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = outMessage, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                }
                return new ActionState()
                {
                    Success = true,
                    Message = "Success!",
                    Data = outMessage,
                    Type = ActionState.SuccessType
                }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message ?? "Some error occurred.", Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult GetLeadCommunicationHtml(int leadId)
        {
            var model = _adminRepo.GetLeadCommunicationByLeadId_Admin(leadId);
            return PartialView("_leadCommunicationList", model); 
        }

        [HttpPost]
        public ActionResult DeleteLeadCommunication(int leadCommunicationId)
        {
            try
            {
                int outFlag; string outMessage;
                _adminRepo.DeleteLeadCommunication(leadCommunicationId, out outFlag, out outMessage);
                return new ActionState { Message = outMessage, Success = outFlag == 0, Type = outFlag == 0 ? ActionState.SuccessType : ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        #endregion

    }
}