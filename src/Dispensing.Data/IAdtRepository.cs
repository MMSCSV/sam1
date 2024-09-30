using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data
{
    /// <summary>
    /// Represents a repository specific to the ADT data model.
    /// </summary>
    public interface IAdtRepository : IRepository
    {
        #region Patient Members

        /// <summary>
        /// Retrieves a collection of <see cref="Patient"/>.
        /// </summary>
        /// <param name="patientKeys">The collection of patient silo keys or NULL for all.</param>
        /// <param name="patientSiloKey"></param>
        /// <param name="patientIdTypeKey"></param>
        /// <param name="patientId"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="Patient"/>.</returns>
        IEnumerable<Patient> GetPatients(IEnumerable<Guid> patientKeys = null, Guid? patientSiloKey = null, Guid? patientIdTypeKey = null, string patientId = null);

        /// <summary>
        /// Gets the patient that matches the specified key.
        /// </summary>
        /// <param name="patientKey">The patient key.</param>
        /// <returns>A <see cref="Patient"/> object otherwise null if not exist.</returns>
        Patient GetPatient(Guid patientKey);

        /// <summary>
        /// Gets the patient that matches the specified key.
        /// </summary>
        /// <param name="patientSiloKey">The patient key.</param>
        /// <param name="primaryPatientIdTypeKey">The primary patient ID type.</param>
        /// <param name="patientId">The patient ID.</param>
        /// <returns>A <see cref="Patient"/> object otherwise null if not exist.</returns>
        Patient GetPatient(Guid patientSiloKey, Guid primaryPatientIdTypeKey, string patientId);
        
        /// <summary>
        /// Adds an patient.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="patient">The patient to be added</param>
        /// <returns>The newly created patient key</returns>
        Guid InsertPatient(Context context, Patient patient);

        /// <summary>
        /// Updates an existing patient.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="patient">The patient to update.</param>
        void UpdatePatient(Context context, Patient patient);
        
        #endregion

        #region PatientMerge Members

        /// <summary>
        /// Adds an patient-merge association
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="survivingPatientKey">The surrogate key of an patient that continues to be used.</param>
        /// <param name="nonSurvivingPatientKey">The surrogate key of an patient that does not continue to be used.</param>
        void InsertPatientMergeAssociation(Context context, Guid survivingPatientKey, Guid nonSurvivingPatientKey);

        /// <summary>
        /// Deletes a patient-merge association
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="survivingPatientKey">The surrogate key of an encounter that continues to be used.</param>
        /// <param name="nonSurvivingPatientKey">The surrogate key of an encounter that does not continue to be used.</param>
        void DeletePatientMergeAssociation(Context context, Guid survivingPatientKey, Guid nonSurvivingPatientKey);

        #endregion

        #region PatientIdentificationType Members

        /// <summary>
        /// Gets the patient ID type that matches the specified key.
        /// </summary>
        /// <param name="patientIdentificationTypeKey">The patient key.</param>
        /// <returns>A <see cref="PatientIdentificationType"/> object otherwise null if not exist.</returns>
        PatientIdentificationType GetPatientIdentificationType(Guid patientIdentificationTypeKey);

        /// <summary>
        /// Gets the patient ID type that matches the specified display code.
        /// </summary>
        /// <param name="displayCode">The display code.</param>
        /// <returns>A <see cref="PatientIdentificationType"/> object otherwise null if not exist.</returns>
        PatientIdentificationType GetPatientIdentificationType(string displayCode);

        /// <summary>
        /// Adds a new <see cref="PatientIdentificationType"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="patientIdentificationType">The patient ID type to be added</param>
        /// <returns>The newly created patient ID type key</returns>
        Guid InsertPatientIdentificationType(Context context, PatientIdentificationType patientIdentificationType);

        /// <summary>
        /// Updates an existing <see cref="PatientIdentificationType"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="patientIdentificationType">The patient ID type to update.</param>
        void UpdatePatientIdentificationType(Context context, PatientIdentificationType patientIdentificationType);

        #endregion

        #region AllergyType Members

        /// <summary>
        /// Gets the <see cref="AllergyType"/> that matches the specified key.
        /// </summary>
        /// <param name="allergyTypeKey">The allergy type key.</param>
        /// <returns>A <see cref="AllergyType"/> object otherwise null if not exist.</returns>
        AllergyType GetAllergyType(Guid allergyTypeKey);

        /// <summary>
        /// Gets the <see cref="AllergyType"/> that matches the specified external system key and allergy type code.
        /// </summary>
        /// <param name="externalSystemKey">The external system key.</param>
        /// <param name="allergyTypeCode"></param>
        /// <returns>A <see cref="AllergyType"/> object otherwise null if not exist.</returns>
        AllergyType GetAllergyType(Guid externalSystemKey, string allergyTypeCode);

        /// <summary>
        /// Adds a new <see cref="AllergyType"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="allergyType">The allergy type to be added</param>
        /// <returns>The newly created allergy type key.</returns>
        Guid InsertAllergyType(Context context, AllergyType allergyType);

        /// <summary>
        /// Updates an existing <see cref="AllergyType"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="allergyType">The allergy type to update.</param>
        void UpdateAllergyType(Context context, AllergyType allergyType);

        /// <summary>
        /// Logically deletes an existing <see cref="AllergyType"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="allergyTypeKey">The allergy type surrogate key.</param>
        void DeleteAllergyType(Context context, Guid allergyTypeKey);

        #endregion

        #region AllergySeverity Members

        /// <summary>
        /// Gets the <see cref="AllergySeverity"/> that matches the specified key.
        /// </summary>
        /// <param name="allergySeverityKey">The allergy severity key.</param>
        /// <returns>A <see cref="AllergySeverity"/> object otherwise null if not exist.</returns>
        AllergySeverity GetAllergySeverity(Guid allergySeverityKey);

        /// <summary>
        /// Gets the <see cref="AllergySeverity"/> that matches the specified external system and allergy severity code.
        /// </summary>
        /// <param name="externalSystemKey">The external system key.</param>
        /// <param name="allergySeverityCode"></param>
        /// <returns>A <see cref="AllergySeverity"/> object otherwise null if not exist.</returns>
        AllergySeverity GetAllergySeverity(Guid externalSystemKey, string allergySeverityCode);

        /// <summary>
        /// Adds a new <see cref="AllergySeverity"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="allergySeverity">The allergy severity to be added</param>
        /// <returns>The newly created allergy severity key.</returns>
        Guid InsertAllergySeverity(Context context, AllergySeverity allergySeverity);

        /// <summary>
        /// Updates an existing <see cref="AllergySeverity"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="allergySeverity">The allergy severity to update.</param>
        void UpdateAllergySeverity(Context context, AllergySeverity allergySeverity);

        /// <summary>
        /// Logically deletes an existing <see cref="AllergySeverity"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="allergySeverityKey">The allergy severity surrogate key.</param>
        void DeleteAllergySeverity(Context context, Guid allergySeverityKey);

        #endregion

        #region Gender Members

        /// <summary>
        /// Gets the <see cref="Gender"/> that matches the specified key.
        /// </summary>
        /// <param name="genderKey">The gender key.</param>
        /// <returns>A <see cref="Gender"/> object otherwise null if not exist.</returns>
        Gender GetGender(Guid genderKey);

        /// <summary>
        /// Gets the gender that matches the specified display code.
        /// </summary>
        /// <param name="displayCode">The display code.</param>
        /// <returns>A <see cref="Gender"/> object otherwise null if not exist.</returns>
        Gender GetGender(string displayCode);

        /// <summary>
        /// Adds a new <see cref="Gender"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="gender">The gender to be added</param>
        /// <returns>The newly gender type key.</returns>
        Guid InsertGender(Context context, Gender gender);

        /// <summary>
        /// Updates an existing <see cref="Gender"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="gender">The gender to update.</param>
        void UpdateGender(Context context, Gender gender);

        #endregion

        #region Encounter Members

        /// <summary>
        /// Retrieves a collection of <see cref="Encounter"/>.
        /// </summary>
        /// <param name="encounterKeys"></param>
        /// <param name="encounterId"></param>
        /// <param name="patientKey"></param>
        /// <param name="patientSiloKey"></param>
        /// <param name="patientIdTypeKey"></param>
        /// <param name="patientId"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="Encounter"/>.</returns>
        IEnumerable<Encounter> GetEncounters(IEnumerable<Guid> encounterKeys = null, string encounterId = null,
                                             Guid? patientKey = null, Guid? patientSiloKey = null,
                                             Guid? patientIdTypeKey = null, string patientId = null);

        /// <summary>
        /// Gets all the encounters that match the specified silo and encounter id
        /// </summary>
        /// <param name="patientSiloKey">The silo key</param>
        /// <param name="encounterId">The visit number (encounter id)</param>
        /// <returns>An enumerable collection of <see cref="Encounter"/> object otherwise null if not exist.</returns>
        IEnumerable<Encounter> GetEncounters(Guid patientSiloKey, string encounterId);

        /// <summary>
        /// Gets the encounter that matches the specified key.
        /// </summary>
        /// <param name="encounterKey">The encounter key.</param>
        /// <returns>A <see cref="Encounter"/> object otherwise null if not exist.</returns>
        Encounter GetEncounter(Guid encounterKey);

        /// <summary>
        /// Gets the encounter that matches the specified patient key and encounter ID.
        /// </summary>
        /// <param name="patientKey">The patient key.</param>
        /// <param name="encounterId">The encounter key.</param>
        /// <returns>A <see cref="Encounter"/> object otherwise null if not exist.</returns>
        Encounter GetEncounter(Guid patientKey, string encounterId);

        /// <summary>
        /// Gets the encounter that matches the specified patient id, visit number (encounterid),
        /// primary patient id type key, and patient silo key
        /// </summary>
        /// <param name="patientId">The patient id</param>
        /// <param name="encounterId">The visit number</param>
        /// <param name="primaryPatientIdTypeKey">Primary patient id type key</param>
        /// <param name="patientSiloKey">Patient Silo key</param>
        /// <returns>A <see cref="Encounter"/> object otherwise null if not exist.</returns>
        Encounter GetEncounter(string patientId, string encounterId, Guid primaryPatientIdTypeKey, Guid patientSiloKey);

        /// <summary>
        /// Checks if an encounter exists within the specified patient silo key and encounter ID, ignoring
        /// the specified patient key.
        /// </summary>
        /// <param name="patientSiloKey"></param>
        /// <param name="encounterId"></param>
        /// <param name="ignorePatientKey"></param>
        /// <returns></returns>
        bool EncounterExists(Guid patientSiloKey, string encounterId, Guid ignorePatientKey);

        /// <summary>
        /// Adds an encounter.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encounter">The encounter to be added</param>
        /// <returns>The newly created encounter key</returns>
        Guid InsertEncounter(Context context, Encounter encounter);

        /// <summary>
        /// Updates an existing encounter.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="encounter">The encounter to update.</param>
        void UpdateEncounter(Context context, Encounter encounter);

        /// <summary>
        /// Updates an encounters last activity date/time
        /// </summary>
        void UpdateEncounterActivity(Context context, Guid encounterKey, DateTime activityUtcDateTime, DateTime activityLocalDateTime);

        /// <summary>
        /// Updates an encounters last activity date/time
        /// </summary>
        void UpdateEncounterActivity(Guid? deviceKey, Guid encounterKey, DateTime activityUtcDateTime, DateTime activityLocalDateTime);

        #endregion

        #region EncounterMerge Members

        /// <summary>
        /// Adds an encounter-merge association
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="survivingEncounterKey">The surrogate key of an encounter that continues to be used.</param>
        /// <param name="nonSurvivingEncounterKey">The surrogate key of an encounter that does not continue to be used.</param>
        void InsertEncounterMergeAssociation(Context context, Guid survivingEncounterKey, Guid nonSurvivingEncounterKey);

        /// <summary>
        /// Deletes a encounter-merge association
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="survivingEncounterKey">The surrogate key of an encounter that continues to be used.</param>
        /// <param name="nonSurvivingEncounterKey">The surrogate key of an encounter that does not continue to be used.</param>
        void DeleteEncounterMergeAssociation(Context context, Guid survivingEncounterKey, Guid nonSurvivingEncounterKey);

        #endregion

        #region HospitalService Members

        /// <summary>
        /// Gets the <see cref="HospitalService"/> that matches the specified key.
        /// </summary>
        /// <param name="hospitalServiceKey">The hospital service key.</param>
        /// <returns>A <see cref="HospitalService"/> object otherwise null if not exist.</returns>
        HospitalService GetHospitalService(Guid hospitalServiceKey);

        /// <summary>
        /// Gets the <see cref="HospitalService"/> that matches the specified external system key and hospital
        /// service code.
        /// </summary>
        /// <param name="externalSystemKey"></param>
        /// <param name="hospitalServiceCode"></param>
        /// <returns>A <see cref="HospitalService"/> object otherwise null if not exist.</returns>
        HospitalService GetHospitalService(Guid externalSystemKey, string hospitalServiceCode);

        /// <summary>
        /// Adds a new <see cref="HospitalService"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hospitalService">The hospital service to be added</param>
        /// <returns>The newly created hospital service key.</returns>
        Guid InsertHospitalService(Context context, HospitalService hospitalService);

        /// <summary>
        /// Updates an existing <see cref="HospitalService"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="hospitalService">The hospital service to update.</param>
        void UpdateHospitalService(Context context, HospitalService hospitalService);

        /// <summary>
        /// Logically deletes an existing <see cref="HospitalService"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="hospitalServiceKey">The hospital service surrogate key.</param>
        void DeleteHospitalService(Context context, Guid hospitalServiceKey);

        #endregion

        #region PatientClass Members

        /// <summary>
        /// Gets the <see cref="PatientClass"/> that matches the specified key.
        /// </summary>
        /// <param name="patientClassKey">The patient class key.</param>
        /// <returns>A <see cref="PatientClass"/> object otherwise null if not exist.</returns>
        PatientClass GetPatientClass(Guid patientClassKey);

        /// <summary>
        /// Gets the <see cref="PatientClass"/> that matches the specified external system key and 
        /// patient class code.
        /// </summary>
        /// <param name="externalSystemKey">The external system key.</param>
        /// <param name="patientClassCode">The patient class code.</param>
        /// <returns>A <see cref="PatientClass"/> object otherwise null if not exist.</returns>
        PatientClass GetPatientClass(Guid externalSystemKey, string patientClassCode);

        /// <summary>
        /// Adds a new <see cref="PatientClass"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="patientClass">The patient class to be added</param>
        /// <returns>The newly created patient class key.</returns>
        Guid InsertPatientClass(Context context, PatientClass patientClass);

        /// <summary>
        /// Updates an existing <see cref="PatientClass"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="patientClass">The patient class to update.</param>
        void UpdatePatientClass(Context context, PatientClass patientClass);

        /// <summary>
        /// Logically deletes an existing <see cref="PatientClass"/>.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="patientClassKey">The patient class surrogate key.</param>
        void DeletePatientClass(Context context, Guid patientClassKey);

        #endregion

        #region User Encounter Association Members

        /// <summary>
        /// Adds a user-encounter association
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userAccountKey">The user account key.</param>
        /// <param name="encounterKey">The encounter key.</param>
        /// <param name="lastAddDateTime"></param>
        /// <param name="lastAddUtcDateTime"></param>
        void InsertUserEncounterAssociation(Context context, Guid userAccountKey, Guid encounterKey, DateTime? lastAddDateTime = null, DateTime? lastAddUtcDateTime = null);

        /// <summary>
        /// Deletes a user-encounter association
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userAccountKey">The user account key.</param>
        /// <param name="encounterKey">The encounter key.</param>
        void DeleteUserEncounterAssociation(Context context, Guid userAccountKey, Guid encounterKey);
        
        #endregion
    }
}
