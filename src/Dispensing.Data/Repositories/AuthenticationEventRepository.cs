using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IAuthenticationEventRepository Interface

    public interface IAuthenticationEventRepository
    {
        AuthenticationEvent GetAuthenticationEvent(Guid authenticationEventKey);

        AuthenticationEvent GetLastSuccessfulAuthenticationEvent(Guid userAccountKey);

        UserRecentAuthenticationAttempt GetRecentAuthenticationAttempt(Guid userAccountKey, Guid ignoreAuthenticationEventKey);

        int GetFailedLoginAttemptsCount(Guid? dispensingDeviceKey, Guid userAccountKey, DateTime startUtcTime, DateTime endUtcTime);

        Guid InsertAuthenticationEvent(AuthenticationEvent authenticationEvent);

        void UpdateAuthenticationEvent(Guid authenticationEventKey, DateTime authenticationUtcDateTime, DateTime authenticationDateTime, bool successfulSignIn, Guid witnessAuthenticationEventKey);

        void UpdateAuthenticationEvent(Context context, Guid authenticationEventKey, SignOutReasonInternalCode? signOutReason = null);

        void UpdateAuthenticationEventSwitchUser(Context context, Guid authenticationEventKey, SignOutReasonInternalCode? signOutReason = null);
    }

    #endregion

    public class AuthenticationEventRepository : RepositoryBase, IAuthenticationEventRepository
    {
        static AuthenticationEventRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(AuthenticationEvent),
                new ColumnAttributeTypeMapper<AuthenticationEvent>());

            SqlMapper.SetTypeMap(
                typeof(AuthenticationAttempt),
                new ColumnAttributeTypeMapper<AuthenticationAttempt>());
        }

        AuthenticationEvent IAuthenticationEventRepository.GetAuthenticationEvent(Guid authenticationEventKey)
        {
            AuthenticationEvent authenticationEvent = null;

            try
            {
                SqlBuilder query = GetAuthenticationEventQuery(true)
                    .WHERE("AuthenticationEventKey = @AuthenticationEventKey");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    authenticationEvent = connectionScope.Query<AuthenticationEvent>(
                        query.ToString(),
                        new { AuthenticationEventKey = authenticationEventKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return authenticationEvent;
        }

        AuthenticationEvent IAuthenticationEventRepository.GetLastSuccessfulAuthenticationEvent(Guid userAccountKey)
        {
            AuthenticationEvent authenticationEvent = null;

            try
            {
                SqlBuilder query = GetAuthenticationEventQuery(true)
                    .WHERE("UserAccountKey = @UserAccountKey")
                    .WHERE("SuccessfulAuthenticationFlag = 1")
                    .ORDER_BY("AuthenticationUTCDateTime DESC");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    authenticationEvent = connectionScope.Query<AuthenticationEvent>(
                        query.ToString(),
                        new { UserAccountKey = userAccountKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return authenticationEvent;
        }

        UserRecentAuthenticationAttempt IAuthenticationEventRepository.GetRecentAuthenticationAttempt(Guid userAccountKey, Guid ignoreAuthenticationEventKey)
        {
            UserRecentAuthenticationAttempt attempt = new UserRecentAuthenticationAttempt();

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multi = connectionScope.QueryMultiple(
                        "Core.bsp_GetLastUserAccountAuthenticationAttempts",
                        new
                        {
                            UserAccountKey = userAccountKey,
                            IgnoreAuthenticationEventKey = ignoreAuthenticationEventKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure))
                    {
                        var successfulAttemptResults = multi.Read<AuthenticationAttempt>();
                        var failedAttemptResults = multi.Read<AuthenticationAttempt>();

                        attempt = new UserRecentAuthenticationAttempt(
                            successfulAttemptResults.FirstOrDefault(),
                            failedAttemptResults.ToList());
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return attempt;
        }

        int IAuthenticationEventRepository.GetFailedLoginAttemptsCount(Guid? dispensingDeviceKey, Guid userAccountKey, DateTime startUtcTime, DateTime endUtcTime)
        {
            int failedAttempts = 0;

            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("COUNT(*) AS FailedAttemptCount")
                    .FROM("Core.AuthenticationEvent")
                    .WHERE("AuthenticationUTCDateTime > @StartAuthenticationUTCDateTime")
                    .WHERE("AuthenticationUTCDateTime <= @EndAuthenticationUTCDateTime")
                    .WHERE("SuccessfulAuthenticationFlag = 0")
                    .WHERE(dispensingDeviceKey != null ? "DispensingDeviceKey = @DispensingDeviceKey" : "DispensingDeviceKey IS NULL")
                    .WHERE("UserAccountKey = @UserAccountKey")
                    .WHERE("(AuthenticationMethodInternalCode = 'UID+PWD' OR AuthenticationMethodInternalCode = 'UID+PWD+W' OR AuthenticationMethodInternalCode = 'RFID+PWD')");
                
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    failedAttempts = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            StartAuthenticationUTCDateTime = startUtcTime,
                            EndAuthenticationUTCDateTime = endUtcTime,
                            UserAccountKey = userAccountKey,
                            DispensingDeviceKey = dispensingDeviceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (int)x.FailedAttemptCount)
                        .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return failedAttempts;
        }

        Guid IAuthenticationEventRepository.InsertAuthenticationEvent(AuthenticationEvent authenticationEvent)
        {
            Guard.ArgumentNotNull(authenticationEvent, "authenticationEvent");
            Guid? authenticationEventKey = null;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@AuthenticationPurposeInternalCode", authenticationEvent.AuthenticationPurposeInternalCode},
                             {"@AuthenticationMethodInternalCode", authenticationEvent.AuthenticationMethodInternalCode},
                             {"@ActiveDirectoryDomainKey", authenticationEvent.ActiveDirectoryDomainKey},
                             {"@UserID", authenticationEvent.UserId},
                             {"@UserAccountKey", authenticationEvent.UserAccountKey},
                             {"@AuthenticationUTCDateTime", authenticationEvent.AuthenticationUtcDateTime},
                             {"@AuthenticationLocalDateTime", authenticationEvent.AuthenticationDateTime},
                             {"@SuccessfulAuthenticationFlag", authenticationEvent.SuccessfullyAuthenticated},
                             {"@AuthenticationFailureReasonInternalCode", authenticationEvent.AuthenticationFailureReasonInternalCode},
                             {"@SuccessfulSignInFlag", authenticationEvent.SuccessfullySignedIn},
                             {"@WitnessAuthenticationEventKey", authenticationEvent.WitnessAuthenticationEventKey},
                             {"@DispensingDeviceKey", authenticationEvent.DispensingDeviceKey},
                             {"@WebBrowserFlag", authenticationEvent.IsWebBrowser},
                             {"@SwitchUserOnSignInFlag", authenticationEvent.SwitchUserOnSignIn},
                             {"@UserScanFlag", authenticationEvent.UserScan},
                             {"@AccessingAddressValue", authenticationEvent.AccessingAddress},
                             {"@AccessingMachineName", authenticationEvent.AccessingMachineName},
                             {"@SystemApplicationInternalCode", authenticationEvent.SystemApplicationInternalCode},
                             {"@AuthenticationResultInternalCode", authenticationEvent.AuthenticationResultInternalCode},
                             {"@CardReaderNotDetectedFlag", authenticationEvent.CardReaderNotDetected}
                         });
                    parameters.Add("@AuthenticationEventKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Execute(
                        "Core.usp_AuthenticationEventInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    authenticationEventKey = parameters.Get<Guid?>("@AuthenticationEventKey");
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return authenticationEventKey.GetValueOrDefault();
        }

        void IAuthenticationEventRepository.UpdateAuthenticationEvent(Guid authenticationEventKey, DateTime authenticationUtcDateTime,
            DateTime authenticationDateTime, bool successfulSignIn, Guid witnessAuthenticationEventKey)
        {
            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@SignOutUTCDateTime", default(DateTime?)},
                             {"@SignOutUTCDateTimeSpecifiedFlag", false},
                             {"@SignOutLocalDateTime", default(DateTime?)},
                             {"@SignOutLocalDateTimeSpecifiedFlag", false},
                             {"@SignOutReasonInternalCode", default(string)},
                             {"@SignOutReasonInternalCodeSpecifiedFlag", false},
                             {"@AuthenticationUTCDateTime", authenticationUtcDateTime},
                             {"@AuthenticationUTCDateTimeSpecifiedFlag", true},
                             {"@AuthenticationLocalDateTime", authenticationDateTime},
                             {"@AuthenticationLocalDateTimeSpecifiedFlag", true},
                             {"@SuccessfulSignInFlag", successfulSignIn},
                             {"@SuccessfulSignInFlagSpecifiedFlag", true},
                             {"@WitnessAuthenticationEventKey", witnessAuthenticationEventKey},
                             {"@WitnessAuthenticationEventKeySpecifiedFlag", true},
                             {"@AuthenticationEventKey", authenticationEventKey},
                         });

                    connectionScope.Execute(
                        "Core.usp_AuthenticationEventParentUpdate",
                        parameters,
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

        void IAuthenticationEventRepository.UpdateAuthenticationEvent(Context context, Guid authenticationEventKey,
            SignOutReasonInternalCode? signOutReason)
        {
            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@SignOutUTCDateTime", context.ActionUtcDateTime},
                             {"@SignOutLocalDateTime", context.ActionDateTime},
                             {"@SignOutReasonInternalCode", signOutReason.ToInternalCode()},
                             {"@AuthenticationEventKey", authenticationEventKey},
                         });

                    connectionScope.Execute(
                        "Core.usp_AuthenticationEventUpdate",
                        parameters,
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

        void IAuthenticationEventRepository.UpdateAuthenticationEventSwitchUser(Context context, Guid authenticationEventKey,
            SignOutReasonInternalCode? signOutReason)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@SignOutUTCDateTime", context.ActionUtcDateTime},
                             {"@SignOutUTCDateTimeSpecifiedFlag", true},
                             {"@SignOutLocalDateTime", context.ActionDateTime},
                             {"@SignOutLocalDateTimeSpecifiedFlag", true},
                             {"@SignOutReasonInternalCode", signOutReason.ToInternalCode()},
                             {"@SignOutReasonInternalCodeSpecifiedFlag", true},
                             {"@AuthenticationUTCDateTime", default(DateTime?)},
                             {"@AuthenticationUTCDateTimeSpecifiedFlag", false},
                             {"@AuthenticationLocalDateTime", default(DateTime?)},
                             {"@AuthenticationLocalDateTimeSpecifiedFlag", false},
                             {"@SuccessfulSignInFlag", default(bool?)},
                             {"@SuccessfulSignInFlagSpecifiedFlag", false},
                             {"@WitnessAuthenticationEventKey", default(Guid?)},
                             {"@WitnessAuthenticationEventKeySpecifiedFlag", false},
                             {"@AuthenticationEventKey", authenticationEventKey},
                         });

                    connectionScope.Execute(
                        "Core.usp_AuthenticationEventParentUpdate",
                        parameters,
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

        private SqlBuilder GetAuthenticationEventQuery(bool selectTop = true)
        {
            SqlBuilder query = new SqlBuilder();
            query.SELECT(selectTop ? "TOP 1 AuthenticationEventKey" : "AuthenticationEventKey")
                ._("AuthenticationPurposeInternalCode")
                ._("AuthenticationMethodInternalCode")
                ._("ActiveDirectoryDomainKey")
                ._("UserID")
                ._("UserAccountKey")
                ._("AuthenticationUTCDateTime")
                ._("AuthenticationLocalDateTime")
                ._("SuccessfulAuthenticationFlag")
                ._("AuthenticationFailureReasonInternalCode")
                ._("SuccessfulSignInFlag")
                ._("SignOutUTCDateTime")
                ._("SignOutLocalDateTime")
                ._("SignOutReasonInternalCode")
                ._("WitnessAuthenticationEventKey")
                ._("DispensingDeviceKey")
                ._("WebBrowserFlag")
                ._("SwitchUserOnSignInFlag")
                ._("SwitchUserOnSignOutFlag")
                ._("UserScanFlag")
                ._("AccessingAddressValue")
                ._("AccessingMachineName")
                ._("SystemApplicationInternalCode")
                ._("AuthenticationResultInternalCode")
                ._("LastModifiedBinaryValue")
                .FROM("Core.AuthenticationEvent");

            return query;
        }
    }
}
