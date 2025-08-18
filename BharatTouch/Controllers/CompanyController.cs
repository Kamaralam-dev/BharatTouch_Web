using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.ViewModels;
using BharatTouch.CommonHelper;

namespace BharatTouch.Controllers
{
    public class CompanyController : Controller
    {

        #region Declaration
        CompanyRepository _compRepo = new CompanyRepository();
        AdminRepository _adminRepo = new AdminRepository();
        YouTubeRepository _youTubeRepository = new YouTubeRepository();
        BlogRepository _blogRepository = new BlogRepository();
        UserRepository _userRepo = new UserRepository();
        CLientTestimonialRepository _clientTestimonialRepo = new CLientTestimonialRepository();
        #endregion

        #region View Page

        public ActionResult Login()
        {
            return View();
        }

        //public ActionResult CompanyIndex()
        //{
        //    var model = new CompanyViewModel();
        //    var companyId = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
        //    if (companyId > 0)
        //    {
        //        model = _adminRepo.GetCompanyById_Admin(companyId);
        //    }

        //    return View(model);
        //}

        public ActionResult CompanyIndex()
        {
            var model = new CompanyViewModel();
            var companyId = Utility.GetCookie("CompanyId_Company").ToIntOrZero();

            if (companyId > 0)
            {
                model = _adminRepo.GetCompanyById_Admin(companyId);
            }

            // Load countries and bind to ViewBag
            var countriesList = new DataAccess.Repository.CountryRepository().GetCountries("BharatTouch/EditProfile/EditProfile.cshtml");
            //ViewBag.Countries = countriesList.Select(n => new SelectListItem
            //{
            //    Text = $"{n.Country} ({n.NumberCode})",
            //    Value = $"{n.CountryId};{n.MinNumberLength};{n.MaxNumberLength}",
            //    Selected = n.CountryId == model.PhoneCountryId
            //}).ToList();
            ViewBag.CountryDataForJs = countriesList;

            return View(model);
        }


        public ActionResult UserIndex()
        {
            return View();
        }

        public ActionResult LeadIndex()
        {
            var com = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
            int totRows = 0;
            var users = _compRepo.GetAllUsersByCompany(com, out totRows, "BharatTouch/Company/LeadIndex/GetUsersByCompany");

            foreach (var u in users)
            {
                u.EmailId = CryptoHelper.IsEncrypted(u.EmailId) ? CryptoHelper.Decrypt(u.EmailId) : u.EmailId;
                u.Password = CryptoHelper.IsEncrypted(u.Password) ? CryptoHelper.Decrypt(u.Password) : u.Password;
                u.Phone = CryptoHelper.IsEncrypted(u.Phone) ? CryptoHelper.Decrypt(u.Phone) : u.Phone;
            }

            ViewBag.AllUsersByCompanyData = users;
            return View();
        }

        public ActionResult PageImpressionIndex()
        {
            var com = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
            int totRows = 0;
            var users = _compRepo.GetAllUsersByCompany(com, out totRows, "BharatTouch/Company/LeadIndex/GetUsersByCompany");

            ViewBag.AllUsersByCompanyData = users;
            return View();
        }

        public ActionResult Logout()
        {
            Utility.RemoveCookie("UserId_Company");
            Utility.RemoveCookie("UserName_Company");
            Utility.RemoveCookie("EmailId_Company");
            Utility.RemoveCookie("UserUrlCode_Company");
            Utility.RemoveCookie("UserType_Company");
            Utility.RemoveCookie("DisplayName_Company");
            Utility.RemoveCookie("IsCompanyAdmin_Company");
            return RedirectToAction("Login");
        }

        #endregion

        #region Login

