using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Web.Mvc;
using System.Drawing;
using System.Windows.Documents;
using Microsoft.Ajax.Utilities;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using DataAccess.Models;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Repository;
using System.Web.Http.Results;

namespace BharatTouch.CommonHelper
{
    public static class Utility
    {
        public static bool IsOdd(int rowIndex)
        {
            return rowIndex % 2 != 0;
        }
        public static int PageNumber()
        {
            int startIndex = StartIndex();
            int pageSize = PageSize();

            if (startIndex == 0 || pageSize == 0) return 0;

            return (startIndex / pageSize);
        }

        public static int StartIndex()
        {
            return Convert.ToInt32(HttpContext.Current.Request.Form.GetValues("start").FirstOrDefault());
        }

        public static int PageSize()
        {
            var length = HttpContext.Current.Request.Form.GetValues("length").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            return pageSize;
        }

        public static string SortBy()
        {
            var context = HttpContext.Current.Request;
            var sortByColumn = context.Form.GetValues("columns[" + context.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            return sortByColumn;
        }

        public static string SortDesc()
        {
            var context = HttpContext.Current.Request;
            var sortColumnDir = context.Form.GetValues("order[0][dir]").FirstOrDefault();
            if (string.IsNullOrEmpty(sortColumnDir))
                sortColumnDir = "asc";

            return sortColumnDir;
        }

        public static string FilterText()
        {
            var context = HttpContext.Current.Request;
            return context.Form.GetValues("search[value]")[0];
        }

        public static void SetCookie(string key, string value)
        {
            HttpCookie StudentCookies = new HttpCookie(key);
            StudentCookies.Value = value;
            StudentCookies.Expires = DateTime.Now.AddHours(24);
            HttpContext.Current.Response.SetCookie(StudentCookies);
        }

        public static string GetCookie(string key)
        {
            var result = "";
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                result = HttpContext.Current.Request.Cookies[key].Value;
            }
            return result;
        }

        public static bool RemoveCookie(string key)
        {
            bool result = false;
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var c = new HttpCookie(key);
                c.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(c);
                result = true;
            }
            return result;
        }

