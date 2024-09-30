using System;
using System.Linq;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public sealed class UserContextInfo
    {
        #region Contructors

        public UserContextInfo()
        {
        }

        public UserContextInfo(Guid signInEventKey)
        {
            SignInEventKey = signInEventKey;
        }

        public UserContextInfo(Guid signInEventKey, AuthUserAccount userAccount)
        {
            Guard.ArgumentNotNull(userAccount, "userAccount");

            SignInEventKey = signInEventKey;
            Key = userAccount.Key;
            SnapshotKey = userAccount.SnapshotKey;
            UserTypeKey = userAccount.UserTypeKey;
            UserId = userAccount.UserId;
            FirstName = userAccount.FirstName;
            MiddleName = userAccount.MiddleName;
            LastName = userAccount.LastName;
            IsSuperUser = userAccount.IsSuperUser;
            IsSupportUser = userAccount.IsSupportUser;
            IsDomainUser = userAccount.IsDomainAccount;
            InitialsText = userAccount.InitialsText;
        }

        public UserContextInfo(AuthUserAccount userAccount)
            : this(Guid.Empty, userAccount)
        {
        }

        #endregion

        #region Operator Overloads

        public static explicit operator Guid?(UserContextInfo userContextInfo)
        {
            return userContextInfo != null ? userContextInfo.Key : default(Guid?);
        }

        #endregion

        #region Public Properties (WCF - Specific)

        [DataMember]
        public Guid Key { get; set; }

        [DataMember]
        public Guid SnapshotKey { get; set; }

        [DataMember]
        public Guid? UserTypeKey { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        //This property is used to display the full descriptive User Name
        [IgnoreDataMember]
        public string DescriptiveUserName
        {
            get
            {
                string[] userNameContents = {
                    FirstName,
                    MiddleName,
                    LastName
                };
                return String.Join(" ", userNameContents.Where(s => !string.IsNullOrWhiteSpace(s)));
            }
        }

        [DataMember]
        public bool IsSuperUser { get; set; }

        [DataMember]
        public bool IsSupportUser { get; set; }

        [DataMember]
        public bool IsDomainUser { get; set; }

        [DataMember]
        public string InitialsText { get; set; }

        #endregion

        #region Public Properties

        public Guid SignInEventKey { get; set; }

        #endregion
    }
}
