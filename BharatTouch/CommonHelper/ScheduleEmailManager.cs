using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using DataAccess.Repository;
using BharatTouch.CommonHelper;
using WebGrease.Css.Ast;

namespace BharatTouch.CommonHelper
{
    public class ScheduleEmailManager
    {
        #region Declares
        EmailRepository _emailRepo = new EmailRepository();
        #endregion

        public void SendProfileCompletionEmails()
        {
            try
            {
                var root = AppDomain.CurrentDomain.BaseDirectory;
                var emailUserList = new UserRepository().GetProfileCompletionUserEmailList();
                //if (emailUserList != null && emailUserList.Count > 0)
                //{
                //    foreach (var user in emailUserList)
                //    {
                //        var link = ConfigValues.WebUrl + user.Displayname;
                //        string body = string.Empty;
                //        using (var reader = new System.IO.StreamReader(root + @"/EmailTemplate/ProfileCompletionEmail.html"))
                //        {
                //            string readFile = reader.ReadToEnd();
                //            string StrContent = string.Empty;
                //            StrContent = readFile;
                //            //Assing the field values in the template
                //            StrContent = StrContent.Replace("[Subject]", user.EmailSubject);
                //            StrContent = StrContent.Replace("[UserName]", user.Username);
                //            StrContent = StrContent.Replace("[XX]", user.ProfileCompletePercent.ToString());
                //            StrContent = StrContent.Replace("[ProfileLink]", link);
                //            body = StrContent.ToString();
                //        }
                //        string outMessage = string.Empty;
                //        Utility.SendEmail(user.EmailId, user.EmailSubject, body, out outMessage, ConfigValues.MailCC, ConfigValues.MailBcc);
                //    }
                //}
            }
            catch (Exception ex)
            {

            }
        }

        public void SendFailedEmails()
        {
            try
            {
                var emails = _emailRepo.GetAllFailedEmails();
                if(emails.Count > 0)
                {
                    foreach(var email in emails)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(async ct =>
                        {
                            try
                            {
                                await Utility.SendEmailAsync(email.ToEmail, email.Subject, email.Body, email.CC, email.BCC, email.AttachmentPaths, email.Id);
                            }
                            catch (Exception ex)
                            {
                                // Catch all other exceptions                                
                            }
                        });
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}