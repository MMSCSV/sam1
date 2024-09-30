using System;
using System.Collections.Generic;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    public class AuthenticationResult
    {
        private AuthUserAccount _authUserAccount;
        private AuthenticationResultCode _resultCode;

        public AuthenticationResult()
        {
        }

        public AuthenticationResult(AuthenticationResultCode resultCode)
        {
            _resultCode = resultCode;
            AccessToken = null;
        }

        public AuthenticationResult(AuthenticationResultCode resultCode, string accessToken)
        {
            _resultCode = resultCode;
            AccessToken = accessToken;
        }

        public AuthenticationResultCode ResultCode
        {
            get { return _resultCode; }
            set { _resultCode = value; }
        }

        public string AccessToken { get; }

        public AuthenticationFailureReasonInternalCode? AuthenticationFailureReason { get; set; }

        public string ErrorMessage { get; set; }

        // TODO: Rename to UserAccount after refactoring.
        public AuthUserAccount AuthUserAccount
        {
            get { return _authUserAccount; }
            set 
            { 
                _authUserAccount = value;
                //UserAccount = _authUserAccount.GetOriginalUserAccount();
            }
        }

        public Guid? SignInEventKey { get; set; }

        public bool IsAuthenticated
        {
            get
            {
                return ResultCode == AuthenticationResultCode.Successful
                    || ResultCode == AuthenticationResultCode.ChangePasswordRequired
                    || ResultCode == AuthenticationResultCode.PasswordExpired;
            }
        }
        public int UserPasswordExpiresInDays { get; set; } = int.MaxValue; //by default set to never expires
    }
    
    public class MultipleUserAuthenticationResult : AuthenticationResult
    {
        public MultipleUserAuthenticationResult(AuthenticationResultCode resultCode)
            : base(resultCode)
        {
            
        }

        //public IEnumerable<UserAccount> MultipleUserAccount { get; set; }
        public IEnumerable<AuthUserAccount> MultipleAuthUserAccount { get; set; }
        public IEnumerable<ActiveDirectoryDomain> DomainList { get; set; }
    }
}
