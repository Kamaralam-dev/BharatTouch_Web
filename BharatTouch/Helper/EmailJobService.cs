using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UpCycle4Good.CommonHelper;

namespace AviCRM.Helper
{
    public static class EmailJobService
    {
        public static async Task VerificationEmailJob(string toEmail, string username, string password, string displayName, string adminEmails, string pageName)
        {
            var activationLink = ConfigurationManager.AppSettings["WebUrl"] + "users/verification?email=" + toEmail;
            var profileLink = ConfigurationManager.AppSettings["WebUrl"] + "/" + displayName;

            string body = SignupTemplate(username, activationLink, toEmail, password, profileLink, pageName);

            await Utility.SendEmailAsync(toEmail, "Bharat Touch - Email verification.", body, "", adminEmails);
        }

        public static string SignupTemplate(string userName, string activationLink, string emailID, string password, string profileLink, string pageName = "")
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

    }
}