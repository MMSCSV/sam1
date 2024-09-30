using System;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data
{
    public interface ICdcRepository : IRepository
    {
        /// <summary>
        /// Gets a <see cref="ClinicalDataSubject"/> by its surrogate key.
        /// </summary>
        /// <param name="clinicalDataSubjectKey">The surrogate key of the <see cref="ClinicalDataSubject"/>.</param>
        /// <returns>The <see cref="ClinicalDataSubject"/> if found, otherwise null.</returns>
        ClinicalDataSubject GetClinicalDataSubject(Guid clinicalDataSubjectKey);

        /// <summary>
        /// Inserts an <see cref="ClinicalDataSubject"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clinicalDataSubject">The clinical data subject.</param>
        /// <returns>A clinical data subject key, which uniquely identifies the clinical data subject in the database.</returns>
        Guid InsertClinicalDataSubject(Context context, ClinicalDataSubject clinicalDataSubject);

        /// <summary>
        /// Updates an existing <see cref="ClinicalDataSubject"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clinicalDataSubject">The clinical data subject.</param>
        void UpdateClinicalDataSubject(Context context, ClinicalDataSubject clinicalDataSubject);

        /// <summary>
        /// Logically deletes an existing <see cref="ClinicalDataSubject"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clinicalDataSubjectKey">The clinical data subject key.</param>
        void DeleteClinicalDataSubject(Context context, Guid clinicalDataSubjectKey);

        /// <summary>
        /// Gets a <see cref="ClinicalDataCategory"/> by its surrogate key.
        /// </summary>
        /// <param name="clinicalDataCategoryKey">The surrogate key of the <see cref="ClinicalDataCategory"/>.</param>
        /// <returns>The <see cref="ClinicalDataCategory"/> if found, otherwise null.</returns>
        ClinicalDataCategory GetClinicalDataCategory(Guid clinicalDataCategoryKey);

        /// <summary>
        /// Inserts an <see cref="ClinicalDataCategory"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clinicalDataCategory">The clinical data subject.</param>
        /// <returns>A clinical data category key, which uniquely identifies the clinical data category in the database.</returns>
        Guid InsertClinicalDataCategory(Context context, ClinicalDataCategory clinicalDataCategory);

        /// <summary>
        /// Updates an existing <see cref="ClinicalDataCategory"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clinicalDataCategory">The clinical data category.</param>
        void UpdateClinicalDataCategory(Context context, ClinicalDataCategory clinicalDataCategory);

        /// <summary>
        /// Logically deletes an existing <see cref="ClinicalDataCategory"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="clinicalDataCategoryKey">The clinical data category key.</param>
        void DeleteClinicalDataCategory(Context context, Guid clinicalDataCategoryKey);
    }
}
