using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Entities;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.TableTypes;
using AdtDAL = Pyxis.Core.Data.Schema.Adt;

namespace CareFusion.Dispensing.Data.Repositories
{
    /// <summary>
    /// Represents an <see cref="IAdtRepository"/>
    /// </summary>
    internal class AdtRepository : LinqBaseRepository, IAdtRepository
    {
        private readonly AdtDAL.PatientRepository _patientRepository;
        private readonly AdtDAL.PatientIdentificationTypeRepository _patientIdentificationRepository;
        private readonly AdtDAL.AllergyTypeRepository _allergyTypeRepository;
        private readonly AdtDAL.AllergySeverityRepository _allergySeverityRepository;
        private readonly AdtDAL.GenderRepository _genderRepository;
        private readonly AdtDAL.EncounterRepository _encounterRepository;
        private readonly AdtDAL.EncounterPatientClassRepository _encounterPatientClassRepository;
        private readonly AdtDAL.HospitalServiceRepository _hospitalServiceRepository;

        public AdtRepository()
        {
            _patientRepository = new AdtDAL.PatientRepository();
            _patientIdentificationRepository = new AdtDAL.PatientIdentificationTypeRepository();
            _allergyTypeRepository = new AdtDAL.AllergyTypeRepository();
            _allergySeverityRepository = new AdtDAL.AllergySeverityRepository();
            _genderRepository = new AdtDAL.GenderRepository();
            _encounterRepository = new AdtDAL.EncounterRepository();
            _encounterPatientClassRepository = new AdtDAL.EncounterPatientClassRepository();
            _hospitalServiceRepository = new AdtDAL.HospitalServiceRepository();
        }

        #region Patient Members

