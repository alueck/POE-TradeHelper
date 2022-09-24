using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace POETradeHelper.Common
{
    /// <summary>
    ///     Source: https://github.com/YohDeadfall/corefx/blob/3b82d9d904143dab57103fcc1a7ef9d610aae865/src/System.Text.Json/src/System/Text/Json/Serialization/JsonSnakeCaseNamingPolicy.cs
    ///     Licensed to the .NET Foundation under one or more agreements.
    ///     The .NET Foundation licenses this file to you under the MIT license.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            // Allocates a string builder with the guessed result length,
            // where 5 is the average word length in English, and
            // max(2, length / 5) is the number of underscores.
            StringBuilder builder = new(name.Length + Math.Max(2, name.Length / 5));
            UnicodeCategory? previousCategory = null;

            for (int currentIndex = 0; currentIndex < name.Length; currentIndex++)
            {
                char currentChar = name[currentIndex];
                if (currentChar == '_')
                {
                    builder.Append('_');
                    previousCategory = null;
                    continue;
                }

                UnicodeCategory currentCategory = char.GetUnicodeCategory(currentChar);

                switch (currentCategory)
                {
                    case UnicodeCategory.UppercaseLetter:
                    case UnicodeCategory.TitlecaseLetter:
                        if (previousCategory == UnicodeCategory.SpaceSeparator ||
                            previousCategory == UnicodeCategory.LowercaseLetter ||
                            previousCategory != UnicodeCategory.DecimalDigitNumber &&
                            currentIndex > 0 &&
                            currentIndex + 1 < name.Length &&
                            char.IsLower(name[currentIndex + 1]))
                        {
                            builder.Append('_');
                        }

                        currentChar = char.ToLower(currentChar);
                        break;

                    case UnicodeCategory.LowercaseLetter:
                    case UnicodeCategory.DecimalDigitNumber:
                        if (previousCategory == UnicodeCategory.SpaceSeparator)
                        {
                            builder.Append('_');
                        }
                        break;

                    default:
                        if (previousCategory != null)
                        {
                            previousCategory = UnicodeCategory.SpaceSeparator;
                        }
                        continue;
                }

                builder.Append(currentChar);
                previousCategory = currentCategory;
            }

            return builder.ToString();
        }
    }
}