using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Models
{
    /// <summary>
    /// Represents the urgency or timing criticality for performing a pharmacy order task.
    /// </summary>
    [Serializable]
    public class TimingRecordPriority : IEntity<Guid>
    {
        #region Constructors

        public TimingRecordPriority()
        {
        }

        public TimingRecordPriority(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator TimingRecordPriority(Guid key)
        {
            return FromKey(key);
        }

        public static TimingRecordPriority FromKey(Guid key)
        {
            return new TimingRecordPriority(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a repeat pattern.
        /// </summary>
        [Column("TimingRecordPriorityKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a timing record priority.
        /// </summary>
        [Column("TimingRecordPriorityCode")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a timing record priority.
        /// </summary>
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        [Column("SortValue")]
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a standard timing record priority.
        /// </summary>
        public string StandardTimingRecordPriorityInternalCode { get; set; }

        /// <summary>
        /// Gets the code that identifies a standard timing record priority.
        /// </summary>
        public StandardTimingRecordPriorityInternalCode? StandardTimingRecordPriority
        {
            get { return StandardTimingRecordPriorityInternalCode.FromNullableInternalCode<StandardTimingRecordPriorityInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the internal code that identifies a standard timing record priority.
        /// </summary>
        public string StandardTimingRecordPriorityDisplayCode { get; internal set; }

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
