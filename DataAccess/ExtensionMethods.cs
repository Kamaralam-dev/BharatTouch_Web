using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public static class ExtensionMethods
    {
        #region Methods

        public static string NullToString(this object value)
        {
            return value == DBNull.Value || value == null ? string.Empty : value.ToString().Trim();
        }

        public static bool ToBoolean(this object value)
        {
            bool result;
            bool.TryParse(value.NullToString(), out result);
            return result;
        }

        public static int ToIntOrZero(this object value)
        {
            int result;
            int.TryParse(value.NullToString(), out result);
            return result;
        }

        public static int? ToIntOrNull(this object value)
        {
            int result;
            if (int.TryParse(value.NullToString(), out result))
                return result;
            else
                return default(int?);
        }

        public static decimal ToDecimalOrZero(this object value)
        {
            decimal result;
            decimal.TryParse(value.NullToString(), out result);
            return result;
        }

        public static decimal? ToDecimalOrNULL(this object value)
        {
            decimal result;
            if (decimal.TryParse(value.NullToString(), out result))
                return result;
            else
                return default(decimal?);
        }

        public static DateTime? ToNullableDateTime(this object value, string format = "MM/dd/yyyy")
        {
            var resultDate = value.ToDateTime(format);
            if (resultDate.Year == 1)
            {
                return default(DateTime?);
            }

            return resultDate;
        }

        public static DateTime ToDateTime(this object value, string format = "MM/dd/yyyy")
        {
            var formats = new string[] { "MM/dd/yyyy", "MM/d/yyyy", "M/d/yyyy", "M/dd/yyyy",
            "dd/MM/yyyy", "d/MM/yyyy", "d/M/yyyy", "dd/M/yyyy",
            "yyyy/dd/MM", "yyyy/d/MM", "yyyy/d/M", "yyyy/dd/M",
            "yyyy/MM/dd", "yyyy/MM/d", "yyyy/M/d", "yyyy/M/dd",
            "MM-dd-yyyy", "MM-d-yyyy", "M-d-yyyy", "M-dd-yyyy",
            "dd-MM-yyyy", "d-MM-yyyy", "d-M-yyyy", "dd-M-yyyy",
            "yyyy-dd-MM", "yyyy-d-MM", "yyyy-d-M", "yyyy-dd-M",
            "yyyy-MM-dd", "yyyy-MM-d", "yyyy-M-d", "yyyy-M-dd"
            };

            DateTime result;
            if (!DateTime.TryParseExact(value.NullToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                DateTime.TryParseExact(value.NullToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }
            return result;
        }

        public static DateTime ToValidDateTime(this object value)
        {
            DateTime result;
            DateTime.TryParse(value.NullToString(), out result);
            return result;
        }

        public static string FormatUnitAmount(this object value)
        {
            return value == null ? string.Empty : value.ToString().Replace("£", "").Replace('$', ' ').Replace(',', ' ').Replace(" ", string.Empty);
        }

        public static string FormatToCurrency(this object value)
        {
            return string.Format("{0:c}", value.ToDecimalOrZero());
        }

        public static decimal CurrencyToDecimal(this object value)
        {
            return value == null ? 0 : double.Parse(value.ToString(), NumberStyles.Currency).ToDecimalOrZero();
        }
        #endregion
    }
}
