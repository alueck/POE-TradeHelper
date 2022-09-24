using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace POETradeHelper.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumType)
        {
            var displayAttribute = enumType
                .GetType()
                .GetMember(enumType.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>();

            return displayAttribute != null
                ? displayAttribute.GetName() ?? enumType.ToString()
                : enumType.ToString();
        }
    }
}