using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an association of a sheet configuration and a facility.
    /// </summary>
    [Serializable]
    public class SheetConfiguration : IEntity<Guid>
    {
        #region Constructors

        public SheetConfiguration() { }

        public SheetConfiguration(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator SheetConfiguration(Guid key)
        {
            return FromKey(key);
        }

        public static SheetConfiguration FromKey(Guid key)
        {
            return new SheetConfiguration(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility sheet configuration.
        /// </summary>
        [Column("SheetConfigKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the name of the facility.
        /// </summary>
        public string FacilityName { get; internal set; }

        /// <summary>
        /// Gets or sets the sheet type internal code.
        /// </summary>
        public string SheetTypeInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the sheet style key.
        /// </summary>
        public Guid SheetStyleKey { get; set; }

        /// <summary>
        /// Gets or sets the sheet reconcile flag.
        /// </summary>
        [Column("ReconcileFlag")]
        public bool Reconcile { get; set; }

        public short DeliverySignatureReceiptPrintQuantity { get; set; }

        [Column("PageBreakByMedClassFlag")]
        public bool PageBreakByMedClass { get; set; }

        [Column("PageBreakByInjectableFlag")]
        public bool PageBreakByInjectable { get; set; }

        [Column("InPageBreakByMedClassFlag")]
        public bool InPageBreakByMedClass { get; set; }

        [Column("InPageBreakByInjectableFlag")]
        public bool InPageBreakByInjectable { get; set; }

        public short BlankSpaceQuantity { get; set; }

        [Column("EmptyPerSheetTransactionFlag")]
        public bool EmptyPerSheetTransaction { get; set; }

        public SheetConfigurationMedClassGroup[] MedClassGroups { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
