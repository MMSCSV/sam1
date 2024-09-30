using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    public class PharmacyOrderRelationshipRecord : Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderRelationshipRecord()
        {
        }

        public PharmacyOrderRelationshipRecord(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderRelationshipRecord(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderRelationshipRecord FromKey(Guid key)
        {
            return new PharmacyOrderRelationshipRecord(key);
        }

        #endregion

        #region Public Properties

        public int MemberNumber { get; set; }

        public PharmacyOrderSpecialRelationshipInternalCode? SpecialServiceRequestRelationship { get; set; }

        public IEnumerable<RelationshipRecordAssociation> RelationshipRecordAssociations { get; set; }

       #endregion
    }
}
