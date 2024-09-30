using Pyxis.Core.Data.Schema.Core.Models;

namespace CareFusion.Dispensing.Data.Models
{
    public class UserRoleResult : UserRole
    {
        public long UserMemberCount { get; set; }
    }
}
