using System;
using System.Diagnostics.CodeAnalysis;

namespace POETradeHelper.ItemSearch.UI.Avalonia.Attributes
{
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Property)]
    public class StyleClassesAttribute : Attribute
    {
        public StyleClassesAttribute(params string[] styleClasses)
        {
            this.StyleClasses = styleClasses;
        }

        public string[] StyleClasses { get; }
    }
}