using System;

namespace CareFusion.Dispensing.Contracts
{
    public class RelationshipRecordAssociation : Entity<Guid>
    {
        #region Constructors

        public RelationshipRecordAssociation()
        {
        }

        public RelationshipRecordAssociation(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator RelationshipRecordAssociation(Guid key)
        {
            return FromKey(key);
        }

        public static RelationshipRecordAssociation FromKey(Guid key)
        {
            return new RelationshipRecordAssociation(key);
        }

        #endregion

        #region Public Properties

        public int MemberNumber { get; set; }
        
        public string PharmacyOrderId { get; set; }

        public Guid? PharmacyOrderKey { get; set; }                        

        #endregion
    }
}
