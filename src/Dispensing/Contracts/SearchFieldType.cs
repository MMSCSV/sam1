using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    public enum SearchFieldType
    {
        [EnumMember]
        String,

        [EnumMember]
        Text,

        [EnumMember]
        Number,

        [EnumMember]
        Boolean,

        [EnumMember]
        List,

        [EnumMember]
        Date,

        [EnumMember]
        Time,

        [EnumMember]
        DateTime,

        [EnumMember]
        Hidden,

        [EnumMember]
        UTCDate,

        [EnumMember]
        Binary

    }
}
