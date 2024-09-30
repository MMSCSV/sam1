using System;
using System.Runtime.Serialization;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class Subscriber : Entity<Guid>
    {
        #region Constructors

        public Subscriber()
        {
        }

        public Subscriber(Guid key)
        {
            Key = key;
        }

        public static implicit operator Subscriber(Guid key)
        {
            return new Subscriber(key);
        }

        public static Subscriber FromKey(Guid key)
        {
            return new Subscriber(key);
        }

        #endregion

        #region Public Properties

        [DataMember(Name = "ExternalSystemKey")]
        public Guid ExternalSystemKey { get; set; }

        [DataMember(Name = "AddressID")]
        public string AddressId { get; set; }

        [DataMember(Name = "AddressValue")]
        public string Address { get; set; }

        [DataMember(Name = "SubscriptionTopics")]
        public SubscriptionTopicInternalCode[] Topics { get; set; }

        [DataMember(Name = "Disconnected")]
        public bool Disconnected { get; set; }

        [DataMember(Name = "IsActive")]
        public bool IsActive { get; set; }

        #endregion
    }
}
