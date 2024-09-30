using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class MedItemEntity
    {
        private EntityRef<UnitOfMeasureEntity> _strengthUnitOfMeasureEntity;
        private EntityRef<UnitOfMeasureEntity> _totalVolumeUnitOfMeasureEntity;
        private EntityRef<UnitOfMeasureEntity> _concentrationVolumeUnitOfMeasureEntity;
        
        partial void OnCreated()
        {
            _strengthUnitOfMeasureEntity = default(EntityRef<UnitOfMeasureEntity>);
            _totalVolumeUnitOfMeasureEntity = default(EntityRef<UnitOfMeasureEntity>);
            _concentrationVolumeUnitOfMeasureEntity = default(EntityRef<UnitOfMeasureEntity>);
        }

        #region Public Properties

        [Association(Name = "StrengthUOMEntity_MedItemEntity", Storage = "_strengthUnitOfMeasureEntity", ThisKey = "StrengthUOMKey", OtherKey = "Key", IsForeignKey = true)]
        public UnitOfMeasureEntity StrengthUOMEntity
        {
            get
            {
                return _strengthUnitOfMeasureEntity.Entity;
            }
            set
            {
                UnitOfMeasureEntity previousValue = _strengthUnitOfMeasureEntity.Entity;
                if (previousValue != value ||
                    _strengthUnitOfMeasureEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _strengthUnitOfMeasureEntity.Entity = value;
                    _StrengthUOMKey = value != null ? value.Key : default(Guid?);

                    SendPropertyChanged("StrengthUOMEntity");
                }
            }
        }

        [Association(Name = "TotalVolumehUOMEntity_MedItemEntity", Storage = "_totalVolumeUnitOfMeasureEntity", ThisKey = "TotalVolumeUOMKey", OtherKey = "Key", IsForeignKey = true)]
        public UnitOfMeasureEntity TotalVolumeUOMEntity
        {
            get
            {
                return _totalVolumeUnitOfMeasureEntity.Entity;
            }
            set
            {
                UnitOfMeasureEntity previousValue = _totalVolumeUnitOfMeasureEntity.Entity;
                if (previousValue != value ||
                    _totalVolumeUnitOfMeasureEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _totalVolumeUnitOfMeasureEntity.Entity = value;
                    _TotalVolumeUOMKey = value != null ? value.Key : default(Guid?);

                    SendPropertyChanged("TotalVolumeUOMEntity");
                }
            }
        }

        [Association(Name = "ConcentrationVolumeUOMEntity_MedItemEntity", Storage = "_concentrationVolumeUnitOfMeasureEntity", ThisKey = "ConcentrationVolumeUOMKey", OtherKey = "Key", IsForeignKey = true)]
        public UnitOfMeasureEntity ConcentrationVolumeUOMEntity
        {
            get
            {
                return _concentrationVolumeUnitOfMeasureEntity.Entity;
            }
            set
            {
                UnitOfMeasureEntity previousValue = _concentrationVolumeUnitOfMeasureEntity.Entity;
                if (previousValue != value ||
                    _concentrationVolumeUnitOfMeasureEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _concentrationVolumeUnitOfMeasureEntity.Entity = value;
                    _ConcentrationVolumeUOMKey = value != null ? value.Key : default(Guid?);

                    SendPropertyChanged("ConcentrationVolumeUOMEntity");
                }
            }
        }

        #endregion
    }
}
