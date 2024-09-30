using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class ProductIDEntity : IContractConvertible<ItemProductId>
    {
        private EntityRef<UserAccountEntity> _linkedByUserAccountEntity;
        private EntityRef<UserAccountEntity> _verifiedByUserAccountEntity;

        partial void OnCreated()
        {
            _linkedByUserAccountEntity = default(EntityRef<UserAccountEntity>);
            _verifiedByUserAccountEntity = default(EntityRef<UserAccountEntity>);
        }

        [Association(Name = "LinkedByUserAccountEntity_ProductIDEntity", Storage = "_linkedByUserAccountEntity", ThisKey = "LinkedByUserAccountKey", OtherKey = "Key", IsForeignKey = true)]
        public UserAccountEntity LinkedByUserAccountEntity
        {
            get
            {
                return _linkedByUserAccountEntity.Entity;
            }
            set
            {
                UserAccountEntity previousValue = _linkedByUserAccountEntity.Entity;
                if (previousValue != value ||
                    _linkedByUserAccountEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _linkedByUserAccountEntity.Entity = value;
                    _LinkedByUserAccountKey = value != null ? value.Key : default(Guid);

                    SendPropertyChanged("LinkedByUserAccountEntity");
                }
            }
        }

        [Association(Name = "VerifiedByUserAccountEntity_ProductIDEntity", Storage = "_verifiedByUserAccountEntity", ThisKey = "VerifiedByUserAccountKey", OtherKey = "Key", IsForeignKey = true)]
        public UserAccountEntity VerifiedByUserAccountEntity
        {
            get
            {
                return _verifiedByUserAccountEntity.Entity;
            }
            set
            {
                UserAccountEntity previousValue = _verifiedByUserAccountEntity.Entity;
                if (previousValue != value ||
                    _verifiedByUserAccountEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _verifiedByUserAccountEntity.Entity = value;
                    _VerifiedByUserAccountKey = value != null ? value.Key : default(Guid);

                    SendPropertyChanged("VerifiedByUserAccountEntity");
                }
            }
        }

        #region IContractConvertible<ItemProductId> Members

        public ItemProductId ToContract()
        {
            return new ItemProductId(Key)
                {
                    ItemKey = ItemKey,
                    ProductId = ProductID,
                    LinkedByUserAccountKey = LinkedByUserAccountKey,
                    LinkedDateTime = LinkedLocalDateTime,
                    LinkedUtcDateTime = LinkedUTCDateTime,
                    VerifiedByUserAccountKey = VerifiedByUserAccountKey,
                    VerifiedDateTime = VerifiedLocalDateTime,
                    VerifiedUtcDateTime = VerifiedUTCDateTime,
                    FromExternalSystem = FromExternalSystemFlag,
                    ScanProductDeleteReason = ScanProductDeleteReasonInternalCode.FromNullableInternalCode<ScanProductDeleteReasonInternalCode>(),
                    OtherItemId = OtherItemID,
                    CreatedByExternalSystemName = CreatedByExternalSystemName,
                    DeletedByExternalSystemName = DeletedByExternalSystemName,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
