using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a message that is received from external systems
    /// </summary>
    public class ReceivedMessage : Entity<Guid>
    {
        #region Constructors

        public ReceivedMessage()
        {

        }

        public ReceivedMessage(Guid key)
        {
            Key = key;
        }


        #endregion

        #region Operator Overloads

        public static implicit operator ReceivedMessage(Guid key)
        {
            return FromKey(key);
        }

        public static ReceivedMessage FromKey(Guid key)
        {
            return new ReceivedMessage(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Get or sets the value that indicates sequence number of the message
        /// </summary>
        public long SequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a received message type.
        /// </summary>
        /// <remarks>
        /// Not updateable.
        /// </remarks>
        public ReceivedMessageTypeInternalCode MessageType { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies an event for example corresponding to an
        /// A01 or an A03.
        /// </summary>
        public string EventInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the name of an external system.
        /// </summary>
        /// <remarks>
        /// Not updateable.
        /// </remarks>
        public string ExternalSystemName { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a facility
        /// </summary>
        public string FacilityCode { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of a message as per a sender.
        /// </summary>
        /// <remarks>
        /// Not updateable.
        /// </remarks>
        public DateTime? MessageDateTime { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time of when a message is created.
        /// </summary>
        /// <remarks>
        /// Not updateable.
        /// </remarks>
        public DateTime CreatedUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of when a message is created.
        /// </summary>
        /// <remarks>
        /// Not updateable.
        /// </remarks>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the XML message.
        /// </summary>
        /// <remarks>
        /// Not updateable.
        /// </remarks>
        public string MessageXml { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a message contains invalid content prior to processing.
        /// </summary>
        /// <remarks>
        /// Not updateable.
        /// </remarks>
        public bool HasContentError { get; set; }

        /// <summary>
        /// Gets or sets the text that describes some content error.
        /// </summary>
        /// <remarks>
        /// Not updateable.
        /// </remarks>
        public string ContentError { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time of when a message is successfully processed regardless of warnings.
        /// </summary>
        public DateTime? SuccessfullyProcessedUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of when a message is successfully processed regardless of warnings.
        /// </summary>
        public DateTime? SuccessfullyProcessedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a received message is available for processing, including re-processing.
        /// </summary>
        public bool AvailableForProcessing { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a message XML has been exported to a text file.
        /// </summary>
        public bool ExportedToTextFile { get; set; }

        /// <summary>
        /// Gets or sets the ID list (IDType^ID~IDType^ID) of a patient
        /// </summary>
        public string PatientIdList { get; set; }

        /// <summary>
        /// Gets or sets the ID of an encounter itself
        /// </summary>
        public string EncounterId { get; set; }

        /// <summary>
        /// Gets or sets the name of a patient silo
        /// </summary>
        public string PatientSiloName { get; set; }

        /// <summary>
        /// Gets or sets the sequence of characters (letters, numbers and/or other symbols) that identify an item
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the ID of a pharmacy order
        /// </summary>
        public string PharmacyOrderId { get; set; }

        /// <summary>
        /// Gets or sets the list ID of a user ID's
        /// </summary>
        public string UserIDListText { get; set; }

        #endregion
    }
}
