using System;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class ClinicalDataSubjectEntity : IContractConvertible<ClinicalDataSubject>
    {
        #region IContractConvertible<ClinicalData> Members

        public ClinicalDataSubject ToContract()
        {
            ClinicalDataCategory category = (ClinicalDataCategoryEntity != null)
                    ? ClinicalDataCategoryEntity.ToContract()
                    : ClinicalDataCategoryKey;

            ClinicalDataSubjectType subjectType = (ClinicalDataSubjectTypeEntity != null)
                    ? ClinicalDataSubjectTypeEntity.ToContract()
                    : ClinicalDataSubjectTypeInternalCode.FromInternalCode<ClinicalDataSubjectTypeInternalCode>();

            ClinicalDataResponse[] responses = ClinicalDataResponseEntities
                .Select(cdr => cdr.ToContract())
                .ToArray();

            Guid[] userTypeKeys = ClinicalDataSubjectUserTypeEntities
                .Select(cdsut => cdsut.UserTypeKey)
                .ToArray();

            return new ClinicalDataSubject(Key)
                       {
                           Category = category,
                           Description = DescriptionText,
                           DisplayOnce = DisplayOnceFlag,
                           StatKit = StatKitFlag,
                           IsActive = ActiveFlag,
                           SubjectType = subjectType,
                           Title = TitleText,
                           Responses = responses,
                           UserTypes = userTypeKeys,
                           LastModified = LastModifiedBinaryValue.ToArray()
                       };
        }

        #endregion
    }
}
