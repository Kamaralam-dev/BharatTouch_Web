using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BharatTouch.CommonHelper
{
    public static class ConfigValues
    {
        public static string ImagePath
        {
            get {
                return  ConfigurationManager.AppSettings["ImagePath"];
            }
        }

        public static string ApiUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ApiUrl"];
            }
        }

        public static bool IsApiEnable
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["IsApiEnable"]);
            }
        }
        
        public static bool IsTaxApplicable
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["isTaxApplicable"]);
            }
        }

        public static string WebUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WebUrl"];
            }
        }
        
        public static string MailCC
        {
            get
            {
                return ConfigurationManager.AppSettings["Mailcc"];
            }
        }
        
        public static string MailBcc
        {
            get
            {
                return ConfigurationManager.AppSettings["MailBcc"];
            }
        }

        public static string WebSocketUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WebSocketUrl"];
            }
        }

        public static string GoogleCaptchaSiteKey
        {
            get
            {
                return ConfigurationManager.AppSettings["CaptchaSiteKey"];
            }
        }

        public static string GoogleCaptchaSecretKey
        {
            get
            {
                return ConfigurationManager.AppSettings["CaptchaSecretKey"];
            }
        }

        public static string GoogleApiClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleApiClientId"];
            }
        }

        public static string GoogleApiClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleApiClientSecret"];
            }
        }

        public static string GoogleApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleApiKey"];
            }
        }

        public static string GoogleAuthRedirectUri
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleAuthRedirectUri"];
            }
        }

        public static string MicrosoftClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["MicrosoftClientId"];
            }
        }

        public static string MicrosoftClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["MicrosoftClientSecret"];
            }
        }

        public static string MicrosoftTenantId
        {
            get
            {
                return ConfigurationManager.AppSettings["MicrosoftTenantId"];
            }
        }

        public static string MicrosoftRedirectUri
        {
            get
            {
                return ConfigurationManager.AppSettings["MicrosoftRedirectUri"];
            }
        }

        public static string RazorPayApiKey
        {
            get
            {
                return ConfigurationManager.AppSettings["RazorPayApiKey"];
            }
        }

        public static string RazorPayApiSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["RazorPayApiSecret"];
            }
        }
        public static string JwtExpireMinutesSetting
        {
            get
            {
                return  ConfigurationManager.AppSettings["config:JwtExpireMinutes"];
            }
        }
        public static string JwtExpireDaysSetting
        {
            get
            {
                return ConfigurationManager.AppSettings["config:JwtExpireDays"];
            }
        }
        public static string JwtKey
        {
            get
            {
                return ConfigurationManager.AppSettings["config:JwtKey"];
            }
        }
        public static string JwtIssuer
        {
            get
            {
                return ConfigurationManager.AppSettings["config:JwtIssuer"];
            }
        }
        public static string JwtAudience
        {
            get
            {
                return ConfigurationManager.AppSettings["config:JwtAudience"];
            }
        }
    }
}