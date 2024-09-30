using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.Ext;
using Pyxis.Core.Data.Schema.TableTypes;

namespace CareFusion.Dispensing.Data.Repositories
{
    internal class ExternalRepository : LinqBaseRepository, IExternalRepository
    {
        private readonly ExternalPatientIdentificationTypeRepository _externalPatientIdTypeRepository;
        private readonly ExternalGenderRepository _externalGenderRepository;
        private readonly ExternalUOMRepository _externalUOMRepository;

        public ExternalRepository()
        {
            _externalPatientIdTypeRepository = new ExternalPatientIdentificationTypeRepository();
            _externalGenderRepository = new ExternalGenderRepository();
            _externalUOMRepository = new ExternalUOMRepository();
        }

        #region IExternalRepository Members

        ExternalPatientIdentificationType IExternalRepository.GetExternalPatientIdentificationType(Guid externalPatientIdentificationTypeKey)
        {
            ExternalPatientIdentificationType externalPatientIdentificationType = null;
            var query = new SqlBuilder();
            query.SELECT("epidt.*")
                ._("pidt.DisplayCode AS PatientIDTypeDisplayCode")
                ._("pidt.InternalCode AS PatientIDTypeInternalCode")
                ._("pidt.DescriptionText AS PatientIDTypeDescriptionText")
                ._("pidt.SortValue AS PatientIDTypeSortValue")
                ._("pidt.ActiveFlag AS PatientIDTypeActiveFlag")
                ._("pidt.LastModifiedBinaryValue AS PatientIDTypeLastModifiedBinaryValue")
                .FROM("Ext.ExternalPatientIDType epidt")
                .LEFT_JOIN("ADT.PatientIDType pidt ON epidt.PatientIDTypeKey = pidt.PatientIDTypeKey")
                .WHERE("epidt.DeleteFlag = 0")
                ._("epidt.ExternalPatientIDTypeKey = @ExternalPatientIDTypeKey");

            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var result = connectionScope.Query<ExternalPatientIdentificationTypeResults>(query.ToString(), new
                {
                    ExternalPatientIDTypeKey = externalPatientIdentificationTypeKey,
                }, commandTimeout: connectionScope.DefaultCommandTimeout, commandType: CommandType.Text).FirstOrDefault();

                if(result != null)
                {
                    externalPatientIdentificationType = CreateExternalPatientIdentificationType(result);
                }
            }

            return externalPatientIdentificationType;
        }

