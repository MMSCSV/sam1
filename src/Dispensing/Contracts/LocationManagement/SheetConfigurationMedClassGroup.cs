using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an association of a med class group and a facility sheet configuration.
    /// </summary>
    [Serializable]
    public class SheetConfigurationMedClassGroup : IEntity<Guid>
    {
        #region Constructors

        public SheetConfigurationMedClassGroup() { }

        public SheetConfigurationMedClassGroup(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator SheetConfigurationMedClassGroup(Guid key)
        {
            return FromKey(key);
        }

        public static SheetConfigurationMedClassGroup FromKey(Guid key)
        {
            return new SheetConfigurationMedClassGroup(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility sheet configuration.
        /// </summary>
        [Column("SheetConfigMedClassGroupKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility sheet configuration .
        /// </summary>
        public Guid SheetConfigKey { get; set; }

        /// <summary>
        /// Gets or sets the med class group key.
        /// </summary>
        public Guid MedClassGroupKey { get; set; }

        /// <summary>
        /// Gets or sets the med class group name.
        /// </summary>
        public string MedClassGroupCode { get; set; }

        /// <summary>
        /// Gets or sets the med class group description text.
        /// </summary>
        [Column("DescriptionText")]
        public string MedClassGroupDescriptionText { get; set; }

        /// <summary>
        /// Gets or sets the sheet config med class group rank value.
        /// </summary>
        public short RankValue { get; set; }

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
