using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class DispensingDeviceOverrideGroupEntity
    {
        private EntityRef<OverrideGroupEntity> _overrideGroupEntity;

        partial void OnCreated()
        {
            _overrideGroupEntity = default(EntityRef<OverrideGroupEntity>);
        }

        [Association(Name = "OverrideGroupEntity_DispensingDeviceOverrideGroupEntity", Storage = "_overrideGroupEntity", ThisKey = "OverrideGroupKey", OtherKey = "Key", IsForeignKey = true)]
        public OverrideGroupEntity OverrideGroupEntity
        {
            get
            {
                return _overrideGroupEntity.Entity;
            }
            set
            {
                OverrideGroupEntity previousValue = _overrideGroupEntity.Entity;
                if (previousValue != value ||
                    _overrideGroupEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _overrideGroupEntity.Entity = value;
                    _OverrideGroupKey = value != null ? value.Key : default(Guid);

                    SendPropertyChanged("OverrideGroupEntity");
                }
            }
        }
    }
}
