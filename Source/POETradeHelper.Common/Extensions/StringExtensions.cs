using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace POETradeHelper.Common.Extensions
{
    public static class StringExtensions
    {
        public static T? ParseToEnumByDisplayName<T>(this string displayName, StringComparison stringComparison = StringComparison.Ordinal)
            where T : struct, Enum
        {
            var enumType = typeof(T);
            var matchingMemberTuple = enumType
                                    .GetMembers()
                                    .Select(member => (member, displayNameAttribute: member.GetCustomAttribute<DisplayAttribute>()))
                                    .FirstOrDefault(tuple => string.Equals(tuple.displayNameAttribute?.GetName(), displayName, stringComparison));

            return matchingMemberTuple.displayNameAttribute != null
                ? (T)Enum.Parse(enumType, matchingMemberTuple.member.Name)
                : (T?)null;
        }
    }
}