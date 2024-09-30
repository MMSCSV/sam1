using System;

namespace CareFusion.Dispensing.Server.Contracts
{
    public class AuthenticationEventData
    {
        public Guid AuthenticationEventKey { get; set; }

        public DateTime AuthenticationUtcDateTime { get; set; }

        public bool SuccessfullyAuthenticated { get; set; }

        public string UserId { get; set; }

        public Guid? UserAccountKey { get; set; }

        public string AccessingAddress { get; set; }

        public string AccessingMachineName { get; set; }
    }
}
