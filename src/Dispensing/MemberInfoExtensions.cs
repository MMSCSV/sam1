using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CareFusion.Dispensing
{
    public static class MemberInfoExtensions
    {
        public static string GetDisplayName(this MemberInfo member)
        {
            return GetDisplayAttribute(member).GetName();
        }

        public static DisplayAttribute GetDisplayAttribute(MemberInfo member)
        {
            var display = member.GetCustomAttributes<DisplayAttribute>();
            if(display.Length == 0)
            {
                // Exception for developers
                throw new ArgumentException(
                    string.Format("The member {0} on type {1} does not have a DisplayAttribute specified. Specify a DisplayAttribute to internationalize a member.", member.Name, member.DeclaringType), 
                    "member");
            }

            return display[0];
        }
    }
}
