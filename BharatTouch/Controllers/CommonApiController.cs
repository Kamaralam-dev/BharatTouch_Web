using DataAccess.ApiHelper;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using BharatTouch.CommonHelper;
using DataAccess;
using System.Configuration;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Office.Interop.Word;
using DataAccess.Repository;
using System.Web.Hosting;
using System.Diagnostics;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CommonApiController : ApiController
    {
        [HttpPost]
        [Route("api/v1/verifyGoogleCaptcha")]
        public async Task<ResponseModel> VerifyGoogleCaptcha([FromBody] RecaptchaModal modal)
        {
            try
            {
                bool isVerified = await IsReCaptchaValid(modal);
                return new ResponseModel()
                {
                    IsSuccess = isVerified,
                    Message = isVerified ? "Captcha verified!" : "Captcha failed!",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/sendEmail")]
        public async Task<ResponseModel> SendEmail([FromBody] EmailRequestModel emailRequest) {
            try
            {
                string outMessage;
                var result = Utility.SendEmail(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body, out outMessage);
                HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                {
                    try
                    {
                        var result1 = await Utility.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body);
                    }
                    catch (Exception ex)
                    {
                        // Catch all other exceptions
                        Trace.TraceError($"[VerificationEmailAsync] Unexpected error for {emailRequest.ToEmail}. Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                    }
                });
                return new ResponseModel()
                {
                    IsSuccess = result,
                    Message = outMessage,
                    Data = null
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/convertPdfToDocx")]
        public ResponseModel ConvertPdfToDocx([FromBody] fileModal modal)
        {
            try
            {
                if (modal.base64Pdf.NullToString() == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Base64 is missing", Data = null };
                
                byte[] pdfBytes = Convert.FromBase64String(modal.base64Pdf);

                string base64Docx = Utility.ConvertPdfToDocx(pdfBytes);

                return new ResponseModel()
                {
                    IsSuccess = base64Docx != "",
                    Message = base64Docx != "" ? "File successfully converted." : "Something went wrong.",
                    Data = base64Docx
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/convertPdfToWordFile")]
        public ResponseModel ConvertPdfToWordFile([FromBody] fileModal modal)
        {
            try
            {
                var baseUrl = ConfigurationManager.AppSettings["WebUrl"].ToString();
                if (modal.base64Pdf.NullToString() == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Base64 is missing", Data = null };

                byte[] pdfBytes = Convert.FromBase64String(modal.base64Pdf);

                byte[] docxbytes;

                using (MemoryStream ms = new MemoryStream(pdfBytes))
                {
                    docxbytes = Freeware.Pdf2Docx.Convert(ms);
                }

                string filePath = "Uploads/AviPdf/WordDocuments/";
                string serverPath = HttpContext.Current.Server.MapPath("~/" + filePath);
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }
                var fileName = "Avipdf_pdfToWord" + Guid.NewGuid().ToString("N") + ".docx";
                string fullPath = Path.Combine(serverPath, fileName);

                File.WriteAllBytes(fullPath, docxbytes);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "File successfully converted.",
                    Data = baseUrl + filePath + fileName
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/convertPdfToWordFileWithFormData")]
        public ResponseModel ConvertPdfToWordFileWithFormData()
        {
            try
            {
                var baseUrl = ConfigurationManager.AppSettings["WebUrl"].ToString();

                if (HttpContext.Current.Request.Files.Count == 0)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "No file found in the request.", Data = null };
                }

                HttpPostedFile uploadedFile = HttpContext.Current.Request.Files[0];

                if (uploadedFile == null || uploadedFile.ContentLength == 0)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "File is empty or not uploaded properly.", Data = null };
                }

                byte[] pdfBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    uploadedFile.InputStream.CopyTo(ms);
                    pdfBytes = ms.ToArray();
                }

                byte[] docxbytes;
                using (MemoryStream ms = new MemoryStream(pdfBytes))
                {
                    docxbytes = Freeware.Pdf2Docx.Convert(ms);
                }

                string filePath = "Uploads/AviPdf/WordDocuments/";
                string serverPath = HttpContext.Current.Server.MapPath("~/" + filePath);
                if (!Directory.Exists(serverPath))
                {
                    Directory.CreateDirectory(serverPath);
                }

                var fileName = "Avipdf_pdfToWord_" + Guid.NewGuid().ToString("N") + ".docx";
                string fullPath = Path.Combine(serverPath, fileName);

                File.WriteAllBytes(fullPath, docxbytes);

                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "File successfully converted.",
                    Data = baseUrl + filePath + fileName
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/v1/configurationTest")]
        public ResponseModel ConfigurationTest()
        {
            try
            {
                var ImagePath = ConfigurationManager.AppSettings["ImagePath"].ToString();
                var ApiUrl = ConfigurationManager.AppSettings["ApiUrl"].ToString();
                var IsApiEnable = ConfigurationManager.AppSettings["IsApiEnable"].ToString();
                var Host = ConfigurationManager.AppSettings["Host"].ToString();
                var FromMail = ConfigurationManager.AppSettings["FromMail"].ToString();
                var FromMailPassword = ConfigurationManager.AppSettings["FromMailPassword"].ToString();
                var Port = ConfigurationManager.AppSettings["Port"].ToString();
                var Mailcc = ConfigurationManager.AppSettings["Mailcc"].ToString();
                var MailBcc = ConfigurationManager.AppSettings["MailBcc"].ToString();
                var WebUrl = ConfigurationManager.AppSettings["WebUrl"].ToString();
                var OwnerEmail = ConfigurationManager.AppSettings["OwnerEmail"].ToString();
                var CaptchaSiteKey = ConfigurationManager.AppSettings["CaptchaSiteKey"].ToString();
                var CaptchaSecretKey = ConfigurationManager.AppSettings["CaptchaSecretKey"].ToString();
                var GoogleApiClientId = ConfigurationManager.AppSettings["GoogleApiClientId"].ToString();
                var GoogleApiClientSecret = ConfigurationManager.AppSettings["GoogleApiClientSecret"].ToString();
                var GoogleApiKey = ConfigurationManager.AppSettings["GoogleApiKey"].ToString();
                var GoogleAuthRedirectUri = ConfigurationManager.AppSettings["GoogleAuthRedirectUri"].ToString();
                var MicrosoftClientId = ConfigurationManager.AppSettings["MicrosoftClientId"].ToString();
                var MicrosoftTenantId = ConfigurationManager.AppSettings["MicrosoftTenantId"].ToString();
                var MicrosoftClientSecret = ConfigurationManager.AppSettings["MicrosoftClientSecret"].ToString();
                var MicrosoftRedirectUri = ConfigurationManager.AppSettings["MicrosoftRedirectUri"].ToString();
                var RazorPayApiKey = ConfigurationManager.AppSettings["RazorPayApiKey"].ToString();
                var RazorPayApiSecret = ConfigurationManager.AppSettings["RazorPayApiSecret"].ToString();
                var JwtKey = ConfigurationManager.AppSettings["config:JwtKey"].ToString();
                var JwtExpireDays = ConfigurationManager.AppSettings["config:JwtExpireDays"].ToString();
                var JwtExpireMinutes = ConfigurationManager.AppSettings["config:JwtExpireMinutes"].ToString();
                var JwtIssuer = ConfigurationManager.AppSettings["config:JwtIssuer"].ToString();
                var JwtAudience = ConfigurationManager.AppSettings["config:JwtAudience"].ToString();


                return new ResponseModel()
                {
                    IsSuccess = true,
                    Message = "All configuration",
                    Data = new
                    {
                        ImagePath,
                        ApiUrl,
                        IsApiEnable,
                        Host,
                        FromMail,
                        FromMailPassword,
                        Port,
                        Mailcc,
                        MailBcc,
                        WebUrl,
                        OwnerEmail,
                        CaptchaSiteKey,
                        CaptchaSecretKey,
                        GoogleApiClientId,
                        GoogleApiClientSecret,
                        GoogleApiKey,
                        GoogleAuthRedirectUri,
                        MicrosoftClientId,
                        MicrosoftTenantId,
                        MicrosoftClientSecret,
                        MicrosoftRedirectUri,
                        RazorPayApiKey,
                        RazorPayApiSecret,
                        JwtKey,
                        JwtExpireDays,
                        JwtExpireMinutes,
                        JwtIssuer,
                        JwtAudience
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        private async Task<bool> IsReCaptchaValid(RecaptchaModal modal)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={modal.secret}&response={modal.token}", null);
                if (response.StatusCode != HttpStatusCode.OK)
                    return false;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject(jsonResponse);
                return result.success == "true";
            }
        }

        public class RecaptchaModal
        {
            public string secret { get; set; }
            public string token { get; set; }
        }
        public class fileModal
        {
            public string base64Pdf { get; set; }
        }
    }
}