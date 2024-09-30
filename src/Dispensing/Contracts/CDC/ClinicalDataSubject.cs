using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of a clinical data subject for a given
    /// period of time.
    /// </summary>
    [Serializable]
    public class ClinicalDataSubject : Entity<Guid>
    {
        #region Constructors

        public ClinicalDataSubject()
        {

        }

        public ClinicalDataSubject(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ClinicalDataSubject(Guid key)
        {
            return FromKey(key);
        }

        public static ClinicalDataSubject FromKey(Guid key)
        {
            return new ClinicalDataSubject(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the clinical data category.
        /// </summary>
        [NotNullValidator]
        public ClinicalDataCategory Category { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a clinical data subject type.
        /// </summary>
        [NotNullValidator]
        public ClinicalDataSubjectType SubjectType { get; set; }

        /// <summary>
        /// Gets or sets the text that contains the title of a subject.
        /// </summary>
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
           MessageTemplateResourceName = "ClinicalDataSubjectTitleRequired")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the text that is displayed during remove workflows.
        /// </summary>
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
           MessageTemplateResourceName = "ClinicalDataSubjectDescriptionRequired")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a clinical data subject should be displayed once.
        /// </summary>
        public bool DisplayOnce { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a clinical data subject applies to stat kits such that
        /// a clinician is required to enter clinical data after dispensing via a stat kit..
        /// </summary>
        public bool StatKit { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a clinical data subject is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the responses associated with a clinical data subject.
        /// </summary>
        public ClinicalDataResponse[] Responses { get; set; }

        /// <summary>
        /// Gets or sets the user types associated with a clinical data subject.
        /// </summary>
        public Guid[] UserTypes { get; set; }

        #endregion
    }
}
