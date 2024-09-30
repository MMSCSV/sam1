using System;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class RestockGroup : Entity<Guid>
    {
        #region Constructors

        public RestockGroup()
        {

        }

        public RestockGroup(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator RestockGroup(Guid key)
        {
            return FromKey(key);
        }

        public static RestockGroup FromKey(Guid key)
        {
            return new RestockGroup(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///  Gets or sets the name that identifies a restock group.
        /// </summary>
        public string RestockGroupName { get; set; }

        /// <summary>
        /// Gets or sets the the text that describes a restock group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the CII Safe device that owns the restock group
        /// </summary>
        public string GcsmDispensingDeviceName { get; set; }

        /// <summary>
        /// Gets or sets the CII Safe device's facility name
        /// </summary>
        public string GcsmDispensingDeviceFacility { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a restock group is active.
        /// </summary>
        public bool IsActive { get; set; }

        #endregion
    }
}
