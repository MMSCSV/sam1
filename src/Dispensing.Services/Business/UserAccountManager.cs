using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Entities;
using CareFusion.Dispensing.Data.Repositories;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Resources;
using LinqKit;
using Pyxis.Core.Data;
using MsValidation = Microsoft.Practices.EnterpriseLibrary.Validation.Validation;

namespace CareFusion.Dispensing.Services.Business
{
    public class UserAccountManager : IUserAccountManager
    {
        private readonly IDispensingSystemRepository _dispensingSystemRepository;
        private readonly IAuthenticationEventRepository _authenticationEventRepository;

        #region Contructors

        public UserAccountManager()
        {
            _dispensingSystemRepository = new DispensingSystemRepository();
            _authenticationEventRepository = new AuthenticationEventRepository();
        }

        #endregion

        #region IUserAccountManager Members

        public bool IsActiveDirectoryIntegrated()
        {
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                // If there is at least one user with a reference to an active directory domain
                // then we are considered integrated.
                return repository.GetQueryableEntity<UserAccountEntity>()
                            .Where(ua => ua.SupportUserFlag == false)
                            .Any(ua => ua.ActiveDirectoryDomainKey != null);
            }
        }

        public UserAccount Get(Guid userAccountKey)
        {
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                return repository.GetUserAccount(userAccountKey);
            }
        }

        public UserRecentAuthenticationAttempt GetRecentAuthenticationAttempt(Guid userAccountKey, Guid ignoreAuthenticationEventKey)
        {
            return _authenticationEventRepository.GetRecentAuthenticationAttempt(userAccountKey, ignoreAuthenticationEventKey);
        }

        public virtual Guid Add(Context context, UserAccount userAccount)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccount, "userAccount");

            Guid userAccountKey;
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                if (context.Actor is UserActor)
                {
                    // Validate the user account
                    Validate(repository, userAccount, context);
                }

                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    // Create user account.
                    userAccountKey = repository.InsertUserAccount(context, userAccount);

                    // Create password credential, if we have one.
                    if (userAccount.PasswordCredential != null)
                    {
                        repository.InsertPasswordCredential(context, userAccountKey,
                                                            userAccount.PasswordCredential);
                    }

                    tx.Complete();
                }
            }

            return userAccountKey;
        }

        public virtual void Update(Context context, UserAccount userAccount)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccount, "userAccount");

            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                // Check if the user account exists.
                if (!repository.Exists<UserAccountEntity>(ua => ua.Key == userAccount.Key))
                    throw new EntityNotFoundException(ValidationStrings.UserAccountDeleted);

                if (context.Actor is UserActor)
                {
                    // Validate the user account
                    Validate(repository, userAccount, context);
                }

                using (new RepositorySessionScope())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    // Update user account
                    repository.UpdateUserAccount(context, userAccount);

                    if (userAccount.PasswordCredential != null)
                    {
                        // Update password credential
                        repository.UpdatePasswordCredential(context, userAccount.Key,
                                                            userAccount.PasswordCredential);
                    }

                    // Complete transaction
                    tx.Complete();
                }
            }
        }

        public void AddUserEncounterAssociation(Context context, Guid userAccountKey, Guid encounterKey)
        {
            Guard.ArgumentNotNull(context, "context");

            using (new RepositorySessionScope())
            using (var tx = TransactionScopeFactory.Create())
            using (IAdtRepository repository = RepositoryFactory.Create<IAdtRepository>())
            {
                repository.InsertUserEncounterAssociation(context, userAccountKey, encounterKey);

                // complete transaction
                tx.Complete();
            }
        }

        public void RemoveUserEncounterAssociations(Context context, Guid userAccountKey, IEnumerable<Guid> encounterKeys)
        {
            Guard.ArgumentNotNull(context, "context");

            using (new RepositorySessionScope())
            using (var tx = TransactionScopeFactory.Create())
            using (IAdtRepository repository = RepositoryFactory.Create<IAdtRepository>())
            {
                // delete encounters
                foreach (var encounterKey in encounterKeys)
                {
                    repository.DeleteUserEncounterAssociation(context, userAccountKey, encounterKey);
                }

                // complete transaction
                tx.Complete();
            }
        }

        public UserMergeStatus MapLocalUserAccountToDomainUserAccount(Context context, Guid localUserKey, Guid domainUserKey)
        {
            UserMergeStatus userMergeStatus = new UserMergeStatus(); 

            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(context, "localUserKey");
            Guard.ArgumentNotNull(context, "domainUserKey");

            using (var tx = TransactionScopeFactory.Create())
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                //save the device context before setting it to null
                var deviceInfo = context.Device;
                //set the context.Device = null as we don't want to set the LastModifiedDispensingDeviceKey when user registers from station. Otherwise server sync will filter out data that gets updated.
                context.Device = null;

                //return error if the domain user is already registered
                userMergeStatus = repository.IsUserAccountRegistered(domainUserKey);
                if (userMergeStatus != null && userMergeStatus.Code == UserMergeStatusCode.UserMergeError)
                    return userMergeStatus;

                //return error if the local user has unresolved My Items
                if (repository.UserHasUnResolvedMyItems(localUserKey))
                {
                    userMergeStatus = new UserMergeStatus
                    {
                        Code = UserMergeStatusCode.UserMergeError,
                        Description = ServiceResources.UserAccountHasUnresolvedMyItems
                    };
                    return userMergeStatus;
                }

            //get the useraccounts
            var localUser = repository.GetUserAccount(localUserKey);
                var domainUser = repository.GetUserAccount(domainUserKey);

                //copy missing fields from localuser account
                if (localUser.FacilityKeys != null)
                {
                    if (domainUser.FacilityKeys == null || domainUser.FacilityKeys.Length == 0)
                        domainUser.FacilityKeys = localUser.FacilityKeys;
                    else
                        domainUser.FacilityKeys = localUser.FacilityKeys.Union(domainUser.FacilityKeys).ToArray();
                }

                if (localUser.UserRoles != null)
                {
                    if (domainUser.UserRoles == null || domainUser.UserRoles.Length == 0)
                        domainUser.UserRoles = localUser.UserRoles;
                    else
                    {
                        var commonUserRoleKeys = localUser.UserRoles
                            .Where(lr =>
                                domainUser.UserRoles.Any(
                                    dr => dr.RoleKey == lr.RoleKey && !lr.MedicationTemporaryAccess))
                            .Select(ur => ur.RoleKey);
                        var localUserOnlyRoles =
                            localUser.UserRoles.Where(lr => !commonUserRoleKeys.Contains(lr.RoleKey));
                        foreach (var commonUserRoleKey in commonUserRoleKeys)
                        {
                            var localUserAreas = localUser.UserRoles.Where(ur => ur.RoleKey == commonUserRoleKey)
                                .SelectMany(ur => ur.Areas);
                            domainUser.UserRoles.Where(dr => dr.RoleKey == commonUserRoleKey).ToList()
                                .ForEach(dr => dr.Areas = dr.Areas.Union(localUserAreas).ToArray());
                        }

                        domainUser.UserRoles = domainUser.UserRoles.Concat(localUserOnlyRoles).ToArray();
                    }
                }

                if (string.IsNullOrEmpty(domainUser.ScanCode))
                    domainUser.ScanCode = localUser.ScanCode;
                if (localUser.IsFingerprintExempt)
                    domainUser.IsFingerprintExempt = true;
                if (domainUser.UserTypeKey == null)
                    domainUser.UserTypeKey = localUser.UserTypeKey;
                if (localUser.IsSuperUser)
                    domainUser.IsSuperUser = true;
                if (domainUser.CardSerialId == null)
                    domainUser.CardSerialId = localUser.CardSerialId;
                if (domainUser.RFIDCardSerialID == null)
                    domainUser.RFIDCardSerialID = localUser.RFIDCardSerialID;
                if (localUser.IsCardPinExempt)
                    domainUser.IsCardPinExempt = true;

                //get domain user's registered finger prints 
                var domainUserFingerPrint = repository.GetUserFingerprint(domainUserKey);
                if (domainUserFingerPrint == null)
                {
                    var userFingerPrints = repository.GetUserFingerprint(localUserKey);
                    if (userFingerPrints != null)
                    {
                        userFingerPrints.UserAccountKey = domainUserKey;

                        //copy local users's finger prints to the domain user
                        repository.InsertUserFingerprint(context, userFingerPrints);
                    }
                }

            //update domain user account 
                repository.UpdateUserAccount(context, domainUser);

                //map all user lists to the domain user
                repository.MapLocalUserListsToDomainUser(context, localUserKey, domainUserKey);

                //deactivate local user after successful conversion
                localUser.IsActive = false;
                repository.UpdateUserAccount(context, localUser);

                //set the device context back as we need the device key to be set while inserting the user merge association (for registrations at the station). 
                //this will not affect sync because the user merge table doesn't have the LastModifiedDepensingDeviceKey
                context.Device = deviceInfo;
                //Insert user merge association
                repository.InsertUserMergeAssociation(context, localUserKey, domainUserKey);

                tx.Complete();

            }

            //set the user merge status to succeesul
            userMergeStatus = new UserMergeStatus
            {
                Code = UserMergeStatusCode.UserMergeSuccessful,
                Description = ServiceResources.UserAccountRegistrationSuccessful
            };
            return userMergeStatus;
        }

        public UserMergeSummary GetMergeSummaryForUser(Guid userAccountKey)
        {
            Guard.ArgumentNotNull(userAccountKey, "userAccountKey");

            using (new RepositorySessionScope())
            using (var tx = TransactionScopeFactory.Create())
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                return repository.GetUserMergeSummary(userAccountKey);
            }
        }

        #endregion

        #region Private Members

        private void Validate(ICoreRepository repository, UserAccount userAccount, Context context)
        {
            Guard.ArgumentNotNull(userAccount, "userAccount");
            DispensingSystem systemSettings = _dispensingSystemRepository.GetDispensingSystem();

            List<ValidationError> validationErrors = new List<ValidationError>();

            // Validate user account
            var validationResults = MsValidation.Validate(userAccount);
            if (validationResults.IsValid)
            {
                // Validate password credential.
                if (userAccount.PasswordCredential != null)
                {
                    PasswordCredential passwordCredential = userAccount.PasswordCredential;
                    if (passwordCredential.Password != null)
                    {
                        // Validate Password Credential
                        validationResults = MsValidation.Validate(userAccount.PasswordCredential);
                        if (!validationResults.IsValid)
                        {
                            validationErrors.AddRange(validationResults.ToValidationErrorsArray());
                        }
                        else
                        {
                            if (systemSettings == null)
                                throw new EntityNotFoundException(DataResources.LoadFailed_DispensingSystemNotFound);

                            // Reset password.
                            passwordCredential.UserChangedOwnPasswordDate = null;
                            passwordCredential.UserChangedOwnPasswordUtcDate = null;

                            // Set the password hash
                            PasswordChangeValidator.SetUserAccountPasswordHash(passwordCredential, systemSettings.EncryptionAlgorithm);

                            // Validate the user password
                            PasswordChangeValidator.ValidateUserPassword(userAccount, systemSettings, passwordCredential);
                        }
                    }
                    else if (!userAccount.IsTransient())
                    {
                        // validate that the password is different than the user ID
                        // (for cases where the user ID was changed but not the password)
                        PasswordChangeValidator.ValidateDifferentFromUserIdAndName(systemSettings,passwordCredential, userAccount);
                    }
                }

                Guid? activeDirectoryDomainKey = userAccount.ActiveDirectoryDomain != null
                                                     ? userAccount.ActiveDirectoryDomain.Key
                                                     : default(Guid?);

                //Fix: Bug# 34060. Exclude domain constraint while checking UserId uniqueness when the context.Actor is UserActor
                Expression<Func<UserAccountEntity, bool>> existsPredicate = null;
                if (context.Actor is UserActor)
                    existsPredicate = (ua) =>
                                      (ua.UserID == userAccount.UserId);

                else
                    existsPredicate = (ua) =>
                                      (ua.UserID == userAccount.UserId &&
                                       object.Equals(ua.ActiveDirectoryDomainKey, activeDirectoryDomainKey));

                if (!userAccount.IsTransient())
                {
                    // If user account is not transient then don't include itself.
                    existsPredicate = existsPredicate.And(ua => ua.Key != userAccount.Key);
                    //identify if userId is changed. (Note: If we dont have this check then any field of the local account cannot be edited)
                    UserAccount oldUserIdAccount = repository.GetUserAccount(userAccount.Key);
                    if (oldUserIdAccount != null)
                    {
                        //If userId is not modified then include domain constraint into predicate
                        if (oldUserIdAccount.UserId.Equals(userAccount.UserId, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (context.Actor is UserActor)
                                existsPredicate =
                                    existsPredicate.And(ua => ua.ActiveDirectoryDomainKey == activeDirectoryDomainKey);
                        }
                    }
                }

                if (repository.Exists(existsPredicate))
                {
                    //Get the existing user account detail to form error string
                    StringBuilder sb = new StringBuilder();
                    sb.Append(ValidationStrings.UserAccountUserIdNotUnique);
                    IEnumerable<UserAccount> existingAccounts = repository.GetUserAccountByUserId(userAccount.UserId);
                    foreach (var existingAccount in existingAccounts)
                    {
                        if (existingAccount.ActiveDirectoryDomain != null)
                        {
                            var domainName = existingAccount.ActiveDirectoryDomain.Name ?? existingAccount.ActiveDirectoryDomain.FullyQualifiedName;
                            sb.Append(domainName + "\\" + existingAccount.FullName + "; ");
                        }
                        else
                            sb.Append(existingAccount.FullName + "; ");
                    }
                    string duplicateUserIdError = sb.ToString();
                    duplicateUserIdError = duplicateUserIdError.TrimEnd(' ', ';');
                    validationErrors.Add(ValidationError.CreateValidationError<UserAccount>(
                        ua => ua.UserId,
                        duplicateUserIdError));
                }
            }
            else
            {
                validationErrors.AddRange(validationResults.ToValidationErrorsArray());
            }

            if (validationErrors.Count > 0)
            {
                throw new ValidationException(
                        string.Format(CultureInfo.CurrentCulture,
                            ServiceResources.Exception_Validation,
                            typeof(UserAccount).Name),
                        validationErrors);
            }
        }
        
        #endregion
    }
}
