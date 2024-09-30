using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    [CollectionDataContract(Name = "{0}EntityCollection", Namespace = ContractConstants.ContractsNamespaceV1)]
    public class EntityCollection<TEntity> : Collection<TEntity>
        where TEntity : IEntity
    {
        #region Constructors

        public EntityCollection()
        {
        }

        public EntityCollection(IList<TEntity> list)
            : base(list)
        {
        }

        public EntityCollection(IEnumerable<TEntity> items)
            : base(new List<TEntity>(items))
        {
        }

        #endregion
    }
}
