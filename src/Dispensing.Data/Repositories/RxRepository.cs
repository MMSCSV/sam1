using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Contracts.LocationManagement;
using Pyxis.Core.Data;
using Dapper;
using RxDAL = Pyxis.Core.Data.Schema.Rx;

namespace CareFusion.Dispensing.Data.Repositories
{
    internal class RxRepository : LinqBaseRepository, IRxRepository
    {
        private RxDAL.IControlledSubstanceLicenseRepository _cslRepository;
        private RxDAL.IMedClassGroupRepository _medClassGroupRepository;
        private RxDAL.IInvoiceTypeRepository _invoiceTypeRepository;

        public RxRepository()
        {
            _cslRepository = new RxDAL.ControlledSubstanceLicenseRepository();
            _medClassGroupRepository = new RxDAL.MedClassGroupRepository();
            _invoiceTypeRepository = new RxDAL.InvoiceTypeRepository();
        }            

        #region ControlledSubstanceLicense

        IEnumerable<ControlledSubstanceLicense> IRxRepository.GetControlledSubstanceLicenses(IEnumerable<Guid> controlledSubstanceLicensesKeys, bool? deleted, Guid? facilityKey)
        {
            IEnumerable<ControlledSubstanceLicense> controlledSubstanceLicenses = new List<ControlledSubstanceLicense>();

            if (controlledSubstanceLicensesKeys != null && !controlledSubstanceLicensesKeys.Any())
            {
                return controlledSubstanceLicenses;
            }

            try
            {
                var query = BuildGetControlledSubstanceLicenseSqlQuery(false);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var queryResult = connectionScope.Connection.Query(
                            query.ToString(),
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text).Select(result => new ControlledSubstanceLicense
                            {
                                Key = result.ControlledSubstanceLicenseKey,
                                LastModified = result.LastModifiedBinaryValue,
                                LicenseID = result.LicenseID,
                                LicenseName = result.LicenseName,
                                DescriptionText = result.DescriptionText,
                                StreetAddressText = result.StreetAddressText,
                                CityName = result.CityName,
                                SubdivisionName = result.SubdivisionName,
                                PostalCode = result.PostalCode,
                                CountryName = result.CountryName,
                            }).ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return controlledSubstanceLicenses;
        }

        ControlledSubstanceLicense IRxRepository.GetControlledSubstanceLicense(Guid controlledSubstanceLicenseKey)
        {
            var controlledSubstanceLicense = new ControlledSubstanceLicense();

            try
            {
                var query = BuildGetControlledSubstanceLicenseSqlQuery(true);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var queryResult = connectionScope.Connection.Query(
                            query.ToString(),
                            new
                            {
                                ControlledSubstanceLicenseKey = controlledSubstanceLicenseKey
                            },
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text).Select(result => new ControlledSubstanceLicense
                            {
                                Key = result.ControlledSubstanceLicenseKey,
                                LastModified = result.LastModifiedBinaryValue,
                                LicenseID = result.LicenseID,
                                LicenseName = result.LicenseName,
                                DescriptionText = result.DescriptionText,
                                StreetAddressText = result.StreetAddressText,
                                CityName = result.CityName,
                                SubdivisionName = result.SubdivisionName,
                                PostalCode = result.PostalCode,
                                CountryName = result.CountryName
                            });

                    controlledSubstanceLicense = queryResult.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return controlledSubstanceLicense;
        }

        Guid IRxRepository.InsertControlledSubstanceLicense(Context context, ControlledSubstanceLicense controlledSubstanceLicense)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(controlledSubstanceLicense, "controlledSubstanceLicense");

            Guid? controlledSubstanceLicenseKey = null;

            try
            {
                controlledSubstanceLicenseKey = _cslRepository.InsertControlledSubstanceLicense(context.ToActionContext(), new RxDAL.Models.ControlledSubstanceLicense
                {
                    CityName = controlledSubstanceLicense.CityName,                    
                    CountryName = controlledSubstanceLicense.CountryName,
                    DescriptionText = controlledSubstanceLicense.DescriptionText,
                    LicenseID = controlledSubstanceLicense.LicenseID,
                    LicenseName = controlledSubstanceLicense.LicenseName,
                    PostalCode = controlledSubstanceLicense.PostalCode,
                    StreetAddressText = controlledSubstanceLicense.StreetAddressText,
                    SubdivisionName = controlledSubstanceLicense.SubdivisionName
                });                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return controlledSubstanceLicenseKey.GetValueOrDefault();
        }

        void IRxRepository.UpdateControlledSubstanceLicense(Context context, ControlledSubstanceLicense controlledSubstanceLicense)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(controlledSubstanceLicense, "controlledSubstanceLicense");

            try
            {
                _cslRepository.UpdateControlledSubstanceLicense(context.ToActionContext(), new RxDAL.Models.ControlledSubstanceLicense
                {
                    CityName = controlledSubstanceLicense.CityName,
                    ControlledSubstanceLicenseKey = controlledSubstanceLicense.Key,
                    CountryName = controlledSubstanceLicense.CountryName,
                    DescriptionText = controlledSubstanceLicense.DescriptionText,
                    LicenseID = controlledSubstanceLicense.LicenseID,
                    LicenseName = controlledSubstanceLicense.LicenseName,
                    PostalCode = controlledSubstanceLicense.PostalCode,
                    StreetAddressText = controlledSubstanceLicense.StreetAddressText,
                    SubdivisionName = controlledSubstanceLicense.SubdivisionName,
                    LastModifiedBinaryValue = controlledSubstanceLicense.LastModified
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }
        }

        void IRxRepository.DeleteControlledSubstanceLicense(Context context, ControlledSubstanceLicense controlledSubstanceLicense)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(controlledSubstanceLicense, "controlledSubstanceLicense");

            try
            {
                _cslRepository.DeleteControlledSubstanceLicense(context.ToActionContext(), controlledSubstanceLicense.Key);                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }
        }

        bool IRxRepository.ControlledSubstanceLicenseAlreadyExists(Guid controlledSubstanceLicenseKey, string controlledSubstanceLicenseID)
        {
            try
            {
                var query = new SqlBuilder();
                query.SELECT("TOP 1 csl.ControlledSubstanceLicenseKey")
                    .FROM("Rx.ControlledSubstanceLicenseSnapshot csl")
                    .WHERE("csl.EndDateTime IS NULL")
                    .WHERE("csl.DeleteFlag = 0")
                    .WHERE("csl.LicenseID = @LicenseID");

                if (controlledSubstanceLicenseKey != default(Guid))
                {
                    query.WHERE("csl.ControlledSubstanceLicenseKey <> @ControlledSubstanceLicenseKey");
                }

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    return connectionScope.Query(
                            query.ToString(),
                            new
                            {
                                ControlledSubstanceLicenseKey = controlledSubstanceLicenseKey,
                                LicenseID = controlledSubstanceLicenseID
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

            return false;
        }

        private static SqlBuilder BuildGetControlledSubstanceLicenseSqlQuery(bool getSingleValue)
        {
            var query = new SqlBuilder();
            query.SELECT()
                ._("csls.ControlledSubstanceLicenseKey")
                ._("csls.LicenseID")
                ._("csls.LicenseName")
                ._("csls.DescriptionText")
                ._("csls.StreetAddressText")
                ._("csls.CityName")
                ._("csls.SubdivisionName")
                ._("csls.PostalCode")
                ._("csls.CountryName")
                ._("csls.LastModifiedBinaryValue")
                .FROM("Rx.ControlledSubstanceLicenseSnapshot csls")
                .WHERE("csls.EndDateTime IS NULL")
                .WHERE("csls.DeleteFlag = 0");

            if (getSingleValue)
            {
                query.WHERE("csls.ControlledSubstanceLicenseKey = @ControlledSubstanceLicenseKey");
            }

            return query;
        }

        #endregion

        #region InvoiceType members

        IEnumerable<InvoiceType> IRxRepository.GetInvoiceTypes(IEnumerable<Guid> invoiceTypeKeys, bool? deleted, Guid? facilityKey)
        {
            IEnumerable<InvoiceType> invoiceTypes = new List<InvoiceType>();

            if (invoiceTypeKeys != null && !invoiceTypeKeys.Any())
            {
                return invoiceTypes;
            }

            try
            {
                var query = BuildGetSqlQueryForInvoiceType(false);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var queryResult = connectionScope.Connection.Query(
                            query.ToString(),
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text).Select(result => new InvoiceType
                            {
                                Key = result.InvoiceTypeKey,
                                FacilityKey = result.FacilityKey,
                                FacilityName = result.FacilityName,
                                InvoiceTypeName = result.InvoiceTypeName,
                                DescriptionText = result.DescriptionText,
                                LastModified = result.LastModifiedBinaryValue
                            });

                    invoiceTypes = facilityKey.HasValue
                        ? queryResult.Where(r => r.FacilityKey == facilityKey).ToList()
                        : queryResult.ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return invoiceTypes;
        }

        InvoiceType IRxRepository.GetInvoiceType(Guid invoiceTypeKey)
        {
            var invoiceType = new InvoiceType();

            try
            {
                var query = BuildGetSqlQueryForInvoiceType(true);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var queryResult = connectionScope.Connection.Query(
                            query.ToString(),
                            new
                            {
                                InvoiceTypeKey = invoiceTypeKey

                            },
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text).Select(result => new InvoiceType
                            {
                                Key = result.InvoiceTypeKey,
                                FacilityKey = result.FacilityKey,
                                FacilityName = result.FacilityName,
                                InvoiceTypeName = result.InvoiceTypeName,
                                DescriptionText = result.DescriptionText,
                                LastModified = result.LastModifiedBinaryValue
                            });

                    invoiceType = queryResult.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return invoiceType;

        }

        Guid IRxRepository.InsertInvoiceType(Context context, InvoiceType invoiceType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(invoiceType, "invoiceType");

            Guid invoiceTypeKey = Guid.Empty;

            try
            {                
                invoiceTypeKey = _invoiceTypeRepository.InsertInvoiceType(context.ToActionContext(),
                    new RxDAL.Models.InvoiceType
                    {
                        FacilityKey = invoiceType.FacilityKey,
                        InvoiceTypeName = invoiceType.InvoiceTypeName,
                        DescriptionText = invoiceType.DescriptionText
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return invoiceTypeKey;
        }

        void IRxRepository.UpdateInvoiceType(Context context, InvoiceType invoiceType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(invoiceType, "invoiceType");

            try
            {
                _invoiceTypeRepository.UpdateInvoiceType(context.ToActionContext(),
                    new RxDAL.Models.InvoiceType
                    {
                        FacilityKey = invoiceType.FacilityKey,
                        InvoiceTypeName = invoiceType.InvoiceTypeName,
                        DescriptionText = invoiceType.DescriptionText,
                        LastModifiedBinaryValue = invoiceType.LastModified,
                        InvoiceTypeKey = invoiceType.Key
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }
        }

        void IRxRepository.DeleteInvoiceType(Context context, IReadOnlyCollection<Guid> invoiceTypeKeys)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {                
                foreach (Guid invoiceTypeKey in invoiceTypeKeys)
                {
                    _invoiceTypeRepository.DeleteInvoiceType(context.ToActionContext(), invoiceTypeKey);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }
        }

        bool IRxRepository.InvoiceTypeAlreadyExists(InvoiceType invoiceType)
        {
            try
            {
                var query = new SqlBuilder();
                query.SELECT("TOP 1 iT.InvoiceTypeKey")
                    .FROM("Rx.InvoiceTypeSnapshot iT")
                    .INNER_JOIN("Location.FacilitySnapshot f on f.FacilityKey = iT.FacilityKey")
                    .WHERE("iT.EndDateTime IS NULL")
                    .WHERE("iT.DeleteFlag = 0")
                    .WHERE("f.EndLocalDateTime IS NULL")
                    .WHERE("iT.InvoiceTypeName = @InvoiceTypeName")
                    .WHERE("f.FacilityKey = @FacilityKey");

                if (invoiceType.Key != default(Guid))
                {
                    query.WHERE("iT.InvoiceTypeKey <> @InvoiceTypeKey");
                }

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    return connectionScope.Query(
                            query.ToString(),
                            new
                            {
                                InvoiceTypeKey = invoiceType.Key,
                                InvoiceTypeName = invoiceType.InvoiceTypeName,
                                FacilityKey = invoiceType.FacilityKey
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

            return false;
        }

        private SqlBuilder BuildGetSqlQueryForInvoiceType(bool getSingleValue)
        {
            var query = new SqlBuilder();
            query.SELECT()
                ._("iT.InvoiceTypeKey")
                ._("iT.InvoiceTypeName")
                ._("iT.DescriptionText")
                ._("iT.LastModifiedBinaryValue")
                ._("iT.FacilityKey")
                ._("f.FacilityName")
                .FROM("Rx.InvoiceTypeSnapshot iT")
                .INNER_JOIN("Location.FacilitySnapshot f on f.FacilityKey = iT.FacilityKey")
                .WHERE("iT.EndDateTime IS NULL")
                .WHERE("f.EndLocalDateTime IS NULL")
                .WHERE("iT.DeleteFlag = 0");

            if (getSingleValue)
            {
                query.WHERE("iT.InvoiceTypeKey = @InvoiceTypeKey");
            }

            return query;
        }

        #endregion

        #region MedClassGroup Members
        IEnumerable<MedClassGroup> IRxRepository.GetMedClassGroups(IEnumerable<Guid> medClassGroupKeys, bool? deleted, Guid? facilityKey)
        {
            IEnumerable<MedClassGroup> medClassGroups = new List<MedClassGroup>();
            if (medClassGroupKeys != null && !medClassGroupKeys.Any())
            {
                return medClassGroups;
            }

            try
            {
                var query = BuildGetMedClassGroupsSqlQuery(false);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var queryResult = connectionScope.Connection.Query(
                            query.ToString(),
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text).Select(result => new MedClassGroup
                            {
                                Key = result.MedClassGroupKey,
                                FacilityKey = result.FacilityKey,
                                FacilityName = result.FacilityName,
                                MedClassGroupCode = result.MedClassGroupCode,
                                DescriptionText = result.DescriptionText,
                                LastModified = result.LastModifiedBinaryValue
                            });

                    medClassGroups = queryResult.ToList();
                    foreach (var medClassGroup in medClassGroups)
                    {
                        var medicationClassesQuery = BuildGetMedicationClassesSqlQuery(false);
                        var medicationClassesQueryResult = connectionScope.Connection.Query(
                            medicationClassesQuery.ToString(),
                            new
                            {
                                MedClassGroupKey = medClassGroup.Key
                            },
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text).Select(result => new MedicationClass
                            {
                                Key = result.MedClassKey,
                                Code = result.MedClassCode,
                                Description = result.DescriptionText,
                                SortOrder = result.SortValue,
                                IsControlled = result.ControlledFlag,
                                IsDeleted = result.DeleteFlag,
                                ExternalSystemKey = result.ExternalSystemKey,
                                FormularyTemplateKey = result.FormularyTemplateKey,
                                ExternalSystemName = String.Empty,
                            });
                        medClassGroup.MedicationClasses = medicationClassesQueryResult.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return medClassGroups;
        }

        MedClassGroup IRxRepository.GetMedClassGroup(Guid MedClassGroupKey)
        {
            var medClassGroup = new MedClassGroup();

            try
            {
                var query = BuildGetMedClassGroupsSqlQuery(true);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var queryResult = connectionScope.Connection.Query(
                            query.ToString(),
                            new
                            {
                                MedClassGroupKey = MedClassGroupKey
                            },
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text).Select(result => new MedClassGroup
                            {
                                Key = result.MedClassGroupKey,
                                FacilityKey = result.FacilityKey,
                                FacilityName = result.FacilityName,
                                MedClassGroupCode = result.MedClassGroupCode,
                                DescriptionText = result.DescriptionText,
                                LastModified = result.LastModifiedBinaryValue
                            });

                    medClassGroup = queryResult.FirstOrDefault();

                    var medicationClassesQuery = BuildGetMedicationClassesSqlQuery(false);
                    var medicationClassesQueryResult = connectionScope.Connection.Query(
                        medicationClassesQuery.ToString(),
                        new
                        {
                            MedClassGroupKey = medClassGroup.Key
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).Select(result => new MedicationClass
                        {
                            Key = result.MedClassKey,
                            Code = result.MedClassCode,
                            Description = result.DescriptionText,
                            SortOrder = result.SortValue,
                            IsControlled = result.ControlledFlag,
                            IsDeleted = result.DeleteFlag,
                            ExternalSystemKey = result.ExternalSystemKey,
                            FormularyTemplateKey = result.FormularyTemplateKey,
                            ExternalSystemName = String.Empty,
                        });
                    medClassGroup.MedicationClasses = medicationClassesQueryResult.ToList();

                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return medClassGroup;
        }

        MedClassGroup IRxRepository.GetMedClassGroup(Guid medClassGroupKey, Guid medClassExternalSystemKey)
        {
            var medClassGroup = new MedClassGroup();

            try
            {
                var query = BuildGetMedClassGroupsSqlQuery(true);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var queryResult = connectionScope.Connection.Query(
                            query.ToString(),
                            new
                            {
                                MedClassGroupKey = medClassGroupKey
                            },
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text).Select(result => new MedClassGroup
                            {
                                Key = result.MedClassGroupKey,
                                FacilityKey = result.FacilityKey,
                                FacilityName = result.FacilityName,
                                MedClassGroupCode = result.MedClassGroupCode,
                                DescriptionText = result.DescriptionText,
                                LastModified = result.LastModifiedBinaryValue
                            });

                    medClassGroup = queryResult.FirstOrDefault();

                    var medicationClassesQuery = BuildGetMedicationClassesSqlQuery(true);
                    var medicationClassesQueryResult = connectionScope.Connection.Query(
                        medicationClassesQuery.ToString(),
                        new
                        {
                            MedClassGroupKey = medClassGroup.Key,
                            ExternalSystemKey = medClassExternalSystemKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text).Select(result => new MedicationClass
                        {
                            Key = result.MedClassKey,
                            Code = result.MedClassCode,
                            Description = result.DescriptionText,
                            SortOrder = result.SortValue,
                            IsControlled = result.ControlledFlag,
                            IsDeleted = result.DeleteFlag,
                            ExternalSystemKey = result.ExternalSystemKey,
                            FormularyTemplateKey = result.FormularyTemplateKey,
                            ExternalSystemName = String.Empty,
                        });
                    medClassGroup.MedicationClasses = medicationClassesQueryResult.ToList();

                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return medClassGroup;
        }

        Guid IRxRepository.InsertMedClassGroup(Context context, MedClassGroup medClassGroup)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(medClassGroup, "medClassGroup");
            Guid medClassGroupKey = Guid.Empty;

            try
            {                
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                { 
                    medClassGroupKey = _medClassGroupRepository.InsertMedClassGroup(context.ToActionContext(),
                        new RxDAL.Models.MedClassGroup
                        {
                            FacilityKey = medClassGroup.FacilityKey,
                            DescriptionText = medClassGroup.DescriptionText,
                            MedClassGroupCode = medClassGroup.MedClassGroupCode
                        });

                    if (medClassGroup.MedicationClasses != null && medClassGroup.MedicationClasses.Any())
                    {
                        foreach (var medicationClass in medClassGroup.MedicationClasses)
                        {
                            _medClassGroupRepository.AssociateMedClassGroupMember(context.ToActionContext(), medClassGroupKey, medicationClass.Key);
                        }
                    }
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))

                {
                    throw;
                }
            }

            return medClassGroupKey;
        }

        void IRxRepository.UpdateMedClassGroup(Context context, MedClassGroup medClassGroup, IReadOnlyCollection<Guid> medClassKeys, Guid medClassExternalSystemKey)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(medClassGroup, "medClassGroup");

            try
            {                
                using (ITransactionScope tx = TransactionScopeFactory.Create())                
                {
                    _medClassGroupRepository.UpdateMedClassGroup(context.ToActionContext(),
                        new RxDAL.Models.MedClassGroup
                        {
                            MedClassGroupKey = medClassGroup.Key,
                            FacilityKey = medClassGroup.FacilityKey,
                            DescriptionText = medClassGroup.DescriptionText,
                            MedClassGroupCode = medClassGroup.MedClassGroupCode,
                            LastModifiedBinaryValue = medClassGroup.LastModified
                        });

                    _medClassGroupRepository.UpdateMedClassGroupAssociations(context.ToActionContext(), medClassGroup.Key, medClassKeys, medClassExternalSystemKey);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }
        }

        void IRxRepository.DeleteMedClassGroups(Context context, IReadOnlyCollection<Guid> medClassGroupKeys)
        {
            foreach (Guid medClassGroupKey in medClassGroupKeys)
            {
                _medClassGroupRepository.DeleteMedClassGroup(context.ToActionContext(), medClassGroupKey);
            }
        }

        private SqlBuilder BuildGetMedClassGroupsSqlQuery(bool getSingleValue)
        {
            var query = new SqlBuilder();
            query.SELECT()
                ._("mgs.MedClassGroupKey")
                ._("mgs.FacilityKey")
                ._("mgs.MedClassGroupCode")
                ._("mgs.DescriptionText")
                ._("mgs.LastModifiedBinaryValue")
                ._("fs.FacilityName")
                .FROM("Rx.MedClassGroupSnapshot mgs")
                .INNER_JOIN("Location.FacilitySnapshot fs on mgs.FacilityKey = fs.FacilityKey")
                .WHERE("fs.EndLocalDateTime IS NULL")
                .WHERE("mgs.EndDateTime IS NULL")
                .WHERE("mgs.DeleteFlag = 0");

            if (getSingleValue)
            {
                query.WHERE("mgs.MedClassGroupKey = @MedClassGroupKey");
            }

            return query;
        }

        private SqlBuilder BuildGetMedicationClassesSqlQuery(bool getValueByExternalSystem)
        {
            var query = new SqlBuilder();
            query.SELECT()
                ._("mcgm.MedClassKey")
                ._("mc.MedClassCode")
                ._("mc.DescriptionText")
                ._("mc.SortValue")
                ._("mc.ControlledFlag")
                ._("mc.DeleteFlag")
                ._("mc.ExternalSystemKey")
                ._("mc.FormularyTemplateKey")
                .FROM("Rx.MedClassGroupMember mcgm")
                .INNER_JOIN("Item.MedClass mc on mcgm.MedClassKey = mc.MedClassKey")
                .WHERE("mcgm.DisassociationDateTime IS NULL")
                .WHERE("mcgm.MedClassGroupKey=@MedClassGroupKey");

            if (getValueByExternalSystem)
            {
                query.WHERE("mc.ExternalSystemKey=@ExternalSystemKey");
            }
            return query;
        }

        #endregion
    }
}
