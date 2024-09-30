using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an association of a notice type and a facility.
    /// </summary>
    [Serializable]
    public class FacilityNoticeType : IEntity<Guid>
    {
        #region Constructors

        public FacilityNoticeType()
        {
        }

        public FacilityNoticeType(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator FacilityNoticeType(Guid key)
        {
            return FromKey(key);
        }

        public static FacilityNoticeType FromKey(Guid key)
        {
            return new FacilityNoticeType(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility notice type.
        /// </summary>
        [Column("NoticeTypeFacilityKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a notice type.
        /// </summary>
        public string NoticeTypeInternalCode { get; set; }

        /// <summary>
        /// Gets the non-localizable code that identifies a notice type.
        /// </summary>
        public NoticeTypeInternalCode NoticeType
        {
            get { return NoticeTypeInternalCode.FromInternalCode<NoticeTypeInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the text that describes a notice type.
        /// </summary>
        [Column("NoticeTypeDescriptionText")]
        public string NoticeTypeDescription { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a notice severity.
        /// </summary>
        public string NoticeSeverityInternalCode { get; set; }

        /// <summary>
        /// Gets the non-localizable code that identifies a notice severity.
        /// </summary>
        public NoticeSeverityInternalCode? Severity
        {
            get { return NoticeSeverityInternalCode.FromNullableInternalCode<NoticeSeverityInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the value that determines whether notices of a given notice type are displayed at a given facility.
        /// </summary>
        [Column("DisplayNoticeTypeFlag")]
        public bool DisplayNoticeType { get; set; }

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
