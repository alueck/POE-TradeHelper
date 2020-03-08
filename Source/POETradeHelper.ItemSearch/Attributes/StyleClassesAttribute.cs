using System;

namespace POETradeHelper.ItemSearch.Attributes
{
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