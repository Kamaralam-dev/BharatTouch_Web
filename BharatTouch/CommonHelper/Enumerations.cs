using System;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.ComponentModel.DataAnnotations;

public static class Enumerations
{
    public static Array GetValues(Type enumType)
    {
        string[] a;
        Array b;

        GetEnumData(enumType, out a, out b);
        return b;
    }
    public static void GetEnumData(Type enumType, out string[] enumNames, out Array enumValues)
    {
        FieldInfo[] fields = enumType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        object[] objArray = new object[fields.Length];
        string[] strArray = new string[fields.Length];
        for (int i = 0; i < fields.Length; i++)
        {
            strArray[i] = fields[i].Name;
            objArray[i] = fields[i].GetRawConstantValue();
        }
        IComparer comparer = Comparer.Default;
        for (int j = 1; j < objArray.Length; j++)
        {
            int index = j;
            string str = strArray[j];
            object y = objArray[j];
            bool flag = false;
            while (comparer.Compare(objArray[index - 1], y) > 0)
            {
                strArray[index] = strArray[index - 1];
                objArray[index] = objArray[index - 1];
                index--;
                flag = true;
                if (index == 0)
                {
                    break;
                }
            }
            if (flag)
            {
                strArray[index] = str;
                objArray[index] = y;
            }
        }

        enumNames = strArray;
        enumValues = objArray;
    }
    public static string GetDescription<T>(this T e) where T : IConvertible
    {
        if (e is Enum)
        {
            Type type = e.GetType();
            Array values = System.Enum.GetValues(type);

            foreach (int val in values)
            {
                if (val == e.ToInt32(CultureInfo.InvariantCulture))
                {
                    var memInfo = type.GetMember(type.GetEnumName(val));
                    var descriptionAttribute = memInfo[0]
                        .GetCustomAttributes(typeof(DescriptionAttribute), false)
                        .FirstOrDefault() as DescriptionAttribute;

                    if (descriptionAttribute != null)
                    {
                        return descriptionAttribute.Description;
                    }
                }
            }
        }

        return null; // could also return string.Empty
    }
}

public enum Priority
{
    High,
    Medium,
    Low,   
}

public enum PhoneType
{
    Home,
    Work,
    Mobile,
}

public enum SocialMedia
{
    Twitter,
    LinkedIn,
    Facebook,
    Instagram
}
public enum DataType
{
    Text,
    LetterOnly,
    Number,
    Decimal,
    DatePicker,
    Email
}

public enum CustomControl
{
    TextBox,
    TextArea,
    CheckBox,
    Radio,
    Select,
    MultiSelect
}

public enum Gender
{
    Male,
    Female
}


public enum SaleType
{
    [Display(Name = "None")]
    None,
    [Display(Name = "New Business")]
    NewBusiness,
    [Display(Name = "Existing Business")]
    ExistingBusiness,
}
public enum ServiceType
{
    New,
    Resale
}

public enum ValidUntil
{
    [Display(Name = "Last 7 Day")]
    Last7Day,
    [Display(Name = "Last 14 Day")]
    Last14Day
}

public enum PaymentType
{
    [Display(Name = "Cash")]
    Cash,
    [Display(Name = "Cheque")]
    Cheque,
    [Display(Name = "Bank Transfer")]
    BankTransfer,
}