        public static string SaveImage(string segment = "temp")
        {
            var context = HttpContext.Current;
            string dbPath = string.Empty;
            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    //string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath = Path.Combine(context.Server.MapPath(ConfigValues.ImagePath), segment);
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, fileName);
                        dbPath = ConfigValues.ImagePath + "/" + segment + "/" + fileName;
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        file.SaveAs(path);
                    }
                    else
                    {
                        dbPath = "0";
                        return dbPath;
                    }
                }
            }
            return dbPath;
        }

        public static string SaveImage(HttpPostedFileBase imgFile)
        {
            var uploadDir = ConfigValues.ImagePath.Substring(1);
            var context = HttpContext.Current;
            string dbPath = string.Empty;
            if (imgFile != null && imgFile.ContentLength > 0)
            {
                string segment = DateTime.Now.ToString("yyyyMMdd");
                string folderPath = Path.Combine(context.Server.MapPath(ConfigValues.ImagePath), segment);
                string fileExtension = Path.GetExtension(imgFile.FileName).ToLower();
                if (string.IsNullOrWhiteSpace(fileExtension))
                {
                    char[] splitImg = { '/' };
                    string[] getExtention = imgFile.ContentType.Split(splitImg);
                    fileExtension = "." + getExtention[1];
                }
                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                {
                    string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                    string CheckImageExtension = Path.GetExtension(fileName);

                    string path = Path.Combine(folderPath, fileName);
                    dbPath = uploadDir + "/" + segment + "/" + fileName;
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    imgFile.SaveAs(path);
                }
                else
                {
                    return "not-valid";
                }
            }
            return dbPath;
        }

        public static string SaveStoryImage(string StoryId)
        {

            var context = HttpContext.Current;
            string dbPath = string.Empty;
            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath = Path.Combine(context.Server.MapPath("~/Uploads/Story"));
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".jpeg")
                    {

                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, "Story" + StoryId + fileExtension);
                        dbPath = "~/Uploads/Story" + "/" + "Story" + StoryId + fileExtension;
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        file.SaveAs(path);
                    }
                    else
                    {
                        dbPath = "0";
                        return dbPath;
                    }
                }
            }
            return dbPath;
        }

        public static string SaveTeamImage(string TeamId)
        {

            var context = HttpContext.Current;
            string dbPath = string.Empty;
            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath = Path.Combine(context.Server.MapPath("~/Uploads/Team"));
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".jpeg")
                    {

                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, "Team" + TeamId + fileExtension);
                        dbPath = "~/Uploads/Team" + "/" + "Team" + TeamId + fileExtension;
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        file.SaveAs(path);
                    }
                    else
                    {
                        dbPath = "0";
                        return dbPath;
                    }
                }
            }
            return dbPath;
        }

        public static string SaveUserImage(string UserId)
        {

            var context = HttpContext.Current;
            string dbPath = string.Empty;
            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    //string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath = Path.Combine(context.Server.MapPath("~/Uploads/Profile"));
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".jpeg")
                    {

                        string fileName = Guid.NewGuid().ToString("N") + fileExtension; ;
                        //string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, fileName);
                        dbPath = "/Uploads/Profile" + "/" + fileName;
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        file.SaveAs(path);
                    }
                    else
                    {
                        dbPath = "0";
                        return dbPath;
                    }
                }
            }
            return dbPath;
        }

        public static string SaveAffiliateImage(string AffiliateId)
        {

            var context = HttpContext.Current;
            string dbPath = string.Empty;
            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath = Path.Combine(context.Server.MapPath("~/Uploads/Affiliate"));
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".jpeg")
                    {

                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, "Affiliate" + AffiliateId + fileExtension);
                        dbPath = "~/Uploads/Affiliate" + "/" + "Affiliate" + AffiliateId + fileExtension;
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        file.SaveAs(path);
                    }
                    else
                    {
                        dbPath = "0";
                        return dbPath;
                    }
                }
            }
            return dbPath;
        }


        public static string SaveItemImage(string ItemId)
        {

            var context = HttpContext.Current;
            string dbPath = string.Empty;
            string path = string.Empty;

            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath;
                    folderPath = Path.Combine(context.Server.MapPath("~/Uploads/Items"), ItemId);

                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }

                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                    {
                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        path = Path.Combine(folderPath, fileName);
                        dbPath = "~/Uploads/Items/" + ItemId + "/" + fileName;

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        file.SaveAs(path);
                    }
                    else
                    {
                        dbPath = "0";
                        return dbPath;
                    }
                }
            }
            return dbPath;
        }

        public static string SaveTempImage()
        {

            var context = HttpContext.Current;
            string dbPath = string.Empty;
            string path = string.Empty;

            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath;
                    folderPath = Path.Combine(context.Server.MapPath("~/Uploads/Temp"));

                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }

                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                    {
                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        path = Path.Combine(folderPath, fileName);
                        dbPath = "~/Uploads/Temp/" + fileName;

                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        file.SaveAs(path);
                    }
                    else
                    {
                        dbPath = "0";
                        return dbPath;
                    }
                }
            }
            return dbPath;
        }
        public static string SaveSponsorImage(string Id)
        {

            var context = HttpContext.Current;
            string dbPath = string.Empty;
            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath = Path.Combine(context.Server.MapPath("~/Uploads/Sponsor"));
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".jpeg")
                    {

                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, "Sponsor" + Id + fileExtension);
                        dbPath = "~/Uploads/Sponsor" + "/" + "Sponsor" + Id + fileExtension;
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        file.SaveAs(path);
                    }
                    else
                    {
                        dbPath = "0";
                        return dbPath;
                    }
                }
            }
            return dbPath;
        }
        public static string ReferenceError(string errorMsg)
        {
            if (errorMsg.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                return "This records is using somewhere else,delete the reference before continue.";
            else
                return errorMsg;
        }

        public static bool SendEmail(String toEmail, String subject, string body, out string outMessage, string cc = "", string bcc = "", string pathToAttachment = "", int emailId = 0)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            bool result = false;
            string message = string.Empty;
            var model = new EmailModel();
            model.Id = emailId;
            model.ToEmail = toEmail;
            model.Subject = subject;
            model.Body = body;
            model.CC = cc;
            model.BCC = bcc;
            model.AttachmentPaths = pathToAttachment;

            //MailMessage mail = new MailMessage();
            //mail.From = new MailAddress("noreply@bharattouch.com");
            //mail.Subject = "This is a test message";
            //mail.Body = "If you can see this mail, your SMTP service is working";
            //mail.To.Add("kamaralamcp@gmail.com");
            //SmtpClient smtp = new SmtpClient("mdin-pp-wb3.webhostbox.net");

            //NetworkCredential credential = new NetworkCredential("noreply@bharattouch.com", "Nav0z7?71");
            //smtp.Credentials = credential;
            //smtp.Port = 25;
            //smtp.Send(mail);
            try
            {
                var host = "mail.bharattouch.com";// "mdin -pp-wb3.webhostbox.net";  //ConfigurationManager.AppSettings["Host"].ToString();
                var fromEmail = "noreply@bharattouch.com"; // ConfigurationManager.AppSettings["FromMail"].ToString();
                var fromEmailPassword = "Nav0z7?71"; // ConfigurationManager.AppSettings["FromMailPassword"].ToString();
                var port = 25; //ConfigurationManager.AppSettings["Port"].ToString();

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;

                if (!string.IsNullOrWhiteSpace(pathToAttachment))
                    mailMessage.Attachments.Add(new Attachment(pathToAttachment));

                string[] ToMultiEmailIds = toEmail.Split(',');
                foreach (string ToEMailId in ToMultiEmailIds)
                {
                    if (!string.IsNullOrWhiteSpace(ToEMailId))
                        mailMessage.To.Add(new MailAddress(ToEMailId)); //adding multiple TO Email Id
                }

                string[] CCIds = cc.Split(',');
                foreach (string CCEmail in CCIds)
                {
                    if (!string.IsNullOrWhiteSpace(CCEmail))
                        mailMessage.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                }

                string[] bccid = bcc.Split(',');
                foreach (string bccEmailId in bccid)
                {
                    if (!string.IsNullOrWhiteSpace(bccEmailId))
                        mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
                }

                SmtpClient smtp = new SmtpClient();
                smtp.Host = host.Trim(); //smtp.gmail.com etc

                //network and security related credentials
                //smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = mailMessage.From.Address;
                NetworkCred.Password = fromEmailPassword;
                //smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = Convert.ToInt32(port);
                //smtp.EnableSsl = true;
                smtp.Send(mailMessage);
                result = true;
                message = "Email sent successfully.";
                outMessage = message;
            }
            catch (Exception ex)
            {
            }
            //catch (Exception ex)
            //{
            //    result = false;
            //    message = ex.Message;
            //}
            outMessage = message;
            model.IsSuccess = result;
            //new EmailRepository().EmailUpsert(model, "Utility.SendEmail");
            return result;
        }

        public static async Task<bool> SendEmailAsync(string toEmail, string subject, string body, string cc = "", string bcc = "", string pathToAttachment = "", int emailId = 0)
        {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var model = new EmailModel();
            model.Id = emailId;
            model.ToEmail = toEmail;
            model.Subject = subject;
            model.Body = body;
            model.CC = cc;
            model.BCC = bcc;
            model.AttachmentPaths = pathToAttachment;
            try
            {
                var host = "mdin-pp-wb3.webhostbox.net";  //ConfigurationManager.AppSettings["Host"].ToString();
                var fromEmail = "noreply@bharattouch.com"; // ConfigurationManager.AppSettings["FromMail"].ToString();
                var fromEmailPassword = "Nav0z7?71"; // ConfigurationManager.AppSettings["FromMailPassword"].ToString();
                var port = 25; //ConfigurationManager.AppSettings["Port"].ToString();

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(fromEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    if (!string.IsNullOrWhiteSpace(pathToAttachment))
                        mailMessage.Attachments.Add(new Attachment(pathToAttachment));

                    string[] toMultiEmailIds = toEmail.Split(',');
                    foreach (string toEmailId in toMultiEmailIds)
                    {
                        if (!string.IsNullOrWhiteSpace(toEmailId))
                            mailMessage.To.Add(new MailAddress(toEmailId));
                    }

                    string[] ccIds = cc.Split(',');
                    foreach (string ccEmail in ccIds)
                    {
                        if (!string.IsNullOrWhiteSpace(ccEmail))
                            mailMessage.CC.Add(new MailAddress(ccEmail));
                    }

                    string[] bccIds = bcc.Split(',');
                    foreach (string bccEmailId in bccIds)
                    {
                        if (!string.IsNullOrWhiteSpace(bccEmailId))
                            mailMessage.Bcc.Add(new MailAddress(bccEmailId));
                    }

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Host = host.Trim();
                        NetworkCredential networkCred = new NetworkCredential
                        {
                            UserName = mailMessage.From.Address,
                            Password = fromEmailPassword
                        };
                        //smtp.UseDefaultCredentials = true;
                        smtp.Credentials = networkCred;
                        smtp.Port = Convert.ToInt32(port);
                        // smtp.EnableSsl = true; // Uncomment and set to true if your SMTP server requires SSL

                        await smtp.SendMailAsync(mailMessage);
                    }
                }

                model.IsSuccess = true;
                new EmailRepository().EmailUpsert(model, "Utility.SendEmailAsync");
                return true;
            }
            catch (Exception ex)
            {
                model.IsSuccess = false;
                new EmailRepository().EmailUpsert(model, "Utility.SendEmailAsync");
                return false;
            }
        }

        //Testing
        public static string GenerateInvoicePDF(string invoiceNumber, string customerName, decimal amount, string savePath)
        {
            string fileName = $"Invoice_{invoiceNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            string fullPath = Path.Combine(savePath, fileName);

            iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(fullPath, FileMode.Create));
            doc.Open();

            // Simple Invoice Content
            doc.Add(new iTextSharp.text.Paragraph("INVOICE"));
            doc.Add(new iTextSharp.text.Paragraph($"Invoice Number: {invoiceNumber}"));
            doc.Add(new iTextSharp.text.Paragraph($"Customer Name: {customerName}"));
            doc.Add(new iTextSharp.text.Paragraph($"Amount: ₹ {amount:N2}"));
            doc.Add(new iTextSharp.text.Paragraph($"Date: {DateTime.Now:dd-MM-yyyy}"));
            doc.Add(new iTextSharp.text.Paragraph("\nThank you for your business!"));

            doc.Close();

            return fullPath; // return full path for use in email attachment
        }


        public static string SaveFile(string dbPath)
        {
            var context = HttpContext.Current;
            string path = string.Empty;
            string dbFilePath = string.Empty;

            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string folderPath = context.Server.MapPath("~" + dbPath);

                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }

                    string fileName = (Guid.NewGuid().ToString("N") + "_separator_" + Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"\s+", "") + fileExtension).ToLower();

                    path = Path.Combine(folderPath, fileName);

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    file.SaveAs(path);
                    dbFilePath = dbPath + fileName;
                }
            }
            return dbFilePath;
        }

        public static string SaveFile(string dbPath, string fileName)
        {
            var context = HttpContext.Current;
            string path = string.Empty;
            string dbFilePath = string.Empty;

            if (context.Request.Files.Count > 0)
            {
                var file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string folderPath = context.Server.MapPath("~" + dbPath);

                    string fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = file.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }

                    path = Path.Combine(folderPath, fileName);

                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    file.SaveAs(path);
                    dbFilePath = Path.Combine(dbPath, fileName);
                }
            }
            return dbFilePath;
        }

        public static bool SaveThumbnailImage(string OrginalImagepath, string destinationfile, int width = 200, int height = 200)
        {
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(OrginalImagepath);
                int srcWidth = image.Width;
                int srcHeight = image.Height;
                int thumbWidth = width;
                int thumbHeight;
                Bitmap bmp;
                if (srcHeight > srcWidth)
                {
                    thumbHeight = (srcHeight / srcWidth) * thumbWidth;
                    bmp = new Bitmap(thumbWidth, thumbHeight);
                }
                else
                {
                    thumbHeight = thumbWidth;
                    thumbWidth = (srcWidth / srcHeight) * thumbHeight;
                    bmp = new Bitmap(thumbWidth, thumbHeight);
                }

                System.Drawing.Graphics gr = System.Drawing.Graphics.FromImage(bmp);
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                System.Drawing.Rectangle rectDestination = new System.Drawing.Rectangle(0, 0, thumbWidth, thumbHeight);
                gr.DrawImage(image, rectDestination, 0, 0, srcWidth, srcHeight, GraphicsUnit.Pixel);
                bmp.Save(destinationfile);
                bmp.Dispose();
                image.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string SaveThumbNailImageWithFilePath(string OriginalImagePath, string destinationDirectory, int width = 200, int height = 200)
        {
            try
            {
                string dbPath = string.Empty;
                if (OriginalImagePath != "")
                {
                    var originalFilePath = HttpContext.Current.Server.MapPath("~" + OriginalImagePath);
                    var filename = Path.GetFileName(originalFilePath);

                    var folderPath = HttpContext.Current.Server.MapPath("~" + destinationDirectory);
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var filePath = Path.Combine(folderPath, filename);
                    var thumbResult = Utility.SaveThumbnailImage(originalFilePath, filePath, width, width);
                    if (thumbResult == true)
                    {
                        dbPath = destinationDirectory + "/" + filename;
                    }

                }
                return dbPath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public static string CompressImageinKbs(HttpPostedFileBase file, string savePath, long targetSizeKB = 400, long initialQuality = 80L)
        {
            try
            {
                string finalSavePath = HttpContext.Current.Server.MapPath(savePath);
                using (Image image = Image.FromStream(file.InputStream))
                {
                    ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders()
                        .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

                    if (jpgEncoder == null) return "0";

                    EncoderParameters encoderParams = new EncoderParameters(1);
                    long quality = initialQuality;

                    // Convert KB to Bytes
                    long targetSizeBytes = targetSizeKB * 1024;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Save the image with initial quality
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                        image.Save(ms, jpgEncoder, encoderParams);

                        // Reduce quality iteratively if file is too large
                        while (ms.Length > targetSizeBytes && quality > 20)
                        {
                            ms.SetLength(0); // Clear MemoryStream
                            quality -= (quality > 50) ? 10 : 5; // Reduce quality
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                            image.Save(ms, jpgEncoder, encoderParams);
                        }

                        // If still too large, resize the image
                        if (ms.Length > targetSizeBytes)
                        {
                            int newWidth = (int)(image.Width * 0.8);
                            int newHeight = (int)(image.Height * 0.8);

                            using (Bitmap resizedImage = new Bitmap(image, newWidth, newHeight))
                            {
                                ms.SetLength(0); // Clear MemoryStream
                                resizedImage.Save(ms, jpgEncoder, encoderParams);
                            }
                        }

                        // Save final compressed image
                        System.IO.File.WriteAllBytes(finalSavePath, ms.ToArray());
                    }
                }
                return savePath;
            }
            catch
            {
                return "0";
            }
        }
        public static string CompressImageinKbs(HttpPostedFile file, string savePath, long targetSizeKB = 400, long initialQuality = 80L)
        {
            try
            {
                string finalSavePath = HttpContext.Current.Server.MapPath(savePath);
                using (Image image = Image.FromStream(file.InputStream))
                {
                    ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageEncoders()
                        .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);

                    if (jpgEncoder == null) return "0";

                    EncoderParameters encoderParams = new EncoderParameters(1);
                    long quality = initialQuality;

                    // Convert KB to Bytes
                    long targetSizeBytes = targetSizeKB * 1024;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Save the image with initial quality
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                        image.Save(ms, jpgEncoder, encoderParams);

                        // Reduce quality iteratively if file is too large
                        while (ms.Length > targetSizeBytes && quality > 20)
                        {
                            ms.SetLength(0); // Clear MemoryStream
                            quality -= (quality > 50) ? 10 : 5; // Reduce quality
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                            image.Save(ms, jpgEncoder, encoderParams);
                        }

                        // If still too large, resize the image
                        if (ms.Length > targetSizeBytes)
                        {
                            int newWidth = (int)(image.Width * 0.8);
                            int newHeight = (int)(image.Height * 0.8);

                            using (Bitmap resizedImage = new Bitmap(image, newWidth, newHeight))
                            {
                                ms.SetLength(0); // Clear MemoryStream
                                resizedImage.Save(ms, jpgEncoder, encoderParams);
                            }
                        }

                        // Save final compressed image
                        System.IO.File.WriteAllBytes(finalSavePath, ms.ToArray());
                    }
                }
                return savePath;
            }
            catch
            {
                return "0";
            }
        }

        public static bool CheckAndCreateDirectory(string path)
        {
            try
            {
                var correctPath = Regex.Replace(path, @"[\\/]", "/").ToLower();
                var folderPath = HttpContext.Current.Server.MapPath("~" + correctPath);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string SaveCompressImages(HttpPostedFileBase imgFile, string fileNameWithoutExention, string directoryPath = "", long targetSizeKB = 400, long initialQuality = 80L)
        {
            string dbPath = string.Empty;
            var context = HttpContext.Current;
            var dirPath = (directoryPath != "" ? directoryPath : ConfigValues.ImagePath.Substring(1)).ToLower();
            var result = "0";
            try
            {
                if (imgFile != null && imgFile.ContentLength > 0)
                {
                    string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath = Path.Combine(context.Server.MapPath("~" + dirPath));
                    string fileExtension = Path.GetExtension(imgFile.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = imgFile.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        string fileName = fileNameWithoutExention + ".png";
                        if (fileNameWithoutExention.NullToString() == "")
                        {
                            fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        }
                        //string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, fileName);
                        dbPath = (dirPath + "/" + fileName).ToLower();
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        result = CompressImageinKbs(imgFile, dbPath, targetSizeKB, initialQuality);
                        return result;
                    }
                    else
                    {
                        return "invalid";
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }
        public static string SaveCompressImages(HttpPostedFile imgFile, string fileNameWithoutExention, string directoryPath = "", long targetSizeKB = 400, long initialQuality = 80L)
        {
            string dbPath = string.Empty;
            var context = HttpContext.Current;
            var dirPath = (directoryPath != "" ? directoryPath : ConfigValues.ImagePath.Substring(1)).ToLower();
            var result = "0";
            try
            {
                if (imgFile != null && imgFile.ContentLength > 0)
                {
                    string segment = DateTime.Now.ToString("yyyyMMdd");
                    string folderPath = Path.Combine(context.Server.MapPath("~" + dirPath));
                    string fileExtension = Path.GetExtension(imgFile.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = imgFile.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        string fileName = fileNameWithoutExention + ".png";
                        if (fileNameWithoutExention.NullToString() == "")
                        {
                            fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        }
                        //string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, fileName);
                        dbPath = (dirPath + "/" + fileName).ToLower();
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        result = CompressImageinKbs(imgFile, dbPath, targetSizeKB, initialQuality);
                        return result;
                    }
                    else
                    {
                        return "invalid";
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }
        //public static string SaveCompressImages(string directoryPath = "", long targetSizeKB = 400, long initialQuality = 80L)
        //{
        //    string dbPath = string.Empty;
        //    var context = HttpContext.Current;
        //    var dirPath = (directoryPath != "" ? directoryPath : ConfigValues.ImagePath.Substring(1)).ToLower();
        //    var result = "0";
        //    try
        //    {
        //        if (context.Request.Files[0] != null && context.Request.Files[0].ContentLength > 0)
        //        {
        //            var imgFile = context.Request.Files[0];
        //            string segment = DateTime.Now.ToString("yyyyMMdd");
        //            string folderPath = Path.Combine(context.Server.MapPath("~" + dirPath), segment);
        //            string fileExtension = Path.GetExtension(imgFile.FileName).ToLower();
        //            if (string.IsNullOrWhiteSpace(fileExtension))
        //            {
        //                char[] splitImg = { '/' };
        //                string[] getExtention = imgFile.ContentType.Split(splitImg);
        //                fileExtension = "." + getExtention[1];
        //            }
        //            if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
        //            {
        //                string fileName = Guid.NewGuid().ToString("N") + fileExtension;
        //                string CheckImageExtension = Path.GetExtension(fileName);

        //                string path = Path.Combine(folderPath, fileName);
        //                dbPath = (dirPath + "/" + segment + "/" + fileName).ToLower();
        //                if (!Directory.Exists(folderPath))
        //                    Directory.CreateDirectory(folderPath);

        //                result = CompressImageinKbs(imgFile, dbPath, targetSizeKB, initialQuality);
        //                return result;
        //            }
        //            else
        //            {
        //                return "invalid";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return result;
        //    }
        //    return result;
        //}
        public static string SaveCompressImagesWithoutSegment(HttpPostedFileBase imgFile, string directoryPath = "", long targetSizeKB = 400, long initialQuality = 80L)
        {
            string dbPath = string.Empty;
            var context = HttpContext.Current;
            var dirPath = (directoryPath != "" ? directoryPath : ConfigValues.ImagePath.Substring(1)).ToLower();
            var result = "0";
            try
            {
                if (imgFile != null && imgFile.ContentLength > 0)
                {
                    string folderPath = Path.Combine(context.Server.MapPath("~" + dirPath));
                    string fileExtension = Path.GetExtension(imgFile.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = imgFile.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, fileName);
                        dbPath = (dirPath + "/" + fileName).ToLower();
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        result = CompressImageinKbs(imgFile, dbPath, targetSizeKB, initialQuality);
                        return result;
                    }
                    else
                    {
                        return "invalid";
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public static string SaveCompressImagesWithoutSegment(HttpPostedFile imgFile, string directoryPath = "", long targetSizeKB = 400, long initialQuality = 80L)
        {
            string dbPath = string.Empty;
            var context = HttpContext.Current;
            var dirPath = (directoryPath != "" ? directoryPath : ConfigValues.ImagePath.Substring(1)).ToLower();
            var result = "0";
            try
            {
                if (imgFile != null && imgFile.ContentLength > 0)
                {
                    string folderPath = Path.Combine(context.Server.MapPath("~" + dirPath));
                    string fileExtension = Path.GetExtension(imgFile.FileName).ToLower();
                    if (string.IsNullOrWhiteSpace(fileExtension))
                    {
                        char[] splitImg = { '/' };
                        string[] getExtention = imgFile.ContentType.Split(splitImg);
                        fileExtension = "." + getExtention[1];
                    }
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        string fileName = Guid.NewGuid().ToString("N") + fileExtension;
                        string CheckImageExtension = Path.GetExtension(fileName);

                        string path = Path.Combine(folderPath, fileName);
                        dbPath = (dirPath + "/" + fileName).ToLower();
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        result = CompressImageinKbs(imgFile, dbPath, targetSizeKB, initialQuality);
                        return result;
                    }
                    else
                    {
                        return "invalid";
                    }
                }
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        public static string GetFileExtensionFromBase64(string base64String)
        {
            try
            {
                byte[] fileBytes = Convert.FromBase64String(base64String);

                // File signature dictionary
                var fileSignatures = new Dictionary<string, string>
            {
                { "FFD8FFE0", ".jpg" },
                { "FFD8FFE1", ".jpg" },
                { "FFD8FFE2", ".jpg" },
                { "FFD8FFE8", ".jpg" },
                { "FFD8FFEB", ".jpg" },
                { "FFD8FFED", ".jpg" },
                { "FFD8FFEE", ".jpg" },
                { "FFD8FFDB", ".jpeg" },
                { "89504E47", ".png" },
                { "47494638", ".gif" },
                { "25504446", ".pdf" },
                { "504B0304", ".zip" },
                { "504B34",   ".docx" }, // Signature for docx files (within zip)
                { "D0CF11E0", ".doc" },   // Signature for older doc files
                { "PK0304", ".pptx" },   // Signature for ppt files
                { "D0CF11E0A1B11AE1", ".xls" },   // Signature for xls files
                // Add more signatures as needed
            };

                // Convert the first few bytes to a hex string
                string fileSignature = BitConverter.ToString(fileBytes).Replace("-", string.Empty).Substring(0, 8);

                // Lookup the extension based on the file signature
                foreach (var signature in fileSignatures)
                {
                    if (fileSignature.StartsWith(signature.Key))
                    {
                        return signature.Value;
                    }
                }

                // Return unknown if the file type isn't recognized
                return "unknown";
            }
            catch (Exception ex)
            {
                return "unknown";
            }
        }

        public static string SaveFileFromBase64(string base64FileString, string dbPath)
        {
            if (string.IsNullOrWhiteSpace(base64FileString))
                return base64FileString;

            var extensionName = GetFileExtensionFromBase64(base64FileString);
            if (extensionName == "unknown")
                return base64FileString;

            var folderPath = HttpContext.Current.Server.MapPath("~" + dbPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            byte[] img = Convert.FromBase64String(base64FileString);
            var imageName = Guid.NewGuid().ToString("N") + extensionName;
            folderPath = Path.Combine(folderPath, imageName);
            using (var file = File.Create(folderPath))
            {
                file.Write(img, 0, img.Length);
            }
            return Path.Combine(dbPath, imageName);
        }

        public static string ConvertPdfToDocx(byte[] pdfBytes)
        {
            using (MemoryStream ms = new MemoryStream(pdfBytes))
            {
                var docxbytes = Freeware.Pdf2Docx.Convert(ms);
                var base64String = Convert.ToBase64String(docxbytes);
                return base64String;
            }
        }

        //public static void ConvertWordToSpecifiedFormat(object input, object output, object format)
        //{
        //    Word._Application application = new Word.Application();
        //    application.Visible = false;
        //    object missing = Missing.Value;
        //    object isVisible = true;
        //    object readOnly = false;
        //    Word._Document document = application.Documents.Open(ref input, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing,
        //                            ref missing, ref missing, ref missing, ref missing, ref isVisible, ref missing, ref missing, ref missing, ref missing);

        //    document.Activate();
        //    document.SaveAs(ref output, ref format, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
        //                    ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
        //    application.Quit(ref missing, ref missing, ref missing);
        //}

        public static string CopyFileToTempFolder(string sourceFilePath)
        {

            try
            {
                string tempFolder = Path.GetTempPath();

                string tempFilePath = Path.Combine(tempFolder, Path.GetFileName(sourceFilePath));

                System.IO.File.Copy(sourceFilePath, tempFilePath, true); // 'true' to overwrite if it exists
                return tempFilePath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static bool IsExistFile(string path)
        {
            try
            {
                var isExist = System.IO.File.Exists(HttpContext.Current.Server.MapPath(path));
                return isExist;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<string> GetFilesFromFolder(string directoryPath)
        {
            List<string> list = new List<string>();
            DirectoryInfo d = new DirectoryInfo(HttpContext.Current.Server.MapPath("~" + directoryPath));//Assuming Test is your Folder
            FileInfo[] Files = null;
            try
            {
                Files = d.GetFiles("*"); //Getting Text files
            }
            catch (Exception ex) { }
            if (Files != null)
            {
                foreach (FileInfo file in Files)
                {
                    var filePath = directoryPath + "/" + file.Name;
                    if (IsExistFile("~" + filePath))
                    {
                        list.Add(filePath);
                    }
                }
            }

            return list;
        }

        public static bool RemoveFile(string filePath)
        {
            try
            {
                string filepath = HttpContext.Current.Server.MapPath(filePath);
                FileInfo file = new FileInfo(filepath);
                if (file.Exists)//check file exsit or not  
                {
                    file.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static string GenerateSHA256Hash(string input, string key)
        {
            using (var hmac = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(key)))
            {
                byte[] hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));

                return BitConverter.ToString(hash).Replace("-", "").ToLower();

            }
        }

        public static ActionState VerifyRazorPayPayment(RazorPaySignatureModel model)
        {
            Dictionary<string, string> attributes = new Dictionary<string, string>
            {
               { "razorpay_order_id", model.razorpay_order_id },
               { "razorpay_payment_id", model.razorpay_payment_id },
                { "razorpay_signature", model.razorpay_signature }
            };


            try
            {
                string expectedSignature = Utility.GenerateSHA256Hash(model.razorpay_order_id + "|" + model.razorpay_payment_id, ConfigValues.RazorPayApiSecret);

                if (expectedSignature != model.razorpay_signature)
                {
                    return new ActionState { Success = false, Message = "Failed!", Data = "Invalid Signature.", Type = ActionState.ErrorType };
                }

                return new ActionState { Success = true, Message = "Congratulation", Data = "Payment Successfull.", Type = ActionState.SuccessType };

            }
            catch (Exception ex)
            {
                return new ActionState { Success = false, Message = "Failed!", Data = ex.Message ?? "Payment error.", Type = ActionState.ErrorType };
            }
        }

        public static string GenerateSlug(string title)
        {
            return Regex.Replace(title.ToLower(), @"[^a-z0-9]+", "-").Trim('-');
        }
    }
}