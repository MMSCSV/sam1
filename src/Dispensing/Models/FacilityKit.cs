using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Models
{
    [Serializable]
    public class FacilityKit : IEntity<Guid>
    {
        #region Constructors

        public FacilityKit()
        {
        }

        public FacilityKit(Guid key)
            : this()
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator FacilityKit(Guid key)
        {
            return FromKey(key);
        }

        public static FacilityKit FromKey(Guid key)
        {
            return new FacilityKit(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility kit.
        /// </summary>
        [Column("FacilityKitKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the name of the facility.
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of the pharmacy information system.
        /// </summary>
        [Column("ExternalSystemKey")]
        public Guid PharmacyInformationSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the name of the pharmacy information system.
        /// </summary>
        [Column("ExternalSystemName")]
        public string PharmacyInformationSystemName { get; set; }

        /// <summary>
        /// Gets or sets the name of the facility kit.
        /// </summary>
        [Column("FacilityKitName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text that describes the facility kit.
        /// </summary>
        /// <value>The description.</value>
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Gets or sets the facility items associated with a facility kit.
        /// </summary>
        public FacilityKitItem[] Items { get; set; }

        /// <summary>
        /// Gets the count of associated devices to a facility kit.
        /// </summary>
        public Guid[] AssociatedDevices { get; set; }

        public int ItemCount 
        { 
            get { return Items != null ? Items.Length : 0; }
        }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
