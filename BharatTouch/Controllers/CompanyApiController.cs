using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;
using BharatTouch.JwtTokens;
using DataAccess;
using DataAccess.AdminApiDto;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.ViewModels;
using BharatTouch.CommonHelper;

namespace BharatTouch.Controllers
{
    public class CompanyApiController : ApiController
    {
        #region Declaration
        CompanyRepository _compRepo = new CompanyRepository();
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

        #region Company Index

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetCompanyDetail")]
        public ResponseModel GetCompanyDetail([FromUri] int companyId)
        {
            try
            {
                var model = _adminRepo.GetCompanyById_Admin(companyId);
                return new ResponseModel()
                { IsSuccess = true, Message = "Operation successful.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel()
                { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetCountries")]
        public ResponseModel GetCountries()
        {
            try
            {
                var model = new DataAccess.Repository.CountryRepository().GetCountries("api/v1/Company/GetCountries");
                return new ResponseModel()
                { IsSuccess = true, Message = "Operation successful.", Data = model };
            }
            catch (Exception ex)
            {
                return new ResponseModel()
                { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpdateContactDetail")]
        public ResponseModel SaveOrUpdateContactDetail([FromBody] CompanyViewModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                var result = _compRepo.Company_Contact_Upsert(model, out outFlag, out outMessage);

                if (!result)
                {
                    return new ResponseModel() { IsSuccess = false, Message = outMessage };
                }
                else
                    return new ResponseModel() { IsSuccess = true, Message = outMessage };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpdateSocialMediaDetail")]
        public ResponseModel SaveOrUpdateSocialMediaDetail([FromBody] CompanyViewModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                var result = _compRepo.Company_SocialMedia_Upsert(model, out outFlag, out outMessage);

                if (!result)
                {
                    return new ResponseModel() { IsSuccess = false, Message = outMessage };
                }
                else
                    return new ResponseModel() { IsSuccess = true, Message = outMessage };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpdateAboutDetail")]
        public ResponseModel SaveOrUpdateAboutDetail([FromBody] CompanyViewModel model)
        {
            try
            {
                int outFlag;
                string outMessage;
                var result = _compRepo.Company_About_Upsert(model, out outFlag, out outMessage);

                if (!result)
                {
                    return new ResponseModel() { IsSuccess = false, Message = outMessage };
                }
                else
                    return new ResponseModel() { IsSuccess = true, Message = outMessage };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpdateCoverImageDetail")]
        public ResponseModel SaveOrUpdateCoverImageDetail()
        {
            try
            {
                int outFlag = 0;
                string outMessage = "";

                var req = HttpContext.Current.Request;
                var Server = HttpContext.Current.Server;
                var model = new CompanyViewModel();

                var CID = req.Form["CompanyId"].ToIntOrZero();
                model.CompanyId = CID;

                var file = req.Files["CoverImagepath"];

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

                        return new ResponseModel() { IsSuccess = outFlag == 0, Message = outMessage };
                    }
                    else
                    {
                        return new ResponseModel() { IsSuccess = false, Message = "Invalid File Format" };

                    }
                }
                else
                {
                    return new ResponseModel() { IsSuccess = false, Message = "No file selected", Data = null };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/DeleteCompanyCoverImage")]
        public ResponseModel DeleteCompanyCoverImage([FromUri] int CompanyId)
        {
            try
            {
                var model = new CompanyViewModel();
                var Server = HttpContext.Current.Server;
                int outFlag = 1;
                string outMessage = "Cover Image Deleted Failed";
                model.CoverImagepath = "";
                model.CompanyId = CompanyId;

                var path = Server.MapPath("~/Uploads/Company/CoverImage/" + CompanyId + ".png");

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
                return new ResponseModel() { IsSuccess = outFlag == 0, Message = outMessage };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetPortfolioImages/{companyId}")]
        public ResponseModel GetPortfolioImages(int companyId)
        {
            try
            {
                var folderPath = HttpContext.Current.Server.MapPath($"~/uploads/Portfolio/Company/{companyId}");
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                List<string> imageUrls = new List<string>();

                if (Directory.Exists(folderPath))
                {
                    var files = Directory.GetFiles(folderPath);
                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileName(file);
                        var imageUrl = $"/uploads/Portfolio/Company/{companyId}/{fileName}";
                        imageUrls.Add(imageUrl);
                    }
                    return new ResponseModel() { IsSuccess = true, Message = "Portfolio images fetched successfully.", Data = imageUrls };

                }
                else
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Portfolio images not there", Data = imageUrls };

                }
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpdatePortfolioImages")]
        public ResponseModel SaveOrUpdatePortfolioImages()
        {
            try
            {
                bool filesValid = true;

                var req = HttpContext.Current.Request;
                var Server = HttpContext.Current.Server;
                var model = new CompanyViewModel();

                var CID = req.Form["CompanyId"].ToIntOrZero();
                model.CompanyId = CID;

                var files = req.Files;
                if (files.Count == 0)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Message = "No files uploaded.",
                        Data = null
                    };
                }

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null)
                    {
                        string fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                        if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                        {
                            filesValid = false;
                            break;
                        }
                    }
                }

                if (!filesValid)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Message = "Only .jpg, .jpeg, and .png files are allowed.",
                        Data = null
                    };
                }

                string uploadPath = Server.MapPath("~/Uploads/Portfolio/Company/" + CID);
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid().ToString("N") + ".png"; // Saving as .png always
                        string relativePath = $"/Uploads/Portfolio/Company/{CID}/{fileName}";
                        Utility.CompressImageinKbs(file, relativePath);
                    }
                }

                return new ResponseModel
                {
                    IsSuccess = true,
                    Message = "Images uploaded successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/DeletePortfolioImage")]
        public ResponseModel DeletePortfolioImage([FromUri] string portfolioImagePath)
        {
            try
            {
                var model = new CompanyViewModel();
                var Server = HttpContext.Current.Server;

                var path = Server.MapPath(portfolioImagePath);

                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                return new ResponseModel() { IsSuccess = true, Message = "Image deleted successfully!" };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetAllYouTubes/{userId}")]
        public ResponseModel GetAllYouTubes(int userId)
        {
            try
            {
                var YouTubeList = _youTubeRepository.GetAllYouTubes(userId, "api/v1/Company/getAllYouTubes/{userId}").OrderByDescending(x => x.CreatedDate).ToList();
                if (YouTubeList.Count > 0)
                {
                    return new ResponseModel() { Message = "Youtube fetched successfully", Data = YouTubeList, IsSuccess = true };
                }
                else
                    return new ResponseModel() { Message = "No records found", Data = null, IsSuccess = false };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Message = ex.Message, Data = null, IsSuccess = false };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetYouTubeById/{youtubeId}")]
        public ResponseModel GetYouTubeById(int youtubeId)
        {
            try
            {
                var model = new YouTubeModel();
                model = _youTubeRepository.GetYouTubeById(youtubeId, "api/v1/Company/getYouTubeById/{youtubeId}");
                return new ResponseModel() { Message = "Youtube detail fetched successfully", Data = model, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Message = ex.Message, IsSuccess = false };
            }

        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpsertYouTube")]
        public ResponseModel UpsertYouTube([FromBody] YouTubeModel model)
        {
            var result = _youTubeRepository.UpsertYouTube(model, "api/v1/Company/UpsertYouTube");
            if (result == 9)
            {
                return new ResponseModel() { Message = "Server Error.", Data = null, IsSuccess = false };
            }

            if (model.YouTubeId > 0 && result == 0)
            {
                return new ResponseModel() { Message = "Youtube detail updated successfully.", Data = null, IsSuccess = true };
            }
            else if (model.YouTubeId == 0 && result == 0)
            {
                return new ResponseModel() { Message = "Youtube detail inserted successfully.", Data = null, IsSuccess = true };
            }
            else return new ResponseModel() { Message = "Some error occurred.", Data = null, IsSuccess = false };

        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/DeleteYoutube/{youtubeId}")]
        public ResponseModel DeleteYoutube(int youtubeId)
        {
            try
            {
                var result = _youTubeRepository.DeleteYouTube(youtubeId, "api/v1/Company/DeleteYoutube/{youtubeId}");
                if (result)
                {
                    return new ResponseModel { IsSuccess = result, Message = "Youtube detail deleted successfully." };
                }
                else
                {
                    return new ResponseModel { IsSuccess = result, Message = "Server Error." };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetAllBlogs/{userId}")]
        public ResponseModel GetAllBlogs(int userId)
        {
            try
            {
                var list = _blogRepository.GetAllBlogs(userId, "api/v1/Company/GetAllBlogs/{userId}").OrderByDescending(x => x.CreatedDate).ToList();
                if (list.Count > 0)
                {
                    return new ResponseModel() { Message = "Blogs fetched successfully", Data = list, IsSuccess = true };
                }
                else
                    return new ResponseModel() { Message = "No records found", Data = null, IsSuccess = false };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Message = ex.Message, Data = null, IsSuccess = false };
            }

        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetBlogById/{blogId}")]
        public ResponseModel GetBlogById(int blogId)
        {
            try
            {
                var model = new BlogModel();
                model = _blogRepository.GetBlogById(blogId, "api/v1/Company/GetBlogById/{blogId}");
                return new ResponseModel() { Message = "Blog detail fetched successfully", Data = model, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Message = ex.Message, IsSuccess = false };
            }

        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/DeleteBlog/{blogId}")]
        public ResponseModel DeleteBlog(int blogId)
        {
            try
            {
                var result = _blogRepository.DeleteBlog(blogId, "api/v1/Company/DeleteBlog/{blogId}");
                if (result)
                {
                    return new ResponseModel { IsSuccess = result, Message = "Blog detail deleted successfully." };
                }
                else
                {
                    return new ResponseModel { IsSuccess = result, Message = "Server Error." };
                }
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpsertBlog")]
        public ResponseModel UpsertBlog([FromBody] BlogModel model)
        {
            var result = _blogRepository.UpsertBlog(model, "api/v1/Company/UpsertBlog");
            if (result == 9)
            {
                return new ResponseModel() { Message = "Server Error.", Data = null, IsSuccess = false };
            }

            if (model.BlogId > 0 && result == 0)
            {
                return new ResponseModel() { Message = "Blog detail updated successfully.", Data = null, IsSuccess = true };
            }
            else if (model.BlogId == 0 && result == 0)
            {
                return new ResponseModel() { Message = "Blog detail inserted successfully.", Data = null, IsSuccess = true };
            }
            else return new ResponseModel() { Message = "Some error occurred.", Data = null, IsSuccess = false };

        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetAllClientTestimonial/{userId}")]
        public ResponseModel GetAllClientTestimonial(int userId)
        {
            try
            {
                int totalRows = 0;
                var testimonials = _testimonialRepo.GetClientTestimonialBy_UserId(userId, out totalRows, "api/v1/Company/GetAllClientTestimonial/{userId}");
                return new ResponseModel { IsSuccess = true, Data = testimonials, Message = "Client Testimonial fetched successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/DeleteTestimonial/{clientId}")]
        public ResponseModel DeleteTestimonial(int clientId)
        {
            try
            {
                _testimonialRepo.DeleteTestimonialBy_Id(clientId, "api/v1/Company/DeleteTestimonial/{clientId}");
                return new ResponseModel { IsSuccess = true, Message = "Client Deleted successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetClientTestimonialDetail/{clientId}")]
        public ResponseModel GetClientTestimonialDetail(int clientId)
        {
            try
            {
                var testimonials = _testimonialRepo.GetClientTestimonialBy_Id(clientId, "api/v1/Company/GetClientTestimonialDetail/{clientId}");
                return new ResponseModel { IsSuccess = true, Data = testimonials, Message = "Client Testimonial detail fetched successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpsertClientTestimonial")]
        public ResponseModel UpsertClientTestimonial()
        {
            try
            {
                bool filesValid = true;

                var req = HttpContext.Current.Request;
                var Server = HttpContext.Current.Server;
                var model = new ClientTestimonialModel();

                var companyName = req.Form["CompanyName"].NullToString();
                var Client_Id = req.Form["Client_Id"].ToIntOrZero();
                var UserId = req.Form["UserId"].ToIntOrZero();
                var CID = req.Form["CompanyId"].ToIntOrZero();
                var ClientName = req.Form["ClientName"].NullToString();
                var Testimonial = req.Form["Testimonial"].NullToString();
                var Designation = req.Form["Designation"].NullToString();

                model.CompanyName = companyName;
                model.Client_Id = Client_Id;
                model.ClientName = ClientName;
                model.Testimonial = Testimonial;
                model.Designation = Designation;
                model.UserId = UserId;

                var files = req.Files;


                if (Client_Id == 0)
                {
                    if (files.Count == 0)
                    {
                        return new ResponseModel
                        {
                            IsSuccess = false,
                            Message = "Please upload client image also.",
                            Data = null
                        };
                    }
                }
                else
                {
                    if (files.Count == 0)
                    {
                        var testimonials = _testimonialRepo.GetClientTestimonialBy_Id(Client_Id, "api/v1/Company/UpsertClientTestimonial");
                        model.PicOfClient = testimonials.PicOfClient;
                    }
                    
                    
                }



                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null)
                    {
                        string fileExtension = Path.GetExtension(file.FileName)?.ToLower();
                        if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
                        {
                            filesValid = false;
                            break;
                        }
                    }
                }

                if (!filesValid)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Message = "Only .jpg, .jpeg, and .png files are allowed.",
                        Data = null
                    };
                }

                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = model.Client_Id == 0 ? DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + model.UserId : model.Client_Id.ToString();
                        var uploadres = Utility.SaveCompressImages(file, fileName, ConfigValues.ImagePath.Substring(1) + "/ClientTestimonial/Company/" + CID, 200);

                        if (uploadres == "invalid")
                        {
                            return new ResponseModel
                            {
                                IsSuccess = false,
                                Message = "Only .jpg, .jpeg, and .png files are allowed.",
                                Data = null
                            };
                        }
                        else if (uploadres == "0")
                        {
                            return new ResponseModel
                            {
                                IsSuccess = false,
                                Message = "Some error occuried.",
                                Data = null
                            };
                        }
                        model.PicOfClient = uploadres;
                    }

                }
                var result = _testimonialRepo.UpsertClientTestimonial(model, "api/v1/Company/UpsertClientTestimonial");
                if (result == 0)
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Message = "Some error occuried.",
                        Data = null
                    };
                }

                return new ResponseModel
                {
                    IsSuccess = true,
                    Message = "Client " + (result == model.Client_Id ? "updated" : "saved") + " successfully!",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/ChangeCompanyAdminPassword")]
        public ResponseModel ChangeCompanyAdminPassword([FromBody] UserModel model)
        {
            model.Password = CryptoHelper.Encrypt(model.Password);
            var result = _compRepo.ChangeCompanyAdminPassword(model, "api/v1/Company/ChangeCompanyAdminPassword");
            if (result == 1)
            {
                return new ResponseModel() { Message = "Password changed successfully.", Data = null, IsSuccess = true };
            }
            else if (result == 9)
            {
                return new ResponseModel() { Message = "Server error.", Data = null, IsSuccess = false };
            }
            else return new ResponseModel() { Message = "User Not found.", Data = null, IsSuccess = false };

        }


        #endregion

        #region User Index

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetAllUsersByCompany/{companyId}")]
        public ResponseModel GetAllUsersByCompany(int companyId)
        {
            try
            {
                int totRows;
                var dt = _compRepo.GetAllUsersByCompany(companyId, out totRows, "api/v1/Company/GetAllUsersByCompany/{companyId}");
                return new ResponseModel
                {
                    IsSuccess = true,
                    Message = "Users fetched successfully",
                    Data = dt
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetUserById/{userId}")]
        public ResponseModel GetUserById(int userId)
        {
            try
            {
                var dt = _userRepo.GetUserById(userId);
                return new ResponseModel() { Message = "User detail fetched successfully", Data = dt, IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { Message = ex.Message, IsSuccess = false };
            }

        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/IsExistCompanyUserDisplayName")]
        public ResponseModel IsExistCompanyUserDisplayName(string displayName)
        {
            try
            {
                if (_compRepo.IsExistCompanyUserDisplayName(displayName, "BharatTouch/Company/CheckCompanyUserDisplayNameAvailability"))
                {
                    return new ResponseModel { Message = "Display Name already exists!", IsSuccess = false };
                }
                else
                    return new ResponseModel { Message = "", IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Message = ex.Message, IsSuccess = false };
            }
        }

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/DeleteUser/{userId}")]
        public ResponseModel DeleteUser(int userId)
        {
            try
            {
                _userRepo.DeleteUser(userId, "BharatTouch/Company/UserIndex/Delete");
                return new ResponseModel { Message = "User deleted successfully.", IsSuccess = true };

            }
            catch (Exception ex)
            {
                return new ResponseModel { Message = ex.Message, IsSuccess = false };

            }
        }

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/UpsertCompanyUser")]
        public ResponseModel UpsertCompanyUser([FromBody] UserModel model)
        {
            try
            {
                int OutUserId;
                int outFlag;
                string outMessage;
                var companyUserName = "";
                var companyUserEmail = "";
                var CompanyName = "";


                var companyDetail = _adminRepo.GetCompanyById_Admin(model.CompanyId);
                if (companyDetail != null)
                {
                    companyUserName = companyDetail.AdminFirstname + " " + companyDetail.AdminLastname;
                    companyUserEmail = companyDetail.AdminEmail;
                    CompanyName = companyDetail.CompanyName;
                }

                _compRepo.Company_User_Upsert(model, out outFlag, out outMessage, out OutUserId);

                if (outFlag == 0)
                {
                    if (model.UserId == 0)
                    {
                        var userLink = "";
                        userLink = ConfigurationManager.AppSettings["WebUrl"].NullToString();
                        userLink = userLink + model.CompanyDisplayName + "/" + model.Displayname;
                        QueueVerificationAdminEmailAsync(model.EmailId, model.FirstName + " " + model.LastName.NullToString(), "Welcome*123", "api/v1/Company/UpsertCompanyUser");
                        CompanyUserEmailToAdmin(model.FirstName, model.EmailId, model.Displayname, CompanyName, userLink, companyUserName + "/" + companyUserEmail, "api/v1/Company/UpsertCompanyUser");
                    }
                    return new ResponseModel { Message = outMessage, IsSuccess = true };
                }
                else
                {
                    return new ResponseModel { Message = outMessage, IsSuccess = false };

                }
            }
            catch (Exception ex)
            {
                return new ResponseModel { Message = ex.Message, IsSuccess = false };
            }

        }

        #endregion

        #region Lead

        [Authorization]
        [HttpPost]
        [Route("api/v1/Company/FetchLeadsByUserId")]
        public ResponseModel FetchLeadsByUserId([FromBody] PaginationModel model)
        {
            try
            {
                int totRows = 0;
                var leads = _userRepo.FetchLeadsByUserId(model.UserId.Value, model.Page, model.Size, model.SortBy, model.SortOrder, model.SearchText, out totRows, "api/v1/Company/FetchLeadsByUserId");
                return new ResponseModel { Data = leads, Message = "Leads fetched successfully", IsSuccess = true, outParam = totRows };
            }
            catch (Exception ex)
            {
                return new ResponseModel { Message = ex.Message, IsSuccess = false };
            }
        }

        #endregion

        #region Page impression

        [Authorization]
        [HttpGet]
        [Route("api/v1/Company/GetViewHistoryByCompanyUser/{userId}")]
        public ResponseModel GetViewHistoryByCompanyUser(int userId)
        {
            try
            {
                int totRows = 0;
                var dt = _compRepo.GetViewHistoryByCompanyUser(userId, out totRows);
                return new ResponseModel { Data = dt, IsSuccess = true, outParam = totRows, Message = "View History fetched successfully" };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message };
            }
        }

        #endregion

        #region  email


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

        public void CompanyUserEmailToAdmin(string name, string email, string displayName, string companyName, string userProfileLink, string companyAdminNameAndEmail, string pageName = "")
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
    }
}