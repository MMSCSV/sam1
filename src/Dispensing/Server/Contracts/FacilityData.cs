using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Server.Contracts
{
    [DataContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/contracts")]
    public class FacilityData
    {
        [DataMember]
        public Guid Key { get; set; }
        
        [DataMember]
        public string Name { get; set; }
    }
}
