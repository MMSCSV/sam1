using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a context.
    /// </summary>
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class Context : ICloneable
    {
        public Context()
            : this(null)
        { }

        public Context(Actor actor)
        {
            DateTime localNow = DateTime.Now;

            ActionDateTime = localNow;
            ActionUtcDateTime = localNow.ToUniversalTime();
            Actor = actor;
        }

        [DataMember]
        public DateTime ActionDateTime { get; set; }

        [DataMember]
        public DateTime ActionUtcDateTime { get; set; }

        [DataMember]
        public Actor Actor { get; set; }

        [DataMember]
        public UserContextInfo User { get; set; }

        [DataMember]
        public DeviceContextInfo Device { get; set; }

        [DataMember]
        public string AccessingAddress { get; set; }

        [DataMember]
        public string AccessingMachineName { get; set; }

        #region Implementation of ICloneable

        /// <summary>
        /// Performs a shallow-copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public Context Clone()
        {
            return (Context)MemberwiseClone();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}