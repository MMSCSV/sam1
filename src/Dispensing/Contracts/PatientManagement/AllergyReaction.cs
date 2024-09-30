using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a reaction that a given patient has to a given allergen.
    /// </summary>
    [Serializable]
    public class AllergyReaction : Entity<Guid>
    {
        #region Constructors

        public AllergyReaction()
        {
        }

        public AllergyReaction(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator AllergyReaction(Guid key)
        {
            return FromKey(key);
        }

        public static AllergyReaction FromKey(Guid key)
        {
            return new AllergyReaction(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the integer that identifies a member within an ordered set.
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies an allergic reaction.
        /// </summary>
        public string ReactionCode { get; set; }

        #endregion
    }
}
