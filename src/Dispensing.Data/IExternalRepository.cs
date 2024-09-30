using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data
{
    /// <summary>
    /// Represents a repository specific to common data entities.
    /// </summary>
    public interface IExternalRepository : IRepository
    {
        #region ExternalPatientIdentificationType Members

        ExternalPatientIdentificationType GetExternalPatientIdentificationType(Guid externalPatientIdentificationTypeKey);
        
        ExternalPatientIdentificationType GetExternalPatientIdentificationType(Guid externalSystemKey, string patientIdTypeCode);

        IEnumerable<ExternalPatientIdentificationType> GetExternalPatientIdentificationTypes(Guid externalSystemKey, Guid patientIdentificationTypeKey);

        Guid InsertExternalPatientIdentificationType(Context context, ExternalPatientIdentificationType externalPatientIdentificationType);

        void UpdateExternalPatientIdentificationType(Context context, ExternalPatientIdentificationType externalPatientIdentificationType);

        void DeleteExternalPatientIdentificationType(Context context, Guid externalPatientIdentificationTypeKey);

        #endregion

        #region ExternalGender Members

        ExternalGender GetExternalGender(Guid externalGenderKey);

        ExternalGender GetExternalGender(Guid externalSystemKey, string genderCode);

        Guid InsertExternalGender(Context context, ExternalGender externalGender);

        void UpdateExternalGender(Context context, ExternalGender externalGender);

        void DeleteExternalGender(Context context, Guid externalGenderKey);

        #endregion

        #region ExternalUnitOfMeasure Members

        IEnumerable<ExternalUnitOfMeasure> GetExternalUnitOfMeasures(IEnumerable<Guid> externalUnitOfMeasureKeys = null,
            bool? deleted = null, Guid? externalSystemKey = null, UOMRoleInternalCode? unitOfMeasureRole = null, 
            string unitOfMeasureCode = null);

        IEnumerable<ExternalUnitOfMeasure> GetExternalUnitOfMeasures(Guid externalSystemKey, string unitOfMeasureCode);

        ExternalUnitOfMeasure GetExternalUnitOfMeasure(Guid externalSystemKey, UOMRoleInternalCode unitOfMeasureRole, string unitOfMeasureCode);

        ExternalUnitOfMeasure GetExternalUnitOfMeasure(Guid externalUnitOfMeasureKey);

        Guid InsertExternalUnitOfMeasure(Context context, ExternalUnitOfMeasure externalUnitOfMeasure);

        void UpdateExternalUnitOfMeasure(Context context, ExternalUnitOfMeasure externalUnitOfMeasure);

        void DeleteExternalUnitOfMeasure(Context context, Guid externalUnitOfMeasureKey);

        #endregion
    }
}

