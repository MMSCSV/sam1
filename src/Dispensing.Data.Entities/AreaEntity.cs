using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class AreaEntity
    {
        private EntitySet<ClinicalDataSubjectAreaEntity> _clinicalDataSubjectAreaEntities;

        partial void OnCreated()
        {
            _clinicalDataSubjectAreaEntities = new EntitySet<ClinicalDataSubjectAreaEntity>();
        }

        [Association(Name = "AreaEntity_ClinicalDataSubjectAreaEntity", Storage = "_clinicalDataSubjectAreaEntities", ThisKey = "Key", OtherKey = "AreaKey")]
        public EntitySet<ClinicalDataSubjectAreaEntity> ClinicalDataSubjectAreaEntities
        {
            get
            {
                return _clinicalDataSubjectAreaEntities;
            }
            set
            {
                _clinicalDataSubjectAreaEntities.Assign(value);
            }
        }
    }
}
