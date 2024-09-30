using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Services.Cryptography;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;

namespace CareFusion.Dispensing.Services
{
    public static class PasswordChangeValidator
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static void SetUserAccountPasswordHash(PasswordCredential passwordCredential, EncryptionAlgorithmInternalCode encryptionAlgorithm)
        {
            ValidatePassword(encryptionAlgorithm, passwordCredential.Password);
            SetPasswordHash(passwordCredential, encryptionAlgorithm);
        }

        public static void SetUserAccountPasswordHashWithoutValidation(PasswordCredential passwordCredential, EncryptionAlgorithmInternalCode encryptionAlgorithm)
        {
            //for domain account pwd and updating with the current encryption algorithm, ignore local pwd policy validation
            SetPasswordHash(passwordCredential, encryptionAlgorithm);
        }

        private static void SetPasswordHash(PasswordCredential passwordCredential, EncryptionAlgorithmInternalCode encryptionAlgorithm)
        {
            string salt = null;
            ITextEncryptor encryptor = TextEncryptorFactory.GetEncryptor(encryptionAlgorithm);
            if (encryptor.SupportsSalt)
                salt = SaltGenerator.GenerateSalt();

            string passwordHash = encryptor.GenerateHash(passwordCredential.Password, salt);

            passwordCredential.EncryptionAlgorithm = encryptionAlgorithm;
            passwordCredential.Salt = salt;
            passwordCredential.PasswordHash = passwordHash;
        }

        public static IEnumerable<string> ListPasswordRules(DispensingSystem systemSettings)
        {
            List<string> rules = new List<string>();

            // user id and password dont match rule
            rules.Add(ValidationStrings.UserAccountUserNameIdPasswordNoMatchRule);

            // min length rule
            rules.Add(string.Format(ValidationStrings.UserAccountPasswordLengthRule, systemSettings.MinimumPasswordLength));

            // max length rule
            if (systemSettings.EncryptionAlgorithm == EncryptionAlgorithmInternalCode.PHA)
            {
                rules.Add(ValidationStrings.UserAccountLegacyPasswordRule);
            }
            else
            {
                rules.Add(ValidationStrings.UserAccountPasswordRule);
            }

            // password strength rule
            string complexity = GetPasswordComplexityString(systemSettings);
            if(!string.IsNullOrEmpty(complexity))
                rules.Add(complexity);

            // uniqueness rule
            rules.Add(string.Format(
                        ValidationStrings.UserAccountPasswordUniquenessRule,
                        systemSettings.PasswordHistory));

            // age rule
            rules.Add(string.Format(
                            ValidationStrings.UserAccountPasswordAgeRule,
                            systemSettings.MinimumPasswordAge));

            return rules;
        }

        public static void ValidatePassword(EncryptionAlgorithmInternalCode algorithm, string password)
        {
            if (algorithm == EncryptionAlgorithmInternalCode.PHA)
            {
                if (password.Length > 8 || ContainsSpecialCharacters(password))
                {
                    throw new ValidationException(
                        ValidationStrings.UserAccountLegacyPasswordRule,
                            new [] {
                            ValidationError.CreateValidationError<PasswordCredential>(
                                p => p.Password,
                        ValidationStrings.UserAccountLegacyPasswordRule)
                    });
                }
            }
            else
            {
                if (password.Length > ValidationConstants.UserPasswordUpperBound)
                {
                    throw new ValidationException(
                        ValidationStrings.UserAccountPasswordRule,
                        new [] { ValidationError.CreateValidationError<PasswordCredential>(
                            p => p.Password,
                        ValidationStrings.UserAccountPasswordRule)
                    });
                }
            }
        }

        private static bool ContainsSpecialCharacters(string password)
        {
            foreach (char c in password)
            {
                if (Char.IsPunctuation(c) || Char.IsSymbol(c))
                    return true;
            }
            return false;
        }

        public static void ValidateUserPasswordChange(
           DispensingSystem systemSettings,
           AuthUserAccount userAccount,
           PasswordCredential currentPasswordCredential,
           IEnumerable<PasswordCredential> pastCredentials,
           PasswordCredential newPasswordCredential,
           string oldPassword)
        {
            List<string> validationErrors = new List<string>();
            //if current pwd does not match then throw exception from here
            string err = ValidateMatch(currentPasswordCredential, oldPassword);
            if (!string.IsNullOrEmpty(err))
                IfErrorThrowDispensingFault(new List<string>{err});

            validationErrors.AddRange(ValidateUserIdLengthStrength(userAccount.UserId, userAccount.LastName, userAccount.FirstName,
                systemSettings, newPasswordCredential));

            err = ValidateUniqueness(newPasswordCredential.Password, pastCredentials, systemSettings.PasswordHistory);
            if (!string.IsNullOrEmpty(err))
                validationErrors.Add(err);

            err = ValidateCharDifference(oldPassword, newPasswordCredential.Password, systemSettings.PasswordContentCheck);
            if (!string.IsNullOrEmpty(err))
                validationErrors.Add(err);

            err = ValidateDictionaryWord(newPasswordCredential.Password, systemSettings.PasswordContentCheck);
            if (!string.IsNullOrEmpty(err))
                validationErrors.Add(err);

            if (!IsPasswordExpired(systemSettings, currentPasswordCredential))
            {
                err = ValidateAge(currentPasswordCredential, systemSettings.MinimumPasswordAge);
                if (!string.IsNullOrEmpty(err))
                    validationErrors.Add(err);
            }
            IfErrorThrowDispensingFault(validationErrors);
        }

