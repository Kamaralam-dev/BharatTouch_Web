using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using System.Windows.Interop;
using BharatTouch.CommonHelper;
using BharatTouch.JwtTokens;
using DataAccess;
using DataAccess.AdminApiDto;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.ViewModels;
using GroopGo.Api.WebHelper;
using Hangfire;
using Org.BouncyCastle.Ocsp;
using Razorpay.Api;
using BharatTouch.CommonHelper;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AdminUsersApiController : ApiController
    {
        #region Declaration
        AdminRepository _adminRepo = new AdminRepository();
        UserRepository _userRepo = new UserRepository();
        PackagesRepository _packageRepo = new PackagesRepository();
        PaymentRepository _paymentRepo = new PaymentRepository();
        SocialRepository _socialRepo = new SocialRepository();
        CommonRepository _commonRepo = new CommonRepository();
        CountryRepository _countryRepo = new CountryRepository();
        EducationRepository _educationRepo = new EducationRepository();
        ProfessionalRepository _professionalRepo = new ProfessionalRepository();
        YouTubeRepository _youTubeRepository = new YouTubeRepository();
        BlogRepository _blogRepository = new BlogRepository();
        ProfileTemplateRepository _profTempRepo = new ProfileTemplateRepository();
        CLientTestimonialRepository _testimonialRepo = new CLientTestimonialRepository();
        CertificationRepository _certificationRepo = new CertificationRepository();
        ScheduleAndMeetingRepository _schedulingRepo = new ScheduleAndMeetingRepository();
        DeviceTokenRepository _deviceTokenRepo = new DeviceTokenRepository();
        #endregion

        #region User

        [HttpPost]
        [Route("api/v1/Admin/Users/SignUp")]
        public ResponseModel SignUpUser([FromBody] SignUpModel model)
        {
            try
            {
                int newUserId = 0;
                int newOrderId = 0;
                if (model.UserType == null || model.UserType.NullToString() == "")
                {
                    model.UserType = "IN";
                }

                var isExist = new CompanyRepository().IsExistCompanyUserDisplayName(model.Displayname, "api/v1/Admin/Users/SignUp");
                if (isExist)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Display Name already exists!.", Data = "1" };
                }


                UserModel user = new UserModel();
                user.UserType = model.UserType;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.EmailId = model.EmailId;
                user.Phone = model.Phone;
                user.Password = model.Password;
                user.CountryId = model.CountryId;
                user.Displayname = model.Displayname;
                user.ReferredByCode = model.ReferredByCode;
                user.UserId = 0;

                if (_userRepo.UpsertUser_V2(user, out newUserId, out newOrderId, "api/v1/Admin/Users/SignUp") == 1)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Email already exists!" };
                }
                else
                {
                    try
                    {
                        var res = QueueVerificationSignupEmailAsync(model.EmailId, model.FirstName + " " + model.LastName.NullToString(), model.Password, model.Displayname, "Signup");
                    }
                    catch (Exception ex)
                    {
                    }


                    try
                    {
                        var a = SignUpEmailToAdmin(model.FirstName + " " + model.LastName.NullToString(), model.EmailId, model.UserType, model.Displayname, newUserId, "Signup Modal Home");
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
                    return new ResponseModel() { IsSuccess = true, Message = "A verification email will be sent to your email address. Please verify it to activate your account.", Data = new { UserId = newUserId, OrderId = newOrderId } };
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #region SignUp Email


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

        public async Task<bool> QueueVerificationSignupEmailAsync(string toEmail, string username, string password, string displayName, string pageName = "")
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
                BackgroundJob.Enqueue(() => VerificationSignupEmailAsync(toEmail, username, password, displayName, adminEmails, pageName));


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

        public async Task<ActionState> VerificationSignupEmailAsync(string toEmail, string username, string password, string displayNameOrCode, string adminEmails, string pageName = "")
        {
            //toEmail = "kamaralamcp@gmail.com";
            //adminEmails = "alam@rnaura.com";

            var outMessage = "";
            var activationLink = ConfigurationManager.AppSettings["WebUrl"].ToString() + "users/verification?email=" + toEmail;
            var profileLink = ConfigurationManager.AppSettings["WebUrl"].ToString() + "/" + displayNameOrCode;

            string body = SignupTemplate(username, activationLink, toEmail, password, profileLink, pageName);

            System.Threading.Thread.Sleep(2000);

            var isSuccess = Utility.SendEmail(toEmail, "Bharat Touch - Email verification.", body, out outMessage, adminEmails);

            //var isSuccess = await Utility.SendEmailAsync(toEmail, "Bharat Touch - Email verification.", body, "", adminEmails);

            return new ActionState { Success = isSuccess, Message = isSuccess ? "Email Send" : "Error in sending email" };
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



        #endregion

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/Get_all_users")]
        public ResponseModel GetAllUsers([FromBody] PaginationModel pagination)
        {
            try
            {
                int TotalRow;
                var list = _adminRepo.GetAllUsers_Admin(pagination.Page, pagination.Size, pagination.SortBy, pagination.SortOrder, pagination.SearchText, out TotalRow, "BharatTouch/AdminUsersApi/Get_all_users");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Users/GetUserDetails/{id}")]
        public ResponseModel GetUserDetails(int id)
        {
            try
            {
                var model = _userRepo.GetUserById(id, "BharatTouch/AdminUsersApi/GetUserDetails");
                if (model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "User not found.", Data = null };
                }
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Users/UserCompleteProfileById/{UserId}")]
        public ResponseModel UserCompleteProfileById(int UserId)
        {
            try
            {
                var model = _adminRepo.UserCompleteProfile(UserId, "BharatTouch/AdminUsersApi/UserCompleteProfileById");
                if (model == null)
                    return new ResponseModel() { IsSuccess = false, Message = "User not found.", Data = null };

                var p = model.Package;
                model.Gallery = Utility.GetFilesFromFolder("/uploads/portfolio/" + UserId);
                model.ShowHideSections.PersonalInfo = true;
                model.ShowHideSections.UploadSection = true;
                model.ShowHideSections.Contact = true;
                model.ShowHideSections.SocialMedia = true;
                model.ShowHideSections.About = true;
                model.ShowHideSections.Gallery = true;
                model.ShowHideSections.Education = true;
                model.ShowHideSections.Experiencee = true;
                model.ShowHideSections.TrainingCertification = true;
                model.ShowHideSections.ClientTestimonials = true;
                model.ShowHideSections.Teams = true;
                model.ShowHideSections.YoutubeVideos = true;
                model.ShowHideSections.PaymentQR = true;
                model.ShowHideSections.Blogs = true;
                model.ShowHideSections.Adhaar = true;
                model.ShowHideSections.MeetingRequest = true;
                model.ShowHideSections.ProfileTemplates = true;
                model.ShowHideSections.ProfileAnalytics = true;
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/Admin/Users/AuthenticateUser")]
        public ResponseModel AuthenticateUser([FromBody] AuthenticateUserDto body)
        {
            try
            {
                //var model = new AdminModel() { EmailId = body.EmailId, Password = body.Password };
                var user = _adminRepo.AuthenticateAdminMobile(body, "BharatTouch/AdminUsersApi/AuthenticateUser");
                if (user == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Email or password was wrong.", Data = null };
                }
                var jwtToken = TokenManager.GenerateJWTAuthetication(user);
                var tokenClaim = TokenManager.ValidateToken(jwtToken);
                if (string.IsNullOrEmpty(tokenClaim.UserId.NullToString()) || tokenClaim.UserId != user.UserId)
                {
                    return new ResponseModel { IsSuccess = false, Message = "Unauthorized login attempt" };
                }

                user.AuthToken = jwtToken;
                return new ResponseModel() { IsSuccess = true, Message = "User Authenticated.", Data = user };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/Admin/Users/AuthenticateAppUser")]
        public ResponseModel AuthenticateAppUser([FromBody] AuthenticateAppUserDto body)
        {
            try
            {
                //var model = new AdminModel() { EmailId = body.EmailId, Password = body.Password };
                var user = _adminRepo.AuthenticateAppUser(body, "BharatTouch/AdminUsersApi/AuthenticateUser");
                if (user == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Email or password was wrong.", Data = null };
                }

                AdminModel adminModel = new AdminModel();
                adminModel.EmailId = user.EmailId;
                adminModel.FirstName = user.FirstName;
                adminModel.LastName = user.LastName;
                adminModel.UserId = user.UserId;
                adminModel.Displayname = user.Displayname;
                adminModel.IsAdmin = user.IsAdmin;

                var jwtToken = TokenManager.GenerateJWTAuthetication(adminModel);
                var tokenClaim = TokenManager.ValidateToken(jwtToken);
                if (string.IsNullOrEmpty(tokenClaim.UserId.NullToString()) || tokenClaim.UserId != user.UserId)
                {
                    return new ResponseModel { IsSuccess = false, Message = "Unauthorized login attempt" };
                }

                user.AuthToken = jwtToken;
                return new ResponseModel() { IsSuccess = true, Message = "User Authenticated.", Data = user };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/DeleteUser/{UserId}")]
        public ResponseModel DeleteUser(int UserId)
        {
            try
            {
                _adminRepo.DeleteUser_Admin(UserId, "BharatTouch/AdminUsersApi/DeleteUser");
                return new ResponseModel() { IsSuccess = true, Message = "User deleted successfully.", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpdateUserActiveStatus")]
        public ResponseModel UpdateUserActiveStatus([FromBody] UpdateUserActiveStatusDto body)
        {
            try
            {
                var model = new UserModel() { UserId = body.UserId, IsActive = body.IsActive };
                var result = _userRepo.UpdateActiveStatus(model, "BharatTouch/AdminUsersApi/UpdateUserActiveStatus");
                var message = "";
                if (result == 0)
                    message = "User Not Found.";
                else if (result == 1)
                    message = "Active status updated successfully.";
                else
                    message = "Server Error.";

                return new ResponseModel() { IsSuccess = result == 1, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpdatePersonalInfo")]
        public ResponseModel UpdatePersonalInfo()
        {
            try
            {
                var req = HttpContext.Current.Request;
                var Server = HttpContext.Current.Server;
                var userId = req.Form["UserId"].ToIntOrZero();
                var docDirectoryPath = "/uploads/pdffiles/" + userId;

                var model = new UserModel();
                model.UserId = userId;
                model.Company = req.Form["Company"];
                model.CompanyTypeId = req.Form["CompanyTypeId"].ToIntOrZero();
                model.CompanyType = req.Form["CompanyType"].NullToString();
                model.FirstName = req.Form["FirstName"].NullToString();
                model.LastName = req.Form["LastName"].NullToString();
                model.CurrentDesignation = req.Form["CurrentDesignation"].NullToString();
                model.Tagline = req.Form["Tagline"].NullToString();
                model.BirthDate = DateTimeFormatter.ConvertToDateTime(req.Form["BirthDate"].NullToString());
                model.Gender = req.Form["Gender"].NullToString();
                model.Website = req.Form["Website"].NullToString();
                model.Displayname = req.Form["Displayname"].NullToString();
                model.UrlCode = req.Form["UrlCode"].NullToString();
                var codeOrNameString = model.UrlCode.NullToString() == "" ? model.Displayname : model.UrlCode;
                model.PortfolioLink = req.Form["PortfolioLink"].NullToString();
                var PortfolioLink = req.Files["PortfolioLink"];

                if (PortfolioLink != null && PortfolioLink.ContentLength > 0)
                {
                    string portfoliofileName;
                    portfoliofileName = PortfolioLink.FileName;
                    var newFileName = "Porfolio_" + codeOrNameString + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(portfoliofileName);

                    string fileExtension = Path.GetExtension(portfoliofileName).ToLower();
                    if (fileExtension != ".pdf")
                        return new ResponseModel { IsSuccess = false, Message = "Portfolio- Only pdf files allowed.", Data = null };

                    var folderPath = Server.MapPath("~" + docDirectoryPath);
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var dbPath = Path.Combine(folderPath, newFileName);
                    PortfolioLink.SaveAs(dbPath);
                    model.PortfolioLink = docDirectoryPath + "/" + newFileName;
                }

                model.ServicesLink = req.Form["ServicesLink"].NullToString();
                var ServicesLink = req.Files["ServicesLink"];

                if (ServicesLink != null && ServicesLink.ContentLength > 0)
                {
                    string servicefileName;
                    servicefileName = ServicesLink.FileName;
                    string fileExtension = Path.GetExtension(servicefileName).ToLower();
                    if (fileExtension != ".pdf")
                        return new ResponseModel { IsSuccess = false, Message = "Services- Only pdf files allowed.", Data = null };

                    var newFileName = "Service_" + codeOrNameString + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(servicefileName);
                    var folderPath = Server.MapPath("~" + docDirectoryPath);
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var dbPath = Path.Combine(folderPath, newFileName);
                    ServicesLink.SaveAs(dbPath);
                    model.ServicesLink = docDirectoryPath + "/" + newFileName;
                }

                model.ResumeLink = req.Form["ResumeLink"].NullToString();
                var ResumeLink = req.Files["ResumeLink"];

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
                        return new ResponseModel { IsSuccess = false, Message = "Resume- Only (pdf, jpg, jpeg, png, doc, docx) files allowed.", Data = null };

                }

                model.MenuLink = req.Form["MenuLink"].NullToString();
                var MenuLink = req.Files["MenuLink"];

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
                        return new ResponseModel { IsSuccess = false, Message = "Menu- Only (pdf, jpg, jpeg, png, doc, docx) files allowed.", Data = null };
                }

                int OutFlag;
                _userRepo.UpdatePersonalInfo(model, out OutFlag, "BharatTouch/AdminUsersApi/UpdatePersonalInfo");
                if (OutFlag == 1)
                    return new ResponseModel { IsSuccess = true, Message = "Personal info updated successfully!", Data = null };
                else if (OutFlag == 9)
                    return new ResponseModel { IsSuccess = false, Message = "Server Error.", Data = null };
                else
                    return new ResponseModel { IsSuccess = false, Message = "User not found.", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message ?? "Some error occurred.", Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpdateContacts")]
        public ResponseModel UpdateContacts([FromBody] UserContactDto model)
        {
            try
            {
                if (model.UserId == 0)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "UserId not found.", Data = null };
                }
                var user = new UserModel()
                {
                    UserId = model.UserId,
                    PersonalEmail = model.PersonalEmail,
                    CountryId = model.CountryId,
                    Phone = model.Phone,
                    WhatsAppCountryId = model.WhatsAppCountryId,
                    Whatsapp = model.Whatsapp,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    City = model.City,
                    StateName = model.StateName,
                    Country = model.Country,
                    Zip = model.Zip,
                    WorkPhoneCountryId = model.WorkPhoneCountryId,
                    WorkPhone = model.WorkPhone,
                    OtherPhoneCountryId = model.OtherPhoneCountryId,
                    OtherPhone = model.OtherPhone,
                    OfficeAddress1 = model.OfficeAddress1,
                    OfficeAddress2 = model.OfficeAddress2,
                    OfficeCity = model.OfficeCity,
                    OfficeStatename = model.OfficeStatename,
                    OfficeCountry = model.OfficeCountry,
                    OfficeZip = model.OfficeZip
                };
                int OutFlag;
                var message = "";
                _userRepo.UpdateContactInfo(user, out OutFlag, "BharatTouch/AdminUsersApi/UpdateContacts");
                if (OutFlag == 9)
                    message = "Server Error.";
                else
                    message = "Contact info " + (OutFlag == 1 ? "updated" : "inserted") + " successfully!";

                return new ResponseModel() { IsSuccess = OutFlag != 9, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpdateAboutInfo")]
        public ResponseModel UpdateAboutInfo([FromBody] AboutInfoDto model)
        {
            try
            {
                if (model.UserId == 0)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "UserId not found.", Data = null };
                }
                var user = new UserModel()
                {
                    UserId = model.UserId,
                    AboutDescription = model.AboutDescription,
                    SkillName1 = model.SkillName1,
                    SkillName2 = model.SkillName2,
                    SkillName3 = model.SkillName3,
                    SkillName4 = model.SkillName4,
                    SkillName5 = model.SkillName5,
                    SkillName6 = model.SkillName6,
                    KnowledgePercent1 = model.KnowledgePercent1,
                    KnowledgePercent2 = model.KnowledgePercent2,
                    KnowledgePercent3 = model.KnowledgePercent3,
                    KnowledgePercent4 = model.KnowledgePercent4,
                    KnowledgePercent5 = model.KnowledgePercent5,
                    KnowledgePercent6 = model.KnowledgePercent6
                };
                int OutFlag;
                var message = "";
                _userRepo.UpdateAboutInfo(user, out OutFlag, "BharatTouch/AdminUsersApi/UpdateContacts");
                if (OutFlag == 9)
                    message = "Server Error.";
                else
                    message = "About info " + (OutFlag == 1 ? "updated" : "inserted") + " successfully!";

                return new ResponseModel() { IsSuccess = OutFlag != 9, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpdateSocialInfo")]
        public ResponseModel UpdateSocialInfo([FromBody] SocialInfoDto model)
        {
            try
            {
                if (model.UserId == 0)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "UserId not passed.", Data = null };
                }
                _socialRepo.DeleteSocialMedia(model.UserId, "BharatTouch/AdminUsersApi/UpdateSocialInfo");

                if (!string.IsNullOrWhiteSpace(model.LinkedIn))
                    _socialRepo.UpsertUserSocial(model.UserId, "LinkedIn", model.LinkedIn, "BharatTouch/AdminUsersApi/UpdateSocialInfo");

                if (!string.IsNullOrWhiteSpace(model.Twitter))
                    _socialRepo.UpsertUserSocial(model.UserId, "Twitter", model.Twitter, "BharatTouch/AdminUsersApi/UpdateSocialInfo");

                if (!string.IsNullOrWhiteSpace(model.Facebook))
                    _socialRepo.UpsertUserSocial(model.UserId, "Facebook", model.Facebook, "BharatTouch/AdminUsersApi/UpdateSocialInfo");

                if (!string.IsNullOrWhiteSpace(model.Instagram))
                    _socialRepo.UpsertUserSocial(model.UserId, "Instagram", model.Instagram, "BharatTouch/AdminUsersApi/UpdateSocialInfo");

                if (!string.IsNullOrWhiteSpace(model.Skype))
                    _socialRepo.UpsertUserSocial(model.UserId, "Skype", model.Skype, "BharatTouch/AdminUsersApi/UpdateSocialInfo");

                if (!string.IsNullOrWhiteSpace(model.Youtube))
                    _socialRepo.UpsertUserSocial(model.UserId, "Youtube", model.Youtube, "BharatTouch/AdminUsersApi/UpdateSocialInfo");

                return new ResponseModel() { IsSuccess = true, Message = "Social media info updated successfully!", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpdateAadharInfo")]
        public ResponseModel UpdateAadharInfo()
        {
            try
            {
                var req = HttpContext.Current.Request;
                var Server = HttpContext.Current.Server;

                var model = new UserModel();
                model.UserId = req.Form["UserId"].ToIntOrZero();
                model.AdhaarFrontImgPath = req.Form["AdhaarFrontImgPath"].NullToString();
                model.AdhaarBackImgPath = req.Form["AdhaarBackImgPath"].NullToString();

                var AdhaarFrontImg = req.Files["AdhaarFrontImgPath"];
                if (AdhaarFrontImg != null && AdhaarFrontImg.ContentLength > 0)
                {
                    var uploadres = Utility.SaveCompressImages(AdhaarFrontImg, "Adhaar_Front_" + model.UserId, ConfigValues.ImagePath.Substring(1) + "/adhaar/" + model.UserId, 300);
                    if (uploadres == "invalid")
                        return new ResponseModel { IsSuccess = false, Message = "Only jpg,jpeg,png files allowed.", Data = null };
                    else if (uploadres == "0")
                        return new ResponseModel { IsSuccess = false, Message = "Some error occurred.", Data = null };

                    model.AdhaarFrontImgPath = uploadres;
                }

                var AdhaarBackImg = req.Files["AdhaarBackImgPath"];
                if (AdhaarBackImg != null && AdhaarBackImg.ContentLength > 0)
                {
                    var uploadres = Utility.SaveCompressImages(AdhaarBackImg, "Adhaar_Back_" + model.UserId, ConfigValues.ImagePath.Substring(1) + "/adhaar/" + model.UserId, 300);
                    if (uploadres == "invalid")
                        return new ResponseModel { IsSuccess = false, Message = "Only jpg,jpeg,png files allowed.", Data = null };
                    else if (uploadres == "0")
                        return new ResponseModel { IsSuccess = false, Message = "Some error occurred.", Data = null };

                    model.AdhaarBackImgPath = uploadres;
                }

                var message = "User not found.";
                var OutFlag = _userRepo.AdhaarUpdate(model, "BharatTouch/AdminUsersApi/UpdatePersonalInfo");
                if (OutFlag == 1)
                    message = "Aadhaar info updated successfully!";
                else if (OutFlag == 9)
                    message = "Server Error.";

                return new ResponseModel { IsSuccess = OutFlag == 1, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message ?? "Some error occurred.", Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/ShowHideSections")]
        public ResponseModel ShowHideSections([FromBody] showHideSectionsDto model)
        {
            try
            {
                // NOTES :- pass these below Types for show/hide section
                // About Section = AB
                // Education Section = ED
                // Skills Section = SK
                // Experience Section = EX
                // Social Media Section = SO
                // Gallery Section = GL
                // Team Section = TM
                // Client Testimonial = CTM
                // Blogs Section = BL
                // Youtube Section = YT
                // Training / Certification = UTC
                // Aadhaar Card Section = ADC

                if (model.UserId == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "UserId not found.", Data = null };

                int OutFlag;
                _userRepo.Showhideprofilesection(model.UserId, model.Type, out OutFlag, "BharatTouch/AdminUsersApi/ShowHideSections");
                var message = "";
                if (OutFlag == 1)
                    message = "Show/hide section updated successfully.";
                else if (OutFlag == 9)
                    message = "Server Error!";
                else
                    message = "User not found.";

                return new ResponseModel() { IsSuccess = OutFlag == 1, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UploadGalleryImages/{UserId}")]
        public ResponseModel UploadGalleryImages(int UserId)
        {
            try
            {
                var req = HttpContext.Current.Request;

                if (req.Files.Count == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "File not found.", Data = null };

                for (int i = 0; i < req.Files.Count; i++)
                {
                    HttpPostedFile file = req.Files[i];
                    if (file == null || string.IsNullOrWhiteSpace(file.FileName))
                    {
                        return new ResponseModel() { IsSuccess = false, Message = "Please choose an image first.", Data = null };
                    }

                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (fileExtension != ".png" && fileExtension != ".jpg" && fileExtension != ".jpeg")
                    {
                        return new ResponseModel() { IsSuccess = false, Message = "Only png, jpg, jpeg files are allowed.", Data = null };
                    }
                }

                // Save all files after validation
                for (int i = 0; i < req.Files.Count; i++)
                {
                    HttpPostedFile file = req.Files[i];
                    if (file != null)
                    {
                        Utility.SaveCompressImagesWithoutSegment(file, "/uploads/portfolio/" + UserId, 400);
                    }
                }

                return new ResponseModel() { IsSuccess = true, Message = "Images uploaded successfully!", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UploadCoverImage/{UserId}")]
        public ResponseModel UploadCoverImage(int UserId)
        {
            try
            {
                var user = _userRepo.GetUserById(UserId);
                if (user == null)
                    return new ResponseModel { IsSuccess = false, Message = "User not found.", Data = null };

                var req = HttpContext.Current.Request;
                user.CoverImage = req.Form["CoverImage"];
                if (req.Files.Count > 0)
                {
                    var coverImageFile = req.Files[0];
                    if (coverImageFile != null && coverImageFile.ContentLength > 0)
                    {

                        var res = Utility.SaveCompressImages(coverImageFile, UserId.NullToString(), ConfigValues.ImagePath.Substring(1) + "/coverImage");
                        if (res == "invalid")
                            return new ResponseModel { IsSuccess = false, Message = "Only jpg, jpeg, png files allowed.", Data = null };
                        else if (res == "0")
                            return new ResponseModel { IsSuccess = false, Message = "Some error occurred while uploading cover image", Data = null };

                        user.CoverImage = res;
                    }
                    else
                    {
                        user.CoverImage = null;
                    }
                }


                var result = _userRepo.UpsertUserProfileAndCoverImage(user, "BharatTouch/AdminUsersApi/UploadCoverImage");
                var message = "Some error occurred";
                if (result)
                    message = "Cover image updated successfully.";

                return new ResponseModel() { IsSuccess = result, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UploadProfileImage/{UserId}")]
        public ResponseModel UploadProfileImage(int UserId)
        {
            try
            {
                var user = _userRepo.GetUserById(UserId);
                if (user == null)
                    return new ResponseModel { IsSuccess = false, Message = "User not found.", Data = null };

                var req = HttpContext.Current.Request;

                if (req.Files.Count == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "File not found.", Data = null };

                var profileImageFile = req.Files[0];
                if (profileImageFile != null && profileImageFile.ContentLength > 0)
                {
                    var res = Utility.SaveCompressImages(profileImageFile, UserId.NullToString(), ConfigValues.ImagePath.Substring(1) + "/profile");
                    if (res == "invalid")
                        return new ResponseModel { IsSuccess = false, Message = "Only jpg, jpeg, png files allowed.", Data = null };
                    else if (res == "0")
                        return new ResponseModel { IsSuccess = false, Message = "Some error occurred while uploading profile image", Data = null };

                    user.ProfileImage = res;
                }

                var result = _userRepo.UpsertUserProfileAndCoverImage(user, "BharatTouch/AdminUsersApi/UploadProfileImage");
                var message = "Some error occurred";
                if (result)
                    message = "Profile image updated successfully.";

                return new ResponseModel() { IsSuccess = result, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/DeletePersonalInfoFiles")]
        public ResponseModel DeletePersonalInfoFiles(PersonalInfoFileDeleteDTO model)
        {
            try
            {
                // Pass Types for personal files
                // "P" for portfolio
                // "S" for services
                // "R" for Resume
                // "M" for Menu

                var success = _userRepo.DeletePersonalFile(model.UserId, model.Type, "BharatTouch/AdminUsersApi/DeletePersonalInfoFiles");
                if (success)
                {
                    Utility.RemoveFile(model.filePath);
                    string fileExtension = Path.GetExtension(Path.GetFileName(model.filePath)).ToLower();
                    if (fileExtension != ".pdf")
                    {
                        string directoryPath = HttpContext.Current.Server.MapPath("~" + Path.GetDirectoryName(model.filePath));
                        string pdfFilepath = Path.Combine(directoryPath, Path.ChangeExtension(Path.GetFileName(model.filePath), ".pdf"));
                        Utility.RemoveFile(pdfFilepath);
                    }
                    return new ResponseModel { IsSuccess = true, Message = "File deleted successfully.", Data = null };
                }

                return new ResponseModel { IsSuccess = true, Message = "Some error occurred", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message ?? "Some error occurred.", Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpsertUserByAdmin")]
        public ResponseModel UpsertUserByAdmin([FromBody] UserByAdminModel model)
        {
            try
            {
                int outFlag;
                string outMessage;

                var result = _adminRepo.SaveOrUpdateUserByAdmin_Admin(model, out outFlag, out outMessage, "api/v1/Admin/Users/UpsertUserByAdmin");
                if (result == false)
                {
                    return new ResponseModel() { IsSuccess = false, Message = outMessage, Data = null };
                }

                return new ResponseModel() { IsSuccess = result, Message = outMessage, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Users/GetAllNfcCardColor")]
        public ResponseModel GetAllNfcCardColor()
        {
            try
            {
                var model = _adminRepo.GetAllNfcCardColor_Admin("api/v1/Admin/Users/GetAllNfcCardColor");
                if (model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "NFCCardColor not found.", Data = null };
                }
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Users/GetAllNfcCardFinishColor")]
        public ResponseModel GetAllNfcCardFinishColor()
        {
            try
            {
                var model = _adminRepo.GetAllNfcCardFinishColor_Admin("api/v1/Admin/Users/GetAllNfcCardFinishColor");
                if (model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "NfcCardFinishColor not found.", Data = null };
                }
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        #endregion

        #region Device Tokens

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/DeviceTokens/UpsertDeviceToken")]
        public ResponseModel UpsertDeviceToken([FromBody] UpsertDeviceTokenDto body)
        {
            try
            {
                var model = new DeviceTokenModel();
                model.UserId = body.UserId;
                model.Device_Id = body.Device_Id;
                model.DeviceDescription = body.DeviceDescription;
                model.Device_Token = body.Device_Token;

                int OutFlag = 0;
                var result = _deviceTokenRepo.UpsertDeviceToken(model, out OutFlag, "BharatTouch/AdminUsersApi/UpsertDeviceToken");
                var message = "Some error occurred.";
                if (result)
                    message = "Operation successfull.";

                return new ResponseModel() { IsSuccess = result, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/DeviceTokens/RemoveDeviceToken")]
        public ResponseModel RemoveDeviceToken([FromBody] RemoveDeviceTokenDto body)
        {
            try
            {
                var model = new DeviceTokenModel();
                model.UserId = body.UserId;
                model.Device_Id = body.Device_Id;

                int OutFlag = 0;
                var result = _deviceTokenRepo.RemoveDeviceToken(model, out OutFlag, "BharatTouch/AdminUsersApi/RemoveDeviceToken");
                var message = "Some error occurred.";
                if (result)
                    message = "Operation successfull.";

                return new ResponseModel() { IsSuccess = result, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Meetings

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Meeting/UpdateMeetingWeekDays")]
        public ResponseModel UpdateMeetingWeekDays(ScheduleOpenWeekDayModel model)
        {
            try
            {
                var result = _schedulingRepo.UpsertScheduleOpenWeekDays(model, "BharatTouch/AdminUsersApi/UpdateMeetingWeekDays");
                var message = "Some error occurred.";
                if (result)
                    message = "Operation successfull.";

                return new ResponseModel() { IsSuccess = result, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Upi Section

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UploadUpiDetails/{UserId}")]
        public ResponseModel UploadUpiDetails(int UserId)
        {
            try
            {
                var req = HttpContext.Current.Request;
                var model = new UpiDetailsModel();
                model.UserId = UserId;
                model.PayeeName = req.Form["PayeeName"];
                model.UpiId = req.Form["UpiId"];
                model.QrImage = req.Form["QrImage"];
                if (req.Files.Count > 0)
                {
                    var paymentQr = req.Files[0];

                    if (paymentQr != null && paymentQr.ContentLength > 0)
                    {
                        var res = Utility.SaveCompressImages(paymentQr, UserId.NullToString(), ConfigValues.ImagePath.Substring(1) + "/paymentsQr", 400);
                        if (res == "invalid")
                            return new ResponseModel { IsSuccess = false, Message = "Only jpg, jpeg, png files allowed.", Data = null };
                        else if (res == "0")
                            return new ResponseModel { IsSuccess = false, Message = "Some error occurred.", Data = null };

                        model.QrImage = res;
                    }
                }
                var isSuccess = _userRepo.UpsertUpiDetails(model, "BharatTouch/AdminUsersApi/UploadUpiDetails");
                var message = "Payment details uploaded successfully.";
                if (!isSuccess)
                    message = "Server error, please try again later.";

                return new ResponseModel { IsSuccess = isSuccess, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        #endregion

        #region Countries

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Countries/FetchAllCountries")]
        public ResponseModel FetchAllCountries()
        {
            try
            {
                var list = _countryRepo.GetCountries("BharatTouch/AdminUsersApi/FetchAllCountries");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Countries/DeleteCountry")]
        public ResponseModel DeleteCountry(int countryId, int deletedBy)
        {
            try
            {
                int outflag;
                string outMessage;
                var list = _countryRepo.SoftDeleteCountry(countryId, deletedBy, out outflag, out outMessage);
                return new ResponseModel() { IsSuccess = true, Message = outMessage, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Countries/GetCountryDetails/{id}")]
        public ResponseModel GetCountryDetails(int id)
        {
            try
            {
                var model = _countryRepo.GetCountryById(id);
                if (model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Country not found.", Data = null };
                }
                return new ResponseModel() { IsSuccess = true, Message = "Country Fetched Successfully", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Countries/UpsertCountry")]
        public ResponseModel UpsertCountry(CountryModel model)
        {
            try
            {
                int outFlag;
                string outMessage = string.Empty;
                var result = _countryRepo.UpsertCountry(model, out outFlag, out outMessage);

                return new ResponseModel() { IsSuccess = result, Message = outMessage, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region BusinessTypes

        [HttpGet]
        [Route("api/v1/Admin/BusinessTypes/FetchBusinessCategories")]
        public ResponseModel FetchBusinessCategories()
        {
            try
            {
                var list = _commonRepo.GetBusinessTypeParentList("BharatTouch/AdminUsersApi/FetchBusinessCategories");
                var sortedList = list
                   .OrderByDescending(x => x.BusinessTypeId == 36) // True gets ordered first
                   .ThenBy(x => x.BusinessTypeId) // Optional: additional ordering
                   .ToList();
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = sortedList, outParam = sortedList.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/Admin/BusinessTypes/FetchBusinessTypes/{parentId}")]
        public ResponseModel FetchBusinessCategories(int parentId)
        {
            try
            {
                if (parentId == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "Please pass parentId.", Data = null, outParam = 0 };
                else if (parentId == 36)
                    return new ResponseModel() { IsSuccess = true, Message = "Show Textbox for selected Others.", Data = null, outParam = 1 };

                var list = _commonRepo.GetBusinessTypeListBtParentId(parentId, "BharatTouch/AdminUsersApi/FetchBusinessCategories");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/Admin/BusinessTypes/FetchBusinessTypeList")]
        public ResponseModel FetchBusinessTypeList()
        {
            try
            {
                var businessTypes = _commonRepo.GetBusinessTypeList("AdminUsersApi/BusinessTypes/FetchBusinessTypeList");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = businessTypes, outParam = businessTypes.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/BusinessTypes/DeleteBusinessTypeById")]
        public ResponseModel DeleteBusinessTypeById(int businessTypeId, int deletedBy)
        {
            try
            {
                int outFlag;
                string outMessage;
                var result = _commonRepo.SoftDeleteBusinessTypeById(businessTypeId, deletedBy, out outFlag, out outMessage);
                return new ResponseModel() { IsSuccess = result, Message = outMessage, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/BusinessTypes/GetBusinessTypeById/{businessTypeId}")]
        public ResponseModel GetBusinessTypeById(int businessTypeId)
        {
            try
            {
                var model = _commonRepo.GetBusinessTypeById(businessTypeId);
                if (model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "BusinessType not found.", Data = null };
                }
                return new ResponseModel() { IsSuccess = true, Message = "BusinessType Fetched Successfully", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/BusinessTypes/UpsertBusinessType")]
        public ResponseModel UpsertBusinessType(BusinessTypeModel model)
        {
            try
            {
                int outFlag;
                string outMessage = string.Empty;
                var result = _commonRepo.UpsertBusinessType(model, out outFlag, out outMessage);

                return new ResponseModel() { IsSuccess = result, Message = outMessage, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Education

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpsertUserEducation")]
        public ResponseModel UpsertUserEducation(UserEducationModel model)
        {
            try
            {
                if (model.UserId == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "UserId not found.", Data = null };

                int newEducationId;
                string message = string.Empty;
                var result = _educationRepo.UpsertUserEducation(model, out newEducationId, "BharatTouch/AdminUsersApi/UpsertUserEducation");
                if (result == 9)
                    message = "Server Error.";
                else
                    message = "Education details " + (model.EducationId > 0 ? "updated" : "inserted") + " successfully!";

                return new ResponseModel() { IsSuccess = result != 9, Message = message, Data = null, outParam = newEducationId };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Experience

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpsertUserExperience")]
        public ResponseModel UpsertUserExperience(UserProfessionalModel model)
        {
            try
            {
                if (model.UserId == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "UserId not found.", Data = null };

                int newExperienceId;
                string message = string.Empty;
                var result = _professionalRepo.UpsertUserProfessional(model, out newExperienceId, "BharatTouch/AdminUsersApi/UpsertUserExperience");
                if (result == 9)
                    message = "Server Error.";
                else
                    message = "Experience details " + (model.ProfessionId > 0 ? "updated" : "inserted") + " successfully!";

                return new ResponseModel() { IsSuccess = result != 9, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Youtube

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpsertYoutubeVideo")]
        public ResponseModel UpsertYoutubeVideo([FromBody] YouTubeModel model)
        {
            try
            {
                if (model.UserId == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "UserId not found.", Data = null };

                string message = string.Empty;
                var result = _youTubeRepository.UpsertYouTube(model, "BharatTouch/AdminUsersApi/UpsertYoutubeVideo");
                if (result == 9)
                    message = "Server Error.";
                else
                    message = "Youtube video " + (model.YouTubeId > 0 ? "updated" : "inserted") + " successfully!";

                return new ResponseModel() { IsSuccess = result != 9, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Blog

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpsertBlog")]
        public ResponseModel UpsertBlog([FromBody] BlogModel model)
        {
            try
            {
                if (model.UserId == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "UserId not found.", Data = null };

                string message = string.Empty;
                var result = _blogRepository.UpsertBlog(model, "BharatTouch/AdminUsersApi/UpsertBlog");
                if (result == 9)
                    message = "Server Error.";
                else
                    message = "Blog detail " + (model.BlogId > 0 ? "updated" : "inserted") + " successfully!";

                return new ResponseModel() { IsSuccess = result != 9, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Client Testimonial

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/ClientTestimonial/UpsertTestimonial")]
        public ResponseModel UpsertTestimonial()
        {
            try
            {
                var req = HttpContext.Current.Request;
                ClientTestimonialModel model = new ClientTestimonialModel();
                model.Client_Id = Convert.ToInt32(req.Form["Client_Id"].ToIntOrNull());
                model.UserId = Convert.ToInt32(req.Form["UserId"].ToIntOrNull());
                model.ClientName = req.Form["ClientName"];
                model.Designation = req.Form["Designation"];
                model.CompanyName = req.Form["CompanyName"];
                model.Testimonial = req.Form["Testimonial"];
                var files = req.Files;
                if (files.Count > 0 && files[0].ContentLength > 0)
                {
                    var fileName = model.Client_Id == 0 ? DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + model.UserId : model.Client_Id.ToString();
                    var result = Utility.SaveCompressImages(files[0], fileName, ConfigValues.ImagePath.Substring(1) + "/ClientTestimonial/" + model.UserId, 200);
                    if (result == "invalid")
                        return new ResponseModel { IsSuccess = false, Message = "Only jpg,jpeg,png files allowed.", Data = null };
                    else if (result == "0")
                        return new ResponseModel { IsSuccess = false, Message = "Some error occurred.", Data = null };
                    model.PicOfClient = result;
                }
                else
                    model.PicOfClient = req.Form["PicOfClient"];
                var res = _testimonialRepo.UpsertClientTestimonial(model, "BharatTouch/AdminUsersApi/UpsertTestimonial");
                var message = "";
                if (res == 0)
                    message = "Some error occurred.";
                else
                    message = "Client " + (res == model.Client_Id ? "updated" : "saved") + " successfully!";
                return new ResponseModel() { IsSuccess = res != 0, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region User Certifications

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UpsertUserCertification")]
        public ResponseModel UpsertUserCertification()
        {
            try
            {
                var req = HttpContext.Current.Request;
                UserCertificationModel model = new UserCertificationModel();
                model.CertificationId = Convert.ToInt32(req.Form["CertificationId"].ToIntOrNull());
                model.UserId = Convert.ToInt32(req.Form["UserId"].ToIntOrNull());
                model.CertificationName = req.Form["CertificationName"];
                model.IssuingOrganization = req.Form["IssuingOrganization"];
                model.OrganizationURL = req.Form["OrganizationURL"];
                var IssueDate = req.Form["IssueDate"];
                if (!string.IsNullOrEmpty(IssueDate))
                {
                    model.IssueDate = DateTime.Parse(IssueDate);
                }
                var ExpirationDate = req.Form["ExpirationDate"];
                if (!string.IsNullOrEmpty(ExpirationDate))
                {
                    model.ExpirationDate = DateTime.Parse(ExpirationDate);
                }
                model.CertifcateNumber = req.Form["CertifcateNumber"];
                model.CertificateURL = req.Form["CertificateURL"];
                model.Description = req.Form["Description"];
                model.CertificateFile = req.Form["CertificateFile"];
                var files = req.Files;
                if (files.Count > 0 && files[0].ContentLength > 0)
                {
                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + model.UserId;
                    if (model.CertificationId != 0)
                    {
                        fileName = model.CertificationId.NullToString() + "_" + model.UserId;
                    }
                    var result = Utility.SaveCompressImages(files[0], fileName, ConfigValues.ImagePath.Substring(1) + "/userCertification/" + model.UserId, 200);
                    if (result == "invalid")
                        return new ResponseModel { IsSuccess = false, Message = "Only jpg,jpeg,png files allowed.", Data = null };
                    else if (result == "0")
                        return new ResponseModel { IsSuccess = false, Message = "Some error occurred.", Data = null };
                    model.CertificateFile = result;
                }
                var res = _certificationRepo.UpsertUserCertification(model, "BharatTouch/AdminUsersApi/UpsertTestimonial");
                var message = "";
                if (res == 0)
                    message = "Some error occurred.";
                else
                    message = "Certificate/Training " + (res == model.CertificationId ? "updated" : "saved") + " successfully!";
                return new ResponseModel() { IsSuccess = res != 0, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Profile Template

        [HttpPost]
        [Route("api/v1/Admin/ProfileTemplate/FetchActiveTemplateList")]
        public ResponseModel FetchActiveTemplateList()
        {
            try
            {
                var list = _profTempRepo.GetAllTemplates("BharatTouch/AdminUsersApi/FetchActiveTemplateList");

                return new ResponseModel() { IsSuccess = true, Message = "Operation successfull.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/ProfileTemplate/ChangeUserProfileTemplate")]
        public ResponseModel ChangeUserProfileTemplate(ChangeProfileTemplateDto model)
        {
            try
            {
                var user = new UserModel() { UserId = model.UserId, ProfileTemplateId = model.ProfileTemplateId };
                var result = _userRepo.ChangeProfileTemplate(user, "BharatTouch/AdminUsersApi/ChangeUserProfileTemplate");
                var message = "";
                if (result == 9)
                    message = "Server error!";
                else if (result == 1)
                    message = "Profile template updated successfully.";
                else
                    message = "User not found!";

                return new ResponseModel() { IsSuccess = result == 1, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        #endregion

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/ViewHistory")]
        public ResponseModel FetchUsersViewHistory([FromBody] UserIdPaginationDto pagination)
        {
            try
            {
                var list = _adminRepo.PaginatedUserHistory(pagination, pagination.UserId, "BharatTouch/AdminUsersApi/ViewHistory");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/ReferredUsersByCode")]
        public ResponseModel PaginatedReferredUsers([FromBody] ReferredUserPaginationDto pagination)
        {
            try
            {
                var list = _adminRepo.PaginatedReferredUsers(pagination, pagination.ReferalCode, "BharatTouch/AdminUsersApi/ReferredUsersByCode");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #region Package
        [HttpGet]
        [Route("api/v1/Admin/Users/PackageList")]
        public ResponseModel PackageList()
        {
            try
            {
                var list = _packageRepo.GetAllPackages();
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/ChangeUserPlan")]
        public ResponseModel ChangeUserPlan([FromBody] ChangeUserPlanDto body)
        {
            try
            {
                var loggedUserId = TokenManager.GetClaim("UserId").ToIntOrZero();
                if (loggedUserId == 0)
                    return new ResponseModel() { IsSuccess = false, Message = "UnAuthorized User.", Data = null };

                var model = new UserPackageViewModel() { UserId = body.UserId, PackageId = body.PackageId, LoggedUserId = loggedUserId };
                var result = _userRepo.UpdateUserPackage_v1(model, "BharatTouch/AdminUsersApi/ChangeUserPlan");
                var message = "";
                if (result == 0)
                    message = "User Not Found.";
                else if (result == 1)
                    message = "Plan updated successfully.";
                else
                    message = "Server Error.";

                return new ResponseModel() { IsSuccess = result == 1, Message = message, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/PlanChangeHistory")]
        public ResponseModel PlanChangeHistory(UserIdPaginationDto model)
        {
            try
            {
                var list = _packageRepo.PaginatedUserPlanChangeHistory(model, model.UserId, "BharatTouch/AdminUsersApi/PlanChangeHistory");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/PackageUsers")]
        public ResponseModel PackageUsers(PackagePaginationDto model)
        {
            try
            {
                var list = _packageRepo.PaginatedPackageUsers(model, model.PackageId, "BharatTouch/AdminUsersApi/PackageUsers");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Payment
        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Users/PaymentDetailsById/{PaymentId}")]
        public ResponseModel PaymentDetailsById(int PaymentId)
        {
            try
            {
                var model = _paymentRepo.GetPaymentById(PaymentId, "BharatTouch/AdminUsersApi/PaymentDetailsById");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Users/UserPaymentsList")]
        public ResponseModel UserPaymentsList(UserIdPaginationDto dto)
        {
            try
            {
                var List = _paymentRepo.GetUsersPaymentsList(dto, dto.UserId, "BharatTouch/AdminUsersApi/UserPaymentsList");
                return new ResponseModel() { IsSuccess = true, Message = "Operation successful.", Data = List, outParam = List.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        #region Discount Coupon

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/DiscountCoupon/DiscountCouponFetchAll")]
        public ResponseModel DiscountCouponFetchAll([FromBody] PaginationModel pagination)
        {
            try
            {
                var list = _paymentRepo.DiscountCouponFetchAll(pagination, "BharatTouch/AdminUsersApi/DiscountCouponFetchAll");
                return new ResponseModel() { IsSuccess = true, Message = "Discount Coupon Fetched Successfully.", Data = list, outParam = list.Count };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/DiscountCoupon/DeleteDiscountCoupon")]
        public ResponseModel DeleteDiscountCoupon(int discountCouponId)
        {
            try
            {
                var result = _paymentRepo.DeleteDiscountCoupon(discountCouponId, "BharatTouch/AdminUsersApi/DeleteDiscountCouponById");

                return new ResponseModel() { IsSuccess = result == 1 ? true : false, Message = result == 1 ? "Delete Successfully" : "Some error occuried", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/DiscountCoupon/GetDiscountCouponById")]
        public ResponseModel GetDiscountCouponById(int discountCouponId)
        {
            try
            {
                var model = _paymentRepo.GetDiscountCouponById(discountCouponId, "BharatTouch/AdminUserApi/GetDiscountCouponById");

                if (model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "DiscountCoupon not found.", Data = null };
                }
                return new ResponseModel() { IsSuccess = true, Message = "DiscountCoupon Detail Fetched Successfully", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/DiscountCoupon/UpsertDiscountCoupon")]
        public ResponseModel UpsertDiscountCoupon(DiscountCouponModel model)
        {
            try
            {
                var result = _paymentRepo.UpsertDiscountCoupon(model);
                return new ResponseModel() { IsSuccess = result > 0 ? true : false, Message = "Discount coupon " + (result == model.DiscountCouponId ? "updated" : "added") + " successfully.", Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }


        #endregion

        #region Company


        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Company/GetAllCompany")]
        public ResponseModel GetAllCompany()
        {
            try
            {
                int totalRows;
                var list = _adminRepo.GetAllCompany_Admin(out totalRows);
                return new ResponseModel() { IsSuccess = true, Message = "Company Fetched Successfully.", Data = list, outParam = totalRows };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Company/DeleteCompany")]
        public ResponseModel DeleteCompany(int companyId)
        {
            try
            {
                int outFlag;
                string outMessage;
                _adminRepo.CompanyDelete_Admin(companyId, out outFlag, out outMessage);
                return new ResponseModel() { IsSuccess = outFlag == 0 ? true : false, Message = outMessage, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Company/GetCompanyById")]
        public ResponseModel GetCompanyById(int companyId)
        {
            try
            {
                var model = _adminRepo.GetCompanyById_Admin(companyId);
                if (model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Company not found.", Data = null };
                }
                return new ResponseModel() { IsSuccess = true, Message = "Company Detail Fetched Successfully", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Company/IsExistCompanyUserDisplayName")]
        public ResponseModel IsExistCompanyUserDisplayName(string name)
        {
            try
            {
                var model = new CompanyRepository().IsExistCompanyUserDisplayName(name, "BharatTouch/admin/CheckUserDisplayNameAvailability");
                if (model)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Display Name already exists!.", Data = "1" };
                }
                return new ResponseModel() { IsSuccess = true, Message = "", Data = "0" };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Company/UpsertCompany")]
        public ResponseModel UpsertCompany([FromBody] CompanyModel model)
        {
            try
            {
                var loggedName = TokenManager.GetClaim("UserName").NullToString();
                var loggedEmail = TokenManager.GetClaim("Email").NullToString();

                int outFlag;
                string outMessage;
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
                        QueueVerificationEmailAsync(model.AdminEmail, model.AdminFirstname + " " + model.AdminLastname.NullToString(), "Welcome*123", "Admin/Company/UpsertCompany");
                        CompanyAdminEmailToSuperAdmin(model.AdminFirstname + " " + model.AdminLastname.NullToString(), model.AdminEmail, userDisplayName, model.CompanyName, userLink, companyAdminNameAndEmail, "api/v1/Admin/Company/UpsertCompany");
                    }

                    return new ResponseModel() { IsSuccess = true, Message = outMessage, Data = null };
                }
                else
                {
                    return new ResponseModel() { IsSuccess = false, Message = outMessage, Data = null };
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
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


        #endregion

        #region Order

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Order/GetAllOrder")]
        public ResponseModel GetAllOrder()
        {
            try
            {
                var orders = _adminRepo.GetAllOrder_Admin();
                return new ResponseModel() { IsSuccess = true, Message = "Order Fetched Successfully.", Data = orders, outParam = orders.Count() };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Order/GetShippingOrderDetail")]
        public ResponseModel GetShippingOrderDetail(int orderId)
        {
            try
            {
                var model = _adminRepo.GetIsShippedById_Admin(orderId);

                return new ResponseModel() { IsSuccess = true, Message = "Shipped Order Detail Fetched Successfully.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Order/GetAllOrderShppingCompany")]
        public ResponseModel GetAllOrderShppingCompany()
        {
            try
            {
                var orders = _adminRepo.GetAllOrderShppingCompany_Admin("api/v1/Admin/Order/GetAllOrderShppingCompany");
                return new ResponseModel() { IsSuccess = true, Message = "Order Fetched Successfully.", Data = orders, outParam = orders.Count() };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Order/OrderPrinting")]
        public ResponseModel SaveOrUpdateOrderPrintingStatus(int orderId)
        {
            try
            {
                string msg;
                string outMessage;

                var result = _adminRepo.OrderPrinting_Admin(orderId, "api/v1/Admin/Order/OrderPrinting");

                if (result == false)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Failed: Some error occurred.", Data = null };
                }
                else
                {
                    var model = new OrderShippingModel();
                    model = _adminRepo.GetIsShippedById_Admin(orderId);
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

                    return new ResponseModel() { IsSuccess = true, Message = msg, Data = null };
                }


            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
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

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Order/OrderShipping")]
        public ResponseModel SaveOrUpdateOrderShipping([FromBody] OrderShippingModel model)
        {
            try
            {
                string msg;
                string outMessage;

                var result = _adminRepo.SaveOrUpdateOrderShipping_Admin(model, "OrderShipped");
                if (result == false)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Failed: Some error occurred while order shipping.", Data = null };
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
                    return new ResponseModel() { IsSuccess = true, Message = msg, Data = null };
                }

            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
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

        #region Lead

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Leads/Lead_ConvertFromOrder")]
        public ResponseModel Lead_ConvertFromOrder(int orderId, int userId)
        {
            try
            {
                int outFlag; string outMessage;
                _adminRepo.Lead_ConvertFromOrder(orderId, userId, out outFlag, out outMessage);
                return new ResponseModel { IsSuccess = outFlag == 0, Message = outMessage };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Leads/GetAllLeads_Admin")]
        public ResponseModel GetAllLeads_Admin()
        {
            try
            {
                var dt = _adminRepo.GetAllLeads_Admin("api/v1/Admin/Leads/GetAllLeads_Admin");
                return new ResponseModel { IsSuccess = true, Message = "Leads fetched successfully", Data = dt };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Users/DeleteLeads_Admin/{leadId}")]
        public ResponseModel DeleteLeads_Admin(int leadId)
        {
            try
            {
                int outFlag;
                string outMessage;
                _adminRepo.DeleteLeads_Admin(leadId, out outFlag, out outMessage);
                return new ResponseModel() { IsSuccess = outFlag == 0, Message = outMessage };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Leads/GetLeadsDetail_Admin/{leadId}")]
        public ResponseModel GetLeadsDetail_Admin(int leadId)
        {
            try
            {
                var dt = _adminRepo.GetLeadById_Admin(leadId, "api/v1/Admin/Leads/GetLeadsDetail_Admin/{leadId}");
                return new ResponseModel { IsSuccess = true, Message = "Leads fetched successfully", Data = dt };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Leads/UpsertLeadByAdmin")]
        public ResponseModel UpsertLeadByAdmin([FromBody] LeadAdminModel model)
        {
            try
            {
                int outFlag;
                string outMessage = string.Empty;
                var result = _adminRepo.Lead_CreateManual(model, out outFlag, out outMessage);

                return new ResponseModel() { IsSuccess = result, Message = outMessage, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Leads/GetLeadCommunicationByLeadId_Admin/{leadId}")]
        public ResponseModel GetLeadCommunicationByLeadId_Admin(int leadId)
        {
            try
            {
                var dt = _adminRepo.GetLeadCommunicationByLeadId_Admin(leadId);
                return new ResponseModel { IsSuccess = true, Message = "Leads Communication fetched successfully", Data = dt };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Leads/GetLeadCommunicationType_Admin")]
        public ResponseModel GetLeadCommunicationType_Admin()
        {
            try
            {
                var dt = _adminRepo.GetLeadCommunicationType_Admin();
                return new ResponseModel { IsSuccess = true, Message = "Leads Communication Type fetched successfully", Data = dt };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/Leads/LeadCommunication_Upsert")]
        public ResponseModel LeadCommunication_Upsert([FromBody] LeadCommunicationModel model)
        {
            try
            {
                int outFlag;
                string outMessage = string.Empty;
                var result = _adminRepo.LeadCommunication_Upsert(model, out outFlag, out outMessage);

                return new ResponseModel() { IsSuccess = result, Message = outMessage, Data = null };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Users/DeleteLeadCommunication/{leadCommunicationId}")]
        public ResponseModel DeleteLeadCommunication(int leadCommunicationId)
        {
            try
            {
                int outFlag; string outMessage;
                _adminRepo.DeleteLeadCommunication(leadCommunicationId, out outFlag, out outMessage);
                return new ResponseModel() { IsSuccess = outFlag == 0, Message = outMessage };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        #endregion

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/User/GetAdminUsers")]
        public ResponseModel GetAdminUsers()
        {
            try
            {
                var dt = _userRepo.GetAdminUsers();
                return new ResponseModel { IsSuccess = true, Message = "Admin User fetched successfully", Data = dt };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Admin/User/GetAssignedToUser")]
        public ResponseModel GetAssignedToUser([FromBody] PaginationModel pagination)
        {
            try
            {
                int totRows;
                var users = _adminRepo.GetAllUsers_Admin(pagination.Page, pagination.Size, pagination.SortBy, pagination.SortOrder, pagination.SearchText, out totRows, "BharatTouch/AdminUsersApi/GetAssignedToUser");
                var dt = users
            .Where(u => u.UserType == "SA" || u.UserType == "LA")
            .ToList();
                return new ResponseModel { IsSuccess = true, Message = "User fetched successfully", Data = dt };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Leads/GetAllLeadsStatus")]
        public ResponseModel GetAllLeadsStatus()
        {
            try
            {
                var dt = _adminRepo.GetAllLeadsStatus_Admin();
                return new ResponseModel { IsSuccess = true, Message = "LeadsStatus fetched successfully", Data = dt };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Admin/Leads/GetOrderByOrderId/{orderId}")]
        public ResponseModel GetOrderByOrderId(int orderId)
        {
            try
            {
                var dt = _adminRepo.GetOrderByOrderId(orderId);
                return new ResponseModel { IsSuccess = true, Message = "Order Detail fetched successfully", Data = dt };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }
    }
}
