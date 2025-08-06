using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BharatTouch.CommonHelper;
using DataAccess;
using DataAccess.Models;
using DataAccess.Repository;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using BharatTouch.CommonHelper;
using iTextSharp.text.pdf.qrcode;
using DataAccess.ApiHelper;
using static System.Windows.Forms.AxHost;
using System.Collections;
using Antlr.Runtime;
using System.Web.Http.Results;

namespace BharatTouch.Controllers
{
    public class OAuthController : Controller
    {

        OAuthHelpers _oauthHelper = new OAuthHelpers();
        OAuthRepository _oauthRepo = new OAuthRepository();
        UserRepository _userRepo = new UserRepository();
        // GET: OAuth
        public ActionResult Index()
        {
            return View();
        }

        #region Google Oauth

        async public Task<ActionResult> SignInWithGoogleCodeAsync(string code, string state)
        {
            var stateValues = System.Web.HttpUtility.ParseQueryString(state);
            string redirectUri = stateValues["redirectUri"];
            var obj = new ActionState() { Success = false, Message = "Failed!", Data = "Server error or you have declined the permission!", Type = ActionState.ErrorType };
            try
            {
                if (code == null)
                {
                    return obj.ToActionResult(System.Net.HttpStatusCode.OK);
                }

                var res = await _oauthHelper.ExchangeCodeForTokenGoogleAsync(code, Guid.NewGuid().ToString(), redirectUri, new[] { "profile", "email" });

                if (res == null)
                {
                    obj.Data = "Error while exchange code for token!";
                    return obj.ToActionResult(System.Net.HttpStatusCode.OK);
                }

                var userInfo = await _oauthHelper.GetGoogleUserInfo(res.access_token);

                var model = new UserModel();
                model.GoogleId = userInfo.sub;
                model.EmailId = userInfo.email;
                model.FirstName = userInfo.given_name;
                model.LastName = userInfo.family_name;
                model.ProfileImage = userInfo.picture;
                model.IsActive = userInfo.verified_email;

                var result = _userRepo.SignWithGoogle(model, "BharatTouch/OAuth/SignInWithGoogleCodeAsync");
                if (result == null)
                {
                    obj.Data = "Server error, please try again.";
                    return obj.ToActionResult(System.Net.HttpStatusCode.OK);
                }

                Utility.SetCookie("UserId_auth", result.UserId.ToString());
                Utility.SetCookie("UserName_auth", result.FirstName.NullToString() + " " + result.LastName.NullToString());
                Utility.SetCookie("EmailId_auth", result.EmailId);
                Utility.SetCookie("UserUrlCode", result.UrlCode.NullToString());
                Utility.SetCookie("UserType_auth", result.UserType.NullToString());
                Utility.SetCookie("DisplayName_auth", result.Displayname.NullToString());
                var codeOrName = result.UrlCode;
                if (!string.IsNullOrWhiteSpace(result.Displayname))
                {
                    codeOrName = result.Displayname.NullToString();
                }

                obj.Success = true;
                obj.Message = "Congratulation";
                obj.Data = "Login successfully.";
                obj.Type = ActionState.SuccessType;
                obj.OptionalValue = codeOrName;
                return obj.ToActionResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                obj.Data = ex.Message ?? "Some error occured";
                return obj.ToActionResult(System.Net.HttpStatusCode.OK);
            }
        }

