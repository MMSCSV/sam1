using System;

namespace CareFusion.Dispensing
{
    /// <summary>
    /// This class is used to map property and/or field info between types
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class MemberInfoAliasAttribute : Attribute
    {
        public MemberInfoAliasAttribute()
        {
        }

        public MemberInfoAliasAttribute(string nameAlias)
        {
            this.NameAlias = nameAlias;
        }

        public string NameAlias { get; set; }
    }
}
