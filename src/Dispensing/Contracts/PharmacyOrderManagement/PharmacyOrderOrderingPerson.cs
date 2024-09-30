using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a person (normally an ordering physician) who is responsible for
    /// ordering a given pharmacy order.
    /// </summary>
    [Serializable]
    public class PharmacyOrderOrderingPerson : Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderOrderingPerson()
        {
        }

        public PharmacyOrderOrderingPerson(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderOrderingPerson(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderOrderingPerson FromKey(Guid key)
        {
            return new PharmacyOrderOrderingPerson(key);
        }

        #endregion

        #region Public Properties

        public int MemberNumber { get; set; }

        public string PersonId { get; set; }

        public string Prefix { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
        
        public string Suffix { get; set; }

        #endregion
    }
}