        async public Task<ActionResult> AuthenticateGoogleUserCodeAsync(string code, string state)
        {
            var stateValues = System.Web.HttpUtility.ParseQueryString(state);
            string userId = stateValues["userId"];
            string originalUrl = stateValues["originalUrl"];
            string redirectUri = stateValues["redirectUri"];
            var obj = new ActionState() { Success = false, Message = "Failed!", Data = "Server error or you have declined the permission!", OptionalValue = originalUrl };
            try
            {
                if (code == null) {
                    return View(obj);
                }

                var res = await _oauthHelper.ExchangeCodeForTokenGoogleAsync(code, userId, redirectUri, new[] { CalendarService.Scope.Calendar });

                if (res == null)
                {
                    obj.Data = "Error while exchange code for token!";
                    return View(obj);
                }

                var model = new OAuthTokenModel();
                model.UserId = userId.ToIntOrZero();
                model.GoogleAccessToken = res.access_token;
                model.GoogleRefreshToken = res.refresh_token;

                var result = _oauthRepo.UpsertOAuthTokens(model, "google", "BharatTouch/OAuth/AuthenticateUserCode");
                if (!result)
                {
                    obj.Data = "Server Error, Please try again later!";
                    return View(obj);
                }
                obj.Success = true;
                obj.Message = "Congratulation";
                obj.Data = "Calander synced successfully.";
                return View(obj);
                //return new ActionState() { Success = true, Message = "Done!", Data = model.AccessToken, Type = ActionState.SuccessType }.ToActionResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex) 
            {
                obj.Data = ex.Message ?? "Some error occured";
                return View(obj);
            }
        }

        async public Task<ActionResult> RefreshGoogleAccessToken(int userId)
        {
            try
            {
                string actionName = "BharatTouch/OAuth/RefreshGoogleAccessToken";
                var dbToken = _oauthRepo.GetOAuthTokenByUserId(userId, actionName);
                if(dbToken == null || dbToken.GoogleRefreshToken == null)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Record not found.", Type = ActionState.ErrorType }.ToActionResult(System.Net.HttpStatusCode.OK);
                }

                var tokenRes = await _oauthHelper.RefreshAccessTokenGoogleAsync(dbToken.GoogleRefreshToken);
                if (tokenRes == null)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Error refreshing token", Type = ActionState.ErrorType }.ToActionResult(System.Net.HttpStatusCode.OK);
                }
                dbToken.GoogleAccessToken = tokenRes.access_token;

                var result = _oauthRepo.UpsertOAuthTokens(dbToken, "google", actionName);

                if (!result)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Server Error!", Type = ActionState.ErrorType }.ToActionResult(System.Net.HttpStatusCode.OK);
                }