        ExternalPatientIdentificationType IExternalRepository.GetExternalPatientIdentificationType(Guid externalSystemKey, string patientIdTypeCode)
        {
            ExternalPatientIdentificationType externalPatientIdentificationType = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("epidt.*")
                    ._("pidt.DisplayCode AS PatientIDTypeDisplayCode")
                    ._("pidt.InternalCode AS PatientIDTypeInternalCode")
                    ._("pidt.DescriptionText AS PatientIDTypeDescriptionText")
                    ._("pidt.SortValue AS PatientIDTypeSortValue")
                    ._("pidt.ActiveFlag AS PatientIDTypeActiveFlag")
                    ._("pidt.LastModifiedBinaryValue AS PatientIDTypeLastModifiedBinaryValue")
                    .FROM("Ext.ExternalPatientIDType epidt")
                    .LEFT_JOIN("ADT.PatientIDType pidt ON epidt.PatientIDTypeKey = pidt.PatientIDTypeKey")
                    .WHERE("epidt.DeleteFlag = 0")
                    ._("epidt.ExternalSystemKey = @ExternalSystemKey")
                    ._("epidt.PatientIDTypeCode = @PatientIDTypeCode");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.Query<ExternalPatientIdentificationTypeResults>(query.ToString(), new
                    {
                        ExternalSystemKey = externalSystemKey,
                        PatientIDTypeCode = patientIdTypeCode
                    }, commandTimeout: connectionScope.DefaultCommandTimeout, commandType: CommandType.Text).FirstOrDefault();

                    if (result != null)
                    {
                        externalPatientIdentificationType = CreateExternalPatientIdentificationType(result);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return externalPatientIdentificationType;
        }

        private ExternalPatientIdentificationType CreateExternalPatientIdentificationType(ExternalPatientIdentificationTypeResults result)
        {
            var externalPatientIdentificationType = new ExternalPatientIdentificationType
            {
                Key = result.ExternalPatientIdTypeKey,
                ExternalSystemKey = result.ExternalSystemKey,
                LastModified = result.LastModifiedBinaryValue,
                PatientIdTypeCode = result.PatientIdTypeCode,
                SortOrder = result.SortValue,
                UseOnOutbound = result.UseOnOutboundFlag
            };

            if (result.PatientIdTypeKey != null)
            {
                var patientIdType = new PatientIdentificationType()
                {
                    Key = (Guid) result.PatientIdTypeKey,
                    DisplayCode = result.PatientIDTypeDisplayCode,
                    InternalCode = result.PatientIDTypeInternalCode,
                    Description = result.PatientIDTypeDescriptionText,
                    SortOrder = result.PatientIDTypeSortValue,
                    IsActive = result.PatientIDTypeActiveFlag.GetValueOrDefault(),
                    LastModified = result.PatientIDTypeLastModifiedBinaryValue,
                };
                externalPatientIdentificationType.PatientIdType = patientIdType;
            }

            return externalPatientIdentificationType;
        }

        IEnumerable<ExternalPatientIdentificationType> IExternalRepository.GetExternalPatientIdentificationTypes(Guid externalSystemKey, Guid patientIdentificationTypeKey)
        {
            IEnumerable<ExternalPatientIdentificationType> externalPatientIdTypes = null;

            try
            {
                var externalPatientIdTypeQuery = new SqlBuilder();
                externalPatientIdTypeQuery.SELECT()
                    ._("epidt.ExternalPatientIDTypeKey AS [Key]")
                    ._("epidt.ExternalSystemKey")
                    ._("epidt.PatientIDTypeCode")
                    ._("epidt.UseOnOutboundFlag")
                    ._("epidt.SortValue AS SortOrder")
                    ._("epidt.LastModifiedBinaryValue AS LastModified")
                    .FROM("Ext.ExternalPatientIDType epidt")
                    .WHERE("epidt.ExternalSystemKey = @ExternalSystemKey")
                    ._("epidt.PatientIDTypeKey = @PatientIDTypeKey")
                    ._("epidt.DeleteFlag = 0");

                var patientIdTypeQuery = new SqlBuilder();
                patientIdTypeQuery.SELECT()
                    ._("pidt.PatientIDTypeKey AS [Key]")
                    ._("pidt.DisplayCode")
                    ._("pidt.InternalCode")
                    ._("pidt.DescriptionText AS Description")
                    ._("pidt.SortValue AS SortOrder")
                    ._("pidt.ActiveFlag AS IsActive")
                    ._("pidt.LastModifiedBinaryValue AS LastModified")
                    .FROM("ADT.PatientIDType pidt");

                var query = new StringBuilder();
                query.AppendLine(externalPatientIdTypeQuery.ToString());
                query.AppendLine(patientIdTypeQuery.ToString());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var results = connectionScope.QueryMultiple(query.ToString(), new
                    {
                        ExternalSystemKey = externalSystemKey,
                        PatientIDTypeKey = patientIdentificationTypeKey
                    }, commandTimeout: connectionScope.DefaultCommandTimeout, commandType: CommandType.Text);

                    externalPatientIdTypes = results.Read<ExternalPatientIdentificationType>();
                    var patientIdTypes = results.Read<PatientIdentificationType>();

                    if(externalPatientIdTypes != null)
                    {
                        foreach (var externalPatientIdentificationType in externalPatientIdTypes)
                        {
                            var patientIdType = patientIdTypes?.Where(x => x.Key == patientIdentificationTypeKey).FirstOrDefault();
                            externalPatientIdentificationType.PatientIdType = patientIdType;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return externalPatientIdTypes;
        }

        Guid IExternalRepository.InsertExternalPatientIdentificationType(Context context, ExternalPatientIdentificationType externalPatientIdentificationType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(externalPatientIdentificationType, "externalPatientIdentificationType");
            Guid? externalPatientIdentificationTypeKey = null;

            try
            {
                Pyxis.Core.Data.Schema.Ext.Models.ExternalPatientIdentificationType model = new Pyxis.Core.Data.Schema.Ext.Models.ExternalPatientIdentificationType()
                {
                    ExternalSystemKey = externalPatientIdentificationType.ExternalSystemKey,
                    PatientIdTypeCode = externalPatientIdentificationType.PatientIdTypeCode,
                    PatientIdTypeKey = externalPatientIdentificationType.PatientIdType != null ? externalPatientIdentificationType.PatientIdType.Key : default(Guid?),
                    UseOnOutboundFlag = externalPatientIdentificationType.UseOnOutbound,
                    SortValue = externalPatientIdentificationType.SortOrder
                };
                externalPatientIdentificationTypeKey = _externalPatientIdTypeRepository.InsertExternalPatientIdentificationType(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return externalPatientIdentificationTypeKey.GetValueOrDefault();
        }

        void IExternalRepository.UpdateExternalPatientIdentificationType(Context context, ExternalPatientIdentificationType externalPatientIdentificationType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(externalPatientIdentificationType, "externalPatientIdentificationType");

            try
            {
                Pyxis.Core.Data.Schema.Ext.Models.ExternalPatientIdentificationType model = new Pyxis.Core.Data.Schema.Ext.Models.ExternalPatientIdentificationType()
                {
                    ExternalPatientIdTypeKey = externalPatientIdentificationType.Key,
                    PatientIdTypeCode = externalPatientIdentificationType.PatientIdTypeCode,
                    PatientIdTypeKey = externalPatientIdentificationType.PatientIdType != null ? externalPatientIdentificationType.PatientIdType.Key : default(Guid?),
                    UseOnOutboundFlag = externalPatientIdentificationType.UseOnOutbound,
                    SortValue = externalPatientIdentificationType.SortOrder,
                    LastModifiedBinaryValue = externalPatientIdentificationType.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }
                };

                _externalPatientIdTypeRepository.UpdateExternalPatientIdentificationType(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IExternalRepository.DeleteExternalPatientIdentificationType(Context context, Guid externalPatientIdentificationTypeKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _externalPatientIdTypeRepository.DeleteExternalPatientIdentificationType(context.ToActionContext(), externalPatientIdentificationTypeKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        ExternalGender IExternalRepository.GetExternalGender(Guid externalGenderKey)
        {
            ExternalGender externalGender = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("eg.*")
                    ._("gen.DisplayCode AS GenderDisplayCode")
                    ._("gen.InternalCode AS GenderInternalCode")
                    ._("gen.DescriptionText AS GenderDescriptionText")
                    ._("gen.SortValue AS GenderSortValue")
                    ._("gen.ActiveFlag AS GenderActiveFlag")
                    ._("gen.LastModifiedBinaryValue AS GenderLastModifiedBinaryValue")
                    .FROM("Ext.ExternalGender eg")
                    .LEFT_JOIN("ADT.Gender gen ON eg.GenderKey = gen.GenderKey")
                    .WHERE("eg.DeleteFlag = 0")
                    ._("eg.ExternalGenderKey = @ExternalGenderKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.Query<ExternalGenderResults>(query.ToString(), new
                    {
                        ExternalGenderKey = externalGenderKey
                    }, commandTimeout: connectionScope.DefaultCommandTimeout, commandType: CommandType.Text).FirstOrDefault();

                    if(result != null)
                    {
                        externalGender = externalGender = CreateExternalGender(result);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return externalGender;
        }

        ExternalGender IExternalRepository.GetExternalGender(Guid externalSystemKey, string genderCode)
        {
            ExternalGender externalGender = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("eg.*")
                    ._("gen.DisplayCode AS GenderDisplayCode")
                    ._("gen.InternalCode AS GenderInternalCode")
                    ._("gen.DescriptionText AS GenderDescriptionText")
                    ._("gen.SortValue AS GenderSortValue")
                    ._("gen.ActiveFlag AS GenderActiveFlag")
                    ._("gen.LastModifiedBinaryValue AS GenderLastModifiedBinaryValue")
                    .FROM("Ext.ExternalGender eg")
                    .LEFT_JOIN("ADT.Gender gen ON eg.GenderKey = gen.GenderKey")
                    .WHERE("eg.DeleteFlag = 0")
                    ._("eg.ExternalSystemKey = @ExternalSystemKey")
                    ._("eg.GenderCode = @GenderCode");


                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.Query<ExternalGenderResults>(query.ToString(), new
                    {
                        ExternalSystemKey = externalSystemKey,
                        GenderCode = genderCode
                    }, commandTimeout: connectionScope.DefaultCommandTimeout, commandType: CommandType.Text).FirstOrDefault();

                    if (result != null)
                    {
                        externalGender = CreateExternalGender(result);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return externalGender;
        }

        private ExternalGender CreateExternalGender(ExternalGenderResults result)
        {
            var externalGender = new ExternalGender
            {
                Key = result.ExternalGenderKey,
                ExternalSystemKey = result.ExternalSystemKey,
                GenderCode = result.GenderCode,
                LastModified = result.LastModifiedBinaryValue,
                UseOnOutbound = result.UseOnOutboundFlag,
                SortOrder = result.SortValue
            };

            if (result.GenderKey != null)
            {
                var gender = new Gender()
                {
                    Key = (Guid)result.GenderKey,
                    DisplayCode = result.GenderDisplayCode,
                    InternalCode = result.GenderInternalCode,
                    Description = result.GenderDescriptionText,
                    SortOrder = result.GenderSortValue,
                    IsActive = result.GenderActiveFlag.GetValueOrDefault(),
                    LastModified = result.GenderLastModifiedBinaryValue,
                };
                externalGender.Gender = gender;
            }

            return externalGender;
        }

        Guid IExternalRepository.InsertExternalGender(Context context, ExternalGender externalGender)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(externalGender, "externalGender");
            Guid? externalGenderKey = null;

            try
            {
                var model = new Pyxis.Core.Data.Schema.Ext.Models.ExternalGender()
                {
                    ExternalSystemKey = externalGender.ExternalSystemKey,
                    GenderCode = externalGender.GenderCode,
                    UseOnOutboundFlag = externalGender.UseOnOutbound,
                    GenderKey = externalGender.Gender != null ? externalGender.Gender.Key : default(Guid?),
                    SortValue = externalGender.SortOrder
                };

                externalGenderKey = _externalGenderRepository.InsertExternalGender(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return externalGenderKey.GetValueOrDefault();
        }

        void IExternalRepository.UpdateExternalGender(Context context, ExternalGender externalGender)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(externalGender, "externalGender");

            try
            {
                var model = new Pyxis.Core.Data.Schema.Ext.Models.ExternalGender()
                {
                    ExternalGenderKey = externalGender.Key,
                    GenderCode = externalGender.GenderCode,
                    GenderKey = externalGender.Gender != null ? externalGender.Gender.Key : default(Guid?),
                    UseOnOutboundFlag = externalGender.UseOnOutbound,
                    SortValue = externalGender.SortOrder,
                    LastModifiedBinaryValue = externalGender.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }
                };

                _externalGenderRepository.UpdateExternalGender(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IExternalRepository.DeleteExternalGender(Context context, Guid externalGenderKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _externalGenderRepository.DeleteExternalGender(context.ToActionContext(), externalGenderKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        IEnumerable<ExternalUnitOfMeasure> IExternalRepository.GetExternalUnitOfMeasures(IEnumerable<Guid> externalUnitOfMeasureKeys, bool? deleted,
            Guid? externalSystemKey, UOMRoleInternalCode? unitOfMeasureRole, string unitOfMeasureCode)
        {
            List<ExternalUnitOfMeasure> externalUnitOfMeasures = new List<ExternalUnitOfMeasure>();
            if (externalUnitOfMeasureKeys != null && !externalUnitOfMeasureKeys.Any())
                return externalUnitOfMeasures; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (externalUnitOfMeasureKeys != null)
                    selectedKeys = new GuidKeyTable(externalUnitOfMeasureKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var externalUnitOfMeasureResults = connectionScope.Query<ExternalUnitOfMeasuresResult>(
                        "Ext.bsp_GetExternalUnitOfMeasures",
                        new
                        {
                            SelectedKeys = selectedKeys.AsTableValuedParameter(),
                            DeleteFlag = deleted,
                            ExternalSystemKey = externalSystemKey,
                            UnitOfMeasureRoleInternalCode = unitOfMeasureRole.ToInternalCode(),
                            UnitOfMeasureCode = unitOfMeasureCode
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);

                    foreach (var externalUnitOfMeasureResult in externalUnitOfMeasureResults)
                    {
                        ExternalUnitOfMeasure externalUnitOfMeasure = new ExternalUnitOfMeasure(externalUnitOfMeasureResult.ExternalUOMKey)
                        {
                            ExternalSystemKey = externalUnitOfMeasureResult.ExternalSystemKey,
                            UnitOfMeasureCode = externalUnitOfMeasureResult.UOMCode,
                            StandardUnitOfMeasureKey = externalUnitOfMeasureResult.UOMKey,
                            StandardUnitOfMeasureDisplayCode = externalUnitOfMeasureResult.UOMDisplayCode,
                            StandardUnitOfMeasureUseDosageForm = externalUnitOfMeasureResult.UOMUseDosageFormFlag.GetValueOrDefault(),
                            UnitOfMeasureRole = externalUnitOfMeasureResult.UOMRoleInternalCode.FromInternalCode<UOMRoleInternalCode>(),
                            UnitOfMeasureRoleDescription = externalUnitOfMeasureResult.UOMRoleDescriptionText,
                            UseOnOutbound = externalUnitOfMeasureResult.UseOnOutboundFlag,
                            SortOrder = externalUnitOfMeasureResult.SortValue,
                            IsDeleted = externalUnitOfMeasureResult.DeleteFlag,
                            LastModified = externalUnitOfMeasureResult.LastModifiedBinaryValue.ToArray()
                        };

                        externalUnitOfMeasures.Add(externalUnitOfMeasure);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return externalUnitOfMeasures;
        }

        IEnumerable<ExternalUnitOfMeasure> IExternalRepository.GetExternalUnitOfMeasures(Guid externalSystemKey, string unitOfMeasureCode)
        {
            return ((IExternalRepository)this).GetExternalUnitOfMeasures(null, false, externalSystemKey, unitOfMeasureCode: unitOfMeasureCode);
        }

        ExternalUnitOfMeasure IExternalRepository.GetExternalUnitOfMeasure(Guid externalSystemKey, UOMRoleInternalCode unitOfMeasureRole, string unitOfMeasureCode)
        {
            var externalUnitOfMeasures =
                ((IExternalRepository)this).GetExternalUnitOfMeasures(null, false, externalSystemKey, unitOfMeasureRole, unitOfMeasureCode);

            return externalUnitOfMeasures.FirstOrDefault();
        }

        ExternalUnitOfMeasure IExternalRepository.GetExternalUnitOfMeasure(Guid externalUnitOfMeasureKey)
        {
            var externalUnitOfMeasures =
                ((IExternalRepository)this).GetExternalUnitOfMeasures(new[] { externalUnitOfMeasureKey }, false);

            return externalUnitOfMeasures.FirstOrDefault();
        }

        Guid IExternalRepository.InsertExternalUnitOfMeasure(Context context, ExternalUnitOfMeasure externalUnitOfMeasure)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(externalUnitOfMeasure, "externalUnitOfMeasure");
            Guid? externalUnitOfMeasureKey = null;

            try
            {
                var model = new Pyxis.Core.Data.Schema.Ext.Models.ExternalUOM()
                {
                    ExternalSystemKey = externalUnitOfMeasure.ExternalSystemKey,
                    UOMRoleInternalCode = externalUnitOfMeasure.UnitOfMeasureRole.ToInternalCode(),
                    UOMCode = externalUnitOfMeasure.UnitOfMeasureCode,
                    UOMKey = externalUnitOfMeasure.StandardUnitOfMeasureKey,
                    UseOnOutboundFlag = externalUnitOfMeasure.UseOnOutbound,
                    SortValue = externalUnitOfMeasure.SortOrder,
                };

                externalUnitOfMeasureKey = _externalUOMRepository.InsertExternalUOM(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return externalUnitOfMeasureKey.GetValueOrDefault();
        }

        void IExternalRepository.UpdateExternalUnitOfMeasure(Context context, ExternalUnitOfMeasure externalUnitOfMeasure)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(externalUnitOfMeasure, "externalUnitOfMeasure");

            try
            {
                var model = new Pyxis.Core.Data.Schema.Ext.Models.ExternalUOM()
                {
                    UOMRoleInternalCode = externalUnitOfMeasure.UnitOfMeasureRole.ToInternalCode(),
                    UOMCode = externalUnitOfMeasure.UnitOfMeasureCode,
                    UOMKey = externalUnitOfMeasure.StandardUnitOfMeasureKey,
                    UseOnOutboundFlag = externalUnitOfMeasure.UseOnOutbound,
                    SortValue = externalUnitOfMeasure.SortOrder,
                    LastModifiedBinaryValue = externalUnitOfMeasure.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    ExternalUOMKey = externalUnitOfMeasure.Key
                };

                _externalUOMRepository.UpdateExternalUOM(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IExternalRepository.DeleteExternalUnitOfMeasure(Context context, Guid externalUnitOfMeasureKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _externalUOMRepository.DeleteExternalUOM(context.ToActionContext(), externalUnitOfMeasureKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion
    }
}
