using System;

namespace CareFusion.Dispensing.Contracts
{
    public class UserMergeSummary
    {
        public Guid UserAccountKey { get; set; }

        public string UserId { get; set; }

        public string UserFirstName { get; set; }

        public string UserMiddleName { get; set; }

        public string UserLastName { get; set; }

        public string Domain { get; set; }

        public DateTime MergedUtcDateTime { get; set; }

        public string MergedAt { get; set; }

        public bool LocalUser { get; set; }

        public bool DomainUser { get; set; }

    }
}
