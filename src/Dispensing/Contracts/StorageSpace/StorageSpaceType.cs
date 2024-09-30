using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    public partial class StorageSpaceType
    {
        #region Additional Properties

        [DataMember]
        public bool DirectlyContainsInventory { get; set; }

        [DataMember]
        public string GenerationCode { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        #endregion
    }
}
