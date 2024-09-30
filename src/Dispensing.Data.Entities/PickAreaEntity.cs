using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class PickAreaEntity
    {
        private EntityRef<FacilityEntity> _facilityEntity;

        partial void OnCreated()
        {
            _facilityEntity = default(EntityRef<FacilityEntity>);
        }

        [Association(Name = "FacilityEntity_PickAreaEntity", Storage = "_facilityEntity", ThisKey = "FacilityKey", OtherKey = "Key", IsForeignKey = true)]
        public FacilityEntity FacilityEntity
        {
            get
            {
                return _facilityEntity.Entity;
            }
            set
            {
                FacilityEntity previousValue = _facilityEntity.Entity;
                if (previousValue != value ||
                    _facilityEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _facilityEntity.Entity = value;
                    _FacilityKey = value != null ? value.Key : default(Guid);

                    SendPropertyChanged("FacilityEntity");
                }
            }
        }
    }
}
