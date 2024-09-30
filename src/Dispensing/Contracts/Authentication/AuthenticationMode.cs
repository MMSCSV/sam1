using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public enum AuthenticationMode
    {
        [EnumMember]
        DeviceAuthenticationMode,

        [EnumMember]
        FingerPrintExemptAuthenticationMode,

        [EnumMember]
        FailoverAuthenticationMode,

        [EnumMember]
        CardPinExemptAuthenticationMode
    }
    /*
     * April 6th 2011, As per Jennifer Reitz, CF Support userID and associated parameters SHOULD NOT be translated for internationalization.
     */
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class AuthenticationConstants
    {
        [DataMember] 
        public const string CFSupport = "CF Support";
        [DataMember]
        public const string TemporaryCPMSText = "{0} - TechSupport - 10LUNS"; //DO NOT CHANGE THE FORMAT OF THE TEXT. IT SHOULD BE xx - yy - zz FORMAT
    }
}