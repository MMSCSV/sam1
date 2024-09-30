using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class DispensingDeviceEntity
    {
        private EntityRef<FacilityEntity> _facilityEntity;
        private EntitySet<AreaDispensingDeviceEntity> _areaDispensingDeviceEntities;
        private EntitySet<ClinicalDataSubjectDispensingDeviceEntity> _clinicalDataSubjectDispensingDeviceEntities;
        private EntitySet<FacilityKitDispensingDeviceEntity> _facilityKitDispensingDeviceEntities;

        partial void OnCreated()
        {
            _facilityEntity = default(EntityRef<FacilityEntity>);
            _areaDispensingDeviceEntities = new EntitySet<AreaDispensingDeviceEntity>();
            _clinicalDataSubjectDispensingDeviceEntities = new EntitySet<ClinicalDataSubjectDispensingDeviceEntity>();
            _facilityKitDispensingDeviceEntities = new EntitySet<FacilityKitDispensingDeviceEntity>();
        }

        [Association(Name = "FacilityEntity_DispensingDeviceEntity", Storage = "_facilityEntity", ThisKey = "FacilityKey", OtherKey = "Key", IsForeignKey = true)]
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

        [Association(Name = "DispensingDeviceEntity_AreaDispensingDeviceEntity", Storage = "_areaDispensingDeviceEntities", ThisKey = "Key", OtherKey = "DispensingDeviceKey")]
        public EntitySet<AreaDispensingDeviceEntity> AreaDispensingDeviceEntities
        {
            get
            {
                return _areaDispensingDeviceEntities;
            }
            set
            {
                _areaDispensingDeviceEntities.Assign(value);
            }
        }

        [Association(Name = "DispensingDeviceEntity_ClinicalDataSubjectDispensingDeviceEntity", Storage = "_clinicalDataSubjectDispensingDeviceEntities", ThisKey = "Key", OtherKey = "DispensingDeviceKey")]
        public EntitySet<ClinicalDataSubjectDispensingDeviceEntity> ClinicalDataSubjectDispensingDeviceEntities
        {
            get
            {
                return _clinicalDataSubjectDispensingDeviceEntities;
            }
            set
            {
                _clinicalDataSubjectDispensingDeviceEntities.Assign(value);
            }
        }

        [Association(Name = "DispensingDeviceEntity_FacilityKitDispensingDeviceEntity", Storage = "_facilityKitDispensingDeviceEntities", ThisKey = "Key", OtherKey = "DispensingDeviceKey")]
        public EntitySet<FacilityKitDispensingDeviceEntity> FacilityKitDispensingDeviceEntities
        {
            get
            {
                return _facilityKitDispensingDeviceEntities;
            }
            set
            {
                _facilityKitDispensingDeviceEntities.Assign(value);
            }
        }
    }
}