        IEnumerable<Patient> IAdtRepository.GetPatients(IEnumerable<Guid> patientKeys, Guid? patientSiloKey, Guid? patientIdTypeKey,
                                       string patientId)
        {
            List<Patient> patients = new List<Patient>();
            if (patientKeys != null && !patientKeys.Any())
                return patients; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (patientKeys != null)
                    selectedKeys = new GuidKeyTable(patientKeys.Distinct());

                using(var connectionScope = ConnectionScopeFactory.Create())
                using (var multi = connectionScope.QueryMultiple(
                    "ADT.bsp_GetPatients",
                    new
                        {
                            SelectedKeys = selectedKeys.AsTableValuedParameter(),
                            PatientSiloKey = patientSiloKey, 
                            PatientIDTypeKey = patientIdTypeKey,
                            PatientID = patientId
                        },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure))
                {
                    var patientResults = multi.Read<PatientResult>();
                    var patientIdSetResults = multi.Read<PatientIDSetResult>();
                    var patientIdResults = multi.Read<PatientIDResult>();
                    var patientAllergySetResults = multi.Read<PatientAllergySetResult>();
                    var patientAllergyResults = multi.Read<PatientAllergyResult>();
                    var patientAllergyReactionResults = multi.Read<PatientAllergyReactionResult>();

                    // Create patient ID sets
                    Dictionary<Guid, PatientIdentificationSet> patientIdSets = new Dictionary<Guid, PatientIdentificationSet>();
                    foreach (var patientIdSetResult in patientIdSetResults)
                    {
                        var identifications = patientIdResults
                            .Where(pid => pid.PatientIDSetKey == patientIdSetResult.PatientIDSetKey)
                            .Select(pid => new PatientIdentification(pid.PatientIDKey)
                                                {
                                                    IdentificationTypeKey = pid.PatientIDTypeKey,
                                                    IdentificationTypeDisplayCode = pid.PatientIDTypeDisplayCode,
                                                    Value = pid.PatientID,
                                                    CheckValue = pid.CheckValue,
                                                    LastModified = pid.LastModifiedBinaryValue.ToArray()
                                                })
                            .ToArray();

                        PatientIdentificationSet patientIdSet = new PatientIdentificationSet(patientIdSetResult.PatientIDSetKey, identifications)
                            {
                                LastModified = patientIdSetResult.LastModifiedBinaryValue.ToArray()
                            };

                        patientIdSets[patientIdSetResult.PatientKey] = patientIdSet;
                    }

                    // Create patient allergy sets
                    Dictionary<Guid, AllergySet> patientAllergySets = new Dictionary<Guid, AllergySet>();
                    foreach (var patientAllergySetResult in patientAllergySetResults)
                    {
                        List<Allergy> allergies = new List<Allergy>();
                        foreach (var patientAllergyResult in patientAllergyResults)
                        {
                            var reactions = patientAllergyReactionResults
                                .Where(par => par.PatientAllergyKey == patientAllergyResult.PatientAllergyKey)
                                .Select(par => new AllergyReaction(par.PatientAllergyReactionKey)
                                    {
                                        MemberNumber = par.MemberNumber,
                                        ReactionCode = par.ReactionCode,
                                        LastModified = par.LastModifiedBinaryValue.ToArray()
                                    })
                                .ToArray();

                            Allergy allergy = new Allergy(patientAllergyResult.PatientAllergyKey)
                                {
                                    AllergenCode = patientAllergyResult.AllergenCode,
                                    AllergenDescription = patientAllergyResult.AllergenDescriptionText,
                                    AllergenDescriptionTruncated = patientAllergyResult.AllergenDescriptionTruncatedFlag,
                                    AllergySeverityKey = patientAllergyResult.AllergySeverityKey,
                                    AllergyTypeKey = patientAllergyResult.AllergyTypeKey,
                                    MemberNumber = patientAllergyResult.MemberNumber,
                                    Reactions = reactions,
                                    SensitivityCode = patientAllergyResult.SensitivityCode,
                                    SensitivityDescription = patientAllergyResult.SensitivityDescriptionText,
                                    UniqueId = patientAllergyResult.AllergyUniqueID,
                                    LastModified = patientAllergyResult.LastModifiedBinaryValue.ToArray()
                                };

                            allergies.Add(allergy);
                        }

                        AllergySet allergySet = new AllergySet(patientAllergySetResult.PatientAllergySetKey, allergies)
                        {
                            LastModified = patientAllergySetResult.LastModifiedBinaryValue.ToArray()
                        };

                        patientAllergySets[patientAllergySetResult.PatientKey] = allergySet;
                    }

                    foreach (var patientResult in patientResults)
                    {
                        Patient patient = new Patient(patientResult.PatientKey, patientResult.PatientSnapshotKey)
                            {
                                Surviving = patientResult.SurvivingFlag,
                                AllergySet = patientAllergySets.ContainsKey(patientResult.PatientKey)
                                    ? patientAllergySets[patientResult.PatientKey] : null,
                                BirthDate = patientResult.BirthLocalDateTime,
                                BirthDateTimePrecisionCode = patientResult.BirthDateTimePrecisionInternalCode.FromNullableInternalCode<DateTimePrecisionInternalCode>(),
                                CreatedAtDispensingDeviceKey = patientResult.CreatedAtDispensingDeviceKey,
                                FirstName = patientResult.FirstName,
                                GenderKey = patientResult.GenderKey,
                                IdentificationSet = patientIdSets.ContainsKey(patientResult.PatientKey) 
                                    ? patientIdSets[patientResult.PatientKey] : null,
                                LastName = patientResult.LastName,
                                PreferredName = patientResult.PreferredName,
                                UnknownLastName = patientResult.UnknownLastNameFlag,
                                MiddleName = patientResult.MiddleName,
                                PatientSiloKey = patientResult.PatientSiloKey,
                                Prefix = patientResult.PrefixText,
                                RestrictedAccess = patientResult.RestrictedAccessFlag,
                                Placeholder = patientResult.PlaceholderFlag,
                                Suffix = patientResult.SuffixText,
                                Deceased = patientResult.DeceasedFlag,
                                LastModified = patientResult.LastModifiedBinaryValue.ToArray()
                            };

                        patients.Add(patient);
                    }

                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return patients;
        }

        Patient IAdtRepository.GetPatient(Guid patientKey)
        {
            IEnumerable<Patient> patients = ((IAdtRepository) this).GetPatients(
                new [] {patientKey});

            return patients.FirstOrDefault();
        }

        Patient IAdtRepository.GetPatient(Guid patientSiloKey, Guid primaryPatientIdTypeKey, string patientId)
        {
            IEnumerable<Patient> patients = ((IAdtRepository)this).GetPatients(
                null, patientSiloKey, primaryPatientIdTypeKey, patientId);

            return patients.FirstOrDefault();
        }

        Guid IAdtRepository.InsertPatient(Context context, Patient patient)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(patient, "patient");
            Guid patientKey = Guid.Empty;

            try
            {                
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                using (ConnectionScopeFactory.Create())
                {
                    // Insert patient
                    patientKey = _patientRepository.InsertPatient(context.ToActionContext(), 
                        new AdtDAL.Models.Patient
                        {
                            PatientSiloKey = patient.PatientSiloKey,
                            PrefixText = patient.Prefix,
                            FirstName = patient.FirstName,
                            MiddleName = patient.MiddleName,
                            LastName = patient.LastName,
                            PreferredName = patient.PreferredName,
                            UnknownLastNameFlag = patient.UnknownLastName,
                            SuffixText = patient.Suffix,
                            BirthLocalDateTime = patient.BirthDate,
                            BirthDateTimePrecisionInternalCode = patient.BirthDateTimePrecisionCode.ToInternalCode(),
                            GenderKey = patient.GenderKey,
                            DeceasedFlag = patient.Deceased,
                            RestrictedAccessFlag = patient.RestrictedAccess,
                            PlaceholderFlag = patient.Placeholder
                        });

                    // insert patient ids
                    if (patient.IdentificationSet != null &&
                        patient.IdentificationSet.Count > 0)
                    {
                        InsertOrUpdatePatientIdentificationSet(_patientRepository, context, patientKey, patient.IdentificationSet);
                    }

                    // insert allergies
                    if (patient.AllergySet != null &&
                        patient.AllergySet.Count > 0)
                    {
                        InsertOrUpdateAllergySet(_patientRepository, context, patientKey, patient.AllergySet);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
            
            return patientKey;
        }

        void IAdtRepository.UpdatePatient(Context context, Patient patient)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(patient, "patient");

            try
            {                
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                using (ConnectionScopeFactory.Create())
                {
                    // Update patient
                    _patientRepository.UpdatePatient(context.ToActionContext(), 
                        new AdtDAL.Models.Patient
                        {
                            PatientSiloKey = patient.PatientSiloKey,
                            PrefixText = patient.Prefix,
                            FirstName = patient.FirstName,
                            MiddleName = patient.MiddleName,
                            LastName = patient.LastName,
                            PreferredName = patient.PreferredName,
                            UnknownLastNameFlag = patient.UnknownLastName,
                            SuffixText = patient.Suffix,
                            BirthLocalDateTime = patient.BirthDate,
                            BirthDateTimePrecisionInternalCode = patient.BirthDateTimePrecisionCode.ToInternalCode(),
                            GenderKey = patient.GenderKey,
                            DeceasedFlag = patient.Deceased,
                            RestrictedAccessFlag = patient.RestrictedAccess,
                            PlaceholderFlag = patient.Placeholder,
                            LastModifiedBinaryValue = patient.LastModified,
                            PatientKey = patient.Key
                        });

                    // update patient ids
                    if (patient.IdentificationSet != null)
                    {
                        if (patient.IdentificationSet.Any())
                            InsertOrUpdatePatientIdentificationSet(_patientRepository, context, patient.Key, patient.IdentificationSet);
                        else
                            DeletePatientIdentificationSet(_patientRepository, context, patient.Key);
                    }

                    // update allergies
                    if (patient.AllergySet != null)
                    {
                        if (patient.AllergySet.Any())
                            InsertOrUpdateAllergySet(_patientRepository, context, patient.Key, patient.AllergySet);
                        else
                            DeleteAllergySet(_patientRepository, context, patient.Key);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region PatientMerge Members

        void IAdtRepository.InsertPatientMergeAssociation(Context context, Guid survivingPatientKey, Guid nonSurvivingPatientKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _patientRepository.AssociatePatientMerge(
                    context.ToActionContext(),
                    survivingPatientKey,
                    nonSurvivingPatientKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAdtRepository.DeletePatientMergeAssociation(Context context, Guid survivingPatientKey, Guid nonSurvivingPatientKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _patientRepository.DisassociatePatientMerge(
                    context.ToActionContext(),
                    survivingPatientKey,
                    nonSurvivingPatientKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region PatientIdentificationType

        PatientIdentificationType IAdtRepository.GetPatientIdentificationType(Guid patientIdentificationTypeKey)
        {
            PatientIdentificationType patientIdentificationType = null;

            try
            {                
                var result = _patientIdentificationRepository.GetPatientIdentificationType(patientIdentificationTypeKey);
                if (result != null)
                {
                    patientIdentificationType = new PatientIdentificationType(result.PatientIdTypeKey)
                    {
                        Description = result.DescriptionText,
                        DisplayCode = result.DisplayCode,
                        InternalCode = result.InternalCode,
                        IsActive = result.ActiveFlag,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return patientIdentificationType;
        }

        PatientIdentificationType IAdtRepository.GetPatientIdentificationType(string displayCode)
        {
            PatientIdentificationType patientIdentificationType = null;

            try
            {                
                var result = _patientIdentificationRepository.GetPatientIdentificationTypeByDisplayCode(displayCode);
                if (result != null)
                {
                    patientIdentificationType = new PatientIdentificationType(result.PatientIdTypeKey)
                    {
                        Description = result.DescriptionText,
                        DisplayCode = result.DisplayCode,
                        InternalCode = result.InternalCode,
                        IsActive = result.ActiveFlag,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return patientIdentificationType;
        }

        Guid IAdtRepository.InsertPatientIdentificationType(Context context, PatientIdentificationType patientIdentificationType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(patientIdentificationType, "patientIdentificationType");
            Guid? patientIdentificationTypeKey = null;

            try
            {                
                patientIdentificationTypeKey = _patientIdentificationRepository.InsertPatientIdentificationType(context.ToActionContext(),
                    new AdtDAL.Models.PatientIdentificationType
                    {
                        PatientIdTypeKey = patientIdentificationType.Key,
                        DisplayCode = patientIdentificationType.DisplayCode,
                        DescriptionText = patientIdentificationType.Description,
                        SortValue = patientIdentificationType.SortOrder,
                        ActiveFlag = patientIdentificationType.IsActive,
                        LastModifiedBinaryValue = patientIdentificationType.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return patientIdentificationTypeKey.GetValueOrDefault();
        }

        void IAdtRepository.UpdatePatientIdentificationType(Context context, PatientIdentificationType patientIdentificationType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(patientIdentificationType, "patientIdentificationType");

            try
            {
                _patientIdentificationRepository.UpdatePatientIdentificationType(context.ToActionContext(),
                    new AdtDAL.Models.PatientIdentificationType
                    {
                        PatientIdTypeKey = patientIdentificationType.Key,
                        DisplayCode = patientIdentificationType.DisplayCode,
                        DescriptionText = patientIdentificationType.Description,
                        SortValue = patientIdentificationType.SortOrder,
                        ActiveFlag = patientIdentificationType.IsActive,
                        LastModifiedBinaryValue = patientIdentificationType.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region AllergyType Members

        AllergyType IAdtRepository.GetAllergyType(Guid allergyTypeKey)
        {
            AllergyType allergyType = null;

            try
            {                
                var result = _allergyTypeRepository.GetAllergyType(allergyTypeKey);
                if (result != null)
                {

                    allergyType = new AllergyType(result.AllergyTypeKey)
                    {
                        Code = result.AllergyTypeCode,
                        Description = result.DescriptionText,
                        ExternalSystemKey = result.ExternalSystemKey,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return allergyType;
        }

        AllergyType IAdtRepository.GetAllergyType(Guid externalSystemKey, string allergyTypeCode)
        {
            AllergyType allergyType = null;

            try
            {                
                var result = _allergyTypeRepository.GetAllergyTypeByCode(externalSystemKey, allergyTypeCode);
                if (result != null)
                {
                    allergyType = new AllergyType(result.AllergyTypeKey)
                    {
                        Code = result.AllergyTypeCode,
                        Description = result.DescriptionText,
                        ExternalSystemKey = result.ExternalSystemKey,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return allergyType;
        }

        Guid IAdtRepository.InsertAllergyType(Context context, AllergyType allergyType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(allergyType, "allergyType");
            Guid? allergyTypeKey = null;

            try
            {                
                allergyTypeKey = _allergyTypeRepository.InsertAllergyType(context.ToActionContext(),
                    new AdtDAL.Models.AllergyType
                    {
                        AllergyTypeKey = allergyType.Key,
                        ExternalSystemKey = allergyType.ExternalSystemKey,
                        AllergyTypeCode = allergyType.Code,
                        DescriptionText = allergyType.Description,
                        SortValue = allergyType.SortOrder,
                        LastModifiedBinaryValue = allergyType.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return allergyTypeKey.GetValueOrDefault();
        }

        void IAdtRepository.UpdateAllergyType(Context context, AllergyType allergyType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(allergyType, "allergyType");

            try
            {
                _allergyTypeRepository.UpdateAllergyType(context.ToActionContext(),
                    new AdtDAL.Models.AllergyType
                    {
                        AllergyTypeKey = allergyType.Key,
                        ExternalSystemKey = allergyType.ExternalSystemKey,
                        AllergyTypeCode = allergyType.Code,
                        DescriptionText = allergyType.Description,
                        SortValue = allergyType.SortOrder,
                        LastModifiedBinaryValue = allergyType.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAdtRepository.DeleteAllergyType(Context context, Guid allergyTypeKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _allergyTypeRepository.DeleteAllergyType(context.ToActionContext(), allergyTypeKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region AllergySeverity Members

        AllergySeverity IAdtRepository.GetAllergySeverity(Guid allergySeverityKey)
        {
            AllergySeverity allergySeverity = null;

            try
            {                
                var result = _allergySeverityRepository.GetAllergySeverity(allergySeverityKey);
                if (result != null)
                {

                    allergySeverity = new AllergySeverity(result.AllergySeverityKey)
                    {
                        Code = result.AllergySeverityCode,
                        Description = result.DescriptionText,
                        ExternalSystemKey = result.ExternalSystemKey,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return allergySeverity;
        }

        AllergySeverity IAdtRepository.GetAllergySeverity(Guid externalSystemKey, string allergySeverityCode)
        {
            AllergySeverity allergySeverity = null;

            try
            {                
                var result = _allergySeverityRepository.GetAllergySeverityByCode(externalSystemKey, allergySeverityCode);
                if (result != null)
                {

                    allergySeverity = new AllergySeverity(result.AllergySeverityKey)
                    {
                        Code = result.AllergySeverityCode,
                        Description = result.DescriptionText,
                        ExternalSystemKey = result.ExternalSystemKey,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return allergySeverity;
        }

        Guid IAdtRepository.InsertAllergySeverity(Context context, AllergySeverity allergySeverity)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(allergySeverity, "allergySeverity");
            Guid? allergySeverityKey = null;

            try
            {                
                allergySeverityKey = _allergySeverityRepository.InsertAllergySeverity(context.ToActionContext(),
                    new AdtDAL.Models.AllergySeverity
                    {
                        AllergySeverityKey = allergySeverity.Key,
                        ExternalSystemKey = allergySeverity.ExternalSystemKey,
                        AllergySeverityCode = allergySeverity.Code,
                        DescriptionText = allergySeverity.Description,
                        SortValue = allergySeverity.SortOrder,
                        LastModifiedBinaryValue = allergySeverity.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return allergySeverityKey.GetValueOrDefault();
        }

        void IAdtRepository.UpdateAllergySeverity(Context context, AllergySeverity allergySeverity)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(allergySeverity, "allergySeverity");

            try
            {
                _allergySeverityRepository.UpdateAllergySeverity(context.ToActionContext(),
                    new AdtDAL.Models.AllergySeverity
                    {
                        AllergySeverityKey = allergySeverity.Key,
                        ExternalSystemKey = allergySeverity.ExternalSystemKey,
                        AllergySeverityCode = allergySeverity.Code,
                        DescriptionText = allergySeverity.Description,
                        SortValue = allergySeverity.SortOrder,
                        LastModifiedBinaryValue = allergySeverity.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAdtRepository.DeleteAllergySeverity(Context context, Guid allergySeverityKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _allergySeverityRepository.DeleteAllergySeverity(context.ToActionContext(), allergySeverityKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Gender Members

        Gender IAdtRepository.GetGender(Guid genderKey)
        {
            Gender gender = null;

            try
            {                
                var result = _genderRepository.GetGender(genderKey);
                if (result != null)
                {
                    gender = new Gender(result.GenderKey)
                    {
                        DisplayCode = result.DisplayCode,
                        InternalCode = result.InternalCode,
                        IsActive = result.ActiveFlag,
                        Description = result.DescriptionText,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return gender;
        }

        Gender IAdtRepository.GetGender(string displayCode)
        {
            Gender gender = null;

            try
            {                
                var result = _genderRepository.GetGenderByDisplayCode(displayCode);
                if (result != null)
                {

                    gender = new Gender(result.GenderKey)
                    {
                        DisplayCode = result.DisplayCode,
                        InternalCode = result.InternalCode,
                        IsActive = result.ActiveFlag,
                        Description = result.DescriptionText,                        
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return gender;
        }

        Guid IAdtRepository.InsertGender(Context context, Gender gender)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(gender, "gender");
            Guid? genderKey = null;

            try
            {
                genderKey = _genderRepository.InsertGender(context.ToActionContext(),
                    new AdtDAL.Models.Gender
                    {
                        GenderKey = gender.Key,
                        DisplayCode = gender.DisplayCode,
                        DescriptionText = gender.Description,
                        SortValue = gender.SortOrder,
                        ActiveFlag = gender.IsActive,
                        LastModifiedBinaryValue = gender.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return genderKey.GetValueOrDefault();
        }

        void IAdtRepository.UpdateGender(Context context, Gender gender)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(gender, "gender");

            try
            {
                _genderRepository.UpdateGender(context.ToActionContext(),
                    new AdtDAL.Models.Gender
                    {
                        GenderKey = gender.Key,
                        DisplayCode = gender.DisplayCode,
                        DescriptionText = gender.Description,
                        SortValue = gender.SortOrder,
                        ActiveFlag = gender.IsActive,
                        LastModifiedBinaryValue = gender.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Encounter Members

        IEnumerable<Encounter> IAdtRepository.GetEncounters(IEnumerable<Guid> encounterKeys, string encounterId, Guid? patientKey,
                                                 Guid? patientSiloKey, Guid? patientIdTypeKey, string patientId)
        {
            List<Encounter> encounters = new List<Encounter>();
            if (encounterKeys != null && !encounterKeys.Any())
                return encounters; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (encounterKeys != null)
                    selectedKeys = new GuidKeyTable(encounterKeys.Distinct());

                using(var connectionScope = ConnectionScopeFactory.Create())
                using (var multi = connectionScope.QueryMultiple(
                    "ADT.bsp_GetEncounters",
                    new
                    {
                        SelectedKeys = selectedKeys.AsTableValuedParameter(),
                        EncounterID = encounterId,
                        PatientKey = patientKey,
                        PatientSiloKey = patientSiloKey,
                        PatientIDTypeKey = patientIdTypeKey,
                        PatientID = patientId
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure))
                {
                    var encounterResults = multi.Read<EncounterResult>();
                    var patientLocationResults = multi.Read<EncounterPatientLocationResult>();
                    var physicianRoleSetResults = multi.Read<EncounterPhysicianRoleSetResult>();
                    var physicianResults = multi.Read<EncounterPhysicianResult>();

                    // Create patient locations
                    Dictionary<Guid, PatientLocation> patientLocations = new Dictionary<Guid, PatientLocation>();
                    foreach (var patientLocationResult in patientLocationResults)
                    {
                        PatientLocation patientLocation = new PatientLocation(patientLocationResult.EncounterPatientLocationKey)
                            {
                                AssignedBedId = patientLocationResult.AssignedBedID,
                                AssignedFacilityKey = patientLocationResult.AssignedFacilityKey,
                                AssignedUnitKey = patientLocationResult.AssignedUnitKey,
                                AssignedUnitRoomKey = patientLocationResult.AssignedUnitRoomKey,
                                TemporaryBedId = patientLocationResult.TemporaryBedID,
                                TemporaryUnitKey = patientLocationResult.TemporaryUnitKey,
                                TemporaryUnitRoomKey = patientLocationResult.TemporaryUnitRoomKey,
                                LastModified = patientLocationResult.LastModifiedBinaryValue.ToArray()
                            };

                        patientLocations[patientLocationResult.EncounterKey] = patientLocation;
                    }

                    // Create attending physician role sets
                    Dictionary<Guid, PhysicianRoleSet> physicianRoleSets = new Dictionary<Guid, PhysicianRoleSet>();
                    foreach (var physicianRoleSetResult in physicianRoleSetResults.Where(epr => 
                        epr.EncounterPhysicianRoleInternalCode == EncounterPhysicianRoleInternalCode.ATT.ToInternalCode()))
                    {
                        var physicians = physicianResults
                            .Where(phy => phy.EncounterPhysicianRoleSetKey == physicianRoleSetResult.EncounterPhysicianRoleSetKey)
                            .Select(phy => new Physician(phy.EncounterPhysicianKey)
                                {
                                    MemberNumber = phy.MemberNumber,
                                    PhysicianId = phy.PhysicianID,
                                    Prefix = phy.PrefixText,
                                    FirstName = phy.FirstName,
                                    MiddleName = phy.MiddleName,
                                    LastName = phy.LastName,
                                    Suffix = phy.SuffixText,
                                    LastModified = phy.LastModifiedBinaryValue.ToArray()
                                })
                            .ToArray();

                        PhysicianRoleSet physicianRoleSet = new PhysicianRoleSet(physicianRoleSetResult.EncounterPhysicianRoleSetKey,
                            physicianRoleSetResult.EncounterPhysicianRoleInternalCode.FromInternalCode<EncounterPhysicianRoleInternalCode>(), 
                            physicians)
                            {
                                LastModified = physicianRoleSetResult.LastModifiedBinaryValue.ToArray()
                            };

                        physicianRoleSets[physicianRoleSetResult.EncounterKey] = physicianRoleSet;
                    }

                    foreach (var encounterResult in encounterResults)
                    {
                        Encounter encounter = new Encounter(encounterResult.EncounterKey, encounterResult.EncounterSnapshotKey)
                        {
                            Surviving = encounterResult.SurvivingFlag,
                            AdmissionStatus = encounterResult.EncounterAdmissionStatusInternalCode.FromInternalCode<EncounterAdmissionStatusInternalCode>(),
                            AdmitDate = encounterResult.AdmitLocalDateTime,
                            AttendingPhysicianRoleSet = physicianRoleSets.ContainsKey(encounterResult.EncounterKey)
                                ? physicianRoleSets[encounterResult.EncounterKey] : null,
                            IsAdmitDateOnly = encounterResult.AdmitDateOnlyFlag,
                            AdmitUtcDate = encounterResult.AdmitUTCDateTime,
                            AccountId = encounterResult.AccountID,
                            AccountIdCheckValue = encounterResult.AccountIDCheckValue,
                            AlternateId = encounterResult.AlternateEncounterID,
                            AlternateIdCheckValue = encounterResult.AlternateEncounterIDCheckValue,
                            PatientClassKey = encounterResult.EncounterPatientClassKey,
                            HospitalServiceKey = encounterResult.HospitalServiceKey,
                            Comment = encounterResult.CommentText,
                            CreatedUtcDateTime = encounterResult.CreatedUTCDateTime,
                            CreatedDateTime = encounterResult.CreatedLocalDateTime,
                            DischargeDate = encounterResult.DischargeLocalDateTime,
                            IsDischargeDateOnly = encounterResult.DischargeDateOnlyFlag,
                            DischargeUtcDate = encounterResult.DischargeUTCDateTime,
                            ExpectedAdmitDate = encounterResult.ExpectedAdmitLocalDateTime,
                            IsExpectedAdmitDateOnly = encounterResult.ExpectedAdmitDateOnlyFlag,
                            ExpectedAdmitUtcDate = encounterResult.ExpectedAdmitUTCDateTime,
                            AlternateAutoDischarge = encounterResult.AlternateAutoDischargeDurationFlag,
                            CancelledUtcDateTime = encounterResult.CancelledUTCDateTime,
                            CancelledDateTime = encounterResult.CancelledLocalDateTime,
                            Id = encounterResult.EncounterID,
                            IdCheckValue = encounterResult.EncounterIDCheckValue,
                            IsRecurring = encounterResult.RecurringFlag,
                            LastItemTransactionUtcDateTime = encounterResult.LastItemTransactionUTCDateTime,
                            LastItemTransactionDateTime = encounterResult.LastItemTransactionLocalDateTime,
                            Location = patientLocations.ContainsKey(encounterResult.EncounterKey)
                                ? patientLocations[encounterResult.EncounterKey] : null,
                            PatientKey = encounterResult.PatientKey,
                            PreAdmitId = encounterResult.PreadmitID,
                            PreAdmitIdCheckValue = encounterResult.PreadmitIDCheckValue,
                            EncounterType = encounterResult.EncounterTypeInternalCode.FromInternalCode<EncounterTypeInternalCode>(),
                            HeightAmount = encounterResult.HeightAmount,
                            HeightUnitOfMeasureKey = encounterResult.HeightUOMKey,
                            HeightObservationMethodInternalCode = encounterResult.HeightMethodInternalCode.FromNullableInternalCode<ObservationMethodInternalCode>(),
                            WeightAmount = encounterResult.WeightAmount,
                            WeightUnitOfMeasureKey = encounterResult.WeightUOMKey,
                            WeightUtcDate = encounterResult.WeightUTCDateTime,
                            WeightDate = encounterResult.WeightLocalDateTime,
                            WeightObservationMethodInternalCode = encounterResult.WeightMethodInternalCode.FromNullableInternalCode<ObservationMethodInternalCode>(),
                            LeaveOfAbsenceEffectiveUtcDate = encounterResult.LeaveOfAbsenceEffectiveUTCDateTime,
                            LeaveOfAbsenceEffectiveDate = encounterResult.LeaveOfAbsenceEffectiveLocalDateTime,
                            LeaveOfAbsenceReturnUtcDate = encounterResult.LeaveOfAbsenceReturnUTCDateTime,
                            LeaveOfAbsenceReturnDate = encounterResult.LeaveOfAbsenceReturnLocalDateTime,
                            CreatedAtDispensingDeviceKey = encounterResult.CreatedAtDispensingDeviceKey,
                            LastModified = encounterResult.LastModifiedBinaryValue.ToArray()
                        };

                        encounters.Add(encounter);
                    }

                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return encounters;
        }

        IEnumerable<Encounter> IAdtRepository.GetEncounters(Guid patientSiloKey, string encounterId)
        {
            return ((IAdtRepository) this).GetEncounters(null, encounterId, patientSiloKey: patientSiloKey);
        }

        Encounter IAdtRepository.GetEncounter(Guid encounterKey)
        {
            IEnumerable<Encounter> encounters = ((IAdtRepository)this).GetEncounters(
                new[] { encounterKey });

            return encounters.FirstOrDefault();
        }

        Encounter IAdtRepository.GetEncounter(Guid patientKey, string encounterId)
        {
            IEnumerable<Encounter> encounters = ((IAdtRepository) this).GetEncounters(null, encounterId, patientKey);

            return encounters.FirstOrDefault();
        }

        Encounter IAdtRepository.GetEncounter(string patientId, string encounterId, 
            Guid primaryPatientIdTypeKey, Guid patientSiloKey)
        {
            IEnumerable<Encounter> encounters = ((IAdtRepository) this).GetEncounters(null, encounterId, null,
                                                                                      patientSiloKey,
                                                                                      primaryPatientIdTypeKey, patientId);

            return encounters.FirstOrDefault();
        }

        bool IAdtRepository.EncounterExists(Guid patientSiloKey, string encounterId, Guid ignorePatientKey)
        {
            bool exists = false;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("TOP 1 es.EncounterKey")
                    .FROM("ADT.vw_EncounterCurrent es")
                    .JOIN("ADT.vw_PatientCurrent p ON p.PatientKey = es.PatientKey")                    
                    .WHERE("p.PatientKey <> @PatientKey")
                    .WHERE("p.PatientSiloKey = @PatientSiloKey")
                    .WHERE("es.EncounterID = @EncounterID");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    exists = connectionScope.Query(
                         query.ToString(),
                         new
                         {
                             PatientKey = ignorePatientKey,
                             PatientSiloKey = patientSiloKey,
                             EncounterID = encounterId
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                         .Any();
                }                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return exists;
        }

        Guid IAdtRepository.InsertEncounter(Context context, Encounter encounter)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(encounter, "encounter");
            Guid encounterKey = Guid.Empty;

            try
            {                
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                using (ConnectionScopeFactory.Create())
                {
                    encounterKey = _encounterRepository.InsertEncounter(context.ToActionContext(), 
                        new AdtDAL.Models.Encounter
                        {
                            EncounterAdmissionStatusInternalCode = encounter.AdmissionStatus.ToInternalCode(),
                            EncounterId = encounter.Id,
                            EncounterIdCheckValue = encounter.IdCheckValue,
                            PreadmitId = encounter.PreAdmitId,
                            PreadmitIdCheckValue = encounter.PreAdmitIdCheckValue,
                            AlternateEncounterId = encounter.AlternateId,
                            AlternateEncounterIdCheckValue = encounter.AlternateIdCheckValue,
                            AccountId = encounter.AccountId,
                            AccountIdCheckValue = encounter.AccountIdCheckValue,
                            PatientKey = encounter.PatientKey,
                            AdmitUtcDateTime = encounter.AdmitUtcDate,
                            AdmitLocalDateTime = encounter.AdmitDate,
                            AdmitDateOnlyFlag = encounter.IsAdmitDateOnly,
                            ExpectedAdmitUtcDateTime = encounter.ExpectedAdmitUtcDate,
                            ExpectedAdmitLocalDateTime = encounter.ExpectedAdmitDate,
                            ExpectedAdmitDateOnlyFlag = encounter.IsExpectedAdmitDateOnly,
                            DischargeUtcDateTime = encounter.DischargeUtcDate,
                            DischargeLocalDateTime = encounter.DischargeDate,
                            DischargeDateOnlyFlag = encounter.IsDischargeDateOnly,
                            AlternateAutoDischargeDurationFlag = encounter.AlternateAutoDischarge,
                            CancelledUtcDateTime = encounter.CancelledUtcDateTime,
                            CancelledLocalDateTime = encounter.CancelledDateTime,
                            EncounterPatientClassKey = encounter.PatientClassKey,
                            HospitalServiceKey = encounter.HospitalServiceKey,
                            CommentText = encounter.Comment,
                            RecurringFlag = encounter.IsRecurring,
                            EncounterTypeInternalCode = encounter.EncounterType.ToInternalCode(),
                            HeightAmount = encounter.HeightAmount,
                            HeightUOMKey = encounter.HeightUnitOfMeasureKey,
                            HeightMethodInternalCode = encounter.HeightObservationMethodInternalCode.ToInternalCode(),
                            WeightAmount = encounter.WeightAmount,
                            WeightUtcDateTime = encounter.WeightUtcDate,
                            WeightLocalDateTime = encounter.WeightDate,
                            WeightUOMKey = encounter.WeightUnitOfMeasureKey,
                            WeightMethodInternalCode = encounter.WeightObservationMethodInternalCode.ToInternalCode(),
                            LeaveOfAbsenceEffectiveUtcDateTime = encounter.LeaveOfAbsenceEffectiveUtcDate,
                            LeaveOfAbsenceEffectiveLocalDateTime = encounter.LeaveOfAbsenceEffectiveDate,
                            LeaveOfAbsenceReturnUtcDateTime = encounter.LeaveOfAbsenceReturnUtcDate,
                            LeaveOfAbsenceReturnLocalDateTime = encounter.LeaveOfAbsenceReturnDate
                        });

                    // insert encounter location
                    if (encounter.Location != null)
                    {
                        _encounterRepository.InsertEncounterPatientLocation(context.ToActionContext(), new AdtDAL.Models.EncounterPatientLocation
                        {
                            EncounterKey = encounterKey,
                            AssignedFacilityKey = encounter.Location.AssignedFacilityKey,
                            AssignedUnitKey = encounter.Location.AssignedUnitKey,
                            AssignedUnitRoomKey = encounter.Location.AssignedUnitRoomKey,
                            AssignedBedId = encounter.Location.AssignedBedId,
                            TemporaryUnitKey = encounter.Location.TemporaryUnitKey,
                            TemporaryUnitRoomKey = encounter.Location.TemporaryUnitRoomKey,
                            TemporaryBedId = encounter.Location.TemporaryBedId
                        });
                    }

                    // insert attending physician role
                    if (encounter.AttendingPhysicianRoleSet != null &&
                        encounter.AttendingPhysicianRoleSet.Count > 0)
                    {
                        InsertOrUpdatePhysicianRoleSet(_encounterRepository, context, encounterKey, encounter.AttendingPhysicianRoleSet);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return encounterKey;
        }

        void IAdtRepository.UpdateEncounter(Context context, Encounter encounter)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(encounter, "encounter");

            try
            {                
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    _encounterRepository.UpdateEncounter(context.ToActionContext(), 
                        new AdtDAL.Models.Encounter
                        {
                            EncounterAdmissionStatusInternalCode = encounter.AdmissionStatus.ToInternalCode(),
                            EncounterId = encounter.Id,
                            EncounterIdCheckValue = encounter.IdCheckValue,
                            PreadmitId = encounter.PreAdmitId,
                            PreadmitIdCheckValue = encounter.PreAdmitIdCheckValue,
                            AlternateEncounterId = encounter.AlternateId,
                            AlternateEncounterIdCheckValue = encounter.AlternateIdCheckValue,
                            AccountId = encounter.AccountId,
                            AccountIdCheckValue = encounter.AccountIdCheckValue,
                            PatientKey = encounter.PatientKey,
                            AdmitUtcDateTime = encounter.AdmitUtcDate,
                            AdmitLocalDateTime = encounter.AdmitDate,
                            AdmitDateOnlyFlag = encounter.IsAdmitDateOnly,
                            ExpectedAdmitUtcDateTime = encounter.ExpectedAdmitUtcDate,
                            ExpectedAdmitLocalDateTime = encounter.ExpectedAdmitDate,
                            ExpectedAdmitDateOnlyFlag = encounter.IsExpectedAdmitDateOnly,
                            DischargeUtcDateTime = encounter.DischargeUtcDate,
                            DischargeLocalDateTime = encounter.DischargeDate,
                            DischargeDateOnlyFlag = encounter.IsDischargeDateOnly,
                            AlternateAutoDischargeDurationFlag = encounter.AlternateAutoDischarge,
                            CancelledUtcDateTime = encounter.CancelledUtcDateTime,
                            CancelledLocalDateTime = encounter.CancelledDateTime,
                            EncounterPatientClassKey = encounter.PatientClassKey,
                            HospitalServiceKey = encounter.HospitalServiceKey,
                            CommentText = encounter.Comment,
                            RecurringFlag = encounter.IsRecurring,
                            EncounterTypeInternalCode = encounter.EncounterType.ToInternalCode(),
                            HeightAmount = encounter.HeightAmount,
                            HeightUOMKey = encounter.HeightUnitOfMeasureKey,
                            HeightMethodInternalCode = encounter.HeightObservationMethodInternalCode.ToInternalCode(),
                            WeightAmount = encounter.WeightAmount,
                            WeightUtcDateTime = encounter.WeightUtcDate,
                            WeightLocalDateTime = encounter.WeightDate,
                            WeightUOMKey = encounter.WeightUnitOfMeasureKey,
                            WeightMethodInternalCode = encounter.WeightObservationMethodInternalCode.ToInternalCode(),
                            LeaveOfAbsenceEffectiveUtcDateTime = encounter.LeaveOfAbsenceEffectiveUtcDate,
                            LeaveOfAbsenceEffectiveLocalDateTime = encounter.LeaveOfAbsenceEffectiveDate,
                            LeaveOfAbsenceReturnUtcDateTime = encounter.LeaveOfAbsenceReturnUtcDate,
                            LeaveOfAbsenceReturnLocalDateTime = encounter.LeaveOfAbsenceReturnDate,
                            LastModifiedBinaryValue = encounter.LastModified,
                            EncounterKey = encounter.Key
                        });

                    // update encounter location
                    if (encounter.Location != null)
                    {
                        _encounterRepository.UpdateEncounterPatientLocation(context.ToActionContext(), new AdtDAL.Models.EncounterPatientLocation
                        {
                            EncounterKey = encounter.Key,
                            AssignedFacilityKey = encounter.Location.AssignedFacilityKey,
                            AssignedUnitKey = encounter.Location.AssignedUnitKey,
                            AssignedUnitRoomKey = encounter.Location.AssignedUnitRoomKey,
                            AssignedBedId = encounter.Location.AssignedBedId,
                            TemporaryUnitKey = encounter.Location.TemporaryUnitKey,
                            TemporaryUnitRoomKey = encounter.Location.TemporaryUnitRoomKey,
                            TemporaryBedId = encounter.Location.TemporaryBedId,
                            LastModifiedBinaryValue = encounter.Location.LastModified
                        });
                    }

                    // update physician roles
                    if (encounter.AttendingPhysicianRoleSet != null)
                    {
                        if (encounter.AttendingPhysicianRoleSet.IsTransient() || 
                            encounter.AttendingPhysicianRoleSet.Count > 0)
                        {
                            InsertOrUpdatePhysicianRoleSet(_encounterRepository, context, encounter.Key,
                                                           encounter.AttendingPhysicianRoleSet);
                        }
                        else
                            DeletePhysicianRoleSet(_encounterRepository, context, encounter.Key, encounter.AttendingPhysicianRoleSet.RoleInternalCode);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        public void UpdateEncounterActivity(Context context, Guid encounterKey, DateTime activityUtcDateTime,
            DateTime activityLocalDateTime)
        {
            UpdateEncounterActivity(
                (Guid?) context.Device,
                encounterKey,
                activityUtcDateTime,
                activityLocalDateTime);
        }

        public void UpdateEncounterActivity(Guid? deviceKey, Guid encounterKey, DateTime activityUtcDateTime, DateTime activityLocalDateTime)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "ADT.usp_EncounterActivityUpdate",
                        new
                        {
                            LastItemTransactionUTCDateTime = activityUtcDateTime,
                            LastItemTransactionLocalDateTime = activityLocalDateTime,
                            LastModifiedDispensingDeviceKey = deviceKey,
                            EncounterKey = encounterKey
                        },
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);                    
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region EncounterMerge Members

        void IAdtRepository.InsertEncounterMergeAssociation(Context context, Guid survivingEncounterKey, Guid nonSurvivingEncounterKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _encounterRepository.AssociateEncounterMerge(
                    context.ToActionContext(),
                    survivingEncounterKey,
                    nonSurvivingEncounterKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAdtRepository.DeleteEncounterMergeAssociation(Context context, Guid survivingEncounterKey, Guid nonSurvivingEncounterKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _encounterRepository.DisassociateEncounterMerge(
                    context.ToActionContext(),
                    survivingEncounterKey,
                    nonSurvivingEncounterKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region HospitalService Members

        HospitalService IAdtRepository.GetHospitalService(Guid hospitalServiceKey)
        {
            HospitalService hospitalService = null;

            try
            {                
                var result = _hospitalServiceRepository.GetHospitalService(hospitalServiceKey);
                if (result != null)
                {

                    hospitalService = new HospitalService(result.HospitalServiceKey)
                    {
                        Code = result.HospitalServiceCode,
                        ExternalSystemKey = result.ExternalSystemKey,                           
                        Description = result.DescriptionText,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return hospitalService;
        }

        HospitalService IAdtRepository.GetHospitalService(Guid externalSystemKey, string hospitalServiceCode)
        {
            HospitalService hospitalService = null;

            try
            {                
                var result = _hospitalServiceRepository.GetHospitalServiceByCode(externalSystemKey, hospitalServiceCode);
                if (result != null)
                {

                    hospitalService = new HospitalService(result.HospitalServiceKey)
                    {
                        Code = result.HospitalServiceCode,
                        ExternalSystemKey = result.ExternalSystemKey,
                        Description = result.DescriptionText,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return hospitalService;
        }

        Guid IAdtRepository.InsertHospitalService(Context context, HospitalService hospitalService)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(hospitalService, "hospitalService");
            Guid? hospitalServiceKey = null;

            try
            {                
                hospitalServiceKey = _hospitalServiceRepository.InsertHospitalService(context.ToActionContext(),
                    new AdtDAL.Models.HospitalService
                    {
                        HospitalServiceKey = hospitalService.Key,
                        ExternalSystemKey = hospitalService.ExternalSystemKey,
                        HospitalServiceCode = hospitalService.Code,
                        DescriptionText = hospitalService.Description,
                        SortValue = hospitalService.SortOrder,
                        LastModifiedBinaryValue = hospitalService.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return hospitalServiceKey.GetValueOrDefault();
        }

        void IAdtRepository.UpdateHospitalService(Context context, HospitalService hospitalService)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(hospitalService, "hospitalService");

            try
            {
                _hospitalServiceRepository.UpdateHospitalService(context.ToActionContext(),
                    new AdtDAL.Models.HospitalService
                    {
                        HospitalServiceKey = hospitalService.Key,
                        ExternalSystemKey = hospitalService.ExternalSystemKey,
                        HospitalServiceCode = hospitalService.Code,
                        DescriptionText = hospitalService.Description,
                        SortValue = hospitalService.SortOrder,
                        LastModifiedBinaryValue = hospitalService.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAdtRepository.DeleteHospitalService(Context context, Guid hospitalServiceKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _hospitalServiceRepository.DeleteHospitalService(context.ToActionContext(), hospitalServiceKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region PatientClass Members

        PatientClass IAdtRepository.GetPatientClass(Guid patientClassKey)
        {
            PatientClass patientClass = null;

            try
            {                
                var result = _encounterPatientClassRepository.GetEncounterPatientClass(patientClassKey);
                if (result != null)
                {

                    patientClass = new PatientClass(result.EncounterPatientClassKey)
                    {
                        Code = result.EncounterPatientClassCode,
                        ExternalSystemKey = result.ExternalSystemKey,
                        Description = result.DescriptionText,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return patientClass;
        }

        PatientClass IAdtRepository.GetPatientClass(Guid externalSystemKey, string patientClassCode)
        {
            PatientClass patientClass = null;

            try
            {                
                var result = _encounterPatientClassRepository.GetEncounterPatientClassByCode(externalSystemKey, patientClassCode);
                if (result != null)
                {

                    patientClass = new PatientClass(result.EncounterPatientClassKey)
                    {
                        Code = result.EncounterPatientClassCode,
                        ExternalSystemKey = result.ExternalSystemKey,
                        Description = result.DescriptionText,
                        LastModified = result.LastModifiedBinaryValue,
                        SortOrder = result.SortValue
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return patientClass;
        }

        Guid IAdtRepository.InsertPatientClass(Context context, PatientClass patientClass)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(patientClass, "patientClass");
            Guid? patientClassKey = null;

            try
            {
                patientClassKey = _encounterPatientClassRepository.InsertEncounterPatientClass(context.ToActionContext(),
                    new AdtDAL.Models.EncounterPatientClass
                    {
                        EncounterPatientClassKey = patientClass.Key,
                        ExternalSystemKey = patientClass.ExternalSystemKey,
                        EncounterPatientClassCode = patientClass.Code,
                        DescriptionText = patientClass.Description,
                        SortValue = patientClass.SortOrder,
                        LastModifiedBinaryValue = patientClass.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return patientClassKey.GetValueOrDefault();
        }

        void IAdtRepository.UpdatePatientClass(Context context, PatientClass patientClass)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(patientClass, "patientClass");

            try
            {
                _encounterPatientClassRepository.UpdateEncounterPatientClass(context.ToActionContext(),
                    new AdtDAL.Models.EncounterPatientClass
                    {
                        EncounterPatientClassKey = patientClass.Key,
                        ExternalSystemKey = patientClass.ExternalSystemKey,
                        EncounterPatientClassCode = patientClass.Code,
                        DescriptionText = patientClass.Description,
                        SortValue = patientClass.SortOrder,
                        LastModifiedBinaryValue = patientClass.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAdtRepository.DeletePatientClass(Context context, Guid patientClassKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _encounterPatientClassRepository.DeleteEncounterPatientClass(context.ToActionContext(), patientClassKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region User Encounter Association Members

        void IAdtRepository.InsertUserEncounterAssociation(Context context, Guid userAccountKey, Guid encounterKey, DateTime? lastAddDateTime, DateTime? lastAddUtcDateTime)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _encounterRepository.AssociateUserAccount(
                    context.ToActionContext(),
                    userAccountKey,
                    encounterKey,
                    lastAddUtcDateTime,
                    lastAddDateTime);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAdtRepository.DeleteUserEncounterAssociation(Context context, Guid userAccountKey, Guid encounterKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _encounterRepository.DisassociateUserAccount(
                    context.ToActionContext(),
                    userAccountKey,
                    encounterKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Helper Methods

        private static void InsertOrUpdatePatientIdentificationSet(AdtDAL.IPatientRepository patientRepository, Context context, Guid patientKey, PatientIdentificationSet patientIdentificationSet)
        {
            Guard.ArgumentNotNull(patientRepository, "patientRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(patientIdentificationSet, "patientIdentificationSet");

            AdtDAL.Models.PatientIdentificationSet set = new AdtDAL.Models.PatientIdentificationSet
            {
                PatientKey = patientKey,
                LastModifiedBinaryValue = patientIdentificationSet.LastModified
            };

            patientIdentificationSet.ForEach(pid => set.Add(new AdtDAL.Models.PatientIdentification
            {
                PatientIdTypeKey = pid.IdentificationTypeKey,
                PatientId = pid.Value,
                CheckValue = pid.CheckValue,
                LastModifiedDispensingDeviceKey = (Guid?)context.Device
            }));

            patientRepository.InsertOrUpdatePatientIdentificationSet(context.ToActionContext(), set);
        }

        private static void DeletePatientIdentificationSet(AdtDAL.IPatientRepository patientRepository, Context context, Guid patientKey)
        {
            Guard.ArgumentNotNull(patientRepository, "patientRepository");
            Guard.ArgumentNotNull(context, "context");

            patientRepository.DeletePatientIdentificationSet(
                context.ToActionContext(),
                patientKey);
        }

        private static void InsertOrUpdateAllergySet(AdtDAL.IPatientRepository patientRepository, Context context, Guid patientKey, AllergySet allergySet)
        {
            Guard.ArgumentNotNull(patientRepository, "patientRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(allergySet, "allergySet");

            AdtDAL.Models.PatientAllergySet set = new AdtDAL.Models.PatientAllergySet
            {
                PatientKey = patientKey,
                LastModifiedBinaryValue = allergySet.LastModified
            };

            foreach (Allergy allergy in allergySet)
            {
                set.Add(new AdtDAL.Models.PatientAllergy
                {
                    MemberNumber = allergy.MemberNumber,
                    AllergyTypeKey = allergy.AllergyTypeKey,
                    AllergenCode = allergy.AllergenCode,
                    AllergenDescriptionText = allergy.AllergenDescription,
                    AllergenDescriptionTruncatedFlag = allergy.AllergenDescriptionTruncated,
                    AllergySeverityKey = allergy.AllergySeverityKey,
                    SensitivityCode = allergy.SensitivityCode,
                    SensitivityDescriptionText = allergy.SensitivityDescription,
                    AllergyUniqueId = allergy.UniqueId,
                    PatientAllergyReactions = allergy.Reactions?
                        .Select(par => new AdtDAL.Models.PatientAllergyReaction
                        {
                            MemberNumber = par.MemberNumber,
                            ReactionCode = par.ReactionCode
                        })
                        .ToArray()
                });
            }

            patientRepository.InsertOrUpdatePatientAllergySet(context.ToActionContext(), set);
        }

        private static void DeleteAllergySet(AdtDAL.IPatientRepository patientRepository, Context context, Guid patientKey)
        {
            Guard.ArgumentNotNull(patientRepository, "patientRepository");
            Guard.ArgumentNotNull(context, "context");

            patientRepository.DeletePatientAllergySet(
                context.ToActionContext(),
                patientKey);
        }

        private static void InsertOrUpdatePhysicianRoleSet(AdtDAL.IEncounterRepository encounterRepository, Context context, Guid encounterKey, PhysicianRoleSet physicianRoleSet)
        {
            Guard.ArgumentNotNull(encounterRepository, "encounterRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(physicianRoleSet, "physicianRoleSet");

            AdtDAL.Models.EncounterPhysicianRoleSet set = new AdtDAL.Models.EncounterPhysicianRoleSet
            {
                EncounterKey = encounterKey,
                EncounterPhysicianRoleInternalCode = physicianRoleSet.RoleInternalCode.ToInternalCode(),
                LastModifiedBinaryValue = physicianRoleSet.LastModified
            };

            physicianRoleSet.ForEach(ep => set.Add(new AdtDAL.Models.EncounterPhysician
            {
                MemberNumber = ep.MemberNumber,
                PhysicianId = ep.PhysicianId,
                LastName = ep.LastName,
                PrefixText = ep.Prefix,
                FirstName = ep.FirstName,
                MiddleName = ep.MiddleName,
                SuffixText = ep.Suffix
            }));

            encounterRepository.InsertOrUpdateEncounterPhysicianRoleSet(context.ToActionContext(), set);
        }

        private static void DeletePhysicianRoleSet(AdtDAL.IEncounterRepository encounterRepository, Context context, Guid encounterKey, EncounterPhysicianRoleInternalCode physicianRole)
        {
            Guard.ArgumentNotNull(encounterRepository, "encounterRepository");
            Guard.ArgumentNotNull(context, "context");

            encounterRepository.DeleteEncounterPhysicianRoleSet(
                context.ToActionContext(),
                encounterKey,
                physicianRole.ToInternalCode());
        }

        #endregion
    }
}
