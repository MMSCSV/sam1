using System.Collections.Generic;
using System.Linq;

namespace CareFusion.Dispensing.Contracts
{
    public class UserRecentAuthenticationAttempt
    {
        private readonly AuthenticationAttempt _successfulAuthenticationAttempt;
        private readonly AuthenticationAttempt _failedAuthenticationAttempt;
        private readonly IReadOnlyCollection<AuthenticationAttempt> _failedAuthenticationAttempts;
 
        public UserRecentAuthenticationAttempt()
        {
            _failedAuthenticationAttempts = new List<AuthenticationAttempt>();    
        }

        public UserRecentAuthenticationAttempt(AuthenticationAttempt successfulAuthenticationAttempt, IReadOnlyCollection<AuthenticationAttempt> failedAuthenticationAttempts)
        {
            _successfulAuthenticationAttempt = successfulAuthenticationAttempt;

            if (failedAuthenticationAttempts != null)
            {
                _failedAuthenticationAttempt = failedAuthenticationAttempts
                    .OrderByDescending(x => x.AuthenticationUtcDateTime)
                    .FirstOrDefault();

                _failedAuthenticationAttempts = failedAuthenticationAttempts;      
            }
            else
            {
                _failedAuthenticationAttempts = new List<AuthenticationAttempt>();
            }
        }

        public AuthenticationAttempt LastSuccessfulAttempt
        {
            get { return _successfulAuthenticationAttempt; }
        }

        public AuthenticationAttempt LastFailedAttempt
        {
            get { return _failedAuthenticationAttempt; }
        }

        public IReadOnlyCollection<AuthenticationAttempt> LastFailedAttempts
        {
            get { return _failedAuthenticationAttempts; }
        } 
    }
}
