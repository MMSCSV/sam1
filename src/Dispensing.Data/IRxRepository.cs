using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Contracts.LocationManagement;

namespace CareFusion.Dispensing.Data
{
    public interface IRxRepository : IRepository
    {
        #region ControlledSubstanceLicense

        IEnumerable<ControlledSubstanceLicense> GetControlledSubstanceLicenses(IEnumerable<Guid> controlledSubstanceLicensesKeys, bool? deleted, Guid? facilityKey = null);

        ControlledSubstanceLicense GetControlledSubstanceLicense(Guid controlledSubstanceLicenseKey);

        Guid InsertControlledSubstanceLicense(Context context, ControlledSubstanceLicense controlledSubstanceLicense);

        void UpdateControlledSubstanceLicense(Context context, ControlledSubstanceLicense controlledSubstanceLicense);

        void DeleteControlledSubstanceLicense(Context context, ControlledSubstanceLicense controlledSubstanceLicense);

        /// <summary>
        ///  Checks if there is an existing Controlled Substance License entity.
        /// </summary>
        /// <param name="controlledSubstanceLicenseKey">Controlled Substance License Key to check.</param>
        /// <param name="controlledSubstanceLicenseID">Controlled Substance License ID to check.</param>
        /// <returns>True if Controlled Substance License already exists, false otherwise.</returns>
        bool ControlledSubstanceLicenseAlreadyExists(Guid controlledSubstanceLicenseKey, string controlledSubstanceLicenseID);

        #endregion

        #region InvoiceType Members

        /// <summary>
        /// Retrieves a collection of <see cref="InvoiceType"/>.
        /// </summary>
        /// <param name="invoiceTypeKeys">The collection of invoice Type keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="facilityKey"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="InvoiceType"/>.</returns>
        IEnumerable<InvoiceType> GetInvoiceTypes(IEnumerable<Guid> invoiceTypeKeys = null, bool? deleted = null, Guid? facilityKey = null);

        /// <summary>
        /// Gets the <see cref="InvoiceType"/> that matches the specified key.
        /// </summary>
        /// <param name="invoiceTypeKey">The invoice Type key.</param>
        /// <returns>A <see cref="InvoiceType"/> object.</returns>
        InvoiceType GetInvoiceType(Guid invoiceTypeKey);

        /// <summary>
        /// Persists the new <see cref="InvoiceType"/> to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="invoiceType">The <see cref="InvoiceType"/> to persist.</param>
        /// <returns>A <see cref="InvoiceType"/> key, which uniquely identifies the <see cref="InvoiceType"/> in the database.</returns>
        Guid InsertInvoiceType(Context context, InvoiceType invoiceType);

        /// <summary>
        /// Updates an existing <see cref="InvoiceType"/> in the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="invoiceType">The <see cref="InvoiceType"/> to update.</param>
        void UpdateInvoiceType(Context context, InvoiceType invoiceType);

        /// <summary>
        /// Logically deleted an existing <see cref="InvoiceType"/> in the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="invoiceTypeKey">The <see cref="InvoiceType"/> key.</param>
        void DeleteInvoiceType(Context context, IReadOnlyCollection<Guid> invoiceTypeKey);

        /// <summary>
        ///  Checks if there is an existing invoice Type in current facility.
        /// </summary>
        /// <param name="invoiceType">The <see cref="InvoiceType"/> to check.</param>
        /// <returns>True if <see cref="InvoiceType"/> already exists, false otherwise.</returns>
        bool InvoiceTypeAlreadyExists(InvoiceType invoiceType);

        #endregion

        #region MedClassGroup Members
        /// <summary>
        /// Retrieves a collection of MedClass Groups
        /// </summary>
        /// <param name="medClassGroupKeys">The collection of MedClass Group Keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="facilityKey"></param>
        /// <returns></returns>
        IEnumerable<MedClassGroup> GetMedClassGroups(IEnumerable<Guid> medClassGroupKeys = null, bool? deleted = null, Guid? facilityKey = null);

        /// <summary>
        /// Gets a <see cref="MedClassGroup"/> by its surrogate key
        /// </summary>
        /// <param name="MedClassGroupKey">The surrogate key of <see cref="MedClassGroup"/></param>
        /// <returns></returns>
        MedClassGroup GetMedClassGroup(Guid MedClassGroupKey);

        /// <summary>
        /// Gets a <see cref="MedClassGroup"/> by its surrogate key
        /// Gets a <see cref="MedClassGroup"/> by its ExternalSystem key
        /// </summary>
        /// <param name="medClassGroupKey">The surrogate key of <see cref="MedClassGroup"/></param>
        /// <param name="medClassExternalSystemKey">The surrogate key of <see cref="MedClassGroup"/></param>
        /// <returns></returns>
        MedClassGroup GetMedClassGroup(Guid medClassGroupKey, Guid medClassExternalSystemKey);


        /// <summary>
        /// Inserts a <see cref="MedClassGroup"/> into the database
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="medClassGroup">The Med Class Group</param>
        /// <returns>A MedClassGroup Key which uniquely identifies the med class group in the database</returns>
        Guid InsertMedClassGroup(Context context, MedClassGroup medClassGroup);

        /// <summary>
        /// Updates an existing <see cref="MedClassGroup"/>
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="medClassGroup">The Med Class Group</param>
        /// <param name="medClassKeys">The collection of Med Class Keys</param>
        /// <param name="medClassExternalSystemKey">The Med Class ExternalSystem key</param>
        void UpdateMedClassGroup(Context context, MedClassGroup medClassGroup, IReadOnlyCollection<Guid> medClassKeys, Guid medClassExternalSystemKey);

        void DeleteMedClassGroups(Context context, IReadOnlyCollection<Guid> medClassGroupKeys);
        #endregion
    }
}
