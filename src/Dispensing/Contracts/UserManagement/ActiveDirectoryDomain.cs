using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class ActiveDirectoryDomain : IEntity<Guid>
    {
        #region Constructors

        public ActiveDirectoryDomain()
        {
        }

        public ActiveDirectoryDomain(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ActiveDirectoryDomain(Guid key)
        {
            return FromKey(key);
        }

        public static ActiveDirectoryDomain FromKey(Guid key)
        {
            return new ActiveDirectoryDomain(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a active directory domain.
        /// </summary>
        [Column("ActiveDirectoryDomainKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the value equal to the latest committed update sequence
        /// number that is assigned by the Active Directory domain's controller.
        /// </summary>
        /// <remarks>
        /// This property is used for synchronization purposes.
        /// </remarks>
        [Column("HighestCommittedUSNValue")]
        public long? HighestCommittedUsn { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time of when an Active Directory domain is last
        /// successfully polled.
        /// </summary>
        /// <remarks>
        /// This property is used for synchronization purposes.
        /// </remarks>
        public DateTime? LastSuccessfulPollUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of when an Active Directory domain
        /// is last successfully polled.
        /// </summary>
        /// <remarks>
        /// This property is used for synchronization purposes.
        /// </remarks>
        [Column("LastSuccessfulPollLocalDateTime")]
        public DateTime? LastSuccessfulPollDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the last polling status.
        /// </summary>
        /// <remarks>
        /// This property is used for synchronization purposes.
        /// </remarks>
        public string LastPollStatusInternalCode { get; set; }

        /// <summary>
        /// Gets the value that indicates the last polling status.
        /// </summary>
        /// <remarks>
        /// This property is used for synchronization purposes.
        /// </remarks>
        public ActiveDirectoryPollStatusInternalCode? LastPollStatus
        {
            get { return LastPollStatusInternalCode.FromNullableInternalCode<ActiveDirectoryPollStatusInternalCode>(); }
        }

        /// <summary>
        /// Gets the value that indicates the last polling status description.
        /// </summary>
        /// <remarks>
        /// This property is used for synchronization purposes.
        /// </remarks>
        [Column("LastPollStatusDescription")]
        public string LastPollStatusDescription { get; internal set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user directory is a workgroup.
        /// </summary>
        [Column("WorkgroupFlag")]
        public bool Workgroup { get; set; }

        /// <summary>
        /// Gets or sets the full name of a domain as per the Domain Naming System, or name of a workgroup.
        /// </summary>
        [Column("FullyQualifiedDomainName")]
        public string FullyQualifiedName { get; set; }

        /// <summary>
        /// Gets or sets the name of a domain that is normally shown to users
        /// that are being authenticated (does not apply to workgroups).
        /// </summary>
        [Column("ShortDomainName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the period of time (in hours) between each polling of a user
        /// directory or workgroup.
        /// </summary>
        [Column("PollingDurationAmount")]
        public int? PollingDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user directory is being actively
        /// used to authenticate users and hence whether polling is on (does not apply to workgroups).
        /// </summary>
        [Column("ActiveFlag")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the IP addres, machine name, or DNS name of a
        /// domain controller (does not apply to workgroups).
        /// </summary>
        [Column("DomainControllerAddressValue")]
        public string ControllerAddress { get; set; }

        /// <summary>
        /// Gets or sets the ID of a system account that is used to connect to a user
        /// directory (does not apply to workgroups).
        /// </summary>
        [Column("SystemAccountID")]
        public string SystemAccountId { get; set; }

        /// <summary>
        /// Gets or sets the decrypted value of a password.
        /// </summary>
        public string SystemAccountPassword { get; set; }

        /// <summary>
        /// Gets or sets the encrypted value of a password used in conjunction
        /// with a connection system account (does not apply workgroups).
        /// </summary>
        [Column("SystemAccountPasswordEncryptedValue")]
        public string SystemAccountPasswordEncrypted { get; set; }

        /// <summary>
        /// Gets or sets the globally unique ID that is assigned to the Active Directory
        /// database itself when a domain controller is first installed, and the changes
        /// whenever a restore operation is performed or when the domain controller
        /// is configured to host an application partition.
        /// </summary>
        /// <remarks>
        /// This property is used for synchronization purposes.
        /// </remarks>
        [Column("InvocationID")]
        public Guid? InvocationId { get; set; }

        /// <summary>
        /// Gets or sets the interval (in days) used by a BioID enabled dispensing device to prompt BioID users
        /// to sign in using a password to prevent the domain user account from expiring due to domain inactivity.
        /// </summary>
        [Column("ScheduledPasswordSignInIntervalAmount")]
        public short? ScheduledPasswordSignInInterval { get; set; }

        public string UserDirectoryTypeInternalCode { get; set; }

        public UserDirectoryTypeInternalCode? UserDirectoryType
        {
            get { return UserDirectoryTypeInternalCode.FromNullableInternalCode<UserDirectoryTypeInternalCode>(); }
        }

        public string EncryptionAlgorithmInternalCode { get; set; }

        public EncryptionAlgorithmInternalCode? EncryptionAlgorithm
        {
            get { return EncryptionAlgorithmInternalCode.FromNullableInternalCode<EncryptionAlgorithmInternalCode>(); }
        }

        ///<summary>
        /// Gets or sets the flag that indicates whether LDAP communication over SSL is supported
        /// </summary>
        [Column("SecuredCommunicationFlag")]
        public bool SecuredCommunication { get; set; }

        ///<summary>
        /// Gets or sets the port number that is specified with the AD address
        /// </summary>
        public int? PortNumber { get; set; }

        ///<summary>
        /// Gets or sets the port number that is specified with the AD address
        /// </summary>
        public string LDAPCertificateFileName { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Gets or sets a collection of user directory groups for an active directory domain.
        /// </summary>
        public UserDirectoryGroup[] Groups { get; set; }

        [Column("SupportDomainFlag")]
        public bool IsSupportDomain { get; set; }

        #endregion

        #region Obsolete Properties

        /// <summary>
        /// Gets or sets the value that indicates whether each user associated with an Active Directory
        /// Domain is a support user.
        /// </summary>
        [Obsolete("Use UserDirectoryGroups instead.")]
        [Column("SupportUserFlag")]
        public bool SupportUsers { get; set; }

        /// <summary>
        /// Gets or sets the name of the group that is used to find the Active Directory
        /// users that are imported into the Dispensing System.
        /// </summary>
        [Obsolete("Use UserDirectoryGroups instead.")]
        public string GroupName { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
