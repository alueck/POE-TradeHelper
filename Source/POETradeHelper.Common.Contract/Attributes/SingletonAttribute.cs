using System;

namespace POETradeHelper.Common.Contract.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SingletonAttribute : Attribute
    {
    }
}