using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Models
{
    [Serializable]
    public class FacilityKitItem : IEntity<Guid>
    {
        #region Constructors

        public FacilityKitItem()
        {
        }

        public FacilityKitItem(Guid key)
            : this()
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator FacilityKitItem(Guid key)
        {
            return FromKey(key);
        }

        public static FacilityKitItem FromKey(Guid key)
        {
            return new FacilityKitItem(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility kit member.
        /// </summary>
        [Column("FacilityKitMemberKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility kit.
        /// </summary>
        public Guid FacilityKitKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility item.
        /// </summary>
        public Guid FacilityItemKey { get; set; }

        /// <summary>
        /// Gets the generic name of the facility item.
        /// </summary>
        public string GenericName { get; set; }

        /// <summary>
        /// Gets the brand name of the facility item.
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        /// Gets the med item display name
        /// </summary>
        public string MedDisplayName { get; set; }

        /// <summary>
        /// Gets the dosage form code
        /// </summary>
        public string DosageFormCode { get; set; }

        /// <summary>
        /// Gets the "ActiveFlag" of the facility item
        /// </summary>
        [Column("ActiveFlag")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the quantity of an item within a facility kit.
        /// </summary>
        [Column("ItemMemberQuantity")]
        public short Quantity { get; set; }

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
