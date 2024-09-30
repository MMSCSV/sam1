using System.Runtime.Serialization;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class Credentials
    {
        [DataMember]
        public AuthenticationMethod AuthenticationMethod { get; set; }

        [DataMember]
        public AuthenticationPurposeInternalCode AuthenticationPurpose { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string Password { get; set; }
        
    }
}
