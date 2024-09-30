using System;
using System.Linq;
using CareFusion.Dispensing.Models;
using Pyxis.Core.Data.InternalCodes;
using PharmacyOrderScheduling = CareFusion.Dispensing.PharmacyOrderScheduling.Models;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of a pharmacy order for a given period of time.
    /// </summary>
    public class PharmacyOrder: Entity<Guid>
    {
        #region Constructors

        public PharmacyOrder()
        {
        }

        public PharmacyOrder(Guid key)
        {
            Key = key;
        }

        public PharmacyOrder(Guid key, Guid snapshotKey)
        {
            Key = key;
            SnapshotKey = snapshotKey;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrder(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrder FromKey(Guid key)
        {
            return new PharmacyOrder(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the UTC date/time of when the order was created.
        /// </summary>
        public DateTime CreatedUtcDateTime { get; set; }

        /// <summary>
        /// Gets the local date/time of when the order was created.
        /// </summary>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the derived UTC date time of the start of a pharmacy order
        /// </summary>
        public DateTime? EffectiveUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the derived local date time of the start of a pharmacy order
        /// </summary>
        public DateTime? EffectiveDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether only a date (and not ime) is recorded for when a pharmacy
        /// order becomes effective.
        /// </summary>
        public bool IsEffectiveDateOnly { get; set; }

        /// <summary>
        /// Gets or sets the derived UTC date time of the end of a pharmacy order
        /// </summary>
        public DateTime? ExpirationUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the derived local date time of the end of a pharmacy order
        /// </summary>
        public DateTime? ExpirationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether only a date (and not ime) is recorded for when a pharmacy
        /// order expires.
        /// </summary>
        public bool IsExpirationDateOnly { get; set; }

        /// <summary>
        /// Gets or sets the derived total number of occurrences a pharmacy order should be administered
        /// </summary>
        public int? TotalOccurrenceCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of occurrences a pharmacy order has been removed
        /// </summary>
        public int NetRemoveOccurrenceCount { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating that the order is completed based on the number of times it has
        /// been removed.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// Gets the value indicating whether or not the order is currently active (i.e. not cancelled, discontinued, etc)
        /// </summary>
        public bool ActiveNow { get; internal set; }

        /// <summary>
        /// Gets the value indicating whether or not the order is currently on hold
        /// </summary>
        public bool OnHoldNow { get; internal set; }

        /// <summary>
        /// Gets the surrogate key of a pharmacy order snapshot.
        /// </summary>
        public Guid SnapshotKey { get; internal set; }

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an encounter.
        /// </summary>
        public Guid EncounterKey { get; set; }

        /// <summary>
        /// Gets or sets the ID of a pharmacy order.
        /// </summary>
        public string PharmacyOrderId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a pharmacy order is discontinued.
        /// </summary>
        public bool IsDiscontinued { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a pharmacy order is cancelled.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets or sets the UTC effective date time of when a pharmacy order is put on hold.
        /// </summary>
        public DateTime? HoldEffectiveUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local effective date time of when a pharmacy order is put on hold.
        /// </summary>
        public DateTime? HoldEffectiveDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether only a date (and not ime) is recorded for when a pharmacy
        /// order is put on hold.
        /// </summary>
        public bool IsHoldEffectiveDateOnly { get; set; }

        /// <summary>
        /// Gets or sets the UTC effective date time of when a pharmacy order is released from being held.
        /// </summary>
        public DateTime? ReleaseHoldEffectiveUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local effective date time of when a pharmacy order is released from being held.
        /// </summary>
        public DateTime? ReleaseHoldEffectiveDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether only a date (and not ime) is recorded for when a pharmacy
        /// order is released.
        /// </summary>
        public bool IsReleaseHoldEffectiveDateOnly { get; set; }

        /// <summary>
        /// Gets or sets the UTC cancelled date time of when a pharmacy order is cancelled.
        /// </summary>
        public DateTime? CancelledUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the UTC cancelled date time of when a pharmacy order is cancelled.
        /// </summary>
        public DateTime? CancelledDateTime { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an item that is either a medical substance or treatment
        /// and that has been ordered to be given to a patient.
        /// </summary>
        public Guid? GiveItemKey { get; set; }

        /// <summary>
        /// Gets or sets the ID of a medical substance or treatment that has been ordered to be 
        /// a given to a patient.
        /// </summary>
        public string GiveId { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a medical substance or treatment that has been
        /// ordered to be given to a patient.
        /// </summary>
        public string GiveDescription { get; set; }

        /// <summary>
        /// Gets or sets the minimum ordered amount in a variable dose ordered service and exact
        /// ordered amount in a non-varying dose ordered service.
        /// </summary>
        public decimal? GiveAmount { get; set; }

        /// <summary>
        /// Gets or sets the maximum ordered amount in a variable dose ordered service.
        /// </summary>
        public decimal? MaximumGiveAmount { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure for a give amount and maximum give amount.
        /// </summary>
        public UnitOfMeasure GiveAmountUnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets an external unit of measure for a give amount and maximum give amount.
        /// </summary>
        public Guid? GiveAmountExternalUnitOfMeasureKey { get; set; }

        public decimal? DispenseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a pharmacy order type.
        /// </summary>
        public PharmacyOrderTypeInternalCode? PharmacyOrderType { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates that there is a warning error on processing
        /// a pharmacy order inbound message.
        /// </summary>
        public bool InboundWarning { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether all the timing records for pharmacy order 
        /// are fully recordable.
        /// </summary>
        public bool SchedulePersistable { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a pharmacy order, such asn an IV order, is to be dispensed 
        /// using an ADM.
        /// </summary>
        public bool DispenseNotUsingDispenseDevice { get; set; }

        public PharmacyOrderRouteSet RouteSet { get; set; }

        public PharmacyOrderTimingRecordSet TimingRecordSet { get; set; }

        public PharmacyOrderComponentSet ComponentSet { get; set; }

        public PharmacyOrderOrderingPersonSet OrderingPersonSet { get; set; }

        public PharmacyOrderAdminInstructionSet AdminInstructionSet { get; set; }

        public PharmacyOrderSupplierDispensingInstructionSet SupplierDispensingInstructionSet { get; set; }

        public PharmacyOrderDeliveryState DeliveryStateInfo { get; set; }

        #endregion

        #region Public Members

        public PharmacyOrderScheduling.Models.PharmacyOrderSpecification ToPharmacyOrderSpecification()
        {
            var pharmacyOrderSpecification = new PharmacyOrderScheduling.Models.PharmacyOrderSpecification
            {
                    PharmacyOrderKey = Key,
                    CreatedUtcDateTime = CreatedUtcDateTime,
                    TotalOccurrenceCount = TotalOccurrenceCount,
                    NetRemoveOccurrenceCount = NetRemoveOccurrenceCount,
                    OnHoldNow = OnHoldNow,
                    PharmacyOrderTypeInternalCode = PharmacyOrderType.ToInternalCode(),
                    SchedulePersistable = SchedulePersistable
                };

            if (TimingRecordSet != null)
            {
                pharmacyOrderSpecification.TimingRecords = TimingRecordSet
                    .OrderBy(tr => tr.MemberNumber)
                    .Select(tr => tr.ToTimingRecordSpecification())
                    .ToArray();
            }

            return pharmacyOrderSpecification;
        }

        #endregion
    }
}
