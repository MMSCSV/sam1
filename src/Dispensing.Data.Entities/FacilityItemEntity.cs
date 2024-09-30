using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class FacilityItemEntity
    {
        private EntityRef<FacilityEntity> _facilityEntity;
        private EntityRef<UnitOfMeasureEntity> _refillUnitOfMeasureEntity;
        private EntityRef<UnitOfMeasureEntity> _issueUnitOfMeasureEntity;
        private EntitySet<ClinicalDataSubjectFacilityItemFunctionEntity> _clinicalDataSubjectFacilityItemFunctionEntities;

        partial void OnCreated()
        {
            _facilityEntity = default(EntityRef<FacilityEntity>);
            _refillUnitOfMeasureEntity = default(EntityRef<UnitOfMeasureEntity>);
            _issueUnitOfMeasureEntity = default(EntityRef<UnitOfMeasureEntity>);
            _clinicalDataSubjectFacilityItemFunctionEntities = new EntitySet<ClinicalDataSubjectFacilityItemFunctionEntity>();
        }

        #region Public Properties

        [Association(Name = "FacilityEntity_FacilityItemEntity", Storage = "_facilityEntity", ThisKey = "FacilityKey", OtherKey = "Key", IsForeignKey = true)]
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

        [Association(Name = "RefillUnitOfMeasureEntity_FacilityItemEntity", Storage = "_refillUnitOfMeasureEntity", ThisKey = "RefillUOMKey", OtherKey = "Key", IsForeignKey = true)]
        public UnitOfMeasureEntity RefillUnitOfMeasureEntity
        {
            get
            {
                return _refillUnitOfMeasureEntity.Entity;
            }
            set
            {
                UnitOfMeasureEntity previousValue = _refillUnitOfMeasureEntity.Entity;
                if (previousValue != value ||
                    _refillUnitOfMeasureEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _refillUnitOfMeasureEntity.Entity = value;
                    _RefillUOMKey = value != null ? value.Key : default(Guid?);

                    SendPropertyChanged("RefillUnitOfMeasureEntity");
                }
            }
        }

        [Association(Name = "IssueUnitOfMeasureEntity_FacilityItemEntity", Storage = "_issueUnitOfMeasureEntity", ThisKey = "IssueUOMKey", OtherKey = "Key", IsForeignKey = true)]
        public UnitOfMeasureEntity IssueUnitOfMeasureEntity
        {
            get
            {
                return _issueUnitOfMeasureEntity.Entity;
            }
            set
            {
                UnitOfMeasureEntity previousValue = _issueUnitOfMeasureEntity.Entity;
                if (previousValue != value ||
                    _issueUnitOfMeasureEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _issueUnitOfMeasureEntity.Entity = value;
                    _IssueUOMKey = value != null ? value.Key : default(Guid?);

                    SendPropertyChanged("IssueUnitOfMeasureEntity");
                }
            }
        }

        [Association(Name = "FacilityItemEntity_ClinicalDataSubjectFacilityItemFunctionEntity", Storage = "_clinicalDataSubjectFacilityItemFunctionEntities", ThisKey = "Key", OtherKey = "FacilityItemKey")]
        public EntitySet<ClinicalDataSubjectFacilityItemFunctionEntity> ClinicalDataSubjectFacilityItemFunctionEntities
        {
            get
            {
                return _clinicalDataSubjectFacilityItemFunctionEntities;
            }
            set
            {
                _clinicalDataSubjectFacilityItemFunctionEntities.Assign(value);
            }
        }

        #endregion
    }
}
