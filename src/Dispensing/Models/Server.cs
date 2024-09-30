using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Resources.Common;

namespace CareFusion.Dispensing.Models
{
    /// <summary>
    /// Represents a server assocation with a facility and number of devices within the facility.
    /// </summary>
    [Serializable]
    public class ServerAssociation
    {
        public string FacilityName { get; set; }

        public int DispensingDeviceCount { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1} {2}", FacilityName, DispensingDeviceCount, CommonResources.Devices);
        }
    }


    /// <summary>
    /// Represents a physical or virtual server that is part of a dispensing-system server.
    /// </summary>
    [Serializable]
    public class Server : IEntity<Guid>
    {
        #region Constructors

        public Server()
        {
        }

        public Server(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Server(Guid key)
        {
            return FromKey(key);
        }

        public static Server FromKey(Guid key)
        {
            return new Server(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a server.
        /// </summary>
        [Column("ServerKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the IP address of a server.
        /// </summary>
        [Column("ServerAddressValue")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the name of a server.
        /// </summary>
        [Column("ServerName")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether one of the server's responsibilities is to be a core server.
        /// </summary>
        [Column("CoreFlag")]
        public bool Core { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether one of the server's responsibilities is to be a sync server.
        /// </summary>
        [Column("SyncFlag")]
        public bool Sync { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Gets a list of associations to the a server.
        /// </summary>
        public IReadOnlyCollection<ServerAssociation> Associations { get; internal set; }
 
        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
