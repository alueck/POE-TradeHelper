using System;
using System.Linq;

using POETradeHelper.Common.Contract.Attributes;

namespace POETradeHelper.Common.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasSingletonAttribute(this Type type)
        {
            return type.GetCustomAttributes(true).Any(attribute => attribute is SingletonAttribute)
                || type.GetInterfaces().Any(HasSingletonAttribute);
        }
    }
}