using System;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing
{
    public class MyEncountersUpdatedEventArgs : EventArgs
    {
        public Guid[] AddedEncounters { get; private set; }
        public Guid[] RemovedEncounters { get; private set; }
        public DateTime AssociationUTCDateTime { get; private set; }

        public MyEncountersUpdatedEventArgs(Guid[] addedEncounters, DateTime associationUTCDateTime, Guid[] removedEncounters)
        {
            AddedEncounters = addedEncounters;
            AssociationUTCDateTime = associationUTCDateTime;
            RemovedEncounters = removedEncounters;
        }
    }
    public class UnregisteredPatientAddedEventArgs : EventArgs
    {
        public Guid EncounterKey { get; private set; }

        public Guid AssignedUnitKey { get; private set; }

        public UnregisteredPatientAddedEventArgs(Guid encounterKey, Guid assignedUnitKey)
        {
            EncounterKey = encounterKey;
            AssignedUnitKey = assignedUnitKey;
        }
    }
    public class RecentEncounterAddedEventArgs : EventArgs
    {
        public Guid EncounterKey { get; private set; }
        public DateTime LastUsedUTCDateTime { get; private set; }

        public RecentEncounterAddedEventArgs(Guid encounterKey, DateTime lastUsedUTCDateTime)
        {
            EncounterKey = encounterKey;
            LastUsedUTCDateTime = lastUsedUTCDateTime;
        }
    }
    public class UserValidatedEventArgs : EventArgs
    {
        public AuthUserAccount AuthUserAccount { get; private set; }

        public UserValidatedEventArgs(AuthUserAccount userAccount)
        {
            AuthUserAccount = userAccount;
        }
    }
    public static class EncounterEvents
    {
        public const string MyEncountersUpdated = "MyEncountersUpdated";
        public const string UnregisteredPatientAdded = "UnregisteredPatientAdded";
        public const string RecentEncountersUpdated = "RecentEncountersUpdated";
    }    

    public static class DataCacheEvents
    {
        public const string CacheRefreshRequested = "CacheRefreshRequested";
    }

    public static class AuthenticationEvents
    {
        public const string AuthenticationStarting = "AuthenticationStarting";
        public const string UserValidated = "UserValidated";
        public const string AuthenticationCancelled = "AuthenticationCancelled";
        public const string UserSessionEnded = "UserSessionEnded";
    }
}
