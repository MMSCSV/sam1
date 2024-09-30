using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Server.Contracts
{
    [DataContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/contracts")]
    public class DispensingDeviceData
    {
        [DataMember]
        public Guid Key { get; set; }
        
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ComputerName { get; set; }

        [DataMember]
        public string SyncServerAddress { get; set; }

        [DataMember]
        public ValidationStatus ValidationStatus { get; set; }
    }

    public enum ValidationStatus
    {
        UnknownError,                  // used when device snapshot is updated and returns a failure
        Success,                       // device successfully validated
        DeviceNotDefined,              // device was not found at server
        DeviceAlreadyAssigned,         // device is already assigned to another computer
        ComputerAlreadyAssigned,       // computer is already assigned to another device
        WebServiceUnreachable,         // web service is unreachable
        NoSyncServerURLDefined         // no sync server url defined in system config snapshot
    }
}
