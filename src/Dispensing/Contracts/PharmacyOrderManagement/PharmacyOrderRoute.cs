using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a combination that may be chosen by a caregiver of administration route, site,
    /// device and method.
    /// </summary>
    [Serializable]
    public class PharmacyOrderRoute: Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderRoute()
        {
        }

        public PharmacyOrderRoute(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderRoute(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderRoute FromKey(Guid key)
        {
            return new PharmacyOrderRoute(key);
        }

        #endregion

        #region Public Properties

        public int MemberNumber { get; set; }

        public string AdministrationRouteCode { get; set; }

        public string AdministrationRouteDescription { get; set; }

        #endregion
    }
}