                return new ActionState() { Success = true, Message = "Done!", Data = dbToken.GoogleAccessToken, Type = ActionState.SuccessType }.ToActionResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(System.Net.HttpStatusCode.OK);
            }
        }

        #endregion

        #region Outlook Oauth

        async public Task<ActionResult> SignInWithMicrosoftCodeAsync(string code, string state)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    string script = "<script type='text/javascript'>window.opener.postMessage({ type: 'microsoftAuthResponse', payload: { success: false, message: 'Authentication failed or you declined permission.' } }); window.close();</script>";
                    return Content(script, "text/html");
                }

                var res = await _oauthHelper.ExchangeCodeForTokenMicrosoftAsync(code, state, "User.Read openid profile email");
                if (res == null)
                {
                    string script = "<script type='text/javascript'>window.opener.postMessage({ type: 'microsoftAuthResponse', payload: { success: false, message: 'Error while exchanging code for token!' } }); window.close();</script>";
                    return Content(script, "text/html");
                }

                var userInfo = await _oauthHelper.GetMicrosoftUserInfo(res.access_token);
                if(userInfo == null)
                {
                    string script = "<script type='text/javascript'>window.opener.postMessage({ type: 'microsoftAuthResponse', payload: { success: false, message: 'Error while reading User Data.' } }); window.close();</script>";
                    return Content(script, "text/html");
                }

                var modal = new UserModel();
                modal.MicrosoftId = userInfo.id;
                modal.FirstName = userInfo.givenName;
                modal.LastName = userInfo.surname;
                modal.EmailId = userInfo.mail;

                var user = _userRepo.SignWithMicrosoft(modal);
                if (user == null)
                {
                    string script = "<script type='text/javascript'>window.opener.postMessage({ type: 'microsoftAuthResponse', payload: { success: false, message: 'Server error during user sign-in.' } }); window.close();</script>";
                    return Content(script, "text/html");
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
                string successScript = $"<script type='text/javascript'>window.opener.postMessage({{ type: 'microsoftAuthResponse', payload: {{ success: true, message: 'Microsoft sign-in successful!', redirectUrl: '/edit/{codeOrName}' }} }}); window.close();</script>";
                return Content(successScript, "text/html");
            }
            catch(Exception ex)
            {
                string script = "<script type='text/javascript'>window.opener.postMessage({ type: 'microsoftAuthResponse', payload: { success: false, message: 'An unexpected error occurred.' } }); window.close();</script>";
                return Content(script, "text/html");
            }
        }

        async public Task<ActionResult> AuthenticateMicrosoftUserCodeAsync(string code, string state)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            var splitState = state.Split(';');

            //var stateValues = System.Web.HttpUtility.ParseQueryString(state);
            string userId = splitState[0];
            string originalUrl = splitState[1];
            //string redirectUri = splitState[2];
            //string scopes = splitState[3];
            var obj = new ActionState() { Success = false, Message = "Failed!", Data = "Server error or you have declined the permission!", OptionalValue = originalUrl };
            
            if (string.IsNullOrEmpty(code))
            {
                return View(obj);
            }

            try
            {
                var res = await _oauthHelper.ExchangeCodeForTokenMicrosoftAsync(code, ConfigValues.MicrosoftRedirectUri, "Calendars.ReadWrite offline_access User.Read");
                if (res == null) {
                    obj.Data = "Error while exchange code for token!";
                    return View(obj);
                }

                var model = new OAuthTokenModel();
                model.UserId = userId.ToIntOrZero();
                model.MicrosoftAccessToken = res.access_token;
                model.MicrosoftRefreshToken = res.refresh_token;

                var result = _oauthRepo.UpsertOAuthTokens(model, "microsoft", "BharatTouch/OAuth/AuthenticateUserCode");
                if (!result)
                {
                    obj.Data = "Server Error, Please try again later!";
                    return View(obj);
                }
                obj.Success = true;
                obj.Message = "Congratulation";
                obj.Data = "Calander synced successfully.";
                return View(obj);
            }
            catch (Exception ex)
            {
                obj.Data = ex.Message ?? "Some error occured";
                return View(obj);
            }
        }

        public async Task<ActionResult> RefreshAccessTokenMicrosoftAsync(int userId)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            try
            {
                string actionName = "BharatTouch/OAuth/RefreshAccessTokenMicrosoftAsync";
                var dbToken = _oauthRepo.GetOAuthTokenByUserId(userId, actionName);
                if (dbToken == null || dbToken.MicrosoftRefreshToken == null)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Record not found.", Type = ActionState.ErrorType }.ToActionResult(System.Net.HttpStatusCode.OK);
                }

                var tokenRes = await _oauthHelper.RefreshAccessTokenMicrosoftAsync(dbToken.MicrosoftRefreshToken);

                if (tokenRes == null) 
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Error refreshing token", Type = ActionState.ErrorType }.ToActionResult(System.Net.HttpStatusCode.OK);
                }

                dbToken.MicrosoftAccessToken = tokenRes.access_token;
                dbToken.MicrosoftRefreshToken = tokenRes.refresh_token;

                var result = _oauthRepo.UpsertOAuthTokens(dbToken, "microsoft", actionName);

                if (!result)
                {
                    return new ActionState() { Success = false, Message = "Failed!", Data = "Server Error!", Type = ActionState.ErrorType }.ToActionResult(System.Net.HttpStatusCode.OK);
                }

                return new ActionState() { Success = true, Message = "Done!", Data = dbToken.MicrosoftAccessToken, Type = ActionState.SuccessType }.ToActionResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ActionState() { Success = false, Message = "Failed!", Data = ex.Message, Type = ActionState.ErrorType }.ToActionResult(System.Net.HttpStatusCode.OK);
            }
        }

        #endregion
    }
}