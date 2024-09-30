using System;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Entities;

namespace CareFusion.Dispensing.Services.Business
{
    internal class FingerprintManager : IFingerprintManager
    {
        #region Implementation of IFingerprintManager

        public bool IsFingerprintRegisteredForUser(Guid userAccountKey)
        {
            bool exists;

            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                exists = repository.GetQueryableEntity<UserFingerprintEntity>()
                    .Where(uf => uf.UserAccountKey == userAccountKey)
                    .Any();
            }

            return exists;
        }

        public UserFingerprint GetFingerprintForUser(Guid userAccountKey)
        {
            UserFingerprint userFingerprint;

            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                userFingerprint = repository.GetUserFingerprint(userAccountKey);
            }

            return userFingerprint;
        }

        public void RegisterFingerprint(Context context, UserFingerprint userFingerprint)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userFingerprint, "userFingerprint");
            
            try
            {
                using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
                {
                    //Check if user is re-registering FP
                    UserFingerprint existingFingerPrint = repository.GetUserFingerprint(userFingerprint.UserAccountKey);
                    if (existingFingerPrint != null)
                    {
                        // Copy to existing
                        existingFingerPrint.Value1 = userFingerprint.Value1;
                        existingFingerPrint.Value1Length = userFingerprint.Value1Length;
                        existingFingerPrint.Value2 = userFingerprint.Value2;
                        existingFingerPrint.Value2Length = userFingerprint.Value2Length;
                        existingFingerPrint.Value3 = userFingerprint.Value3;
                        existingFingerPrint.Value3Length = userFingerprint.Value3Length;

                        repository.UpdateUserFingerprint(context, existingFingerPrint);
                    }
                    else
                        repository.InsertUserFingerprint(context, userFingerprint);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion
    }
}
