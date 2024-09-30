using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{    
    /// <summary>
    /// Represents a Delivery state and delivery location    
    /// </summary>
    [Serializable]
    public class PharmacyOrderDeliveryState : Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderDeliveryState()
        {
        }

        public PharmacyOrderDeliveryState(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderDeliveryState(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderDeliveryState FromKey(Guid key)
        {
            return new PharmacyOrderDeliveryState(key);
        }

        #endregion

        #region Public Properties

        public Guid PharmacyOrderKey { get; set; }

        public ItemDeliveryTrackingStatusInternalCode DeliveryTrackingStatusInternalCode { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }        

        public string ContactInformation { get; set; }

        public DateTime DeliveryStateEnteredUtcDateTime { get; set; }

        public DateTime DeliveryStateEnteredLocalDateTime
        {
            get { return DeliveryStateEnteredUtcDateTime.ToLocalTime(); }
        }

        public string DeliveryLocationName { get; set; }

        #endregion
    }
}
