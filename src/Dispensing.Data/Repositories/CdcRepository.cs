using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CareFusion.Dispensing.Contracts;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.CDCat;

namespace CareFusion.Dispensing.Data.Repositories
{
    internal class CdcRepository : LinqBaseRepository, ICdcRepository
    {
        private readonly ClinicalDataSubjectRepository _clinicalDataSubjectRepo;
        private readonly ClinicalDataCategoryRepository _clinicalDataCategoryRepo;
        private readonly ClinicalDataResponseRepository _clinicalDataResponseRepo;

        public CdcRepository()
        {
            _clinicalDataSubjectRepo = new ClinicalDataSubjectRepository();
            _clinicalDataCategoryRepo = new ClinicalDataCategoryRepository();
            _clinicalDataResponseRepo = new ClinicalDataResponseRepository();
        }

        #region Implementation of ICdcRepository
        ClinicalDataSubject ICdcRepository.GetClinicalDataSubject(Guid clinicalDataSubjectKey)
        {
            ClinicalDataSubject clinicalDataSubject = null;

            try
            {
                var query = CreateGetClinicalDataSubjectQuery();
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var multiReader = connectionScope.QueryMultiple(
                    query.ToString(),
                    new
                    {
                        ClinicalDataSubjectKey = clinicalDataSubjectKey
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.Text);

                    var subjectResult = multiReader.Read<Pyxis.Core.Data.Schema.CDCat.Models.ClinicalDataSubject>().FirstOrDefault();
                    var categoryResul = multiReader.Read<ClinicalDataCategory>();
                    var subjectTypeResult = multiReader.Read<ClinicalDataSubjectType>();
                    var clinicalDataResponseResult = multiReader.Read<Pyxis.Core.Data.Schema.CDCat.Models.ClinicalDataResponse>();
                    var clinicalDataSubjectUserTypeResult = multiReader.Read<Guid>();

                    if(subjectResult != null)
                    {
                        clinicalDataSubject = new ClinicalDataSubject()
                        {
                            Key = subjectResult.ClinicalDataSubjectKey,
                            Description = subjectResult.DescriptionText,
                            DisplayOnce = subjectResult.DisplayOnceFlag,
                            StatKit = subjectResult.StatKitFlag,
                            IsActive = subjectResult.ActiveFlag,
                            Title = subjectResult.TitleText,
                            LastModified = subjectResult.LastModifiedBinaryValue
                        };

                        clinicalDataSubject.Category = categoryResul?.Where(x => x.Key == subjectResult.ClinicalDataCategoryKey).FirstOrDefault();
                        clinicalDataSubject.SubjectType = subjectTypeResult?.Where(x =>
                        x.InternalCode == subjectResult.ClinicalDataSubjectTypeInternalCode.FromNullableInternalCode<ClinicalDataSubjectTypeInternalCode>()).FirstOrDefault();

                        if(clinicalDataResponseResult != null)
                        {
                            var responses = new List<ClinicalDataResponse>();
                            foreach (var model in clinicalDataResponseResult)
                            {
                                var clinicalDataResponse = new ClinicalDataResponse()
                                {
                                    Key = model.ClinicalDataResponseKey,
                                    Assent = model.ClinicalDataAssentInternalCode.FromNullableInternalCode<ClinicalDataAssentInternalCode>(),
                                    Response = model.ResponseText,
                                    Instruction = model.InstructionText,
                                    LastModified = model.LastModifiedBinaryValue,
                                };
                                responses.Add(clinicalDataResponse);
                            }
                            clinicalDataSubject.Responses = responses.ToArray();
                        }

                        clinicalDataSubject.UserTypes = clinicalDataSubjectUserTypeResult.ToArray();
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return clinicalDataSubject;
        }

        Guid ICdcRepository.InsertClinicalDataSubject(Context context, ClinicalDataSubject clinicalDataSubject)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataSubject, "clinicalDataSubject");
            Guid? clinicalDataSubjectKey = null;

            try
            {
                var model = new Pyxis.Core.Data.Schema.CDCat.Models.ClinicalDataSubject()
                {
                    ClinicalDataCategoryKey = clinicalDataSubject.Category != null ? clinicalDataSubject.Category.Key : default(Guid),
                    ClinicalDataSubjectTypeInternalCode = clinicalDataSubject.SubjectType != null ? clinicalDataSubject.SubjectType.InternalCode.ToInternalCode() : default(string),
                    DisplayOnceFlag = clinicalDataSubject.DisplayOnce,
                    TitleText = clinicalDataSubject.Title,
                    DescriptionText = clinicalDataSubject.Description,
                    StatKitFlag = clinicalDataSubject.StatKit,
                    ActiveFlag = clinicalDataSubject.IsActive,
                };

                using (var tx = TransactionScopeFactory.Create())
                {
                    clinicalDataSubjectKey = _clinicalDataSubjectRepo.InsertClinicalDataSubject(context.ToActionContext(), model);

                    if (clinicalDataSubjectKey.HasValue)
                    {
                        if (clinicalDataSubject.Responses != null)
                        {
                            InsertClinicalDataResponses(
                                context,
                                clinicalDataSubjectKey.Value,
                                clinicalDataSubject.Responses);
                        }

                        if (clinicalDataSubject.UserTypes != null)
                        {
                            InsertClinicalDataUserTypes(
                                context,
                                clinicalDataSubjectKey.Value,
                                clinicalDataSubject.UserTypes);
                        }
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return clinicalDataSubjectKey.GetValueOrDefault();
        }

        void ICdcRepository.UpdateClinicalDataSubject(Context context, ClinicalDataSubject clinicalDataSubject)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataSubject, "clinicalDataSubject");

            try
            {
                var model = new Pyxis.Core.Data.Schema.CDCat.Models.ClinicalDataSubject()
                {
                    ClinicalDataCategoryKey = clinicalDataSubject.Category != null ? clinicalDataSubject.Category.Key : default(Guid),
                    ClinicalDataSubjectTypeInternalCode = clinicalDataSubject.SubjectType != null ? clinicalDataSubject.SubjectType.InternalCode.ToInternalCode() : default(string),
                    DisplayOnceFlag = clinicalDataSubject.DisplayOnce,
                    TitleText = clinicalDataSubject.Title,
                    DescriptionText = clinicalDataSubject.Description,
                    StatKitFlag = clinicalDataSubject.StatKit,
                    ActiveFlag = clinicalDataSubject.IsActive,
                    LastModifiedBinaryValue = clinicalDataSubject.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    ClinicalDataSubjectKey = clinicalDataSubject.Key,
                };

                using (var tx = TransactionScopeFactory.Create())
                {
                    _clinicalDataSubjectRepo.UpdateClinicalDataSubject(context.ToActionContext(), model);

                    UpdateClinicalDataResponses(
                        context,
                        clinicalDataSubject.Key,
                        clinicalDataSubject.Responses ?? new ClinicalDataResponse[0]);

                    UpdateClinicalDataUserTypes(
                        context,
                        clinicalDataSubject.Key,
                        clinicalDataSubject.UserTypes ?? new Guid[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICdcRepository.DeleteClinicalDataSubject(Context context, Guid clinicalDataSubjectKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _clinicalDataSubjectRepo.DeleteClinicalDataSubject(context.ToActionContext(), clinicalDataSubjectKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        ClinicalDataCategory ICdcRepository.GetClinicalDataCategory(Guid clinicalDataCategoryKey)
        {
            ClinicalDataCategory clinicalDataCategory = null;

            try
            {
                var model = _clinicalDataCategoryRepo.GetClinicalDataCategory(clinicalDataCategoryKey);
                if(model != null)
                {
                    clinicalDataCategory = new ClinicalDataCategory()
                    {
                        Key = model.ClinicalDataCategoryKey,
                        Description = model.DescriptionText,
                        LastModified = model.LastModifiedBinaryValue,
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return clinicalDataCategory;
        }

        Guid ICdcRepository.InsertClinicalDataCategory(Context context, ClinicalDataCategory clinicalDataCategory)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataCategory, "clinicalDataCategory");
            Guid? clinicalDataCategoryKey = null;

            try
            {
                var model = new Pyxis.Core.Data.Schema.CDCat.Models.ClinicalDataCategory()
                {
                    DescriptionText = clinicalDataCategory.Description,
                };

                clinicalDataCategoryKey = _clinicalDataCategoryRepo.InsertClinicalDataCategory(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return clinicalDataCategoryKey.GetValueOrDefault();
        }

        void ICdcRepository.UpdateClinicalDataCategory(Context context, ClinicalDataCategory clinicalDataCategory)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataCategory, "clinicalDataCategory");

            try
            {
                var model = new Pyxis.Core.Data.Schema.CDCat.Models.ClinicalDataCategory()
                {
                    DescriptionText = clinicalDataCategory.Description,
                    LastModifiedBinaryValue = clinicalDataCategory.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    ClinicalDataCategoryKey = clinicalDataCategory.Key
                };

                _clinicalDataCategoryRepo.UpdateClinicalDataCategory(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICdcRepository.DeleteClinicalDataCategory(Context context, Guid clinicalDataCategoryKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _clinicalDataCategoryRepo.DeleteClinicalDataCategory(context.ToActionContext(), clinicalDataCategoryKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Private Members

        private void InsertClinicalDataResponses(Context context, Guid clinicalDataSubjectKey, IEnumerable<ClinicalDataResponse> clinicalDataResponses)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataResponses, "clinicalDataResponses");

            foreach (ClinicalDataResponse clinicalDataResponse in clinicalDataResponses)
            {
                var model = new Pyxis.Core.Data.Schema.CDCat.Models.ClinicalDataResponse()
                {
                    ClinicalDataSubjectKey = clinicalDataSubjectKey,
                    ClinicalDataAssentInternalCode = clinicalDataResponse.Assent != null ? clinicalDataResponse.Assent.ToInternalCode() : default(string),
                    ResponseText = clinicalDataResponse.Response,
                    InstructionText = clinicalDataResponse.Instruction,
                };
                _clinicalDataResponseRepo.InsertClinicalDataResponse(context.ToActionContext(), model);
            }
        }

        private void UpdateClinicalDataResponses(Context context, Guid clinicalDataSubjectKey, IEnumerable<ClinicalDataResponse> clinicalDataResponses)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataResponses, "clinicalDataResponses");

            // Get the list of clinical data responses entities associated with this clincial data subject.
            IEnumerable<Guid> currentClinicalDataResponseKeys;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("cdr.ClinicalDataResponseKey")
                    .FROM("CDCat.vw_ClinicalDataResponseCurrent cdr")
                    .WHERE("cdr.ClinicalDataSubjectKey = @ClinicalDataSubjectKey");

                currentClinicalDataResponseKeys = connectionScope.Query<Guid>(
                query.ToString(),
                new
                {
                    ClinicalDataSubjectKey = clinicalDataSubjectKey
                },
                commandTimeout: connectionScope.DefaultCommandTimeout,
                commandType: CommandType.Text).ToList();
            }

            // Find the clinical data subjects that were removed.
            var removedClinicalDataResponseKeys = currentClinicalDataResponseKeys.Except(
            clinicalDataResponses.Where(cdr => !cdr.IsTransient())
                .Select(cdr => cdr.Key));

            using (ITransactionScope tx = TransactionScopeFactory.Create())
            {
                // Remove clinical data responses that are no longer associated with this clinical data subject.
                foreach (var clinicalDataResponseKey in removedClinicalDataResponseKeys)
                {
                    _clinicalDataResponseRepo.DeleteClinicalDataResponse(context.ToActionContext(), clinicalDataResponseKey);
                }

                // Find the clinical data responses that were added or updated.
                List<ClinicalDataResponse> addedClinicalDataResponses = new List<ClinicalDataResponse>();
                foreach (ClinicalDataResponse clinicalDataResponse in clinicalDataResponses)
                {
                    if (clinicalDataResponse.IsTransient())
                    {
                        addedClinicalDataResponses.Add(clinicalDataResponse);
                        continue;
                    }

                    var model = new Pyxis.Core.Data.Schema.CDCat.Models.ClinicalDataResponse()
                    {
                        ClinicalDataSubjectKey = clinicalDataSubjectKey,
                        ClinicalDataAssentInternalCode = clinicalDataResponse.Assent != null ? clinicalDataResponse.Assent.ToInternalCode() : default(string),
                        ResponseText = clinicalDataResponse.Response,
                        InstructionText = clinicalDataResponse.Instruction,
                        LastModifiedBinaryValue = clinicalDataResponse.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                        ClinicalDataResponseKey = clinicalDataResponse.Key
                    };

                    _clinicalDataResponseRepo.UpdateClinicalDataResponse(context.ToActionContext(), model);
                }

                // Add the new clinical data responses.
                InsertClinicalDataResponses(
                    context,
                    clinicalDataSubjectKey,
                    addedClinicalDataResponses);

                tx.Complete();
            }
        }

        private  void InsertClinicalDataUserTypes(Context context, Guid clinicalDataSubjectKey, IEnumerable<Guid> userTypes)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userTypes, "userTypes");

            foreach (Guid userTypeKey in userTypes)
            {
                _clinicalDataSubjectRepo.AssociateClinicalDataSubjectUserType(context.ToActionContext(), clinicalDataSubjectKey, userTypeKey);  
            }
        }

        private void UpdateClinicalDataUserTypes(Context context, Guid clinicalDataSubjectKey, IEnumerable<Guid> userTypes)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userTypes, "userTypes");

            // Get the list of user types associated with this clincial data subject.
            IEnumerable<Guid> currentUserTypeKeys;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("cdsut.UserTypeKey")
                    .FROM("CDCat.vw_ClinicalDataSubjectUserTypeCurrent cdsut")
                    .WHERE("cdsut.ClinicalDataSubjectKey = @ClinicalDataSubjectKey");

                currentUserTypeKeys = connectionScope.Query<Guid>(
                query.ToString(),
                new
                {
                    ClinicalDataSubjectKey = clinicalDataSubjectKey
                },
                commandTimeout: connectionScope.DefaultCommandTimeout,
                commandType: CommandType.Text).ToList();
            }

            // Find the user types that were removed.
            var removedUserTypeKeys = currentUserTypeKeys.Except(userTypes);

            // Find the user types that were added
            var addedUserTypeKeys = userTypes.Except(currentUserTypeKeys);

            using (ITransactionScope tx = TransactionScopeFactory.Create())
            {
                // Remove user types that are no longer associated with this clinical data subject.
                foreach (var userTypeKey in removedUserTypeKeys)
                {
                    _clinicalDataSubjectRepo.DisassociateClinicalDataSubjectUserType(context.ToActionContext(), clinicalDataSubjectKey, userTypeKey);
                }

                // Add the new user types.
                InsertClinicalDataUserTypes(
                    context,
                    clinicalDataSubjectKey,
                    addedUserTypeKeys);

                tx.Complete();
            }
        }

        private string CreateGetClinicalDataSubjectQuery()
        {
            var clinicalDataSubjectQuery = new SqlBuilder();
            clinicalDataSubjectQuery.SELECT("*")
                .FROM("CDCat.ClinicalDataSubjectSnapshot cdss")
                .WHERE("cdss.EndUTCDateTime IS NULL")
                .WHERE("cdss.DeleteFlag = 0")
                .WHERE("cdss.ClinicalDataSubjectKey = @ClinicalDataSubjectKey");

            var clinicalDataCategoryQuery = new SqlBuilder();
            clinicalDataCategoryQuery.SELECT("cdc.ClinicalDataCategoryKey AS [Key]")
                ._("cdc.DescriptionText AS Description")
                ._("cdc.LastModifiedBinaryValue AS LastModified")
                .FROM("CDCat.ClinicalDataCategory cdc")
                .WHERE("cdc.DeleteFlag = 0");

            var clinicalSubjectTypeQuery = new SqlBuilder();
            clinicalSubjectTypeQuery.SELECT("cdst.ClinicalDataSubjectTypeInternalCode")
                ._("cdst.DescriptionText AS Description")
                ._("cdst.SortValue")
                .FROM("CDCat.ClinicalDataSubjectType cdst");

            var clinicalDataResponsesQuery = new SqlBuilder();
            clinicalDataResponsesQuery.SELECT("cdrs.*")
                .FROM("CDCat.ClinicalDataResponseSnapshot cdrs")
                .JOIN("CDCat.ClinicalDataSubjectSnapshot cdss ON cdrs.ClinicalDataSubjectKey = cdss.ClinicalDataSubjectKey")
                .WHERE("cdss.ClinicalDataSubjectKey = @ClinicalDataSubjectKey")
                ._("cdrs.EndUTCDateTime IS NULL")
                ._("cdrs.DeleteFlag = 0")
                ._("cdss.EndUTCDateTime IS NULL")
                ._("cdss.DeleteFlag = 0");

            var clinicalUserTypesQuery = new SqlBuilder();
            clinicalUserTypesQuery.SELECT("cdsut.UserTypeKey")
                .FROM("CDCat.ClinicalDataSubjectUserType cdsut")
                .WHERE("cdsut.ClinicalDataSubjectKey = @ClinicalDataSubjectKey")
                ._("cdsut.DisassociationUTCDateTime IS NULL");

            var query = new StringBuilder();
            query.AppendLine(clinicalDataSubjectQuery.ToString());
            query.AppendLine(clinicalDataCategoryQuery.ToString());
            query.AppendLine(clinicalSubjectTypeQuery.ToString());
            query.AppendLine(clinicalDataResponsesQuery.ToString());
            query.AppendLine(clinicalUserTypesQuery.ToString());

            return query.ToString();
        }

        #endregion
    }
}
