using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using CareFusion.Dispensing.Resources;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.Core;
using Pyxis.Core.Data.Schema.TableTypes;
using CoreDAL = Pyxis.Core.Data.Schema.Core;

namespace CareFusion.Dispensing.Data.Repositories
{
    public class CoreRepository : LinqBaseRepository, ICoreRepository
    {
        private readonly IDistributorRepository _distributorRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserTypeRepository _userTypeRepository;

        public CoreRepository()
        {
            _distributorRepository = new DistributorRepository();
            _userAccountRepository = new UserAccountRepository();
            _userRoleRepository = new UserRoleRepository();
            _userTypeRepository = new UserTypeRepository();

            SqlMapper.SetTypeMap(typeof(Facility), new ColumnAttributeTypeMapper<Facility>());
        }

        #region ICoreRepository Members

        IEnumerable<Distributor> ICoreRepository.GetDistributors(IEnumerable<Guid> distributorKeys, bool? deleted, string distributorId)
        {
            List<Distributor> distributors = new List<Distributor>();
            if (distributorKeys != null && !distributorKeys.Any())
                return distributors; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (distributorKeys != null)
                    selectedKeys = new GuidKeyTable(distributorKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var distributorResults = connectionScope.Query<DistributorsResult>(
                        "Core.bsp_GetDistributors",
                        new { SelectedKeys = selectedKeys.AsTableValuedParameter(), DeleteFlag = deleted, DistributorID = distributorId },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);

                    foreach (var distributorResult in distributorResults)
                    {
                        var distributor = new Distributor(distributorResult.DistributorKey)
                        {
                            DistributorId = distributorResult.DistributorID,
                            DistributorName = distributorResult.DistributorName,
                            StreetAddress = distributorResult.StreetAddressText,
                            CityName = distributorResult.CityName,
                            SubdivisionName = distributorResult.SubdivisionName,
                            CountryName = distributorResult.CountryName,
                            PostalCode = distributorResult.PostalCode,
                            ContactName = distributorResult.ContactName,
                            ContactPhoneNumber = distributorResult.ContactPhoneNumberText,
                            IsDeleted = distributorResult.DeleteFlag,
                            TotalRows = distributorResult.TotalRows,
                            LastModified = distributorResult.LastModifiedBinaryValue
                        };

                        distributors.Add(distributor);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return distributors;
        }

        Distributor ICoreRepository.GetDistributor(string distributorId)
        {
            var distributors =
                ((ICoreRepository)this).GetDistributors(null, false, distributorId);

            return distributors.FirstOrDefault();
        }

        Guid ICoreRepository.InsertDistributor(Context context, Distributor distributor)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(distributor, "distributor");

            Guid? distributorKey = null;

            try
            {
                distributorKey = _distributorRepository.InsertDistributor(context.ToActionContext(), new CoreDAL.Models.Distributor
                {
                    CityName = distributor.CityName,
                    ContactName = distributor.ContactName,
                    ContactPhoneNumberText = distributor.ContactPhoneNumber,
                    CountryName = distributor.CountryName,
                    DistributorId = distributor.DistributorId,
                    DistributorKey = distributor.Key,
                    DistributorName = distributor.DistributorName,
                    LastModifiedBinaryValue = distributor.LastModified,
                    PostalCode = distributor.PostalCode,
                    StreetAddressText = distributor.StreetAddress,
                    SubdivisionName = distributor.SubdivisionName
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return distributorKey.GetValueOrDefault();
        }

        void ICoreRepository.UpdateDistributor(Context context, Distributor distributor)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(distributor, "distributor");

            try
            {
                _distributorRepository.UpdateDistributor(context.ToActionContext(), new CoreDAL.Models.Distributor
                {
                    CityName = distributor.CityName,
                    ContactName = distributor.ContactName,
                    ContactPhoneNumberText = distributor.ContactPhoneNumber,
                    CountryName = distributor.CountryName,
                    DistributorId = distributor.DistributorId,
                    DistributorKey = distributor.Key,
                    DistributorName = distributor.DistributorName,
                    LastModifiedBinaryValue = distributor.LastModified,
                    PostalCode = distributor.PostalCode,
                    StreetAddressText = distributor.StreetAddress,
                    SubdivisionName = distributor.SubdivisionName
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.DeleteDistributor(Context context, Guid distributorKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _distributorRepository.DeleteDistributor(context.ToActionContext(), distributorKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        AuthUserAccount ICoreRepository.GetAuthenticationUserAccount(Guid userAccountKey)
        {
            AuthUserAccount authUserAccount = null;

            try
            {
                var result = _userAccountRepository.GetUserAccount(userAccountKey);
                authUserAccount = result?.ToAuthUserContract();
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return authUserAccount;
        }

        AuthUserAccount ICoreRepository.GetAuthenticationUserAccount(string cardSerialID)
        {
            AuthUserAccount authUserAccount = null;

            try
            {
                var query = GetAuthUserBaseQuery();
                query.WHERE("uac.CardSerialID = @CardSerialID");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.QueryFirstOrDefault<CoreDAL.Models.UserAccount>(
                         query.ToString(),
                         new
                         {
                             CardSerialID = cardSerialID
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);

                    authUserAccount = result?.ToAuthUserContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return authUserAccount;
        }

        AuthUserAccount ICoreRepository.GetAuthUserAccountByRFIDCardSerialID(string rfidCardSerialID)
        {
            AuthUserAccount authUserAccount = null;

            try
            {
                var query = GetAuthUserBaseQuery();
                query.WHERE("uac.RFIDCardSerialID = @RFIDCardSerialID");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.QueryFirstOrDefault<CoreDAL.Models.UserAccount>(
                         query.ToString(),
                         new
                         {
                             RFIDCardSerialID = rfidCardSerialID
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);

                    authUserAccount = result?.ToAuthUserContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return authUserAccount;
        }

        AuthUserAccount ICoreRepository.GetAuthenticationUserAccount(string domainNameOrFullyQualifiedName, string userId)
        {
            AuthUserAccount authUserAccount = null;

            try
            {
                var query = GetAuthUserBaseQuery();
                var parameters = new DynamicParameters();
                parameters.Add("UserId", userId);

                if (string.IsNullOrEmpty(domainNameOrFullyQualifiedName))
                {
                    query.WHERE("uac.UserId = @UserId");
                    query.WHERE("uac.ActiveDirectoryDomainKey IS NULL");
                }
                else
                {
                    query.JOIN("Core.vw_ActiveDirectoryDomainCurrent adc ON uac.ActiveDirectoryDomainKey = adc.ActiveDirectoryDomainKey");
                    query.WHERE("uac.UserId = @UserId");
                    query.WHERE("(adc.FullyQualifiedDomainName = @FullyQualifiedDomainName OR adc.ShortDomainName = @FullyQualifiedDomainName)");
                    parameters.Add("FullyQualifiedDomainName", domainNameOrFullyQualifiedName);
                }
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.QueryFirstOrDefault<CoreDAL.Models.UserAccount>(
                         query.ToString(),
                         parameters,
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);

                    authUserAccount = result?.ToAuthUserContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return authUserAccount;
        }

        IEnumerable<AuthUserAccount> ICoreRepository.GetAuthenticationUserAccounts(string userId)
        {
            IEnumerable<AuthUserAccount> userAccounts = null;

            try
            {
                var query = GetAuthUserBaseQuery();
                query.WHERE("uac.UserID = @UserID");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.Query<CoreDAL.Models.UserAccount>(
                         query.ToString(),
                          new
                          {
                              UserID = userId
                          },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);

                    userAccounts = result?.Select(r => r.ToAuthUserContract());
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userAccounts;
        }

        AuthUserAccount ICoreRepository.GetAuthenticationUserAccountByScanCode(string domainNameOrFullyQualifiedName, string userScanCode)
        {
            AuthUserAccount authUserAccount = null;

            try
            {
                var query = GetAuthUserBaseQuery();
                var parameters = new DynamicParameters();

                if (string.IsNullOrEmpty(domainNameOrFullyQualifiedName))
                {
                    query.WHERE("uac.ScanCodeValue = @ScanCodeValue");
                    query.WHERE("uac.ActiveDirectoryDomainKey IS NULL");
                    parameters.Add("ScanCodeValue", userScanCode);
                }
                else
                {
                    query.JOIN("Core.vw_ActiveDirectoryDomainCurrent adc ON uac.ActiveDirectoryDomainKey = adc.ActiveDirectoryDomainKey");
                    query.WHERE("uac.ScanCodeValue = @ScanCodeValue");
                    query.WHERE("adc.FullyQualifiedDomainName = @FullyQualifiedDomainName OR adc.ShortDomainName = @FullyQualifiedDomainName");
                    parameters.Add("ScanCodeValue", userScanCode);
                    parameters.Add("FullyQualifiedDomainName", domainNameOrFullyQualifiedName);
                }

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.QueryFirstOrDefault<CoreDAL.Models.UserAccount>(
                         query.ToString(),
                         parameters,
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);

                    authUserAccount = result?.ToAuthUserContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return authUserAccount;
        }

        IEnumerable<AuthUserAccount> ICoreRepository.GetAuthenticationUserAccountsByScanCode(string userScanCode)
        {
            IEnumerable<AuthUserAccount> userAccounts = null;

            try
            {
                var query = GetAuthUserBaseQuery();
                query.WHERE("uac.ScanCodeValue = @ScanCodeValue");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.Query<CoreDAL.Models.UserAccount>(
                         query.ToString(),
                         new
                         {
                             ScanCodeValue = userScanCode
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);

                    userAccounts = result?.Select(r => r.ToAuthUserContract());
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userAccounts;
        }

        PasswordCredential ICoreRepository.GetPasswordCredential(Guid userAccountKey)
        {
            PasswordCredential passwordCredential = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("pc.*")
                    .FROM("Core.PasswordCredential pc")
                    .WHERE("pc.EndUTCDateTime IS NULL")
                    .WHERE("pc.UserAccountKey = @UserAccountKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    passwordCredential = connectionScope.QueryFirstOrDefault<PasswordCredentialResult>(
                         query.ToString(),
                         new { UserAccountKey = userAccountKey },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                         ?.ToContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return passwordCredential;
        }

        bool ICoreRepository.UserHasFacilityPrivileges(Guid facilityKey, Guid userAccountKey)
        {
            bool userHasFacilityPrivileges = false;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("uac.UserAccountKey")
                    .FROM("Core.vw_UserAccountCurrent uac")
                    .JOIN("Core.vw_UserRoleMemberCurrent mrc ON uac.UserAccountKey = mrc.UserAccountKey")
                    .JOIN("Core.vw_UserRoleCurrent urc ON mrc.UserRoleKey = urc.UserRoleKey")
                    .WHERE("urc.ActiveFlag = 'true'")
                    .WHERE("uac.UserAccountKey = @UserAccountKey")
                    .WHERE("urc.FacilityKey = @FacilityKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    userHasFacilityPrivileges = connectionScope.Query<Guid>(query.ToString(),
                         new
                         {
                             UserAccountKey = userAccountKey,
                             FacilityKey = facilityKey
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

            return userHasFacilityPrivileges;
        }

        IEnumerable<PermissionInternalCode> ICoreRepository.GetUserPermissions(Guid userAccountKey, bool isSupportUser)
        {
            IEnumerable<PermissionInternalCode> userPermissions = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("DISTINCT(PermissionInternalCode)")
                    .FROM("Core.vw_UserAuthorizationCurrent ua")
                    .WHERE("ua.UserAccountKey = @UserAccountKey");

                if (isSupportUser)
                {
                    query.WHERE("ua.FacilityKey IS NULL");
                }

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    userPermissions = connectionScope.Query<string>(query.ToString(),
                         new
                         {
                             UserAccountKey = userAccountKey
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                        .Select(x => x.FromInternalCode<PermissionInternalCode>()).ToHashSet();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userPermissions;
        }

        IEnumerable<PermissionInternalCode> ICoreRepository.GetUserPermissions(Guid userAccountKey, Guid dispensingDeviceKey, bool isSupportUser)
        {
            IEnumerable<PermissionInternalCode> userPermissions = null;

            try
            {
                CommandType commandType = default;
                string sql = default;

                if (isSupportUser)
                {
                    commandType = CommandType.Text;
                    sql = new SqlBuilder()
                        .SELECT("DISTINCT(ua.PermissionInternalCode)")
                        .FROM("Strg.vw_DispensingDeviceCurrent dd")
                        .JOIN("Core.vw_UserAuthorizationCurrent ua ON dd.FacilityKey = ua.FacilityKey")
                        .WHERE("dd.DispensingDeviceKey = @DispensingDeviceKey")
                        .WHERE("ua.UserAccountKey = @UserAccountKey")
                        .WHERE("ua.FacilityKey IS NOT NULL")
                        .ToString();
                }
                else
                {
                    commandType = CommandType.StoredProcedure;
                    sql = "Core.GetClinicalUserPermissions";
                }

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    userPermissions = connectionScope.Query<string>(
                        sql,
                        new
                        {
                            UserAccountKey = userAccountKey,
                            DispensingDeviceKey = dispensingDeviceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: commandType)
                        .Select(x => x.FromInternalCode<PermissionInternalCode>()).ToHashSet();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userPermissions;
        }

        UserAccount ICoreRepository.GetUserAccount(Guid userAccountKey)
        {
            UserAccount userAccount = null;

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                using (var multi = connectionScope.QueryMultiple(
                    "Core.bsp_GetUserAccounts",
                    new
                    {
                        UserAccountKey = userAccountKey
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure))
                {
                    var users = multi.Read<CoreDAL.Models.UserAccount>();
                    var domains = multi.Read<CoreDAL.Models.ActiveDirectoryDomain>();
                    var passwords = multi.Read<PasswordCredentialResult>();
                    var roles = multi.Read<CoreDAL.Models.UserRoleMember>();
                    var areas = multi.Read<CoreDAL.Models.UserRoleMemberArea>();
                    var facilities = multi.Read<UserFacilityResult>();

                    var user = users.FirstOrDefault();
                    if (user == null)
                    {
                        return null;
                    }

                    var result = user.ToContract(domains.FirstOrDefault(),
                                                 passwords.FirstOrDefault(),
                                                 roles,
                                                 areas,
                                                 facilities.Select(f => f.FacilityKey));
                    return result;
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userAccount;
        }

        IEnumerable<UserAccount> ICoreRepository.GetUserAccountByUserId(string userId)
        {
            List<UserAccount> userAccounts = null;

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                using (var multi = connectionScope.QueryMultiple(
                    "Core.bsp_GetUserAccounts",
                    new
                    {
                        UserID = userId
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure))
                {
                    var users = multi.Read<CoreDAL.Models.UserAccount>();
                    var domains = multi.Read<CoreDAL.Models.ActiveDirectoryDomain>();
                    var passwords = multi.Read<PasswordCredentialResult>();
                    var roles = multi.Read<CoreDAL.Models.UserRoleMember>();
                    var areas = multi.Read<CoreDAL.Models.UserRoleMemberArea>();
                    var facilities = multi.Read<UserFacilityResult>();

                    userAccounts = new List<UserAccount>();
                    foreach(var user in users)
                    {
                        var domain = domains.FirstOrDefault(d => d.ActiveDirectoryDomainKey == user.ActiveDirectoryDomainKey);
                        var password = passwords.FirstOrDefault(d => d.UserAccountKey == user.UserAccountKey);
                        var userRoles = roles.Where(r => r.UserAccountKey == user.UserAccountKey);
                        var userAreas = areas.Where(a => userRoles.Select(r => r.UserRoleMemberKey).Contains(a.UserRoleMemberKey));
                        var facility = facilities.Where(f => f.UserAccountKey == user.UserAccountKey).Select(f => f.FacilityKey);
                        userAccounts.Add(user.ToContract(domain, password, userRoles, userAreas, facility));
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userAccounts;
        }

        UserAccount ICoreRepository.GetUserAccountByUserId(string userId, Guid? activeDirectoryDomainKey)
        {
            UserAccount userAccount = null;

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                using (var multi = connectionScope.QueryMultiple(
                    "Core.bsp_GetUserAccounts",
                    new
                    {
                        userID = userId,
                        ActiveDirectoryDomainKey = activeDirectoryDomainKey
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure))
                {
                    var users = multi.Read<CoreDAL.Models.UserAccount>();
                    var domains = multi.Read<CoreDAL.Models.ActiveDirectoryDomain>();
                    var passwords = multi.Read<PasswordCredentialResult>();
                    var roles = multi.Read<CoreDAL.Models.UserRoleMember>();
                    var areas = multi.Read<CoreDAL.Models.UserRoleMemberArea>();
                    var facilities = multi.Read<UserFacilityResult>();

                    var user = users.FirstOrDefault();
                    if (user == null)
                    {
                        return null;
                    }

                    var result = user.ToContract(domains.FirstOrDefault(),
                                                 passwords.FirstOrDefault(),
                                                 roles,
                                                 areas,
                                                 facilities.Select(f => f.FacilityKey));
                    return result;
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userAccount;
        }

        DateTime? ICoreRepository.GetLastUnlockedUtcDateTime(Guid userAccountKey, DateTime sinceUtcDateTime)
        {
            DateTime? lastUnlockedUtcDateTime = null;

            try
            {
                var query = new SqlBuilder();
                query.Append("DECLARE @LastLockedUTCDateTime datetime2(7);");
                query.AppendLine();
                query.SELECT("@LastLockedUTCDateTime = MAX(ISNULL(EndUTCDateTime, StartUTCDateTime))")
                    .FROM("Core.UserAccountSnapshot")
                    .WHERE("UserAccountKey = @UserAccountKey")
                    .WHERE("LockedFlag = 1")
                    .WHERE("StartUTCDateTime > @SinceUtcDateTime");
                query.AppendLine();
                query.SELECT("MIN(StartUTCDateTime) AS LastUnlockedUTCDateTime")
                    .FROM("Core.UserAccountSnapshot")
                    .WHERE("UserAccountKey = @UserAccountKey")
                    .WHERE("LockedFlag = 0")
                    .WHERE("StartUTCDateTime >= @LastLockedUTCDateTime");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    lastUnlockedUtcDateTime = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            UserAccountKey = userAccountKey,
                            SinceUtcDateTime = sinceUtcDateTime
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (DateTime?)x.LastUnlockedUTCDateTime)
                        .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return lastUnlockedUtcDateTime;
        }

        DateTime? ICoreRepository.GetLastUndeletedUtcDateTime(Guid userAccountKey, DateTime sinceUtcDateTime)
        {
            DateTime? lastUndeletedUtcDateTime = null;

            try
            {
                var query = new SqlBuilder();
                query.Append("DECLARE @LastDeletedUTCDateTime datetime2(7);");
                query.AppendLine();
                query.SELECT("@LastDeletedUTCDateTime = MAX(ISNULL(EndUTCDateTime, StartUTCDateTime))")
                    .FROM("Core.UserAccountSnapshot")
                    .WHERE("UserAccountKey = @UserAccountKey")
                    .WHERE("DeleteFlag = 1")
                    .WHERE("StartUTCDateTime > @SinceUtcDateTime");
                query.AppendLine();
                query.SELECT("MIN(StartUTCDateTime) AS LastUndeletedUTCDateTime")
                    .FROM("Core.UserAccountSnapshot")
                    .WHERE("UserAccountKey = @UserAccountKey")
                    .WHERE("DeleteFlag = 0")
                    .WHERE("StartUTCDateTime >= @LastDeletedUTCDateTime");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    lastUndeletedUtcDateTime = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            UserAccountKey = userAccountKey,
                            SinceUtcDateTime = sinceUtcDateTime
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (DateTime?)x.LastUndeletedUTCDateTime)
                        .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return lastUndeletedUtcDateTime;
        }

        void ICoreRepository.DeleteMultiUserAccounts(Context context, IEnumerable<Guid> userAccountKeys)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                // Build our xml schema to pass to the stored procedure.
                // The schema should look like the following:
                // <keys>
                //    <key Value = [value] SnapshotValue = [value]/>
                // </keys >
                XElement userAccountKeysElement = new XElement("keys");
                foreach (var guid in userAccountKeys)
                {
                    XElement keyElement = new XElement("key");
                    keyElement.SetAttributeValue("Value", guid);
                    userAccountKeysElement.Add(keyElement);
                }

                XDocument userAccountKeysXml = new XDocument(new XDeclaration("1.0", "utf-8", ""),
                                                           userAccountKeysElement);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.bsp_MultiUserAccountDelete",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            LastModifiedActorKey = (Guid?)context.Actor,
                            UserAccountXML = userAccountKeysXml.Root,
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.UpdateMultiActiveDirectoryUserAccounts(Context context, IEnumerable<UserAccount> attributes)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                // Build our xml schema to pass to the stored procedure.
                // The schema should look like the following:
                // <keys>
                //    <key Value = [value] UserId = [value] FirstName = [value] and so on.../>
                // </keys >

                XElement activeDirectoryUserAccounts = new XElement("keys");
                foreach (UserAccount userAccountAttribs in attributes)
                {
                    XElement keyElement = CreateUserAccountXmlElement(userAccountAttribs);
                    activeDirectoryUserAccounts.Add(keyElement);
                }

                XDocument userAccountKeysXml = new XDocument(new XDeclaration("1.0", "utf-8", ""),
                                                           activeDirectoryUserAccounts);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.bsp_MultiUserAccountActiveDirectoryUpdate",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            LastModifiedActorKey = (Guid?)context.Actor,
                            UserAccountXML = userAccountKeysXml.Root,
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.InsertMultiActiveDirectoryUserAccounts(Context context, IEnumerable<UserAccount> attributes)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                // Build our xml schema to pass to the stored procedure.
                // The schema should look like the following:
                // <keys>
                //    <key Value = [value] UserId = [value] FirstName = [value] and so on.../>
                // </keys >

                XElement activeDirectoryUserAccounts = new XElement("keys");
                foreach (UserAccount userAccountAttribs in attributes)
                {
                    XElement keyElement = CreateUserAccountXmlElement(userAccountAttribs);
                    activeDirectoryUserAccounts.Add(keyElement);
                }

                XDocument userAccountKeysXml = new XDocument(new XDeclaration("1.0", "utf-8", ""),
                                                           activeDirectoryUserAccounts);

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.bsp_MultiUserAccountActiveDirectoryInsert",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            LastModifiedActorKey = (Guid?)context.Actor,
                            UserAccountXML = userAccountKeysXml.Root,
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        Guid ICoreRepository.UpsertSupportUserAccount(Context context, UserAccount userAccount)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccount, "userAccount");
            Guid newUserAccountKey = default(Guid);

            try
            {
                using (var tx = TransactionScopeFactory.Create())
                {
                    Guid? newKey = null;

                    if (userAccount.Key != Guid.Empty)
                    {
                        newKey = userAccount.Key;
                    }

                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                        {
                        {"@StartUTCDateTime", context.ActionUtcDateTime },
                        {"@StartLocalDateTime", context.ActionDateTime },
                        {"@ActiveDirectoryDomainKey", userAccount.ActiveDirectoryDomain != null ? userAccount.ActiveDirectoryDomain.Key : default(Guid?) },
                        {"@UserID", userAccount.UserId },
                        {"@FirstName", userAccount.FirstName },
                        {"@MiddleName", userAccount.MiddleName },
                        {"@LastName", userAccount.LastName },
                        {"@SuffixText", userAccount.Suffix },
                        {"@FullName", userAccount.FullName },
                        {"@InitialsText", userAccount.Initials },
                        {"@JobTitleText", userAccount.JobTitle },
                        {"@EmailAddressValue", userAccount.EmailAddress },
                        {"@ContactInformation", userAccount.ContactInformation },
                        {"@ScanCodeValue", userAccount.ScanCode },
                        {"@AccountExpirationUTCDateTime", userAccount.AccountExpirationUtcDate },
                        {"@AccountExpirationLocalDateTime", userAccount.AccountExpirationDate },
                        {"@FingerprintExemptFlag", userAccount.IsFingerprintExempt },
                        {"@TemporaryFlag", userAccount.IsTemporary },
                        {"@LockedFlag", userAccount.IsLocked },
                        {"@SuperUserFlag", userAccount.IsSuperUser },
                        {"@SupportUserFlag", userAccount.IsSupportUser },
                        {"@UserTypeKey", userAccount.UserTypeKey },
                        {"@ActiveDirectoryObjectGloballyUniqueID", userAccount.ActiveDirectoryObjectGuid },
                        {"@DirectoryChangePasswordUTCDateTime", userAccount.DirectoryChangePasswordUtcDateTime },
                        {"@DirectoryChangePasswordLocalDateTime", userAccount.DirectoryChangePasswordDateTime },
                        {"@CardPINExemptFlag", userAccount.IsCardPinExempt },
                        {"@CardSerialID", userAccount.CardSerialId },
                        {"@RFIDCardSerialID", userAccount.RFIDCardSerialID },
                        {"@ActiveFlag", userAccount.IsActive },
                        {"@LastModifiedActorKey", (Guid?)context.Actor },
                        {"@UserAccountSnapshotKey", userAccount.SnapshotKey },
                        {"@UserAccountKey", newKey },
                        }
                    );

                    using (var connectionScope = ConnectionScopeFactory.Create())
                    {
                        newKey = connectionScope.Query<Guid>(
                            "Core.bsp_UserAccountInsertBySystem",
                            parameters,
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.StoredProcedure)
                            .FirstOrDefault();

                        if (newKey.HasValue)
                        {
                            newUserAccountKey = newKey.Value;
                        }
                    }
                    // Commit transaction
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return newUserAccountKey;
        }

        Guid ICoreRepository.InsertUserAccount(Context context, UserAccount userAccount)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccount, "userAccount");
            Guid newUserAccountKey = default(Guid);

            try
            {
                Guid? newKey = null;

                using (var tx = TransactionScopeFactory.Create())
                {
                    newKey = _userAccountRepository.InsertUserAccount(context.ToActionContext(), userAccount.ToModel());

                    if (newKey.HasValue)
                    {
                        newUserAccountKey = newKey.Value;

                        if (userAccount.UserRoles != null)
                        {
                            InsertUserAccountRoleMembers(context, newUserAccountKey, userAccount.UserRoles);
                        }
                    }

                    // Commit transaction
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return newUserAccountKey;
        }

        void ICoreRepository.UpdateUserAccount(Context context, UserAccount userAccount)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccount, "userAccount");

            try
            {
                using (var tx = TransactionScopeFactory.Create())
                {
                    _userAccountRepository.UpdateUserAccount(context.ToActionContext(), userAccount.ToModel());

                    // If RoleKeys is null then we assume all associations were removed.
                    UpdateUserAccountRoleMembers(
                        context,
                        userAccount.Key,
                        userAccount.UserRoles ?? new UserRole[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.UpdateUserAccountActivity(Context context, Guid userAccountKey, DateTime? lastPasswordExpirationNoticeUtcDateTime,
                                              DateTime? lastPasswordExpirationNoticeDateTime)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.usp_UserAccountParentUpdate",
                        new
                        {
                            LastPasswordExpirationNoticeUTCDateTime = lastPasswordExpirationNoticeUtcDateTime,
                            LastPasswordExpirationNoticeUTCDateTimeSpecifiedFlag = true,
                            LastPasswordExpirationNoticeLocalDateTime = lastPasswordExpirationNoticeDateTime,
                            LastPasswordExpirationNoticeLocalDateTimeSpecifiedFlag = true,
                            LastSuccessfulPasswordAuthenticationUTCDateTime = default(DateTime?),
                            LastSuccessfulPasswordAuthenticationUTCDateTimeSpecifiedFlag = false,
                            LastSuccessfulPasswordAuthenticationLocalDateTime = default(DateTime?),
                            LastSuccessfulPasswordAuthenticationLocalDateTimeSpecifiedFlag = false,
                            LastFacilityKey = default(Guid?),
                            LastFacilityKeySpecifiedFlag = false,
                            LastAreaKey = default(Guid?),
                            LastAreaKeySpecifiedFlag = false,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            UserAccountKey = userAccountKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.UpdateUserAccountLastSuccessfulPasswordAuthentication(Guid? dispensingDeviceKey, Guid userAccountKey, DateTime? lastSuccessfulPasswordAuthenticationUtcDateTime,
                                       DateTime? lastSuccessfulPasswordAuthenticationDateTime)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.usp_UserAccountParentUpdate",
                        new
                        {
                            LastPasswordExpirationNoticeUTCDateTime = default(DateTime?),
                            LastPasswordExpirationNoticeUTCDateTimeSpecifiedFlag = false,
                            LastPasswordExpirationNoticeLocalDateTime = default(DateTime?),
                            LastPasswordExpirationNoticeLocalDateTimeSpecifiedFlag = false,
                            LastSuccessfulPasswordAuthenticationUTCDateTime = lastSuccessfulPasswordAuthenticationUtcDateTime,
                            LastSuccessfulPasswordAuthenticationUTCDateTimeSpecifiedFlag = true,
                            LastSuccessfulPasswordAuthenticationLocalDateTime = lastSuccessfulPasswordAuthenticationDateTime,
                            LastSuccessfulPasswordAuthenticationLocalDateTimeSpecifiedFlag = true,
                            LastFacilityKey = default(Guid?),
                            LastFacilityKeySpecifiedFlag = false,
                            LastAreaKey = default(Guid?),
                            LastAreaKeySpecifiedFlag = false,
                            LastModifiedDispensingDeviceKey = dispensingDeviceKey,
                            UserAccountKey = userAccountKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.DeleteUserAccount(Context context, Guid userAccountKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _userAccountRepository.DeleteUserAccount(context.ToActionContext(), userAccountKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.UndeleteActiveDirectoryUserAccount(Context context, Guid userAccountKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.usp_UserAccountUndelete",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            LastModifiedActorKey = (Guid?)context.Actor,
                            UserAccountKey = userAccountKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        public void LockUserAccount(Context context, Guid userAccountKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.usp_UserAccountLockFlagUpdate",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            LockedFlag = true,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            LastModifiedActorKey = (Guid?)context.Actor,
                            UserAccountKey = userAccountKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.UnlockUserAccount(Context context, Guid userAccountKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.usp_UserAccountLockFlagUpdate",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            LockedFlag = false,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            LastModifiedActorKey = (Guid?)context.Actor,
                            UserAccountKey = userAccountKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.InsertPasswordCredential(Context context, Guid userAccountKey, PasswordCredential passwordCredential)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(passwordCredential, "passwordCredential");

            try
            {
                _userAccountRepository.InsertPasswordCredential(context.ToActionContext(), new CoreDAL.Models.PasswordCredential
                {
                    EncryptionAlgorithmInternalCode = passwordCredential.EncryptionAlgorithm.ToInternalCode(),
                    PasswordHashValue = passwordCredential.PasswordHash,
                    PasswordSaltValue = passwordCredential.Salt,
                    UserChangedOwnPasswordLocalDateTime = passwordCredential.UserChangedOwnPasswordDate,
                    UserChangedOwnPasswordUtcDateTime = passwordCredential.UserChangedOwnPasswordUtcDate,
                    UserAccountKey = userAccountKey
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.UpdatePasswordCredential(Context context, Guid userAccountKey, PasswordCredential passwordCredential)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(passwordCredential, "passwordCredential");

            try
            {
                _userAccountRepository.UpdatePasswordCredential(context.ToActionContext(), new CoreDAL.Models.PasswordCredential
                {
                    EncryptionAlgorithmInternalCode = passwordCredential.EncryptionAlgorithm.ToInternalCode(),
                    PasswordHashValue = passwordCredential.PasswordHash,
                    PasswordSaltValue = passwordCredential.Salt,
                    UserChangedOwnPasswordLocalDateTime = passwordCredential.UserChangedOwnPasswordDate,
                    UserChangedOwnPasswordUtcDateTime = passwordCredential.UserChangedOwnPasswordUtcDate,
                    UserAccountKey = userAccountKey,
                    LastModifiedBinaryValue = passwordCredential.LastModified
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        IEnumerable<PasswordCredential> ICoreRepository.GetPasswordHistory(Guid userAccountKey, int passwordCount)
        {
            IEnumerable<PasswordCredential> previousPasswordCredentials = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("*")
                    .FROM("Core.PasswordCredential pc")
                    .WHERE("pc.UserAccountKey = @UserAccountKey")
                    .WHERE("pc.EndUTCDateTime IS NOT NULL")
                    .ORDER_BY("pc.EndUTCDateTime DESC");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    previousPasswordCredentials = connectionScope.Query<PasswordCredentialResult>(query.ToString(),
                         new
                         {
                             UserAccountKey = userAccountKey
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                        .Take(passwordCount)
                        .Select(p => p.ToContract())
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return previousPasswordCredentials;
        }

        UserFingerprint ICoreRepository.GetUserFingerprint(Guid userAccountKey)
        {
            UserFingerprint userFingerprint = null;

            try
            {
                userFingerprint = _userAccountRepository.GetUserFingerprintByUserAccount(userAccountKey)?.ToContract();
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userFingerprint;
        }

        void ICoreRepository.InsertUserFingerprint(Context context, UserFingerprint userFingerprint)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userFingerprint, "userFingerprint");

            try
            {
                _userAccountRepository.InsertUserFingerprint(context.ToActionContext(), new CoreDAL.Models.UserFingerprint
                {
                    Fingerprint1LengthQuantity = userFingerprint.Value1Length,
                    Fingerprint1Value = userFingerprint.Value1,
                    Fingerprint2LengthQuantity = userFingerprint.Value2Length,
                    Fingerprint2Value = userFingerprint.Value2,
                    Fingerprint3LengthQuantity = userFingerprint.Value3Length,
                    Fingerprint3Value = userFingerprint.Value3,
                    LastModifiedBinaryValue = userFingerprint.LastModified,
                    UserAccountKey = userFingerprint.UserAccountKey,
                    UserFingerprintKey = userFingerprint.Key
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.UpdateUserFingerprint(Context context, UserFingerprint userFingerprint)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userFingerprint, "userFingerprint");

            try
            {
                _userAccountRepository.UpdateUserFingerprint(context.ToActionContext(), new CoreDAL.Models.UserFingerprint
                {
                    Fingerprint1LengthQuantity = userFingerprint.Value1Length,
                    Fingerprint1Value = userFingerprint.Value1,
                    Fingerprint2LengthQuantity = userFingerprint.Value2Length,
                    Fingerprint2Value = userFingerprint.Value2,
                    Fingerprint3LengthQuantity = userFingerprint.Value3Length,
                    Fingerprint3Value = userFingerprint.Value3,
                    LastModifiedBinaryValue = userFingerprint.LastModified,
                    UserAccountKey = userFingerprint.UserAccountKey,
                    UserFingerprintKey = userFingerprint.Key
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        Role ICoreRepository.LoadRole(Guid roleKey)
        {
            Role role = ((ICoreRepository) this).GetRole(roleKey);
            if (role == null)
                throw new EntityNotFoundException(DataResources.LoadFailed_RoleNotFound, roleKey);

            return role;
        }

        Role ICoreRepository.GetRole(Guid roleKey)
        {
            Role roleResult = null;

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                using (var multi = connectionScope.QueryMultiple(
                    "Core.bsp_GetRoles",
                    new
                    {
                        RoleKey = roleKey,
                        VisitorInternalCode = PermissionInternalCode.GrantVisitorAccess.ToInternalCode()
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure))
                {
                    var roles = multi.Read<UserRoleResult>();
                    var facilities = multi.Read<Facility>();
                    var permissions = multi.Read<RolePermissionResult>();
                    var securityGroups = multi.Read<PermissionSecurityGroupResult>();
                    var overrideGroups = multi.Read<CoreDAL.Models.UserRoleOverrideGroup>();
                    var visitingRoles = multi.Read<CoreDAL.Models.UserRolePermissionRole>();

                    var role = roles.FirstOrDefault();
                    if (role == null)
                    {
                        return null;
                    }

                    roleResult = new Role(role.UserRoleKey)
                    {
                        AllowTemporaryUsers = role.AllowForTemporaryUserFlag,
                        Description = role.DescriptionText,
                        Facility = facilities?.FirstOrDefault(),
                        InternalCode = role.InternalCode?.FromInternalCode<UserRoleInternalCode>(),
                        IsActive = role.ActiveFlag,
                        LastModified = role.LastModifiedBinaryValue,
                        Name = role.RoleName,
                        OverrideGroups = overrideGroups?.Select(o => o.OverrideGroupKey).ToArray(),
                        UserMemberCount = role.UserMemberCount,
                        VisitingUserRoles = visitingRoles?.Select(v => v.UserRoleKey).ToArray(),
                        Permissions = permissions.Select(p => new RolePermission(p.UserRolePermissionKey)
                        {
                            Permission = new Permission(p.PermissionInternalCode, p.DescriptionText)
                            {
                                Delete = p.DeleteFlag,
                                ESSystem = p.ESSystemFlag,
                                GCSM = p.GCSMFlag,
                                Hide = p.HideFlag,
                                PermissionName = p.PermissionName,
                                PermissionInternalCode = p.PermissionInternalCode,
                                PermissionTypeInternalCode = p.PermissionTypeInternalCode,
                                Pharmogistics = p.PharmogisticsFlag,
                                SecurityGroupApplicable = p.SecurityGroupApplicableFlag,
                                SupportUserOnly = p.SupportUserOnlyFlag,
                                Version = p.VersionText,
                                PermissionType = new PermissionType(p.PermissionTypeInternalCode, p.PermissionTypeInternalCodeDescription)
                            },
                            SecurityGroups = securityGroups?.Where(x => x.UserRolePermissionKey == p.UserRolePermissionKey).Select(o => o.SecurityGroupKey).ToArray()
                        }).ToArray()
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return roleResult;
        }

        Guid ICoreRepository.InsertRole(Context context, Role role)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(role, "role");
            Guid newRoleKey = default(Guid);

            try
            {
                Guid? newKey = null;
                newKey = _userRoleRepository.InsertUserRole(context.ToActionContext(), new CoreDAL.Models.UserRole
                {
                    ActiveFlag = role.IsActive,
                    AllowForTemporaryUserFlag = role.AllowTemporaryUsers,
                    DescriptionText = role.Description,
                    FacilityKey = role.Facility?.Key,
                    LastModifiedBinaryValue = role.LastModified,
                    RoleName = role.Name,
                    UserRoleKey = role.Key
                });

                if (newKey.HasValue)
                {
                    newRoleKey = newKey.Value;

                    if (role.Permissions != null)
                        InsertUserRolePermissions(
                            context,
                            newRoleKey,
                            role.Permissions);

                    if (role.OverrideGroups != null)
                        InsertUserRoleOverrideGroups(
                            context,
                            newRoleKey,
                            role.OverrideGroups);

                    if (role.VisitingUserRoles != null)
                    {
                        var rolePermissionKey = GetRolePermissionKeyByUserRole(newRoleKey);
                        if (rolePermissionKey.HasValue)
                        {
                            InsertUserRolePermissionsRole(
                                context,
                                rolePermissionKey.Value,
                                role.VisitingUserRoles);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return newRoleKey;
        }

        void ICoreRepository.UpdateRole(Context context, Role role)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(role, "role");

            try
            {
                using (var tx = TransactionScopeFactory.Create())
                {
                    _userRoleRepository.UpdateUserRole(context.ToActionContext(), new CoreDAL.Models.UserRole
                    {
                        ActiveFlag = role.IsActive,
                        AllowForTemporaryUserFlag = role.AllowTemporaryUsers,
                        DescriptionText = role.Description,
                        FacilityKey = role.Facility != null ? role.Facility.Key : default(Guid?),
                        LastModifiedBinaryValue = role.LastModified,
                        RoleName = role.Name,
                        UserRoleKey = role.Key
                    });

                    // If Permissions is null then we assume all associations were removed.
                    UpdateUserRolePermissions(
                        context,
                        role.Key,
                        role.Permissions ?? new RolePermission[0]);

                    // If OverrideGroups is null then we assume all associations were removed.
                    UpdateUserRoleOverrideGroups(
                        context,
                        role.Key,
                        role.OverrideGroups ?? new Guid[0]);

                    //If VisitingUserRoles is null then we assume all association were removed
                    UpdateUserRolePermissionsRoles(
                        context,
                        role.Key,
                        role.VisitingUserRoles ?? new Guid[0]);

                    // Commit transaction
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.DeleteRole(Context context, Guid roleKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _userRoleRepository.DeleteUserRole(context.ToActionContext(), roleKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        Guid ICoreRepository.InsertUserRoleMember(Context context, Guid userAccountKey, Guid roleKey, bool medicationTemporaryAccess)
        {
            Guard.ArgumentNotNull(context, "context");
            Guid? userRoleMemberKey = null;

            try
            {
                userRoleMemberKey = _userRoleRepository.AssociateUserRoleMember(context.ToActionContext(), roleKey, userAccountKey, medicationTemporaryAccess);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userRoleMemberKey.GetValueOrDefault();
        }

        void ICoreRepository.DeleteUserRoleMember(Context context, Guid userAccountKey, Guid roleKey, bool medicationTemporaryAccess)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _userRoleRepository.DisassociateUserRoleMember(context.ToActionContext(), roleKey, userAccountKey, medicationTemporaryAccess);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.DeleteUserRoleMember(Context context, Guid userRoleMemberKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.usp_UserRoleMemberByPKDelete",
                        new
                        {
                            UserRoleMemberKey = userRoleMemberKey,
                            DisassociationUTCDateTime = context.ActionUtcDateTime,
                            DisassociationLocalDateTime = context.ActionDateTime,
                            DisassociationActorKey = (Guid?)context.Actor,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.InsertUserRoleMemberArea(Context context, Guid userRoleMemberKey, Guid areaKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _userRoleRepository.AssociateUserRoleMemberArea(context.ToActionContext(), userRoleMemberKey, areaKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.DeleteUserRoleMemberArea(Context context, Guid userRoleMemberKey, Guid areaKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _userRoleRepository.DisassociateUserRoleMemberArea(context.ToActionContext(), userRoleMemberKey, areaKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        IEnumerable<UserType> ICoreRepository.GetUserTypes()
        {
            IEnumerable<UserType> userTypes = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("*")
                    .FROM("Core.vw_UserTypeCurrent ut");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    userTypes = connectionScope.Query<UserTypeResult>(query.ToString(),
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                        .Select(u => new UserType(u.UserTypeKey)
                        {
                            Description = u.DescriptionText,
                            DisplayCode = u.DisplayCode,
                            InternalCode = u.InternalCode.FromNullableInternalCode<UserTypeInternalCode>(),
                            LastModified = u.LastModifiedBinaryValue,
                            SortOrder = u.SortValue,
                            StandardUserTypeDescription = u.StandardUserTypeDescriptionText,
                            StandardUserTypeInternalCode = u.StandardUserTypeInternalCode
                        })
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userTypes;
        }

        UserType ICoreRepository.GetUserType(Guid userTypeKey)
        {
            UserType userType = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("*")
                    .FROM("Core.vw_UserTypeCurrent ut")
                    .WHERE("ut.UserTypeKey = @UserTypeKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.QueryFirstOrDefault<UserTypeResult>(query.ToString(),
                        new
                        {
                            UserTypeKey = userTypeKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text);

                    if (result != null)
                    {
                        userType = new UserType(result.UserTypeKey)
                        {
                            Description = result.DescriptionText,
                            DisplayCode = result.DisplayCode,
                            InternalCode = result.InternalCode.FromNullableInternalCode<UserTypeInternalCode>(),
                            LastModified = result.LastModifiedBinaryValue,
                            SortOrder = result.SortValue,
                            StandardUserTypeDescription = result.StandardUserTypeDescriptionText,
                            StandardUserTypeInternalCode = result.StandardUserTypeInternalCode
                        };
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userType;
        }

        Guid ICoreRepository.InsertUserType(Context context, UserType userType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userType, "userType");
            Guid? userTypeKey = null;

            try
            {
                userTypeKey = _userTypeRepository.InsertUserType(context.ToActionContext(), new CoreDAL.Models.UserType
                {
                    DescriptionText = userType.Description,
                    DisplayCode = userType.DisplayCode,
                    LastModifiedBinaryValue = userType.LastModified,
                    SortValue = userType.SortOrder,
                    StandardUserTypeInternalCode = userType.StandardUserTypeInternalCode,
                    UserTypeKey = userType.Key
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userTypeKey.GetValueOrDefault();
        }

        void ICoreRepository.UpdateUserType(Context context, UserType userType)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userType, "userType");

            try
            {
                _userTypeRepository.UpdateUserType(context.ToActionContext(), new CoreDAL.Models.UserType
                {
                    DescriptionText = userType.Description,
                    DisplayCode = userType.DisplayCode,
                    LastModifiedBinaryValue = userType.LastModified,
                    SortValue = userType.SortOrder,
                    StandardUserTypeInternalCode = userType.StandardUserTypeInternalCode,
                    UserTypeKey = userType.Key
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.DeleteUserType(Context context, Guid userTypeKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _userTypeRepository.DeleteUserType(context.ToActionContext(), userTypeKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        bool ICoreRepository.PasswordContainsDictionaryWord(string passwordText)
        {
            var passwordContainsDictionaryWordResult = false;
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.QueryFirstOrDefault<bool?>(
                        "Core.bsp_PasswordContainsDictionaryWord",
                        new
                        {
                            PasswordText = passwordText
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);

                    passwordContainsDictionaryWordResult = result.GetValueOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
            return passwordContainsDictionaryWordResult;
        }

        void ICoreRepository.MapLocalUserListsToDomainUser(Context context, Guid localUserAccountKey, Guid domainUserAccountKey)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.bsp_MapLocalUserToDomainUserMultiUpdate",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            DispensingDeviceKey = context.Device?.Key,
                            OldUserAccountKey = localUserAccountKey,
                            NewUserAccountKey = domainUserAccountKey,
                            LastModifiedActorKey = context.Actor.Key
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void ICoreRepository.InsertUserMergeAssociation(Context context, Guid localUserAccountKey,
            Guid domainUserAccountKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _userAccountRepository.AssociateUserAccountMerge(context.ToActionContext(), domainUserAccountKey, localUserAccountKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        bool ICoreRepository.UserHasUnResolvedMyItems(Guid localUserKey)
        {

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                        {
                            {"@LocalUserAccountKey", localUserKey }
                        }
                    );

                    var result = connectionScope.Query<int>(
                        "Tx.bsp_UserHasUnresolvedMyItems",
                        parameters,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                    return result != 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        UserMergeStatus ICoreRepository.IsUserAccountRegistered(Guid domainUserAccountKey)
        {
            UserMergeStatus userMergeStatus = new UserMergeStatus();
            Guard.ArgumentNotNull(domainUserAccountKey, "domainUserAccountKey");

            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT()
                    ._("ads.ShortDomainName as DomainName")
                    ._("uas.UserID as DomainUserID")
                    .FROM("Core.UserAccountMerge uam")
                    .INNER_JOIN("Core.UserAccountSnapshot uas on uam.SurvivingUserAccountKey = uas.UserAccountKey")
                    .INNER_JOIN("Core.ActiveDirectoryDomainSnapshot ads on uas.ActiveDirectoryDomainKey = ads.ActiveDirectoryDomainKey")
                    .WHERE("uam.SurvivingUserAccountKey = @UserAccountKey")
                    .WHERE("uam.DisassociationUTCDateTime IS NULL")
                    .WHERE("uas.EndUTCDateTime IS NULL")
                    .WHERE("ads.EndUTCDateTime IS NULL");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.Query(
                            query.ToString(),
                            new { UserAccountKey = domainUserAccountKey },
                            commandTimeout: connectionScope.DefaultCommandTimeout)
                        .FirstOrDefault();
                    if (result != null)
                    {
                        userMergeStatus.Code = UserMergeStatusCode.UserMergeError;
                        userMergeStatus.Description = string.Format(CultureInfo.CurrentCulture,
                            ServiceResources.UserAccountAlreadyRegistered, result.DomainName, result.DomainUserID);
                        return userMergeStatus;

                    }
                }

            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return null;
        }

        UserMergeSummary ICoreRepository.GetUserMergeSummary(Guid userAccountKey)
        {
            Guard.ArgumentNotNull(userAccountKey, "userAccountKey");

            UserMergeSummary userMergeSummary = new UserMergeSummary();
            try
            {
                SqlBuilder subQuery = new SqlBuilder();
                subQuery.SELECT()
                    ._("uam.NonSurvivingUserAccountKey AS UserAccountKey")
                    ._("uam.AssociationUTCDateTime AS MergedUtcDateTime")
                    ._("1 AS LocalUser")
                    ._("0 AS DomainUser")
                    ._("uam.DispensingDeviceKey")
                    .FROM("Core.UserAccountMerge uam")
                    .WHERE("uam.SurvivingUserAccountKey = @UserAccountKey")
                    .UNION()
                    .SELECT()
                    ._("uam.SurvivingUserAccountKey  AS UserAccountKey")
                    ._("uam.AssociationUTCDateTime AS MergedUtcDateTime")
                    ._("0 AS LocalUser")
                    ._("1 AS DomainUser")
                    ._("uam.DispensingDeviceKey")
                    .FROM("Core.UserAccountMerge uam")
                    .WHERE("uam.NonSurvivingUserAccountKey = @UserAccountKey");

                SqlBuilder query = new SqlBuilder();
                query.SELECT()
                    ._("uas.UserAccountKey AS UserAccountKey")
                    ._("uas.UserID AS UserId")
                    ._("uas.LastName AS UserLastName")
                    ._("uas.FirstName AS UserFirstName")
                    ._("uas.MiddleName AS UserMiddleName")
                    ._("ads.FullyQualifiedDomainName AS Domain")
                    ._("mergeQuery.MergedUtcDateTime")
                    ._("dds.DispensingDeviceName AS MergedAt")
                    ._("mergeQuery.LocalUser")
                    ._("mergeQuery.DomainUser")
                    .FROM("Core.UserAccountSnapshot uas")
                    .INNER_JOIN("({0}) mergeQuery ON uas.UserAccountKey = mergeQuery.UserAccountKey ", subQuery.ToString())
                    .LEFT_JOIN(
                        "Core.ActiveDirectoryDomainSnapshot ads on uas.ActiveDirectoryDomainKey = ads.ActiveDirectoryDomainKey")
                    .LEFT_JOIN("Strg.DispensingDeviceSnapshot dds on mergeQuery.DispensingDeviceKey = dds.DispensingDeviceKey")
                    .WHERE("ads.EndUTCDateTime IS NULL")
                    .WHERE("uas.EndUTCDateTime IS NULL")
                    .WHERE("dds.EndUTCDateTime IS NULL");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    userMergeSummary = connectionScope.Query<UserMergeSummary>(
                        query.ToString(),
                        new {UserAccountKey = userAccountKey},
                        commandTimeout: connectionScope.DefaultCommandTimeout).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return userMergeSummary;
        }

        #endregion

        #region Private Methods

        private void InsertUserAccountRoleMembers(Context context, Guid userAccountKey, IEnumerable<UserRole> userRoles)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userRoles, "userRoles");

            using (ITransactionScope tx = TransactionScopeFactory.Create())
            {
                foreach (UserRole userRole in userRoles)
                {
                    Guid? userRoleKey = _userRoleRepository.AssociateUserRoleMember(context.ToActionContext(), userRole.RoleKey, userAccountKey);
                    if (userRoleKey.HasValue)
                    {
                        // Areas
                        InsertUserRoleMemberAreas(context, userRoleKey.Value, userRole.Areas ?? new Guid[0]);
                    }
                }

                tx.Complete();
            }
        }

        private void UpdateUserAccountRoleMembers(Context context, Guid userAccountKey, IEnumerable<UserRole> userRoles)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userRoles, "userRoles");

            // Get the list of user role member entities associated with this user account.
            // Exclude role members that are temporary.

            var query = new SqlBuilder();
            query.SELECT("*")
                .FROM("Core.vw_UserRoleMemberCurrent ur")
                .WHERE("ur.UserAccountKey = @UserAccountKey")
                .WHERE("ur.MedTemporaryAccessFlag = @MedTemporaryAccessFlag");

            IEnumerable<CoreDAL.Models.UserRoleMember> currentUserRoleMembers;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                currentUserRoleMembers = connectionScope.Query<CoreDAL.Models.UserRoleMember>(query.ToString(),
                     new
                     {
                         UserAccountKey = userAccountKey,
                         MedTemporaryAccessFlag = false
                     },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text);
            }

            //Find the user role members that were removed.
            var removedUserRoleMembers = currentUserRoleMembers
                .Select(m => new { m.UserRoleKey })
                .Except(userRoles.Select(ur => new { UserRoleKey = ur.RoleKey }));

            using (var tx = TransactionScopeFactory.Create())
            {
                //Remove user role member entities that are no longer associated with this user account.
                foreach (var userRoleMember in removedUserRoleMembers)
                {
                    // Execute the stored procedure for deleting user role member.
                    _userRoleRepository.DisassociateUserRoleMember(context.ToActionContext(), userRoleMember.UserRoleKey, userAccountKey);
                }

                //Find the user role members that were added or area assoication updates.
                var addedUserRoleMembers = new List<UserRole>();
                foreach (var userRole in userRoles)
                {
                    if (!currentUserRoleMembers.Select(r => r.UserRoleKey).Contains(userRole.RoleKey))
                    {
                        addedUserRoleMembers.Add(userRole);
                        continue;
                    }

                    UpdateUserRoleMemberAreas(
                        context,
                        userRole.Key,
                        userRole.Areas ?? new Guid[0]);
                }

                // Add the new user role members.
                InsertUserAccountRoleMembers(context, userAccountKey, addedUserRoleMembers);

                tx.Complete();
            }
        }

        private void InsertUserRoleMemberAreas(Context context, Guid userRoleMemberKey, IEnumerable<Guid> areaKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(areaKeys, "areaKeys");

            foreach (Guid areaKey in areaKeys)
            {
                _userRoleRepository.AssociateUserRoleMemberArea(context.ToActionContext(), userRoleMemberKey, areaKey);
            }
        }

        private void UpdateUserRoleMemberAreas(Context context, Guid userRoleMemberKey, IEnumerable<Guid> areaKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(areaKeys, "areaKeys");

            // Get the list of area keys associated with this user role member.
            var query = new SqlBuilder();
            query.SELECT("ur.AreaKey")
                .FROM("Core.vw_UserRoleMemberAreaCurrent ur")
                .WHERE("ur.UserRoleMemberKey = @UserRoleMemberKey");

            IEnumerable<Guid> currentAreaKeys;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                currentAreaKeys = connectionScope.Query<Guid>(query.ToString(),
                     new
                     {
                         UserRoleMemberKey = userRoleMemberKey,
                     },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text);
            }

            // Find the area keys that were removed.
            var removedAreaKeys = currentAreaKeys.Except(areaKeys);

            // Remove areas that are no longer associated with this user role member.
            foreach (Guid areaKey in removedAreaKeys)
            {
                _userRoleRepository.DisassociateUserRoleMemberArea(context.ToActionContext(), userRoleMemberKey, areaKey);
            }

            // Find the areas that were added
            var addedAreaKeys = areaKeys.Except(currentAreaKeys);
            InsertUserRoleMemberAreas(context, userRoleMemberKey, addedAreaKeys);
        }

        private void InsertUserRolePermissions(Context context, Guid roleKey, IEnumerable<RolePermission> rolePermissions)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(rolePermissions, "rolePermissions");

            using (var tx = TransactionScopeFactory.Create())
            {
                foreach (RolePermission rolePermission in rolePermissions)
                {
                    Guid? rolePermissionKey = _userRoleRepository.AssociateUserRolePermission(context.ToActionContext(), roleKey, rolePermission.Permission.PermissionInternalCode);

                    if (rolePermissionKey.HasValue && rolePermission.SecurityGroups != null)
                    {
                        InsertUserRolePermissionSecurityGroups(
                            context,
                            rolePermissionKey.Value,
                            rolePermission.SecurityGroups);
                    }
                }

                tx.Complete();
            }
        }

        private void UpdateUserRolePermissions(Context context, Guid roleKey, IEnumerable<RolePermission> rolePermissions)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(rolePermissions, "rolePermissions");

            // Get the list of user role permission entities associated with this role
            var query = new SqlBuilder();
            query.SELECT("ur.*")
                .FROM("Core.vw_UserRolePermissionCurrent ur")
                .WHERE("ur.UserRoleKey = @UserRoleKey");

            IEnumerable<CoreDAL.Models.UserRolePermission> rolePermissionResults;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                rolePermissionResults = connectionScope.Query<CoreDAL.Models.UserRolePermission>(query.ToString(),
                     new
                     {
                         UserRoleKey = roleKey
                     },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text);
            }

            var currentRolePermissions = rolePermissionResults.Select(urp => new
            {
                Key = urp.UserRolePermissionKey,
                Permission = urp.PermissionInternalCode.FromInternalCode<PermissionInternalCode>()
            }).ToArray();

            //Find the role permissions that were removed.
            var removedRolePermissions = currentRolePermissions.Except(rolePermissions.Select(rp => new { rp.Key, Permission = rp.Permission.InternalCode}));

            using (var tx = TransactionScopeFactory.Create())
            {
                //Remove user role permission entities that are no longer associated with this role.
                foreach (var rolePermission in removedRolePermissions)
                {
                    //Execute the stored procedure for deleting user role permission.
                    _userRoleRepository.DisassociateUserRolePermission(context.ToActionContext(), roleKey, rolePermission.Permission.ToInternalCode());
                }

                //Find the role permissions that were added or security group assoication updates.
                List<RolePermission> addedRolePermissions = new List<RolePermission>();
                foreach (RolePermission rolePermission in rolePermissions)
                {
                    if (!currentRolePermissions.Contains(
                            new {rolePermission.Key, Permission = rolePermission.Permission.InternalCode}))
                    {
                        addedRolePermissions.Add(rolePermission);
                        continue;
                    }

                    UpdateUserRolePermissionSecurityGroups(
                        context,
                        rolePermission.Key,
                        rolePermission.SecurityGroups ?? new Guid[0]);
                }

                // Add the new role permssions.
                InsertUserRolePermissions(
                    context,
                    roleKey,
                    addedRolePermissions);

                tx.Complete();
            }
        }

        private void InsertUserRolePermissionSecurityGroups(Context context, Guid userRolePermissionKey, IEnumerable<Guid> securityGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(securityGroupKeys, "securityGroupKeys");

            foreach (Guid securityGroupKey in securityGroupKeys)
            {
                _userRoleRepository.AssociateUserRolePermissionSecurityGroup(context.ToActionContext(), userRolePermissionKey, securityGroupKey);
            }
        }

        private void UpdateUserRolePermissionSecurityGroups(Context context, Guid userRolePermissionKey, IEnumerable<Guid> securityGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(securityGroupKeys, "securityGroupKeys");

            // Get the list of security group keys associated with this med item
            var query = new SqlBuilder();
            query.SELECT("ur.SecurityGroupKey")
                .FROM("Core.vw_UserRolePermissionSecurityGroupCurrent ur")
                .WHERE("ur.UserRolePermissionKey = @UserRolePermissionKey");

            IEnumerable<Guid> currentSecurityGroupKeys;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                currentSecurityGroupKeys = connectionScope.Query<Guid>(query.ToString(),
                     new
                     {
                         UserRolePermissionKey = userRolePermissionKey
                     },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text);
            }

            // Find the security group keys that were removed.
            IEnumerable<Guid> removedSecurityGroupKeys = currentSecurityGroupKeys.Except(securityGroupKeys);

            // Remove security groups that are no longer associated with this role permission.
            foreach (Guid securityGroupKey in removedSecurityGroupKeys)
            {
                _userRoleRepository.DisassociateUserRolePermissionSecurityGroup(context.ToActionContext(), userRolePermissionKey, securityGroupKey);
            }

            // Find the security groups that were added
            IEnumerable<Guid> addedSecurityGroupKeys = securityGroupKeys.Except(currentSecurityGroupKeys);
            InsertUserRolePermissionSecurityGroups(context, userRolePermissionKey, addedSecurityGroupKeys);
        }

        private void InsertUserRoleOverrideGroups(Context context, Guid userRoleKey, IEnumerable<Guid> overrideGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroupKeys, "overrideGroupKeys");

            foreach (Guid overrideGroupKey in overrideGroupKeys)
            {
                _userRoleRepository.AssociateUserRoleOverrideGroup(context.ToActionContext(), userRoleKey, overrideGroupKey);
            }
        }

        private void InsertUserRolePermissionsRole(Context context, Guid rolePermissionKey, Guid[] associatedRoleKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(associatedRoleKeys, "associatedRoleKeys");

            foreach (Guid roleKey in associatedRoleKeys)
            {
                _userRoleRepository.AssociateUserRolePermissionRole(context.ToActionContext(), rolePermissionKey, roleKey);
            }
        }

        private void UpdateUserRoleOverrideGroups(Context context, Guid userRoleKey, IEnumerable<Guid> overrideGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroupKeys, "overrideGroupKeys");

            //Get the list of override group keys associated with this med item
            var query = new SqlBuilder();
            query.SELECT("ur.OverrideGroupKey")
                .FROM("Core.vw_UserRoleOverrideGroupCurrent ur")
                .WHERE("ur.UserRoleKey = @UserRoleKey");

            IEnumerable<Guid> currentoverrideGroupKeys;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                currentoverrideGroupKeys = connectionScope.Query<Guid>(query.ToString(),
                     new
                     {
                         UserRoleKey = userRoleKey
                     },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text);
            }

            //Find the override group keys that were removed.
            IEnumerable<Guid> removedOverrideGroupKeys = currentoverrideGroupKeys.Except(overrideGroupKeys);

            //Remove override groups that are no longer associated with this med item.
            foreach (Guid overrideGroupKey in removedOverrideGroupKeys)
            {
                _userRoleRepository.DisassociateUserRoleOverrideGroup(context.ToActionContext(), userRoleKey, overrideGroupKey);
            }

            // Find the override groups that were added
            IEnumerable<Guid> addedOverrideGroupKeys = overrideGroupKeys.Except(currentoverrideGroupKeys);
            InsertUserRoleOverrideGroups(context, userRoleKey, addedOverrideGroupKeys);
        }

        private void UpdateUserRolePermissionsRoles(Context context, Guid roleKey, Guid[] visitingUserRoleKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(visitingUserRoleKeys, "visitingUserRoleKeys");

            Guid? userRolePermissionKey = GetRolePermissionKeyByUserRole(roleKey);

            if (!userRolePermissionKey.HasValue)
            {
                return;
            }

            var query = new SqlBuilder();
            query.SELECT("ur.UserRoleKey")
                .FROM("Core.vw_UserRolePermissionRoleCurrent ur")
                .WHERE("ur.UserRolePermissionKey = @UserRolePermissionKey");

            IEnumerable<Guid> currentVisitingRoleKeys;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                currentVisitingRoleKeys = connectionScope.Query<Guid>(query.ToString(),
                     new
                     {
                         UserRolePermissionKey = userRolePermissionKey
                     },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text);
            }

            var removedRoleKeys = currentVisitingRoleKeys.Except(visitingUserRoleKeys);

            foreach(var removedRoleKey in removedRoleKeys)
            {
                _userRoleRepository.DisassociateUserRolePermissionRole(context.ToActionContext(), userRolePermissionKey.Value, removedRoleKey);
            }

            var addedRoleKeys = visitingUserRoleKeys.Except(currentVisitingRoleKeys);
            InsertUserRolePermissionsRole(context, userRolePermissionKey.Value, addedRoleKeys.ToArray());
        }

        private Guid? GetRolePermissionKeyByUserRole(Guid roleKey)
        {
            var query = new SqlBuilder();
            query.SELECT("ur.UserRolePermissionKey")
                .FROM("Core.vw_UserRolePermissionCurrent ur")
                .WHERE("ur.UserRoleKey = @UserRoleKey")
                .WHERE("ur.PermissionInternalCode = @PermissionInternalCode");

            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                return connectionScope.QueryFirstOrDefault<Guid>(query.ToString(),
                     new
                     {
                         UserRoleKey = roleKey,
                         PermissionInternalCode = PermissionInternalCode.GrantVisitorAccess.ToInternalCode()
                     },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text);
            }
        }

        private static XElement CreateUserAccountXmlElement(UserAccount userAccountAttribs)
        {
            XElement keyElement = new XElement("key");
            keyElement.SetAttributeValue("ActiveDirectoryObjectGloballyUniqueID", userAccountAttribs.ActiveDirectoryObjectGuid);
            keyElement.SetAttributeValue("ActiveDirectoryDomainKey", userAccountAttribs.ActiveDirectoryDomain.Key);
            keyElement.SetAttributeValue("UserID", userAccountAttribs.UserId);
            keyElement.SetAttributeValue("FirstName", userAccountAttribs.FirstName);
            keyElement.SetAttributeValue("MiddleName", userAccountAttribs.MiddleName);
            keyElement.SetAttributeValue("LastName", userAccountAttribs.LastName);
            keyElement.SetAttributeValue("FullName", userAccountAttribs.FullName);
            keyElement.SetAttributeValue("InitialsText", userAccountAttribs.Initials);
            keyElement.SetAttributeValue("EmailAddressValue", userAccountAttribs.EmailAddress);
            keyElement.SetAttributeValue("JobTitleText", userAccountAttribs.JobTitle);
            if (userAccountAttribs.AccountExpirationDate.HasValue) //add dates only if it has value.
                keyElement.SetAttributeValue("AccountExpirationLocalDateTime", userAccountAttribs.AccountExpirationDate.Value);
            if (userAccountAttribs.AccountExpirationUtcDate.HasValue)
                keyElement.SetAttributeValue("AccountExpirationUTCDateTime", userAccountAttribs.AccountExpirationUtcDate.Value);
            if (userAccountAttribs.DirectoryChangePasswordDateTime.HasValue) //add dates only if it has value.
                keyElement.SetAttributeValue("DirectoryChangePasswordLocalDateTime", userAccountAttribs.DirectoryChangePasswordDateTime.Value);
            if (userAccountAttribs.DirectoryChangePasswordUtcDateTime.HasValue)
                keyElement.SetAttributeValue("DirectoryChangePasswordUTCDateTime", userAccountAttribs.DirectoryChangePasswordUtcDateTime.Value);
            //replace boolean with int
            keyElement.SetAttributeValue("FingerprintExemptFlag", userAccountAttribs.IsFingerprintExempt ? 1 : 0);
            //  TODO
            //  keyElement.SetAttributeValue("CardPinExemptFlag", userAccountAttribs.IsCardPinExempt ? 1 : 0);
            keyElement.SetAttributeValue("SupportUserFlag", userAccountAttribs.IsSupportUser ? 1 : 0);
            if (userAccountAttribs.IsActive)
                keyElement.SetAttributeValue("ActiveFlag", 1);
            else
                keyElement.SetAttributeValue("ActiveFlag", 0);
            return keyElement;
        }

        private SqlBuilder GetAuthUserBaseQuery()
        {
            var query = new SqlBuilder();
            query.SELECT("uac.UserAccountKey")
                ._("uac.UserAccountSnapshotKey")
                ._("uac.LastPasswordExpirationNoticeUTCDateTime")
                ._("uac.LastPasswordExpirationNoticeLocalDateTime")
                ._("uac.LastSuccessfulPasswordAuthenticationUTCDateTime")
                ._("uac.LastSuccessfulPasswordAuthenticationLocalDateTime")
                ._("uac.LastFacilityKey")
                ._("uac.LastAreaKey")
                ._("uac.ActiveDirectoryDomainKey")
                ._("uac.CreatedAtDispensingDeviceKey")
                ._("uac.UserID")
                ._("uac.FirstName")
                ._("uac.MiddleName")
                ._("uac.LastName")
                ._("uac.SuffixText")
                ._("uac.FullName")
                ._("uac.InitialsText")
                ._("uac.JobTitleText")
                ._("uac.EmailAddressValue")
                ._("uac.ContactInformation")
                ._("uac.ScanCodeValue")
                ._("uac.AccountExpirationUTCDateTime")
                ._("uac.AccountExpirationLocalDateTime")
                ._("uac.FingerprintExemptFlag")
                ._("uac.TemporaryFlag")
                ._("uac.LockedFlag")
                ._("uac.SuperUserFlag")
                ._("uac.SupportUserFlag")
                ._("uac.UserTypeKey")
                ._("uac.ActiveDirectoryObjectGloballyUniqueId")
                ._("uac.DirectoryChangePasswordUTCDateTime")
                ._("uac.DirectoryChangePasswordLocalDateTime")
                ._("uac.CardPINExemptFlag")
                ._("uac.CardSerialID")
                ._("uac.RFIDCardSerialID")
                ._("uac.ActiveFlag")
                ._("uac.LastModifiedDispensingDeviceKey")
                ._("uac.LastModifiedActorKey")
                ._("uac.LastModifiedBinaryValue")
                .FROM("Core.vw_UserAccountCurrent uac");

            return query;
        }

        #endregion
    }
}