        [HttpPost]
        public ActionResult AuthenticateAdmin(AdminModel model)
        {
            try
            {
                // check with plain credentials
                var companyPlainDetails = _compRepo.AuthenticateCompanyAdmin(model, "BharatTouch/company/login/authenticate");

                if (companyPlainDetails != null)
                {
                    if (BharatTouch.CommonHelper.CryptoHelper.IsEncrypted(companyPlainDetails.EmailId) == false)
                    {
                        UserModel userAllData = new UserModel();
                        userAllData = _userRepo.GetUserByCodeOrName(companyPlainDetails.Displayname, "BharatTouch/EditProfile/EditProfile");

                        //update as encrypted
                        var useremail = CryptoHelper.Encrypt(model.EmailId);

                        var userPassword = CryptoHelper.Encrypt(model.Password);

                        _userRepo.UpdateUserEncryptDetail(companyPlainDetails.UserId, useremail, userPassword, CryptoHelper.Encrypt(userAllData.PersonalEmail), CryptoHelper.Encrypt(userAllData.Phone), CryptoHelper.Encrypt(userAllData.Whatsapp), CryptoHelper.Encrypt(userAllData.WorkPhone), CryptoHelper.Encrypt(userAllData.OtherPhone));
                    }
                }

                //if user not exists with plain details then check agaian with encrypted details
                var encryptEmail = CryptoHelper.Encrypt(model.EmailId);

                var encryptPassword = CryptoHelper.Encrypt(model.Password);

                AdminModel encryptedModel = new AdminModel();
                encryptedModel.EmailId = encryptEmail;
                encryptedModel.Password = encryptPassword;

                var user = _compRepo.AuthenticateCompanyAdmin(encryptedModel, "BharatTouch/company/login/authenticate");
                if (user != null)
                {
                    Utility.SetCookie("UserId_Company", user.UserId.ToString());
                    Utility.SetCookie("UserName_Company", user.FirstName.NullToString() + " " + user.LastName.NullToString());
                    Utility.SetCookie("EmailId_Company", user.EmailId);
                    Utility.SetCookie("CompanyName_Company", user.CompanyName);
                    Utility.SetCookie("UserUrlCode_Company", user.UrlCode.NullToString());
                    Utility.SetCookie("UserType_Company", user.UserType.NullToString());
                    Utility.SetCookie("DisplayName_Company", user.Displayname.NullToString());
                    Utility.SetCookie("IsCompanyAdmin_Company", user.IsCompanyAdmin.NullToString());
                    Utility.SetCookie("CompanyId_Company", user.CompanyId.NullToString());
                    Utility.SetCookie("CompanyDisplayName_Company", user.CompanyDisplayName.NullToString());
                    var codeOrName = user.UrlCode;
                    if (!string.IsNullOrWhiteSpace(user.Displayname))
                    {
                        codeOrName = user.Displayname.NullToString();
                    }
                    Utility.SetCookie("UserUrlCodeDisplayName_Company", codeOrName);

                    if (user.CompanyId == 0 || !user.IsCompanyAdmin)
                    {
                        return new ActionState { Message = "Failed!", Data = "invalid login crediencial for company admin", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
                    }

                    return new ActionState { Message = "Congratulations!", Data = "Login successfully.", Success = true, Type = ActionState.SuccessType, OptionalValue = codeOrName }.ToActionResult(HttpStatusCode.OK);
                }
                else
                    return new ActionState { Message = "Failed!", Data = "Email or password is wrong or only company admin have access to login.", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        #endregion

        #region Company Index

        [HttpPost]
        public ActionResult SaveOrUpdateContactDetail(CompanyViewModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                model.CompanyId = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
                var result = _compRepo.Company_Contact_Upsert(model, out outFlag, out outMessage);
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

        [HttpPost]
        public ActionResult SaveOrUpdateSocialMediaDetail(CompanyViewModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                model.CompanyId = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
                var result = _compRepo.Company_SocialMedia_Upsert(model, out outFlag, out outMessage);
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

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveOrUpdateAboutDetail(CompanyViewModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                model.CompanyId = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
                var result = _compRepo.Company_About_Upsert(model, out outFlag, out outMessage);
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

        [HttpPost]
        public ActionResult SaveOrUpdateCoverImageDetail()
        {
            try
            {
                int outFlag = 0;
                string outMessage = "";
                var file = Request.Files["CoverImagepath"];
                var CID = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
                var model = new CompanyViewModel();
                model.CompanyId = CID;

                if (file != null && file.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg")
                    {

                        var uploadPath = Server.MapPath("~/Uploads/Company/CoverImage");

                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);


                        Utility.CompressImageinKbs(file, "/Uploads/Company/CoverImage/" + CID + ".png");

                        model.CoverImagepath = "/Uploads/Company/CoverImage/" + CID + ".png";

                        var status = _compRepo.Company_CoverImage_Upsert(model, out outFlag, out outMessage);
                        return Json(new
                        {
                            Success = outFlag == 0,
                            Data = outMessage,
                            Type = outFlag == 0 ? "success" : "Failed"
                        });

                    }
                    else
                    {
                        return new ActionState { Data = "Invalid File Format", Success = false }.ToActionResult(System.Net.HttpStatusCode.OK);

                    }
                }
                else
                {
                    return new ActionState { Data = "No file selected", Success = false }.ToActionResult(System.Net.HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return new ActionState { Data = ex.Message, Success = false }.ToActionResult(System.Net.HttpStatusCode.OK);
            }
        }

        [HttpPost]
        public ActionResult DeleteCompanyCoverImage()
        {
            var model = new CompanyViewModel();

            var CID = Utility.GetCookie("CompanyId_Company").ToIntOrZero();

            int outFlag = 1;
            string outMessage = "Cover Image Deleted Failed";
            model.CoverImagepath = "";
            model.CompanyId = CID;

            var path = Server.MapPath("~/Uploads/Company/CoverImage/" + CID + ".png");

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            var status = _compRepo.Company_CoverImage_Upsert(model, out outFlag, out outMessage);

            if (status)
            {
                outFlag = 0;
                outMessage = "Cover Image Deleted Successfully";
            }

            return Json(new
            {
                Success = outFlag == 0,
                Data = outMessage,
                Type = outFlag == 0 ? "success" : "Failed"
            });
        }

        [HttpPost]
        public ActionResult SaveOrUpdatePortfolioImages(HttpPostedFileBase[] PortfolioImage)
        {
            var CID = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
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
                        var uploadPath = Server.MapPath("~/Uploads/Portfolio/Company/" + CID);

                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);
                        var fileName = Guid.NewGuid().ToString("N") + ".png";
                        Utility.CompressImageinKbs(file, "/uploads/Portfolio/Company/" + CID + "/" + fileName);
                    }
                }
                return new ActionState { Message = "Done!", Data = "Images uploaded successfully!", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
        }

        public ActionResult BindPortfolioImages()
        {
            var CID = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
            return PartialView("_portfolioImages", CID);
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

        public ActionResult BindYouTubeMethod()
        {
            var userId = Utility.GetCookie("UserId_Company").ToIntOrZero();
            return PartialView("_youtubeList", userId);
        }

        public ActionResult OpenYouTubeModelMethod(int? id)
        {
            YouTubeModel model = new YouTubeModel();
            var userId = Utility.GetCookie("UserId_Company").ToIntOrZero();
            model.UserId = userId;
            if (id != null)
                model = _youTubeRepository.GetYouTubeById(id.Value, "BharatTouch/CompanyIndex/OpenYouTubeModel");

            return PartialView("_youtubeCreate", model);
        }

        [HttpPost]
        public ActionResult SaveYouTube(YouTubeModel model)
        {
            try
            {
                var userId = Utility.GetCookie("UserId_Company").ToIntOrZero();
                model.UserId = userId;
                var result = _youTubeRepository.UpsertYouTube(model, "BharatTouch/CompanyIndex/SaveYouTube");
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

        public ActionResult DeleteYouTubeMethod(int id)
        {
            try
            {
                var result = _youTubeRepository.DeleteYouTube(id, "BharatTouch/CompanyIndex/DeleteYouTube");
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


        public ActionResult BindBlogMethod()
        {
            var userId = Utility.GetCookie("UserId_Company").ToIntOrZero();
            return PartialView("_blogList", userId);
        }

        public ActionResult OpenBlogModelMethod(int? id)
        {
            BlogModel model = new BlogModel();
            var userId = Utility.GetCookie("UserId_Company").ToIntOrZero();
            model.UserId = userId;
            if (id != null)
                model = _blogRepository.GetBlogById(id.Value, "BharatTouch/CompanyIndex/OpenBlogModel");

            return PartialView("_blogCreate", model);
        }

        public ActionResult DeleteBlogMethod(int id)
        {
            try
            {
                _blogRepository.DeleteBlog(id, "BharatTouch/CompanyIndex/DeleteBlog");
                return new ActionState { Message = "Done!", Data = "Blog detail deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
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
                var userId = Utility.GetCookie("UserId_Company").ToIntOrZero();
                model.UserId = userId;
                var result = _blogRepository.UpsertBlog(model, "BharatTouch/CompanyIndex/SaveBlog");

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

        [HttpPost]
        public ActionResult ChangeCompanyAdminPassword(UserModel user)
        {
            try
            {
                var userId = Utility.GetCookie("UserId_Company").ToIntOrZero();
                var com = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
                user.UserId = userId;
                user.CompanyId = com;
                var result = _compRepo.ChangeCompanyAdminPassword(user, "Bharattouch/CompanyIndex/ChangePassword");
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
        public ActionResult UpsertClientTestimonial(ClientTestimonialModel model, HttpPostedFileBase CLientImagePath)
        {
            try
            {
                model.UserId = Utility.GetCookie("UserId_Company").ToIntOrZero();
                var CID = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
                if (CLientImagePath != null && CLientImagePath.ContentLength > 0)
                {
                    var fileName = model.Client_Id == 0 ? DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + model.UserId : model.Client_Id.ToString();
                    var uploadres = Utility.SaveCompressImages(CLientImagePath, fileName, ConfigValues.ImagePath.Substring(1) + "/ClientTestimonial/Company/" + CID, 200);
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
                var result = _clientTestimonialRepo.UpsertClientTestimonial(model, "BharatTouch/Company/CompanyIndex/UpsertClientTestimonial");
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

        public ActionResult BindClientTestimonials()
        {
            var userId = Utility.GetCookie("UserId_Company").ToIntOrZero();
            return PartialView("_clientTestimonialList", userId);
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
        #endregion

        #region User Index

        [HttpPost]
        public ActionResult GetUsersByCompany()
        {
            var com = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
            int totRows = 0;
            var users = _compRepo.GetAllUsersByCompany(com, out totRows, "BharatTouch/Company/UserIndex/GetUsersByCompany");
            foreach (var u in users)
            {
                u.EmailId = CryptoHelper.IsEncrypted(u.EmailId) ? CryptoHelper.Decrypt(u.EmailId) : u.EmailId;
                u.Password = CryptoHelper.IsEncrypted(u.Password) ? CryptoHelper.Decrypt(u.Password) : u.Password;
                u.Phone = CryptoHelper.IsEncrypted(u.Phone) ? CryptoHelper.Decrypt(u.Phone) : u.Phone;
            }

            return Json(new { recordsFiltered = totRows, recordsTotal = totRows, data = users }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddEditUserByCompany(int? id)
        {
            var model = new UserModel();
            var com = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
            var comdisplayname = Utility.GetCookie("CompanyDisplayName_Company");
            model.CompanyId = com;
            model.CompanyDisplayName = comdisplayname;
            if (id.HasValue)
            {
                model = _userRepo.GetUserById(id.Value);
            }

            return PartialView("_addEditUserByCompany", model);

        }

        public ActionResult CheckCompanyUserDisplayNameAvailability(string name)
        {

            if (_compRepo.IsExistCompanyUserDisplayName(name, "BharatTouch/Company/CheckCompanyUserDisplayNameAvailability"))
            {
                return new ActionState { Message = "Display Name already exists!", Data = "1", Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }
            else
                return new ActionState { Message = "", Data = "0", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);

        }

        [HttpPost]
        public async Task<ActionResult> SaveOrUpdateUserByCompany(UserModel model)
        {
            try
            {
                int OutUserId;
                int outFlag;
                string outMessage;

                var com = Utility.GetCookie("CompanyId_Company").ToIntOrZero();
                var companyUserName = Utility.GetCookie("UserName_Company").NullToString();
                var companyUserEmail = Utility.GetCookie("EmailId_Company").NullToString();
                var CompanyName = Utility.GetCookie("CompanyName_Company").NullToString();

                model.CompanyId = com;
                model.EmailId = CryptoHelper.Encrypt(model.EmailId);
                model.Phone = CryptoHelper.Encrypt(model.Phone);

                _compRepo.Company_User_Upsert(model, out outFlag, out outMessage, out OutUserId);

                if (outFlag == 0)
                {
                    if (model.UserId == 0)
                    {
                        var userLink = "";
                        userLink = ConfigurationManager.AppSettings["WebUrl"].NullToString();
                        userLink = userLink + model.CompanyDisplayName +"/"+ model.Displayname;
                        QueueVerificationAdminEmailAsync(model.EmailId, model.FirstName + " " + model.LastName.NullToString(), "Welcome*123", "Company/SaveOrUpdateUserByCompany");
                        CompanyUserEmailToAdmin(model.FirstName, model.EmailId, model.Displayname, CompanyName, userLink, companyUserName + "/" + companyUserEmail, "Company/SaveOrUpdateUserByCompany");
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


        public void QueueVerificationAdminEmailAsync(string toEmail, string username, string password, string pageName = "")
        {
            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            {
                try
                {
                    var result = await VerificationAdminEmailAsync(toEmail, username, password, pageName);

                }
                catch (Exception ex)
                {
                    // Catch all other exceptions
                    Trace.TraceError($"[VerificationEmailAsync] Unexpected error for {toEmail}. Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                }
            });
        }

        public async Task<ActionState> VerificationAdminEmailAsync(string toEmail, string username, string password, string pageName = "")
        {

            var activationLink = ConfigurationManager.AppSettings["WebUrl"].ToString() + "admin/adminverification?email=" + toEmail;

            string body = CompanyAdminVerificationEmail(username, activationLink, toEmail, password, pageName);

            var isSuccess = await Utility.SendEmailAsync(toEmail, "Welcome to BharatTouch – Your Digital Profile is Ready!", body);

            return new ActionState { Success = isSuccess, Message = isSuccess ? "Email Send" : "Error in sending email" };
        }

        public string CompanyAdminVerificationEmail(string userName, string activationLink, string emailID, string password, string pageName = "")
        {
            string body = string.Empty;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/CompanyUserEmail.html"))
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

        public void CompanyUserEmailToAdmin(string name, string email, string displayName,string companyName, string userProfileLink,string companyAdminNameAndEmail, string pageName = "")
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
                        StrContent = StrContent.Replace("{userProfileLink}",  userProfileLink);
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

        [HttpPost]
        public ActionResult DeleteUserByCompany(int id)
        {
            try
            {
                _userRepo.DeleteUser(id, "BharatTouch/Company/UserIndex/Delete");
                return new ActionState { Message = "Done!", Data = "User deleted successfully.", Success = true, Type = ActionState.SuccessType }.ToActionResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState { Message = "Failed!", Data = ex.Message, Success = false, Type = ActionState.ErrorType }.ToActionResult(HttpStatusCode.OK);
            }

        }

        #endregion

        #region Lead Index

        [HttpPost]
        public ActionResult GetLeadsByUser(int userId)
        {
            int totRows = 0;
            var leads = _userRepo.FetchLeadsByUserId(userId, Utility.StartIndex(), Utility.PageSize(), Utility.SortBy(), Utility.SortDesc(), Utility.FilterText(), out totRows, "BharatTouch/Company/LeadIndex/GetLeadsByUser");

            return Json(new { recordsFiltered = totRows, recordsTotal = totRows, data = leads }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region PageImpressionIndex

        [HttpPost]
        public ActionResult GetViewHistoryByCompanyUser(int userId)
        {
            int totRows = 0;
            var leads = _compRepo.GetViewHistoryByCompanyUser(userId, out totRows);
            ViewBag.PageViewed = totRows;
            return Json(new { recordsFiltered = totRows, recordsTotal = totRows, data = leads }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}