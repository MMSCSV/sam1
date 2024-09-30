namespace CareFusion.Dispensing.Services.Business
{
    public enum IdentityServerAuthenticationFailureCode
    {
        Failed,
        AccountLocked,
        NextFailLock,
        PasswordChangeRequired,
        TempPasswordExpired
    }
}