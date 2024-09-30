using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CareFusion.Dispensing.Data.Entities;

namespace CareFusion.Dispensing.Data
{
    public interface IRepository : IDisposable
    {
        byte[] MinTickValue { get; }

        IQueryable<TEntity> GetQueryableEntity<TEntity>()
            where TEntity : class;

        IQueryable<TemporaryTableEntity> GetQueryableTemporaryTable(IEnumerable<Guid> tableKeys);

        string GetItemDisplayName(string itemDisplayName, string genericName, string itemName, string itemTypeInternalCode, string itemSubTypeInternalCode,
            decimal? strengthAmount, string strengthDisplayCode, decimal? concentrationVolumeAmount, string concentrationVolumeDisplayCode,
            decimal? totalVolumeAmount, string totalVolumeDisplayCode, string dosageFormCode);

        byte[] GetMaxTickValue();

        bool Exists<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        long Count<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class;

        TContract Get<TEntity, TContract>(object key)
            where TContract : class
            where TEntity : class, IContractConvertible<TContract>;

        IEnumerable<TContract> All<TEntity, TContract>()
            where TContract : class
            where TEntity : class, IContractConvertible<TContract>;

        IEnumerable<TContract> FindAll<TEntity, TContract>(Expression<Func<TEntity, bool>> predicate)
            where TContract : class
            where TEntity : class, IContractConvertible<TContract>;

        TContract FindOne<TEntity, TContract>(Expression<Func<TEntity, bool>> predicate)
            where TContract : class
            where TEntity : class, IContractConvertible<TContract>;

        IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters);
    }
}
