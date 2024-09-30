using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public enum AuthenticationResultCode
    {
        [EnumMember]
        AuthenticationCancelled,

        [EnumMember]
        AuthenticationFailed,

        [EnumMember]
        Successful,

        [EnumMember]
        ChangePasswordRequired,

        [EnumMember]
        NotFound,

        [EnumMember]
        IncorrectPassword,

        [EnumMember]
        AccountLocked,

        [EnumMember]
        AccountAlreadyLocked,

        [EnumMember]
        AccountExpired,

        [EnumMember]
        PasswordExpired,

        [EnumMember]
        AccountInactive,

        [EnumMember]
        AccountDeleted,

        [EnumMember]
        AuthenticationMethodNotSupported,

        [EnumMember]
        WarnAccountLockout,

        [EnumMember]
        MultipleUserId,

        [EnumMember]
        DomainError,

        [EnumMember]
        DeviceOutOfService,

        [EnumMember]
        TempPasswordExpired,

        [EnumMember]
        AccountLocking,

        [EnumMember]
        InactiveDomain,

        [EnumMember]
        NoRecentAccessLock,

        [EnumMember]
        IdentityServerUrlNotConfigured,

        [EnumMember]
        SmartCardCertificateRevokedOrInvalid,

        [EnumMember]
        IdentityServerUrlNotReachable,

        [EnumMember]
        RequestTimedOut
    }
}
