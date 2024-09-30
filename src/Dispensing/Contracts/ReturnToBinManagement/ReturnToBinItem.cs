using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a composite view of data needed for Return To Bin Item for Dispensing and Inventory.
    /// </summary>
  
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public enum ReturnBinAccessStatus
    {
        [EnumMember]
        Available,

        [EnumMember]
        FailedDrawer,

        [EnumMember]
        InaccessibleSecurityGroup
    }
    
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class ReturnToBinItem
    {
        #region item properties

        [DataMember]
        public Guid ItemKey { get; set; }

        [DataMember]
        public string BrandName { get; set; }

        [DataMember]
        public string GenericName { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public string MedId { get; set; }

        [DataMember]
        public string DosageForm { get; set; }

        [DataMember]
        public decimal? StrengthAmount { get; set; }

        [DataMember]
        public string StrengthUnitOfMeasure { get; set; }

        [DataMember]
        public string HazardousWasteDisposalInstructions{ get; set; }

        [DataMember]
        public string HazardousWasteDescription { get; set; }

        [DataMember]
        public decimal? TotalVolume{ get; set; }
        
        [DataMember]
        public string TotalVolumeUnitOfMeasureDisplayCode { get; set; }

        [DataMember]
        public decimal? ConcentrationVolume { get; set; }

        [DataMember]
        public string ConcentrationUnitOfMeasureDisplayCode { get; set; }

        #endregion

        #region storageSpaceItem properties

        [DataMember]
        public Guid StorageSpaceItemKey { get; set; }

        [DataMember]
        public decimal InventoryQuantity { get; set; }

        [DataMember]
        public DateTime? EarliestNextExpirationDate { get; set; }

        [DataMember]
        public decimal? ParQuantity { get; set; }

        [DataMember]
        public decimal? PhysicalMaximumQuantity { get; set; }

        #endregion

        #region storagespace properties

        [DataMember]
        public Guid StorageSpaceKey { get; set; }

        [DataMember]
        public string StorageSpaceName { get; set; }

        [DataMember]
        public string StorageSpacePositionID { get; set; }

        [DataMember]
        public bool? IsClosed { get; set; }

        [DataMember]
        public bool? IsLocked { get; set; }

        [DataMember]
        public bool? IsFailed { get; set; }

        [DataMember]
        public string FailureReason { get; set; }

        [DataMember]
        public bool IsExternalBin { get; set; }

        [DataMember]
        public bool IsInternalBin { get; set; }

        #endregion

        #region UnitOfMeasure Properties

        [DataMember]
        public string IssueUnitOfMeasure { get; set; }

        [DataMember]
        public string RefillUnitOfMeasure { get; set; }

        [DataMember]
        public decimal? UnitsOfIssuePerUnitOfRefill { get; set; }

        [DataMember]
        public Guid? RefillUOMKey { get; set; }

        #endregion

        #region Return Bin Item filters

        [DataMember]
        public bool IsControlledMed { get; set; }

        [DataMember]
        public bool IsWitnessRequired { get; set; }

        #endregion

        #region Additional Transaction properties

        #region keys
        [DataMember]
        public Guid FacilityItemKey { get; set; }

        [DataMember]
        public Guid FacilityItemSnapshotKey { get; set; }

        [DataMember]
        public Guid ItemSnapshotKey { get; set; }

        [DataMember]
        public Guid MedItemKey { get; set; }

        [DataMember]
        public Guid MedItemSnapshotKey { get; set; }

        [DataMember]
        public Guid? IssueUOMKey { get; set; }

        [DataMember]
        public Guid StorageSpaceItemSnapshotKey { get; set; }

        [DataMember]
        public Guid StorageSpaceSnapshotKey { get; set; }

        [DataMember]
        public Guid StorageSpaceInventoryKey { get; set; }

        [DataMember]
        public Guid? RelatedItemTransactionKey { get; set; }

        #endregion

        [DataMember]
        public decimal EnteredQuantity { get; set; }

        [DataMember]
        public UserContextInfo WitnessUserInfo { get; set; }

        [DataMember]
        public bool IsCancelledTransaction { get; set; }

        [DataMember]
        public bool AutoResolveDiscrepancy { get; set; }
        

        #endregion
    }
}
