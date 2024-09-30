using System.Runtime.Serialization;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Extension to the auto-generated SubscriptionTopic class.
    /// </summary>
    public partial class SubscriptionTopic
    {
        [DataMember]
        public SubscriptionTopicTypeInternalCode TopicType { get; set; }

        [DataMember]
        public string TopicId { get; set; }
    }
}
