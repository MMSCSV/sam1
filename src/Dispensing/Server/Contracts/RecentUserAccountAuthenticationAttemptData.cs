using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Server.Contracts
{
    [DataContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/contracts")]
    public class RecentUserAccountAuthenticationAttemptData
    {
        [DataMember]
        public Guid? LastSuccessfulAuthenticationEventKey { get; set; }

        [DataMember]
        public DateTime? LastSuccessfulAuthenticationUtcDateTime { get; set; }

        [DataMember]
        public string LastSuccessfulAccessingAddress { get; set; }

        [DataMember]
        public string LastSuccessfulAccessingMachineName { get; set; }

        [DataMember]
        public Guid? LastFailedAuthenticationEventKey { get; set; }

        [DataMember]
        public DateTime? LastFailedAuthenticationUtcDateTime { get; set; }

        [DataMember]
        public string LastFailedAccessingAddress { get; set; }

        [DataMember]
        public string LastFailedAccessingMachineName { get; set; }


        [DataMember]
        public int LastFailedAttemptsCount { get; set; }
    }
}
