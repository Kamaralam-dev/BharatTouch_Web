using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace BharatTouch.CommonHelper
{
    public class XmlResult : ActionResult
    {
        private readonly List<SitemapUrlViewModel> _urls;

        public XmlResult(List<SitemapUrlViewModel> urls)
        {
            _urls = urls;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/xml";

            using (var writer = XmlWriter.Create(context.HttpContext.Response.OutputStream, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                foreach (var url in _urls)
                {
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", url.Loc);
                    if (url.LastMod.HasValue)
                        writer.WriteElementString("lastmod", url.LastMod.Value.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                    writer.WriteElementString("priority", url.Priority);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }
    }

}