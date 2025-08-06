using NUglify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace BharatTouch.Helper
{
    public class CustomJsMinify : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse response)
        {
            var minified = Uglify.Js(response.Content);
            if (minified.HasErrors)
            {
                // Log or handle minification errors if necessary
                foreach (var error in minified.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(error.Message);
                }
            }
            response.Content = minified.Code ?? response.Content; // Fall back to the original if there's an error
            response.ContentType = "text/javascript";
        }
    }
}