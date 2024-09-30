using System;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Services.Business
{
    internal interface IFingerprintManager
    {
        bool IsFingerprintRegisteredForUser(Guid userAccountKey);

        UserFingerprint GetFingerprintForUser(Guid userAccountKey);

        void RegisterFingerprint(Context context, UserFingerprint userFingerprint);
    }
}
