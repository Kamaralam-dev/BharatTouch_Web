
using DataAccess.ApiHelper;
using DataAccess.Models;
using DataAccess.Repository;
using DataAccess.ViewModels;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using SautinSoft;
using System.Web.Http;
using SautinSoft.Document;
using System.Text.RegularExpressions;
using SautinSoft.Document.Drawing;
using DataAccess;

namespace BharatTouch.Controllers
{

    public class UploadApiController : ApiController
    {

        [HttpGet]
        [Route("api/v1/files/getFileAsBase64")]
        public ResponseModel GetFileAsBase64([FromUri] string url)
        {
            try
            {
                if (url.NullToString() == "")
                    return new ResponseModel() { IsSuccess = false, Message = "File url is missing", Data = null };

                string base64String = "";
                using (var client = new WebClient())
                {
                    var filePath = HttpContext.Current.Server.MapPath("~" + url);
                    var fileBytes = client.DownloadData(filePath);
                    var fileStream = new MemoryStream(fileBytes);

                    // Convert the file to a Base64 string
                    base64String = Convert.ToBase64String(fileBytes);
                }

                return new ResponseModel()
                {
                    IsSuccess = base64String != "",
                    Message = "Download FIle",
                    Data = base64String
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel() { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [HttpGet]
        [Route("api/v1/files/download")]
        public HttpResponseMessage DownloadFile([FromUri] string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File URL is missing");
                }

                string filePath = HttpContext.Current.Server.MapPath("~" + url);
                if (!System.IO.File.Exists(filePath))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File not found");
                }

                using (var client = new WebClient())
                {
                    // Use WebClient to download the file as byte array
                    byte[] fileBytes = client.DownloadData(filePath);
                    string fileName = Path.GetFileName(filePath);
                    string mimeType = MimeMapping.GetMimeMapping(filePath);

                    // Prepare the response to download the file
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };

                    // Set the content headers for file download
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                    return response;
                }

            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }




        [Route("api/Common/PDFReplaceText")]
        [System.Web.Http.HttpPost]
        public ResponseModel PDFReplaceText([FromBody] PDFReplaceTextModel model)
        {
            try
            {
                string srcFile = HttpContext.Current.Server.MapPath("~/uploads/Source" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
                string ResultFile = HttpContext.Current.Server.MapPath("~/uploads/Results" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
                byte[] bytess = Convert.FromBase64String(model.OriginalFile);

                System.IO.File.WriteAllBytes(srcFile, bytess);
                //using (PdfReader reader = new PdfReader(srcFile))
                //{
                //    for (int i = 1; i <= reader.NumberOfPages; i++)
                //    {
                //        byte[] contentBytes = reader.GetPageContent(i);
                //        string contentString = PdfEncodings.ConvertToString(contentBytes, PdfObject.TEXT_PDFDOCENCODING);
                //        contentString = contentString.Replace(model.origText, model.replaceText);
                //        reader.SetPageContent(i, PdfEncodings.ConvertToBytes(contentString, PdfObject.TEXT_PDFDOCENCODING));
                //    }
                //    new PdfStamper(reader, new FileStream(ResultFile, FileMode.Create, FileAccess.Write)).Close();
                //}
                DocumentCore dc = DocumentCore.Load(srcFile);
                Regex regex = new Regex(model.OrigText, RegexOptions.IgnoreCase);

                foreach (ContentRange item in dc.Content.Find(regex).Reverse())
                {
                    item.Replace(model.ReplaceText);
                }
                dc.Save(ResultFile, new PdfSaveOptions());
                Byte[] resultbytes = File.ReadAllBytes(ResultFile);
                String resultfile = Convert.ToBase64String(resultbytes);
                File.Delete(srcFile);
                File.Delete(ResultFile);

                return new ResponseModel { IsSuccess = true, Message = "pdf base64string", Data = resultfile };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        [Route("api/Common/PDFRemovePages")]
        [HttpPost]
        /*Passing Remove Pages as range like 1-3 or comma separated 1,2,3*/
        public ResponseModel RemovePages([FromBody] FileTextReplaceModel model)
        {
            try
            {
                string srcFile = HttpContext.Current.Server.MapPath("~/uploads/Source" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
                string ResultFile = HttpContext.Current.Server.MapPath("~/uploads/Results" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
                byte[] bytess = Convert.FromBase64String(model.OriginalFile);

                System.IO.File.WriteAllBytes(srcFile, bytess);
                using (PdfReader reader = new PdfReader(srcFile))
                {
                    var removepages = "";
                    if (model.RemovePages != "")
                    {
                        var pages = model.RemovePages.Split('-');
                        if (pages.Length > 0)
                        {
                            //for (int i = Convert.ToInt32(pages[0]); i <= Convert.ToInt32(pages[1]); i++)
                            //{
                            //    removepages += "!" + i.ToString() + ",";
                            //}

                            for (int i = 0; i <= pages.Length - 1; i++)
                            {
                                removepages += "!" + pages[i].ToString() + ",";
                            }

                        }
                    }
                    else
                    {
                        removepages = string.Join(",", model.RemovePages.Split(',').Select(x => "!" + x)).TrimEnd(',');
                    }
                    // reader.SelectPages(model.KeepPages);
                    reader.SelectPages("0-" + reader.NumberOfPages + "," + removepages.TrimEnd(','));

                    using (PdfStamper stamper = new PdfStamper(reader, File.Create(ResultFile)))
                    {
                        stamper.Close();
                    }
                }
                Byte[] resultbytes = File.ReadAllBytes(ResultFile);
                String resultfile = System.Convert.ToBase64String(resultbytes);

                return new ResponseModel { IsSuccess = true, Message = "Success", Data = resultfile };
            }
            catch (Exception ex)
            {
                return new ResponseModel { IsSuccess = false, Message = ex.Message, Data = null };
            }
        }

        public class PDFReplaceTextModel
        {
            public string OriginalFile { get; set; }
            public string OrigText { get; set; }
            public string ReplaceText { get; set; }

        }

        public class FileTextReplaceModel
        {
            public string OriginalFile { get; set; }
            public string OrigText { get; set; }
            public string ReplaceText { get; set; }
            public string RemovePages { get; set; }
            public string KeepPages { get; set; }
            public string[] Files { get; set; }
        }

        //[Route("api/Common/PDFSplit")]
        //[HttpPost]
        ///*Pass base 64 string  in OriginalFile property */
        //public ResponseModel PDFSplit([FromBody] FileTextReplaceModel model)
        //{
        //    try
        //    {
        //        string srcFile = HttpContext.Current.Server.MapPath("~/uploads/Source" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");

        //        byte[] bytess = System.Convert.FromBase64String(model.OriginalFile);
        //        System.IO.File.WriteAllBytes(srcFile, bytess);
        //        SautinSoft.PdfVision v = new SautinSoft.PdfVision();

        //        DirectoryInfo outFolder = new DirectoryInfo(HttpContext.Current.Server.MapPath("~/uploads/pages/pdf_" + DateTime.Now.ToString("ddMMyyyyhhmmss")));

        //        if (!outFolder.Exists)
        //            outFolder.Create();

        //        int ret = v.SplitPDFFileToPDFFolder(srcFile, outFolder.FullName);
        //        List<Base64> splittedfiles = new List<Base64>();
        //        foreach (FileInfo file in outFolder.GetFiles())
        //        {
        //            Byte[] resultbytes = File.ReadAllBytes(file.FullName);
        //            String resultfile = System.Convert.ToBase64String(resultbytes);
        //            Base64 obj = new Base64();
        //            obj.PDFData = resultfile;
        //            splittedfiles.Add(obj);
        //            file.Delete();
        //        }

        //        return new ResponseModel { IsSuccess = true, Data = splittedfiles.ToList(), Message = "Success" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseModel { IsSuccess = false, Data = "", Message = ex.Message };
        //    }
        //}

        //public class Base64
        //{
        //    public string PDFData { get; set; }
        //}
        //[Route("api/Common/PDFMerge")]
        //[HttpPost]
        ///*Pass string []  in Files property */
        //public ResponseModel PDFMerge([FromBody] FileTextReplaceModel model)
        //{
        //    try
        //    {
        //        string ResultFile = HttpContext.Current.Server.MapPath("~/uploads/Results" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
        //        SautinSoft.PdfMetamorphosis p = new SautinSoft.PdfMetamorphosis();
        //        List<string> files = new List<string>();
        //        foreach (var file in model.Files)
        //        {
        //            string srcFile = HttpContext.Current.Server.MapPath("~/uploads/Source" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
        //            byte[] bytess = System.Convert.FromBase64String(file);
        //            System.IO.File.WriteAllBytes(srcFile, bytess);
        //            files.Add(srcFile);
        //        }
        //        var spath = HttpContext.Current.Server.MapPath("~/uploads/Single" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
        //        p.MergePDFFileArrayToPDFFile(files.ToArray(), spath);
        //        Byte[] resultbytes = File.ReadAllBytes(spath);
        //        String resultfile = System.Convert.ToBase64String(resultbytes);
        //        return new ResponseModel { IsSuccess = true, Data = resultfile, Message = "Success" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseModel { IsSuccess = false, Data = "", Message = ex.Message };
        //    }
        //}

        //[Route("api/Common/PDFESign")]
        //[HttpPost]
        ///*Pass base 64 string  in OriginalFile property */
        //public ResponseModel PDFESign([FromBody]string pdfBase64, string signPicBase64, string CertificatePassword, string Location, string Reason, string ContactInfo)
        //{
        //    try
        //    {
        //        string srcFile = HttpContext.Current.Server.MapPath("~/uploads/Source" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");
        //        string ResultFile = HttpContext.Current.Server.MapPath("~/uploads/Results" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".pdf");

        //        var filePath = string.Empty;
        //        string outDocName = string.Empty;
        //        string dateStamp = DateTime.Now.ToString("ddMMyyyyhhmmss");

        //        if (!string.IsNullOrWhiteSpace(signPicBase64))
        //        {
        //            filePath = Path.Combine(HttpContext.Current.Server.MapPath(ConfigValues.ImagePath), dateStamp);

        //            if (!Directory.Exists(filePath))
        //                Directory.CreateDirectory(filePath);

        //            byte[] bytes = Convert.FromBase64String(signPicBase64);
        //            filePath = Path.Combine(filePath, "signpic");
        //            FileStream file = File.Create(filePath);
        //            file.Write(bytes, 0, bytes.Length);
        //            file.Close();

        //        }

        //        byte[] bytess = System.Convert.FromBase64String(pdfBase64);
        //        System.IO.File.WriteAllBytes(srcFile, bytess);
        //        DocumentCore dc = DocumentCore.Load(srcFile);

        //        // Create a new invisible Shape for the digital signature.
        //        // Place the Shape into top-left corner (0 mm, 0 mm) of page.
        //        Shape signatureShape = new Shape(dc, Layout.Floating(new HorizontalPosition(0f, LengthUnit.Millimeter, HorizontalPositionAnchor.LeftMargin),
        //                                new VerticalPosition(0f, LengthUnit.Millimeter, VerticalPositionAnchor.TopMargin), new Size(1, 1)));
        //        ((FloatingLayout)signatureShape.Layout).WrappingStyle = WrappingStyle.InFrontOfText;
        //        signatureShape.Outline.Fill.SetEmpty();

        //        // Find a first paragraph and insert our Shape inside it.
        //        Paragraph firstPar = dc.GetChildElements(true).OfType<Paragraph>().LastOrDefault();
        //        firstPar.Inlines.Add(signatureShape);

        //        // Picture which symbolizes a handwritten signature.
        //        Picture signaturePict = new Picture(dc, filePath);// HttpContext.Current.Server.MapPath("/slick.png"));

        //        // Signature picture will be positioned:
        //        // 14.5 cm from Top of the Shape.
        //        // 4.5 cm from Left of the Shape.
        //        signaturePict.Layout = Layout.Floating(
        //           new HorizontalPosition(4.5, LengthUnit.Centimeter, HorizontalPositionAnchor.Page),
        //           new VerticalPosition(14.5, LengthUnit.Centimeter, VerticalPositionAnchor.Page),
        //           new Size(20, 10, LengthUnit.Millimeter));

        //        PdfSaveOptions options = new PdfSaveOptions();

        //        // Path to the certificate (*.pfx).
        //        options.DigitalSignature.CertificatePath = HttpContext.Current.Server.MapPath("~/sautinsoft.pfx");

        //        // The password for the certificate.
        //        // Each certificate is protected by a password.
        //        // The reason is to prevent unauthorized the using of the certificate.
        //        options.DigitalSignature.CertificatePassword = CertificatePassword;/// "123456789";

        //        // Additional information about the certificate.
        //        options.DigitalSignature.Location = Location;// "World Wide Web";
        //        options.DigitalSignature.Reason = Reason;// "Document.Net by SautinSoft";
        //        options.DigitalSignature.ContactInfo = ContactInfo;// "info@sautinsoft.com";

        //        // Placeholder where signature should be visualized.
        //        options.DigitalSignature.SignatureLine = signatureShape;

        //        // Visual representation of digital signature.
        //        options.DigitalSignature.Signature = signaturePict;

        //        dc.Save(ResultFile, options);
        //        Byte[] resultbytes = File.ReadAllBytes(ResultFile);
        //        String resultfile = System.Convert.ToBase64String(resultbytes);
        //        return new ResponseModel { IsSuccess = true, Data = resultfile, Message = "Success" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResponseModel { IsSuccess = false, Data = "", Message = ex.Message };
        //    }
        //}

    }
}
