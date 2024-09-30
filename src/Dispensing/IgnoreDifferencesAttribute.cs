using System;

namespace CareFusion.Dispensing
{
    /// <summary>
    /// Causes the decorated property or field to be excluded from the differences algorithm
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IgnoreDifferencesAttribute : Attribute
    {
    }
}
