using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace climatepi.Shared.Model
{
    public enum PeriodType
    {
        [Description("Today")]
        Today = 1,

        [Description("Yesterday")]
        Yesterday = 3,

        [Description("Last 7 Days")]
        LastSeven = 4,

        [Description("Current Month")]
        CurrentMonth = 5,

        [Description("Last Month")]
        PreviousMonth = 6,

        [Description("Current Year")]
        CurrentYear = 7,

        [Description("Last Year")]
        PreviousYear = 8
    }

    public enum Temperature
    {
        [Description("Celsius")]
        Celsius = 0,

        [Description("Fahrenheit")]
        Fahrenheit = 1,

        [Description("Kelvin")]
        Kelvin = 2
    }

    public static class EnumExtensionMethods
    {
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }
    }

}