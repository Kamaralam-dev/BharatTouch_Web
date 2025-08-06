using DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace BharatTouch
{
    public static class HelperExtensions
    {

        public static DateTime Merge(this DateTime date, TimeSpan time)
        {
            return new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
        }

        public static int? ParseAsInt32(object val, int? defaultVal)
        {
            int pv;
            if (Int32.TryParse(Convert.ToString(val), out pv))
                return pv;

            return defaultVal;
        }
        public static T2 GetValueFromDictionary<T1, T2>(this Dictionary<T1, T2> dict, T1 key)
        {
            T2 result;
            if (dict != null && dict.TryGetValue(key, out result))
                return result;
            return default(T2);
        }

        #region HtmlHelper Extension

        public static bool IsDynamicDataNull(dynamic data)
        {
            return data == null || Convert.IsDBNull(data);
        }

        public static string DisplayText(this HtmlHelper helper, dynamic text, string altText)
        {
            if (text == null || Convert.IsDBNull(text))
                return altText;
            else
                return Convert.ToString(text);
        }

        public static MvcHtmlString TimeZoneDropDownList(this HtmlHelper helper, string name, string workTimeZone, object htmlAttributes)
        {
            var timeZoneList = (from a in TimeZoneInfo.GetSystemTimeZones()
                                select new SelectListItem() { Text = "(" + a.BaseUtcOffset + ") " + a.Id, Value = a.Id, Selected = workTimeZone == a.Id }).ToList();
            timeZoneList.Insert(0, new SelectListItem() { Text = "Unspecified", Value = "", Selected = string.IsNullOrWhiteSpace(workTimeZone) });

            return SelectExtensions.DropDownList(helper, name, timeZoneList, htmlAttributes);
        }

        public static string RadioButtonList(this HtmlHelper helper,
            string name, string[] names, Type enumType, string val)
        {
            StringBuilder html = new StringBuilder();
            string[] values = Enum.GetNames(enumType);
            if (names == null)
                names = values;

            string currentVal = string.Empty;

            for (int i = 0; i < names.Length; i++)
            {
                html.Append("<span>");
                TagBuilder tg = new TagBuilder("input");
                tg.MergeAttribute("name", name);
                tg.MergeAttribute("type", "radio");

                if (i < values.Length)
                {
                    //currentVal = Convert.ToString(values.GetValue(i));
                    currentVal = values[i];
                    tg.MergeAttribute("value", currentVal);

                    if (val == currentVal)
                        tg.MergeAttribute("checked", "checked");
                }

                html.Append(tg.ToString());
                html.Append(" ");
                html.Append(names[i]);
                html.Append("</span>");
            }

            return html.ToString();
        }

        public static string HourSelect(this HtmlHelper helper,
            string id, string name, string cssClass, string style, int hour,
            string onchange = "", string option = "")
        {
            TagBuilder tg = new TagBuilder("Select");
            tg.MergeAttribute("id", id);
            tg.MergeAttribute("name", name);
            tg.MergeAttribute("style", style);
            tg.MergeAttribute("class", cssClass);

            StringBuilder options = new StringBuilder();
            if (!string.IsNullOrEmpty(option))
                options.Append("<option>" + option + "</option>");

            for (int i = 0; i < 24; i++)
            {
                if (i == hour)
                    options.Append("<option selected='selected' value='" + i + "'>" + i + "</option>");
                else
                    options.Append("<option value='" + i + "'>" + i + "</option>");
            }

            tg.InnerHtml = options.ToString();

            return tg.ToString();
        }

        public static string MinuteSelect(this HtmlHelper helper,
            string id, string name, string cssClass, string style, int minute,
            string onchange = "", string option = "")
        {
            TagBuilder tg = new TagBuilder("Select");
            tg.MergeAttribute("id", id);
            tg.MergeAttribute("name", name);
            tg.MergeAttribute("style", style);
            tg.MergeAttribute("class", cssClass);

            StringBuilder options = new StringBuilder();
            if (!string.IsNullOrEmpty(option))
                options.Append("<option>" + option + "</option>");
            for (int i = 0; i < 60; i++)
            {
                if (i == minute)
                    options.Append("<option selected='selected' value='" + i + "'>" + i + "</option>");
                else
                    options.Append("<option value='" + i + "'>" + i + "</option>");
            }

            tg.InnerHtml = options.ToString();

            return tg.ToString();
        }

        public static string AlternateCssStyle(
            this HtmlHelper helper,
            string evenStyle, string oddStyle, int num)
        {
            return num % 2 == 0 ? evenStyle : oddStyle;
        }


        /// <summary>
        /// Img tag helper
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="src">Name of the image, do not pass in path of the image.
        /// The path is hard coded to be /Content/UploadFiles/</param>
        /// <param name="alt"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>


        public static MvcHtmlString BuildSelectByEnum(
            this HtmlHelper helper,
            string id, string name,
            Type enumType)
        {
            return BuildSelectByEnum(helper, id, name, -1, enumType, false);
        }

        public static MvcHtmlString BuildSelectByEnum(
            this HtmlHelper helper,
            string id, string name,
            Type enumType, int[] excludeValue)
        {
            return BuildSelectByEnum(helper, id, name, -1, enumType, excludeValue, false);
        }

        public static MvcHtmlString BuildSelectByEnum(
            this HtmlHelper helper,
            string id, string name, int value,
            Type enumType,
            bool withEmptyOption, object htmlAttributes = null)
        {
            return BuildSelectByEnum(helper, id, name, value, enumType, null, withEmptyOption, htmlAttributes);
        }

        public static MvcHtmlString BuildSelectByEnum(
            this HtmlHelper helper,
            string id, string name, int value,
            Type enumType, int[] excludeValue,
            bool withEmptyOption, object htmlAttributes = null)
        {
            TagBuilder select = new TagBuilder("Select");
            select.MergeAttribute("id", id);
            select.MergeAttribute("name", name);

            if (htmlAttributes != null)
            {
                RouteValueDictionary rvd = new RouteValueDictionary(htmlAttributes);
                foreach (var i in rvd)
                {
                    select.MergeAttribute(i.Key.Replace("_", "-"), Convert.ToString(i.Value));
                }
            }

            string[] names = Enum.GetNames(enumType);
            Array values = Enum.GetValues(enumType);

            StringBuilder options = new StringBuilder();
            if (withEmptyOption)
                options.Append("<option value=''>Please select</option>");

            int temp;
            for (int i = 0; i < names.Length; i++)
            {
                temp = Convert.ToInt32(values.GetValue(i));

                if (excludeValue != null && excludeValue.Contains(temp))
                    continue;

                if (temp != value)
                    options.Append("<option value='" + temp + "'>" + names[i] + "</option>");
                else
                    options.Append("<option value='" + temp + "' selected='selected'>" + names[i] + "</option>");
            }

            select.InnerHtml = options.ToString();

            return MvcHtmlString.Create(select.ToString());
        }

        public static MvcHtmlString BuildCheckBox(
            this HtmlHelper helper, string name, bool isChecked, bool isdisabled, object htmlAttributes)
        {
            Dictionary<string, object> attrs = new Dictionary<string, object>();

            if (isChecked)
                attrs.Add("checked", "checked");
            if (isdisabled)
                attrs.Add("disabled", "disabled");

            return helper.CheckBox(name, isChecked, attrs);
        }

        public static MvcHtmlString BuildCheckBox(
                this HtmlHelper helper, string name, string value, bool isChecked, object htmlAttributes)
        {
            TagBuilder checkbox = new TagBuilder("input");
            checkbox.MergeAttribute("type", "checkbox");
            checkbox.MergeAttribute("name", name);
            checkbox.MergeAttribute("value", value);
            if (isChecked)
                checkbox.MergeAttribute("checked", "checked");

            RouteValueDictionary rvd = new RouteValueDictionary(htmlAttributes);
            checkbox.MergeAttributes(rvd);

            return MvcHtmlString.Create(checkbox.ToString());
        }

        /// <summary>
        /// Build an A-Z Index List
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="func">Javascript function. Like func(IndexKey,link)</param>
        /// <returns></returns>
        public static string RenderIndexList(
            this HtmlHelper helper, string func)
        {
            StringBuilder list = new StringBuilder();

            TagBuilder select = new TagBuilder("a");
            select.MergeAttribute("href", "javascript:void(0);");
            select.MergeAttribute("onclick", func + "(null,this)");
            select.InnerHtml = "All";
            list.Append(select.ToString());

            char currIndex = 'A';
            while (currIndex <= 'Z')
            {
                select = new TagBuilder("a");
                select.MergeAttribute("href", "javascript:void(0);");
                select.MergeAttribute("onclick", func + "('" + currIndex.ToString() + "',this)");
                select.InnerHtml = currIndex.ToString();

                list.Append(" ");
                list.Append(select.ToString());

                currIndex++;
            }

            return list.ToString();
        }

        public static string RequestQueryString(this HtmlHelper helper, string key, string defaultVal)
        {
            string val = helper.ViewContext.RequestContext.HttpContext.Request.QueryString[key];

            if (string.IsNullOrEmpty(val))
                return defaultVal;
            else
                return val;
        }

        public static List<SelectListItem> ParseEnumOptions(this HtmlHelper helper, Type enumType, string value)
        {
            var names = Enum.GetNames(enumType);
            //var values = Enum.GetValues(enumType);

            List<SelectListItem> items = new List<SelectListItem>();

            for (int i = 0; i < names.Length; i++)
            {
                var temp = new SelectListItem()
                {
                    Text = names[i].Replace("_", " "),
                    Value = names[i]
                };

                temp.Selected = temp.Value == value;
                items.Add(temp);
            }

            return items;
        }

        public static DateTime? TryParse(string dateString, DateTime? defaultVal)
        {
            DateTime temp;
            if (DateTime.TryParse(dateString, out temp))
            {
                return temp;
            }

            return defaultVal;
        }

        #endregion

        #region AjaxHelper Extension

        public static MvcHtmlString ReplaceModeRouteLink(
            this AjaxHelper helper,
            string linkText, string routeName, string updateTargetId, object routeValues)
        {
            return helper.RouteLink(linkText, routeName, routeValues,
                          new AjaxOptions
                          {
                              HttpMethod = "POST",
                              InsertionMode = InsertionMode.Replace,
                              UpdateTargetId = updateTargetId
                          });
        }

        public static string ImageActionLink(
            this AjaxHelper helper,
            string imageUrl, string altText, string cssclass, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("class", cssclass);
            var link = helper.ActionLink("[replaceme]", actionName, routeValues, ajaxOptions).ToString();
            return link.Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing));
        }

        #endregion

        #region New methods introduced during the responsive implementation process

        public static IEnumerable<SelectListItem> ConvertStringArrayAsSelectListItem(string[] options)
        {
            if (options == null || options.Length == 0)
                return Enumerable.Empty<SelectListItem>();

            return options.Select(t => new SelectListItem { Text = t, Value = t });
        }

        #endregion

        #region Method for video url from iframe
        public static string GetVideoUrlFromIframe(string iframeUrl)
        {
            string videoUrl = "";
            Match matchdec = Regex.Match(iframeUrl.Replace("\"", "'"), @"\ssrc='\b(\S*)\b", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
            while (matchdec.Success)
            {
                if (matchdec.Groups.Count > 1)
                {
                    videoUrl = matchdec.Groups[1].Value;
                }
                matchdec = matchdec.NextMatch();
            }
            return videoUrl;
        }
        #endregion

        public static MvcHtmlString TemplateCustomControl(this HtmlHelper helper, AccountCustomFieldViewModel field, string name, string classname = "")
        {
            var typeClass = string.Empty;
            classname += field.IsRequired ? " required required-left " : "";

            if (field.DataType == DataType.Number.ToString())
                classname += " number ";
            else if (field.DataType == DataType.Email.ToString())
                classname += " email ";
            else if (field.DataType == DataType.Decimal.ToString())
                classname += " decimalnumber ";
            else if (field.DataType == DataType.LetterOnly.ToString())
                classname += " letterswithspace ";

            var fieldType = field.Control.ToLower();
            if (fieldType == "textbox")
            {
                StringBuilder html = new StringBuilder();

                if (field.DataType == DataType.DatePicker.ToString())
                    html.Append("<input type='text' name='" + name + "' class='" + classname + "' data-plugin-datepicker='' value='" + field.FieldValue + "'/>");
                else
                    html.Append("<input type='text' name='" + name + "' class='" + classname + "' value='" + field.FieldValue + "'/>");

                return MvcHtmlString.Create(html.ToString());
            }
            else if (fieldType == "textarea")
            {
                StringBuilder html = new StringBuilder();
                html.Append("<textarea name='" + name + "' class='" + classname + "'>" + field.FieldValue + "</textarea>");
                return MvcHtmlString.Create(html.ToString());
            }
            else if (fieldType == "radio")
            {
                StringBuilder list = new StringBuilder();
                var fieldOptions = field.ControlOption.Split(',');
                for (int i = 0; i <= fieldOptions.Length - 1; i++)
                {
                    TagBuilder radio = new TagBuilder("input");
                    radio.MergeAttribute("type", "radio");
                    radio.MergeAttribute("name", name);
                    radio.MergeAttribute("id", "rdbOpt_" + i);
                    radio.MergeAttribute("class", classname);
                    radio.MergeAttribute("value", fieldOptions[i]);

                    if (fieldOptions[i].ToString() == field.FieldValue)
                        radio.MergeAttribute("checked", "checked");

                    list.Append("<div class='" + classname + "'>" + radio.ToString() + "<label for=" + "rdbOpt_" + i + ">" + fieldOptions[i] + "</label></div>");
                }

                return MvcHtmlString.Create(list.ToString());
            }
            else if (fieldType == "checkbox")
            {
                StringBuilder list = new StringBuilder();
                var fieldOptions = field.ControlOption.Split(',');

                for (int i = 0; i <= fieldOptions.Length - 1; i++)
                {
                    TagBuilder checkbox = new TagBuilder("input");
                    checkbox.MergeAttribute("type", "checkbox");
                    checkbox.MergeAttribute("id", "chkOpt_" + i);
                    checkbox.MergeAttribute("name", name);
                    checkbox.MergeAttribute("class", classname);
                    checkbox.MergeAttribute("value", fieldOptions[i]);
                    if (!string.IsNullOrWhiteSpace(field.FieldValue) && field.FieldValue.ToString().Contains(fieldOptions[i]))
                        checkbox.MergeAttribute("checked", "checked");

                    list.Append("<div class='" + classname + "'>" + checkbox.ToString() + "<label for=" + "chkOpt_" + i + ">" + fieldOptions[i] + "</label></div>");
                }


                return MvcHtmlString.Create(list.ToString());
            }
            else if (fieldType == "select")
            {
                StringBuilder select = new StringBuilder();
                select.Append("<select data-plugin-selectTwo class='" + classname + "' placeholder='Select' name='" + name + "'>");
                var fieldOptions = field.ControlOption.Split(',');
                for (int i = 0; i <= fieldOptions.Length - 1; i++)
                {
                    if (!string.IsNullOrWhiteSpace(field.FieldValue) && field.FieldValue.ToString().Contains(fieldOptions[i]))
                        select.Append("<option value='" + fieldOptions[i] + "' selected='selected'>" + fieldOptions[i] + "</option>");
                    else
                        select.Append("<option value='" + fieldOptions[i] + "'>" + fieldOptions[i] + "</option>");
                }
                select.Append("</select>");
                return MvcHtmlString.Create(select.ToString());
            }
            else if (fieldType == "multiselect")
            {
                StringBuilder select = new StringBuilder();
                select.Append("<select multiple data-plugin-selectTwo  class='" + classname + "'  placeholder='Select Multiple' name='" + name + "'>");
                var fieldOptions = field.ControlOption.Split(',');
                for (int i = 0; i <= fieldOptions.Length - 1; i++)
                {
                    if (!string.IsNullOrWhiteSpace(field.FieldValue) && field.FieldValue.ToString().Contains(fieldOptions[i]))
                        select.Append("<option value='" + fieldOptions[i] + "' selected='selected'>" + fieldOptions[i] + "</option>");
                    else
                        select.Append("<option value='" + fieldOptions[i] + "'>" + fieldOptions[i] + "</option>");
                }
                select.Append("</select>");
                return MvcHtmlString.Create(select.ToString());
            }
            return InputExtensions.TextBox(helper, name, "value");
        }
   
        public static string Controller(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }

        public static string Action(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }
    }
}