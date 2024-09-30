using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Services.Business
{
    public interface IUserAccountManager
    {
        bool IsActiveDirectoryIntegrated();

        UserAccount Get(Guid userAccountKey);

        UserRecentAuthenticationAttempt GetRecentAuthenticationAttempt(Guid userAccountKey, Guid ignoreAuthenticationEventKey);

        Guid Add(Context context, UserAccount userAccount);

        void Update(Context context, UserAccount userAccount);

        void AddUserEncounterAssociation(Context context, Guid userAccountKey, Guid encounterKey);

        void RemoveUserEncounterAssociations(Context context, Guid userAccountKey, IEnumerable<Guid> encounterKeys);

        UserMergeStatus MapLocalUserAccountToDomainUserAccount(Context context, Guid localUserKey, Guid domainUserKey);

        UserMergeSummary GetMergeSummaryForUser(Guid userAccountKey);
    }
}
