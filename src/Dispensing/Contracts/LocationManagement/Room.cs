using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of a unit room for a given period of time.
    /// </summary>
    [Serializable]
    public class Room : Entity<Guid>
    {
        #region Constructors

        public Room()
        {
        }

        public Room(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Room(Guid key)
        {
            return FromKey(key);
        }

        public static Room FromKey(Guid key)
        {
            return new Room(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a unit.
        /// </summary>
        public Guid UnitKey { get; set; }

        /// <summary>
        /// Gets the name of the unit.
        /// </summary>
        public string UnitName { get; internal set; }

        /// <summary>
        /// Gets or sets the name of a room.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a room.
        /// </summary>
        public string Description { get; set; }

        #endregion
    }
}
