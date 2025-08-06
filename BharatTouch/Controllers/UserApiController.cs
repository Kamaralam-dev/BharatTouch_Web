using DataAccess;
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using SautinSoft.Document.Drawing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Services.Description;
using BharatTouch.CommonHelper;

namespace BharatTouch.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserApiController : ApiController
    {
        UserRepository _userRepo = new UserRepository();

        [HttpPost]
        [Route("api/Users/Login")]
        public ResponseModel Login([FromBody] AviPdfUserModel user)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(user.ProfilePicture) && Utility.GetFileExtensionFromBase64(user.ProfilePicture) != "unknown")
                {
                    var folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Images/");
                    var dBPath = "/Uploads/Images/";
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    byte[] img = Convert.FromBase64String(user.ProfilePicture);
                    var imageName = Guid.NewGuid().ToString("N") + ".Png";
                    folderPath = Path.Combine(folderPath, imageName);
                    using (var file = File.Create(folderPath))
                    {
                        file.Write(img, 0, img.Length);
                    }
                    user.ProfilePicture = Path.Combine(dBPath, imageName);
                }

                var model = _userRepo.AviPdfUserLogin(user);
                return new ResponseModel() { IsSuccess = true, Message = "User Authenticated", Data = model };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/Users/UpdateAccount")]
        public ResponseModel UpdateAccount([FromBody] AviPdfUserModel user)
        {
            try
            {
                if(!string.IsNullOrWhiteSpace(user.ProfilePicture) && Utility.GetFileExtensionFromBase64(user.ProfilePicture) != "unknown")
                {
                    var folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Images/");
                    var dBPath = "/Uploads/Images/";
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    byte[] img = Convert.FromBase64String(user.ProfilePicture);
                    var imageName = Guid.NewGuid().ToString("N") + ".Png";
                    folderPath = Path.Combine(folderPath, imageName);
                    using (var file = File.Create(folderPath))
                    {
                        file.Write(img, 0, img.Length);
                    }
                    user.ProfilePicture = Path.Combine(dBPath, imageName);
                }

                var model = _userRepo.AviPdfUserUpdate(user);
                return new ResponseModel()
                {
                    IsSuccess = model != null,
                    Message = model != null ? "User has been updated." : "Id is missing or incorrect.",
                    Data = model != null ? model : null
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/Users/DeleteAccount/{Id}")]
        public ResponseModel DeleteAccount(int Id)
        {
            try
            {
                _userRepo.AviPdfUserDelete(Id);
                return new ResponseModel() { IsSuccess = true, Message = "User deleted successfully.", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/Users/GetUserList")]
        public ResponseModel GetUserList()
        {
            try
            {
                int totalRows = 0;
                List<UserModel> users = _userRepo.GetAllUsers(0, 20, "FirstName", "ASC", "", out totalRows, "BharatTouch/Api/GetUserList");
                return new ResponseModel() { IsSuccess = true, Message = "User List!", Data = users, outParam = totalRows };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/Users/LoginWIthEmail")]
        public ResponseModel LoginWIthEmail([FromBody] AviPdfUserModel user)
        {
            try
            {
                var model = _userRepo.AviPdfUserLoginWithEmail(user);
                if (model != null)
                    return new ResponseModel() { IsSuccess = true, Message = "User Authenticated", Data = model };
                else
                    return new ResponseModel() { IsSuccess = false, Message = "Email or password is wrong or Account is not activate.", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/Users/SignUpUser")]
        public ResponseModel SignUpUser([FromBody] AviPdfUserModel user)
        {
            try
            {
                var model = _userRepo.AviPdfUserSignUp(user);
                if(model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Email already exists.", Data = null };
                }

                var webUrl = ConfigurationManager.AppSettings["WebUrl"].ToString();

                string outMessage;
                string body = @"
                                <html>
                                <head>
                                    <style>
                                        body {
                                            font-family: Arial, sans-serif;
                                            color: #333333;
                                            line-height: 1.6;
                                        }
                                        .container {
                                            max-width: 600px;
                                            margin: 0 auto;
                                            padding: 20px;
                                            background-color: #f9f9f9;
                                            border: 1px solid #dddddd;
                                            border-radius: 8px;
                                        }
                                        .header {
                                            font-size: 20px;
                                            font-weight: bold;
                                            margin-bottom: 20px;
                                            color: #007BFF;
                                        }
                                        .content {
                                            font-size: 16px;
                                            margin-bottom: 20px;
                                        }
                                        .verify-button {
                                            display: inline-block;
                                            padding: 10px 20px;
                                            background-color: #007BFF;
                                            color: #ffffff;
                                            text-decoration: none;
                                            border-radius: 5px;
                                            font-size: 16px;
                                        }
                                        .footer {
                                            font-size: 14px;
                                            color: #777777;
                                            margin-top: 30px;
                                        }
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <div class='header'>Hello,</div>
                                        <div class='content'>
                                            <p>Thank you for registering with AviPdf!</p>
                                            <p>Please click the button below to verify your email address:</p>
                                            <a href='" + webUrl + "users/VerificationAviPdfUser?email=" + model.Email + @"' class='verify-button'>Verify Email</a>
                                        </div>
                                        <div class='footer'>
                                            Regards,<br/>The AviPdf Team
                                        </div>
                                    </div>
                                </body>
                                </html>";

                Utility.SendEmail(model.Email, "Verify Your Email Address", body, out outMessage);

                return new ResponseModel() { IsSuccess = true, Message = "Verification Email was sent to your email.", Data = null };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/Users/UpdatePassword")]
        public ResponseModel UpdatePassword([FromBody] AviPdfUserModel user)
        {
            try
            {
                var isUpdated = _userRepo.AviPdfUserUpdatePassword(user);
                return new ResponseModel()
                {
                    IsSuccess = isUpdated,
                    Message = isUpdated ? "Password is updated successfully." : "Id is missing or incorrect.",
                    Data = null
                };
            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpPost]
        [Route("api/Users/ForgetPassword")]
        public ResponseModel ForgetPassword([FromBody] AviPdfUserModel user)
        {
            try
            {
                if (user.Email.NullToString() == "")
                    return new ResponseModel() { IsSuccess = false, Message = "Email is Missing", Data = null };

                var newPassword = user.Email.Substring(0, 4)?.NullToString() + "@256*#";
                user.Password = char.ToUpper(newPassword[0]) + newPassword.Substring(1);

                var model = _userRepo.AviPdfUserForgetPassword(user);
                if (model == null)
                {
                    return new ResponseModel() { IsSuccess = false, Message = "Email was wrong", Data = null };
                }

                string outMessage;
                string body = @"
                                <html>
                                <head>
                                    <style>
                                        body {
                                            font-family: Arial, sans-serif;
                                            color: #333333;
                                            line-height: 1.6;
                                        }
                                        .container {
                                            max-width: 600px;
                                            margin: 0 auto;
                                            padding: 20px;
                                            background-color: #f9f9f9;
                                            border: 1px solid #dddddd;
                                            border-radius: 8px;
                                        }
                                        .header {
                                            font-size: 20px;
                                            font-weight: bold;
                                            margin-bottom: 20px;
                                            color: #007BFF;
                                        }
                                        .content {
                                            font-size: 16px;
                                            margin-bottom: 20px;
                                        }
                                        .verify-button {
                                            display: inline-block;
                                            padding: 10px 20px;
                                            background-color: #007BFF;
                                            color: #ffffff;
                                            text-decoration: none;
                                            border-radius: 5px;
                                            font-size: 16px;
                                        }
                                        .footer {
                                            font-size: 14px;
                                            color: #777777;
                                            margin-top: 30px;
                                        }
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <div class='header'>Hello,</div>
                                        <div class='content'>
                                            <p>We have generated a new password for your account:</p>
                                            <p><strong>Your New Password: " + model.Password.NullToString() + @"</strong></p>
                                            <p> Please log in with this password and change the password for security reasons after login.</p>
                                        </div>
                                        <div class='footer'>
                                            Regards,<br/>The AviPdf Team
                                        </div>
                                    </div>
                                </body>
                                </html>";

                Utility.SendEmail(model.Email, "Password Reset: Your New Password", body, out outMessage);
                return new ResponseModel() { IsSuccess = true, Message = "Your new password is sent to your email.", Data = null };

            }
            catch(Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

    }
}