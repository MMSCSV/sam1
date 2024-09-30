using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class FacilityEntity
    {
        private EntityRef<ExternalSystemEntity> _pharmacyInformationSystemEntity;

        partial void OnCreated()
        {
            _pharmacyInformationSystemEntity = default(EntityRef<ExternalSystemEntity>);
        }

        [Association(Name = "FacilityEntity_PharmacyInformationSystemEntity", Storage = "_pharmacyInformationSystemEntity", ThisKey = "PharmacyInformationSystemKey", OtherKey = "Key", IsForeignKey = true)]
        public ExternalSystemEntity PharmacyInformationSystemEntity
        {
            get
            {
                return _pharmacyInformationSystemEntity.Entity;
            }
            set
            {
                ExternalSystemEntity previousValue = _pharmacyInformationSystemEntity.Entity;
                if (previousValue != value ||
                    _pharmacyInformationSystemEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _pharmacyInformationSystemEntity.Entity = value;
                    _PharmacyInformationSystemKey = value != null ? value.Key : default(Guid?);

                    SendPropertyChanged("PharmacyInformationSystemEntity");
                }
            }
        }
    }
}
