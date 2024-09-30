using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class LocationRepeatPatternTimeSummary
    {
        [DataMember]
        public Guid Key { get; set;}

        [DataMember]
        public Guid RepeatPatternKey { get; set; }

        [DataMember]
        public int TimeOfDay { get; set; }

        public TimeSpan TimeOfDayTimeSpan
        {
            get { return TimeSpan.FromMinutes(TimeOfDay); }
        }
    }
}
