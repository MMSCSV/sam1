using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class TherapeuticClassEntity : IContractConvertible<TherapeuticClass>
    {
        private EntityRef<ExternalSystemEntity> _externalSystemEntity;

        partial void OnCreated()
        {
            _externalSystemEntity = default(EntityRef<ExternalSystemEntity>);
        }

        [Association(Name = "ExternalSystemEntity_TherapeuticClassEntity", Storage = "_externalSystemEntity", ThisKey = "ExternalSystemKey", OtherKey = "Key", IsForeignKey = true)]
        public ExternalSystemEntity ExternalSystemEntity
        {
            get
            {
                return _externalSystemEntity.Entity;
            }
            set
            {
                ExternalSystemEntity previousValue = _externalSystemEntity.Entity;
                if (previousValue != value ||
                    _externalSystemEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _externalSystemEntity.Entity = value;
                    _ExternalSystemKey = value != null ? value.Key : default(Guid);

                    SendPropertyChanged("ExternalSystemEntity");
                }
            }
        }

        #region IContractConvertible<TherapeuticClass> Members

        public TherapeuticClass ToContract()
        {
            string externalSystemName = null;
            if (ExternalSystemEntity != null)
            {
                externalSystemName = ExternalSystemEntity.ExternalSystemName;
            }

            return new TherapeuticClass(Key)
                {
                    Code = TherapeuticClassCode,
                    Description = DescriptionText,
                    ExternalSystemKey = ExternalSystemKey,
                    ExternalSystemName = externalSystemName,
                    SortOrder = SortValue,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
