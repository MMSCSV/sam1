using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class PatientSiloEntity
    {
        private EntityRef<ExternalSystemEntity> _patientAllergyProviderEntity;
        private EntitySet<FacilityPatientSiloEntity> _facilityPatientSiloEntities;

        partial void OnCreated()
        {
            _patientAllergyProviderEntity = default(EntityRef<ExternalSystemEntity>);
            _facilityPatientSiloEntities = new EntitySet<FacilityPatientSiloEntity>();
        }

        [Association(Name = "PatientAllergyProviderEntity_PatientSiloEntity", Storage = "_patientAllergyProviderEntity", ThisKey = "PatientAllergyProviderKey", OtherKey = "Key", IsForeignKey = true)]
        public ExternalSystemEntity PatientAllergyProviderEntity
        {
            get
            {
                return _patientAllergyProviderEntity.Entity;
            }
            set
            {
                ExternalSystemEntity previousValue = _patientAllergyProviderEntity.Entity;
                if (previousValue != value ||
                    _patientAllergyProviderEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _patientAllergyProviderEntity.Entity = value;
                    _PatientAllergyProviderKey = value != null ? value.Key : default(Guid?);

                    SendPropertyChanged("PatientAllergyProviderEntity");
                }
            }
        }

        [Association(Name = "PatientSiloEntity_FacilityPatientSiloEntity", Storage = "_facilityPatientSiloEntities", ThisKey = "Key", OtherKey = "PatientSiloKey")]
        public EntitySet<FacilityPatientSiloEntity> FacilityPatientSiloEntities
        {
            get
            {
                return _facilityPatientSiloEntities;
            }
            set
            {
                _facilityPatientSiloEntities.Assign(value);
            }
        }
    }
}
