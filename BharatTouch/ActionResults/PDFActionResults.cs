using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

public class PDFActionResult : ActionResult
{
    private string Url { get; set; }
    private string FileName { get; set; }
    public PDFActionResult(string url, string fileName)
    {
        Url = url;
        FileName = fileName;
    }

    public override void ExecuteResult(ControllerContext context)
    {
        var curContext = HttpContext.Current;
        curContext.Response.Clear();
        curContext.Response.AddHeader("content-disposition", "attachment;filename=" + FileName);
        curContext.Response.Charset = "";
        curContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        curContext.Response.ContentType = "application/ms-excel";

        StringBuilder sb = new StringBuilder();
        FileInfo bootStrapCss = new FileInfo(curContext.Server.MapPath("~/Content/vendor/bootstrap/css/bootstrap.css"));
        FileInfo reportCss = new FileInfo(curContext.Server.MapPath("~/Content/vendor/font-awesome/css/font-awesome.css"));        
        FileInfo reportCss2 = new FileInfo(curContext.Server.MapPath("~/Content/stylesheets/theme.css"));
        FileInfo reportCss3 = new FileInfo(curContext.Server.MapPath("~/Content/stylesheets/skins/default.css"));

        StreamReader srBootStrap = bootStrapCss.OpenText();
        StreamReader srReport = reportCss.OpenText();
        
        StreamReader srReport2 = reportCss2.OpenText();
        StreamReader srReport3 = reportCss3.OpenText();
        
        while (srBootStrap.Peek() >= 0)
        {
            sb.Append(srBootStrap.ReadLine());
        }
        while (srReport.Peek() >= 0)
        {
            sb.Append(srReport.ReadLine());
        }

        while (srReport2.Peek() >= 0)
        {
            sb.Append(srReport2.ReadLine());
        }
        while (srReport3.Peek() >= 0)
        {
            sb.Append(srReport3.ReadLine());
        }

        srBootStrap.Close();
        srReport.Close();
        
        srReport2.Close();
        srReport3.Close();

        var combineCss = "<style id='dynamicCss'>.sub-title label {font-size: 20px!important;}" + sb.ToString() + "</style>";

        var wreq = (HttpWebRequest)WebRequest.Create(Url);
        var wres = (HttpWebResponse)wreq.GetResponse();
        var s = wres.GetResponseStream();
        var sr = new StreamReader(s, Encoding.ASCII);

        var pageHtml = sr.ReadToEnd();        
        pageHtml = pageHtml.Replace("<link href='/Content/stylesheets/theme.css' rel='stylesheet'/>", "");       
        pageHtml = pageHtml.Replace("<link href='/Content/stylesheets/skins/default.css' rel='stylesheet'/>", "");
        pageHtml = pageHtml.Replace("<link href='/Content/vendor/bootstrap/css/bootstrap.css' rel='stylesheet'/>", "");

        curContext.Response.Write(pageHtml);
        curContext.Response.End();
    }

}