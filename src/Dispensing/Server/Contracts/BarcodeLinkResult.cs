using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Server.Contracts
{
    [DataContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/contracts")]
    public class BarcodeLinkResult
    {        
        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public BarcodeLinkStatus MessageCode { get; set; }
    }
    public enum BarcodeLinkStatus
    {
        Success,                // Successfully linked
        ItemAlreadyLinked,      // When item is linked with someother scancode value
        ScancodeAlreadyExists,  // Scancode is linked with some med
        ServiceException,       // any service exception
        Failure                 // failure case
    }
}
