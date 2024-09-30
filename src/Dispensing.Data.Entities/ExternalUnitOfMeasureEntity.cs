using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class ExternalUnitOfMeasureEntity
    {
        private EntityRef<UnitOfMeasureEntity> _unitOfMeasureEntity;
        private EntityRef<UnitOfMeasureRoleEntity> _unitOfMeasureRoleEntity;

        partial void OnCreated()
        {
            _unitOfMeasureEntity = default(EntityRef<UnitOfMeasureEntity>);
            _unitOfMeasureRoleEntity = default(EntityRef<UnitOfMeasureRoleEntity>);
        }

        #region Public Properties

        [Association(Name = "UOMEntity_ExternalUnitOfMeasureEntity", Storage = "_unitOfMeasureEntity", ThisKey = "UOMKey", OtherKey = "Key", IsForeignKey = true)]
        public UnitOfMeasureEntity UOMEntity
        {
            get
            {
                return _unitOfMeasureEntity.Entity;
            }
            set
            {
                UnitOfMeasureEntity previousValue = _unitOfMeasureEntity.Entity;
                if (previousValue != value ||
                    _unitOfMeasureEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _unitOfMeasureEntity.Entity = value;
                    _UOMKey = value != null ? value.Key : default(Guid?);

                    SendPropertyChanged("UOMEntity");
                }
            }
        }

        [Association(Name = "UOMRoleEntity_ExternalUnitOfMeasureEntity", Storage = "_unitOfMeasureRoleEntity", ThisKey = "UOMRoleInternalCode", OtherKey = "InternalCode", IsForeignKey = true)]
        public UnitOfMeasureRoleEntity UOMRoleEntity
        {
            get
            {
                return _unitOfMeasureRoleEntity.Entity;
            }
            set
            {
                UnitOfMeasureRoleEntity previousValue = _unitOfMeasureRoleEntity.Entity;
                if (previousValue != value ||
                    _unitOfMeasureRoleEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _unitOfMeasureRoleEntity.Entity = value;
                    _UOMRoleInternalCode = value != null ? value.InternalCode : default(string);

                    SendPropertyChanged("UOMRoleEntity");
                }
            }
        }

        #endregion
    }
}
