using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class DosageFormEntity : IContractConvertible<DosageForm>
    {
        private EntityRef<ExternalSystemEntity> _externalSystemEntity;

        partial void OnCreated()
        {
            _externalSystemEntity = default(EntityRef<ExternalSystemEntity>);
        }

        [Association(Name = "ExternalSystemEntity_DosageFormEntity", Storage = "_externalSystemEntity", ThisKey = "ExternalSystemKey", OtherKey = "Key", IsForeignKey = true)]
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

        #region IContractConvertible<DosageForm> Members

        public DosageForm ToContract()
        {
            string externalSystemName = null;
            if (ExternalSystemEntity != null)
            {
                externalSystemName = ExternalSystemEntity.ExternalSystemName;
            }

            return new DosageForm(Key)
               {
                   ExternalSystemKey = ExternalSystemKey,
                   ExternalSystemName = externalSystemName,
                   Code = DosageFormCode,
                   Description = DescriptionText,
                   EquivalencyDosageFormGroupKey = EquivalencyDosageFormGroupKey,
                   SortOrder = SortValue,
                   LastModified = LastModifiedBinaryValue.ToArray()
               };
        }

        #endregion
    }
}