        private static string ValidateCharDifference(string oldPwd, string newPwd, bool passwordContentcheck)
        {
            if (passwordContentcheck)
            {
                var oldPassword = oldPwd.ToUpper();
                var newPassword = newPwd.ToUpper();
                if (oldPassword.Length > newPassword.Length) newPassword = newPassword.PadRight(oldPassword.Length);
                else oldPassword = oldPassword.PadRight(newPassword.Length);
                if ((oldPassword.Zip(newPassword, (b, a) => a != b).Count(t => t)) < 4)
                {
                    return ValidationStrings.PasswordFourCharDiffRule;
                }
            }
            return null;
        }

        private static IEnumerable<ValidationError> CreateValidationErrorList(List<string> errors)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            foreach (string error in errors)
            {
                validationErrors.Add(ValidationError.CreateValidationError<PasswordCredential>(error));
            }
            return validationErrors;
        }

        private static void IfErrorThrowDispensingFault(List<string> validationErrors)
        {
            if (validationErrors.Count > 0)
            {
                throw new ValidationException(
                    ValidationStrings.PasswordComplianceError,
                    CreateValidationErrorList(validationErrors));
            }
        }

        public static void ValidateUserPassword(
            UserAccount account,
            DispensingSystem systemSettings,
            PasswordCredential newPasswordCredential)
        {
            IfErrorThrowDispensingFault(ValidateUserIdLengthStrength(account.UserId, account.LastName, account.FirstName,
                systemSettings, newPasswordCredential));
        }

        private static List<string> ValidateUserIdLengthStrength(string userId, string lastName, string firstName,
            DispensingSystem systemSettings,
            PasswordCredential newPasswordCredential)
        {
            List<string> validationErrors = new List<string>();
            string err = ValidatePasswordForUserIdAndName(systemSettings,userId, lastName, firstName, newPasswordCredential);
            if (!string.IsNullOrEmpty(err))
                validationErrors.Add(err);
            err = ValidateLength(newPasswordCredential.Password, systemSettings.MinimumPasswordLength);
            if (!string.IsNullOrEmpty(err))
                validationErrors.Add(err);
            err = ValidateStrength(newPasswordCredential.Password, systemSettings);
            if (!string.IsNullOrEmpty(err))
                validationErrors.Add(err);
            return validationErrors;
        }

        public static bool IsPasswordExpired(DispensingSystem systemSettings, PasswordCredential passwordCredential)
        {

            //if user has never changed password or pwd is reset then redirect him to change password
            if (!passwordCredential.UserChangedOwnPasswordUtcDate.HasValue)
                return true;

            //Dispensing System has pwd policy to be never expired
            if (!systemSettings.PasswordExpiration.HasValue)
                return false;

            //check if current datetime is greater than (last modified + expire duration)
            if (DateTime.UtcNow > passwordCredential.UserChangedOwnPasswordUtcDate.Value.AddDays(systemSettings.PasswordExpiration.Value))
                return true;

            return false;
        }

        public static void ValidateDifferentFromUserIdAndName(DispensingSystem systemsettings,PasswordCredential credential, UserAccount account)
        {
            string err = ValidatePasswordForUserIdAndName(systemsettings,account.UserId, account.LastName, account.FirstName, credential);
            if (!string.IsNullOrEmpty(err))
                IfErrorThrowDispensingFault(new List<string>{err});
        }

        #region Private Helper Methods

        private static string ValidatePasswordForUserIdAndName(DispensingSystem systemsettings,string userId, string lastName, string firstName, PasswordCredential credential)
        {
            /* if PasswordCredential has Password value in plain text then we will do all our check against
             * plain text. This guarantee no case sensitive check.
             * If it does not containt plain text password then we will match hash value in which case
             * we cannot guarantee case in-sensitive match.
             */
            if (string.IsNullOrEmpty(credential.Password))
                return ValidateDifferentFromHashUserIdAndName(userId, lastName, firstName, credential);
            return ValidateDifferentFromTextUserIdAndName(systemsettings,userId, lastName, firstName, credential);
        }

        private static string ValidateDifferentFromHashUserIdAndName(string userId, string lastName, string firstName, PasswordCredential credential)
        {
            // hash the user id using the salt of the credential
            ITextEncryptor encryptor = TextEncryptorFactory.GetEncryptor(credential.EncryptionAlgorithm);

            string passwordHash = encryptor.GenerateHash(userId.Trim(), credential.Salt);
            string err = ValidateDifferentFromUserIdName(credential.PasswordHash, passwordHash);
            if (!string.IsNullOrEmpty(err))
                return err;
            string lastNameHash = encryptor.GenerateHash(lastName.Trim(), credential.Salt);
            //no gurantee of case in-sensitive match
            err = ValidateDifferentFromUserIdName(credential.PasswordHash, lastNameHash);
            if (!string.IsNullOrEmpty(err))
                return err;
            if (!string.IsNullOrEmpty(firstName))
            {
                string firstNameHash = encryptor.GenerateHash(firstName.Trim(), credential.Salt);
                //no gurantee of case in-sensitive match
                err = ValidateDifferentFromUserIdName(credential.PasswordHash, firstNameHash);
                if (!string.IsNullOrEmpty(err))
                    return err;
            }

            return null;
        }

        private static string ValidateDifferentFromTextUserIdAndName(DispensingSystem systemsettings, string userId, string lastName, string firstName, PasswordCredential credential)
        {
            if (!systemsettings.PasswordContentCheck)
            {
                //gurantees case insensitive match))
                if (credential.Password.Equals(userId, StringComparison.CurrentCultureIgnoreCase) ||
                    credential.Password.Equals(lastName, StringComparison.CurrentCultureIgnoreCase) ||
                    ((!string.IsNullOrEmpty(firstName)) &&
                     credential.Password.Equals(firstName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return ValidationStrings.UserAccountUserNameIdPasswordNoMatchRule;
                }
                return null;
            }

            //if DoD, check if password contains UserId, Last Name or First Name
            if (Regex.IsMatch(credential.Password, userId, RegexOptions.IgnoreCase)||
                    Regex.IsMatch(credential.Password, lastName, RegexOptions.IgnoreCase) ||
                    ((!string.IsNullOrEmpty(firstName)) &&
                     Regex.IsMatch(credential.Password, firstName, RegexOptions.IgnoreCase)))
            {
                return ValidationStrings.UserAccountNoPersonalInfoRule;
            }
            return null;

        }

        private static string ValidateDifferentFromUserIdName(string password, string userIdOrName)
        {

            if (password.Equals(userIdOrName, StringComparison.CurrentCultureIgnoreCase))
            {
                return ValidationStrings.UserAccountUserNameIdPasswordNoMatchRule;
            }

            return null;
        }

        private static string ValidateMatch(PasswordCredential currentCredential, string currentPassword)
        {
            if (!PasswordMatches(currentCredential, currentPassword))
                return ValidationStrings.UserAccountPasswordDoesNotMatch;

            return null;
        }

        private static bool PasswordMatches(PasswordCredential currentCredential, string currentPassword)
        {
            ITextEncryptor encryptor = TextEncryptorFactory.GetEncryptor(currentCredential.EncryptionAlgorithm);
            return encryptor.IsMatch(
                currentPassword,
                currentCredential.Salt,
                currentCredential.PasswordHash);
        }

        private static string ValidateLength(string password, int minLength)
        {
            if (password.Length < minLength)
            {
                return string.Format(ValidationStrings.UserAccountPasswordLengthRule, minLength);
            }

            return null;
        }

        private static string ValidateStrength(string password, DispensingSystem dispensingSystem)
        {
            if (!IsStrengthValid(password, dispensingSystem))
            {
                return GetPasswordComplexityString(dispensingSystem);
            }

            return null;
        }


        private static string GetPasswordComplexityString(DispensingSystem dispensingSystem)
        {
            bool isSetting = false;
            StringBuilder sb = new StringBuilder();
            sb.Append(ValidationStrings.UserAccountPasswordComplexityRule);
            if (dispensingSystem.PasswordMinimumUppercaseQuantity > 0)
            {
                sb.Append(string.Format(ValidationStrings.UserAccountPasswordComplexityRuleUpperCase,
                                        dispensingSystem.PasswordMinimumUppercaseQuantity));
                sb.Append(',');
                isSetting = true;
            }
            if (dispensingSystem.PasswordMinimumLowercaseQuantity > 0)
            {
                sb.Append(string.Format(ValidationStrings.UserAccountPasswordComplexityRuleLowerCase,
                                        dispensingSystem.PasswordMinimumLowercaseQuantity));
                sb.Append(',');
                isSetting = true;
            }
            if (dispensingSystem.PasswordMinimumDigitQuantity > 0)
            {
                sb.Append(string.Format(ValidationStrings.UserAccountPasswordComplexityRuleDigit,
                                        dispensingSystem.PasswordMinimumDigitQuantity));
                sb.Append(',');
                isSetting = true;
            }
            if (dispensingSystem.PasswordMinimumSpecialCharacterQuantity > 0)
            {
                sb.Append(string.Format(ValidationStrings.UserAccountPasswordComplexityRuleSpecialChars,
                                        dispensingSystem.PasswordMinimumSpecialCharacterQuantity));
                sb.Append(',');
                isSetting = true;
            }
            //Remove last ,
            sb = sb.Remove(sb.ToString().Length - 1, 1);
            if(isSetting)
                return sb.ToString();
            return "";
        }

        private static bool IsStrengthValid(IEnumerable<char> password, DispensingSystem dispensingSystem)
        {
            //Note RDS 10-6-2010: dispensingSystem values can be null having the validator always return false
            //Vasant you might want to refactor this or not allow nulls
            DispensingSystem ds = NullsToZero(dispensingSystem);

            int upperCaseCount = 0, lowerCaseCount = 0, digitCount = 0, specialCharsCount = 0;
            foreach (char c in password)
            {
                if (Char.IsLower(c))
                    lowerCaseCount++;
                if (Char.IsUpper(c))
                    upperCaseCount++;
                if (Char.IsDigit(c))
                    digitCount++;
                if (Char.IsSymbol(c) || Char.IsPunctuation(c))
                    specialCharsCount++;
            }
            if(upperCaseCount >= ds.PasswordMinimumUppercaseQuantity &&
                lowerCaseCount >= ds.PasswordMinimumLowercaseQuantity &&
                digitCount >= ds.PasswordMinimumDigitQuantity &&
                specialCharsCount >= ds.PasswordMinimumSpecialCharacterQuantity)
                return true;

            return false;
        }

        private static DispensingSystem NullsToZero(DispensingSystem dispensingSystem)
        {
            var result = dispensingSystem;
            result.PasswordMinimumDigitQuantity = dispensingSystem.PasswordMinimumDigitQuantity;
            result.PasswordMinimumLowercaseQuantity = dispensingSystem.PasswordMinimumLowercaseQuantity;
            result.PasswordMinimumSpecialCharacterQuantity = dispensingSystem.PasswordMinimumSpecialCharacterQuantity;
            result.PasswordMinimumUppercaseQuantity = dispensingSystem.PasswordMinimumUppercaseQuantity;
            return result;
        }

        private static string ValidateUniqueness(string password, IEnumerable<PasswordCredential> history, short? passwordHistory)
        {
            if (passwordHistory.HasValue &&
                !IsUniquenessValid(password, history))
            {
                return string.Format(ValidationStrings.UserAccountPasswordUniquenessRule,
                                     passwordHistory.Value);
            }

            return null;
        }

        private static bool IsUniquenessValid(string password, IEnumerable<PasswordCredential> history)
        {
            foreach (PasswordCredential credential in history)
            {
                if (IsMatch(password, credential))
                    return false;
            }
            return true;
        }

        private static bool IsMatch(string password, PasswordCredential credential)
        {
            ITextEncryptor encryptor = TextEncryptorFactory.GetEncryptor(credential.EncryptionAlgorithm);
            return encryptor.IsMatch(password, credential.Salt, credential.PasswordHash);
        }

        private static string ValidateAge(PasswordCredential passwordCredential, int minimumAgeInDays)
        {
            if (passwordCredential.UserChangedOwnPasswordUtcDate != null)
            {
                DateTime nextAllowableChange =
                    passwordCredential.UserChangedOwnPasswordUtcDate.Value.AddDays(minimumAgeInDays);

                // Is minimum age valid
                if (DateTime.UtcNow < nextAllowableChange)
                {
                    return string.Format(
                        ValidationStrings.UserAccountPasswordAgeRule, minimumAgeInDays);
                }
            }

            return null;
        }

        private static string ValidateDictionaryWord(string passwordText, bool passwordContentcheck)
        {
            if (passwordContentcheck)
            {
               var result = false;
                Stopwatch sw = Stopwatch.StartNew();
                using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
                {
                   result = repository.PasswordContainsDictionaryWord(passwordText);
                }
                Log.Debug("GetPasswordCredential() query took {0} milliseconds.", sw.ElapsedMilliseconds);
                if (result)
                {
                    return ValidationStrings.PasswordContainNoDictionaryWord;
                }
            }
            return null;
        }

        #endregion
    }
}
