using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareFusion.Dispensing.Contracts
{
    public class AuthenticationAttempt
    {
        public Guid AuthenticationEventKey { get; set; }

        public DateTime AuthenticationUtcDateTime { get; set; }

        [Column("AccessingAddressValue")]
        public string AccessingAddress { get; set; }

        public string AccessingMachineName { get; set; }
    }
}
