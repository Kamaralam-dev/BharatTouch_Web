using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BharatTouch.CommonHelper
{
    public class DateTimeFormatter
    {
        public static string
            DefaultStyle = "dd/MM/yyyy",
            Style1 = "MM/dd/yyyy",
            Style2 = "hh:mm tt",
            Style3 = "HH:mm",
            Style4 = "MM/dd/yyyy HH:mm",
            Style6 = "dddd, MMMM dd yyyy",
            Style7 = "dd MMMM, yyyy",
            Style8 = "dd MMMM, yyyy hh:mm tt",
            Style9 = "MMMM, yyyy",
            Style10 = "MM/yyyy",
            Style11 = "yyyy";
        public static string ConvertToString(DateTime[] dt, char splitor, string format)
        {
            StringBuilder str = new StringBuilder();
            foreach (var i in dt)
            {
                str.Append(i.ToString(format));
                str.Append(splitor);
            }

            if (str.Length > 0)
                str.Remove(str.Length - 1, 1);

            return str.ToString();
        }

        public static string ConvertToString(DateTime dt, string format)
        {
            return dt.ToString(format);
        }

        public static string ConvertToString(DateTime dt)
        {
            return dt.ToString(DefaultStyle);
        }

        public static string ConvertToString(DateTime? dt, string er)
        {
            if (dt.HasValue)
                return dt.Value.ToString(DefaultStyle);
            else
                return er;
        }

        public static string ConvertToString(DateTime? dt, string er, string format)
        {
            if (dt.HasValue)
                return dt.Value.ToString(format);
            else
                return er;
        }

        public static DateTime? ConvertToDateTime(string dateVal, string format = "dd/MM/yyyy")
        {
            if (!string.IsNullOrEmpty(dateVal))
            {
                DateTime parsedDate = DateTime.ParseExact(dateVal, format, CultureInfo.InvariantCulture);
                return parsedDate;
            }
            return null;
        }

        public static bool GetDateTimeIntersect(
            DateTime range1Start, DateTime range1End,
            DateTime range2Start, DateTime range2End,
            out DateTime begin, out DateTime end)
        {
            begin = DateTime.MinValue;
            end = DateTime.MinValue;

            if (range1Start > range1End || range2Start > range2End)
                throw new ArgumentException("Start date must be smaller than or equals to end date.");

            if (range1Start <= range2Start && range2Start <= range1End)
                begin = range2Start;
            else
            {
                if (range2Start < range1Start)
                    begin = range1Start;
                else
                    return false;//range2Start>range1End, no intersection
            }

            if ((range1Start <= range2End && range2End <= range1End))
                end = range2End;
            else
            {
                if (range2End > range1End)
                    end = range1End;
                else
                    return false;//range2End<range1Start, no intersection
            }

            return true;
        }

        public static DateTime Merge(DateTime o, TimeSpan t)
        {
            return new DateTime(
                o.Year,
                o.Month,
                o.Day,
                t.Hours,
                t.Minutes,
                t.Seconds,
                o.Kind);
        }
    }
}
