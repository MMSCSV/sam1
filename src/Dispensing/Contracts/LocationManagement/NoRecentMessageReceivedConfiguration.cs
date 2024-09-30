using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of a no recent message received configuration for a
    /// given period of time.
    /// </summary>
    [Serializable]
    public class NoRecentMessageReceivedConfiguration : IEntity<Guid>
    {
        #region Constructors

        public NoRecentMessageReceivedConfiguration()
        {
        }

        public NoRecentMessageReceivedConfiguration(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator NoRecentMessageReceivedConfiguration(Guid key)
        {
            return FromKey(key);
        }

        public static NoRecentMessageReceivedConfiguration FromKey(Guid key)
        {
            return new NoRecentMessageReceivedConfiguration(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a no recent message received configuration.
        /// </summary>
        [Column("NoRecentMessageReceivedConfigKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a no recent message received type.
        /// </summary>
        public string NoRecentMessageReceivedTypeInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the name of message processing behind configuration period.
        /// </summary>
        [Column("ConfigName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the time of day in minutes of when a message processing behind
        /// configuration period starts.
        /// </summary>
        [Column("StartTimeOfDayValue")]
        public short StartTimeOfDay { get; set; }

        /// <summary>
        /// Gets or sets the time of day in minutes of when a message processing behind
        /// configuration period ends.
        /// </summary>
        [Column("EndTimeOfDayValue")]
        public short EndTimeOfDay { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Monday.
        /// </summary>
        [Column("MondayFlag")]
        public bool Monday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Tuesday.
        /// </summary>
        [Column("TuesdayFlag")]
        public bool Tuesday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Wednesday.
        /// </summary>
        [Column("WednesdayFlag")]
        public bool Wednesday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Thursday.
        /// </summary>
        [Column("ThursdayFlag")]
        public bool Thursday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Friday.
        /// </summary>
        [Column("FridayFlag")]
        public bool Friday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Saturday.
        /// </summary>
        [Column("SaturdayFlag")]
        public bool Saturday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Sunday.
        /// </summary>
        [Column("SundayFlag")]
        public bool Sunday { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after which there is deemed to be out-of-date
        /// data because there is a lack of recently received messages.
        /// </summary>
        [Column("NoMessageReceivedDurationAmount")]
        public short? NoMessageReceivedDuration { get; set; }

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
