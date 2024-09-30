using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using CareFusion.Dispensing.Data.Entities;
using CareFusion.Dispensing.Data.Logging;
using Pyxis.Core.Data;
using Mms.Logging;
using DataException = System.Data.DataException;

namespace CareFusion.Dispensing.Data.Repositories
{
    /// <summary>
    /// Represents a base class for Linq repositories.
    /// </summary>
    public class LinqBaseRepository : IRepository
    {
        private sealed class MaxTickValueQueryResult
        {
            public int DatabaseVersionKey { get; set; }

            public byte[] MaxTickValue { get; set; }
        }
        public static MappingSource MappingSource
        {
            get { return _mappingSource; }
        }
        public const string EnableSqlLoggingAppSettingName = "EnableSqlLogging";
        private static readonly byte[] _minTickValue = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
        private static readonly MappingSource _mappingSource = new AttributeMappingSource();

        private IConnectionScope _connectionScope;
        private bool _disposeConnectionScope;
        private BaseDataContext _dataContext;
        private bool _disposeDataContext;
        private readonly bool _enableSqlLogging;

        protected LinqBaseRepository()
        {
            bool enableSqlLogging;
            if (bool.TryParse(ConfigurationManager.AppSettings[EnableSqlLoggingAppSettingName], out enableSqlLogging))
                _enableSqlLogging = enableSqlLogging;
            else
                _enableSqlLogging = false;

            RepositorySessionScope sessionScope = RepositorySessionScope.Current;
            if (sessionScope != null)
            {
                _connectionScope = sessionScope.ConnectionScope;
                _disposeConnectionScope = false;

                _dataContext = sessionScope.Context;
                _disposeDataContext = false;
            }
        }

        protected IConnectionScope ConnectionScope
        {
            get
            {
                if (_connectionScope == null)
                {
                    _connectionScope = ConnectionScopeFactory.Create();
                    _disposeConnectionScope = true;
                }

                return _connectionScope;
            }
        }

        protected TDataContext CreateDataContext<TDataContext>()
            where TDataContext : DataContext
        {
            return CreateDataContext<TDataContext>(false);
        }

        protected TDataContext CreateDataContext<TDataContext>(bool lazyLoad)
            where TDataContext : DataContext
        {
            return CreateDataContext<TDataContext>(null, lazyLoad);
        }

        protected TDataContext CreateDataContext<TDataContext>(DataLoadOptions dataLoadOptions)
            where TDataContext : DataContext
        {
            return CreateDataContext<TDataContext>(dataLoadOptions, false);
        }

        protected TDataContext CreateDataContext<TDataContext>(DataLoadOptions dataLoadOptions, bool lazyLoad)
            where TDataContext : DataContext
        {
            // Create data context
            TDataContext dataContext = (TDataContext)Activator.CreateInstance(typeof(TDataContext), ConnectionScope.Connection, _mappingSource);
            dataContext.LoadOptions = dataLoadOptions;
            dataContext.CommandTimeout = ConnectionScope.DefaultCommandTimeout;

            if (lazyLoad)
            {
                // Enable lazy loading
                dataContext.ObjectTrackingEnabled = true;
                dataContext.DeferredLoadingEnabled = true;
            }
            else
            {
                // Always turn off object tracking since we do not use data context for persistence.
                dataContext.ObjectTrackingEnabled = false;
            }

            // Route log messages to a logger
            if (_enableSqlLogging)
                dataContext.Log = new DataContextLogger(LogManager.GetLogger(typeof(TDataContext)));

            return dataContext;
        }

        #region IRepository Members

        public byte[] MinTickValue
        {
            get { return _minTickValue; }
        }

        public IQueryable<TEntity> GetQueryableEntity<TEntity>()
            where TEntity : class
        {
            IQueryable<TEntity> queryable = null;

            try
            {
                if (_dataContext == null)
                {
                    _dataContext = CreateDataContext<BaseDataContext>(true);

                    // Enable lazy loading
                    _dataContext.ObjectTrackingEnabled = true;
                    _dataContext.DeferredLoadingEnabled = true;

                    _disposeDataContext = true;
                }

                queryable = _dataContext.GetTable<TEntity>();
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return queryable;
        }

        public IQueryable<TemporaryTableEntity> GetQueryableTemporaryTable(IEnumerable<Guid> tableKeys)
        {
            IQueryable<TemporaryTableEntity> queryable = null;

            try
            {
                if (_dataContext == null)
                {
                    _dataContext = CreateDataContext<BaseDataContext>(true);

                    // Enable lazy loading
                    _dataContext.ObjectTrackingEnabled = true;
                    _dataContext.DeferredLoadingEnabled = true;

                    _disposeDataContext = true;
                }

                string keyList = string.Join(",", tableKeys);
                queryable = _dataContext.CreateTemporaryTable(keyList, ',');
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return queryable;
        }

        public string GetItemDisplayName(string itemDisplayName, string genericName, string itemName, string itemTypeInternalCode, string itemSubTypeInternalCode,
           decimal? strengthAmount, string strengthDisplayCode, decimal? concentrationVolumeAmount, string concentrationVolumeDisplayCode,
           decimal? totalVolumeAmount, string totalVolumeDisplayCode, string dosageFormCode)
        {
            string displayName = null;

            try
            {
                if (_dataContext == null)
                {
                    _dataContext = CreateDataContext<BaseDataContext>(true);

                    // Enable lazy loading
                    _dataContext.ObjectTrackingEnabled = true;
                    _dataContext.DeferredLoadingEnabled = true;

                    _disposeDataContext = true;
                }

                displayName = _dataContext.ItemDisplayName(
                    itemDisplayName,
                    genericName,
                    itemName,
                    itemTypeInternalCode,
                    itemSubTypeInternalCode,
                    strengthAmount,
                    strengthDisplayCode,
                    concentrationVolumeAmount,
                    concentrationVolumeDisplayCode,
                    totalVolumeAmount,
                    totalVolumeDisplayCode,
                    dosageFormCode
                    );
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return displayName;
        }

        public byte[] GetMaxTickValue()
        {
            byte[] maxTickValue = null;

            try
            {
                using (DataContext dataContext = CreateDataContext<DataContext>())
                {
                    // In order to properly support Database IsolationLevel.Snapshot, we need to bundle an
                    // an actual table read in order to start the snaphot isolation.
                    var tempResults = dataContext.ExecuteQuery<MaxTickValueQueryResult>(
                        "SELECT DatabaseVersionKey, CAST(min_active_rowversion() - CAST(1 AS bigint) AS binary(8)) AS MaxTickValue FROM Core.DatabaseVersion");
                    if (tempResults != null)
                    {
                        maxTickValue = tempResults.Select(mtv => mtv.MaxTickValue)
                            .FirstOrDefault();
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return maxTickValue;
        }

        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            bool exists = default(bool);

            try
            {
                using (DataContext dataContext = CreateDataContext<DataContext>())
                {
                    IQueryable<TEntity> query = dataContext.GetTable<TEntity>();
                    if (predicate != null)
                        query = query.Where(predicate);

                    exists = query.Any();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return exists;
        }

        public long Count<TEntity>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            long count = default(long);

            try
            {
                using (DataContext dataContext = CreateDataContext<DataContext>())
                {
                    IQueryable<TEntity> query = dataContext.GetTable<TEntity>();
                    if (predicate != null)
                        query = query.Where(predicate);

                    count = query.LongCount();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return count;
        }

        public TContract Get<TEntity, TContract>(object key)
            where TEntity : class, IContractConvertible<TContract>
            where TContract : class
        {
            TContract result = default(TContract);

            try
            {
                using (DataContext dataContext = CreateDataContext<DataContext>())
                {
                    // Enable lazy loading
                    dataContext.ObjectTrackingEnabled = true;
                    dataContext.DeferredLoadingEnabled = true;

                    var table = dataContext.GetTable<TEntity>();
                    var mapping = dataContext.Mapping.GetTable(typeof(TEntity));

                    var pkfield = mapping.RowType.DataMembers.SingleOrDefault(d => d.IsPrimaryKey);
                    if (pkfield == null)
                        throw new DataException(String.Format("Table {0} does not contain a Primary Key field.",
                                                              mapping.TableName));

                    var param = Expression.Parameter(typeof(TEntity), "e");
                    var predicate =
                        Expression.Lambda<Func<TEntity, bool>>(
                            Expression.Equal(Expression.Property(param, pkfield.Name), Expression.Constant(key)),
                            param);

                    result = table.Where(predicate)
                        .Select(entity => entity.ToContract())
                        .SingleOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return result;
        }

        public TContract FindOne<TEntity, TContract>(Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, IContractConvertible<TContract>
            where TContract : class
        {
            IEnumerable<TContract> results = FindAll<TEntity, TContract>(predicate);
            
            return results.FirstOrDefault();
        }

        public IEnumerable<TContract> All<TEntity, TContract>()
            where TContract : class
            where TEntity : class, IContractConvertible<TContract>
        {
            return FindAll<TEntity, TContract>(null);
        }

        public IEnumerable<TContract> FindAll<TEntity, TContract>(Expression<Func<TEntity, bool>> predicate)
            where TContract : class
            where TEntity : class, IContractConvertible<TContract>
        {
            TContract[] array = null;

            try
            {
                using (DataContext dataContext = CreateDataContext<DataContext>())
                {
                    // Enable lazy loading
                    dataContext.ObjectTrackingEnabled = true;
                    dataContext.DeferredLoadingEnabled = true;

                    IQueryable<TEntity> query = dataContext.GetTable<TEntity>();
                    if (predicate != null)
                        query = query.Where(predicate);

                    array = query.Select(entity => entity.ToContract())
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return array;
        }

        public IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters)
        {
            IEnumerable<TResult> results = null;

            try
            {
                using (DataContext dataContext = CreateDataContext<DataContext>())
                {
                    var tempResults = dataContext.ExecuteQuery<TResult>(query, parameters);
                    if (tempResults != null)
                        results = tempResults.ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return results;
        }

        #endregion      

        #region Dispose
        bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed)
                return;

            FreeManagedObjects();

            _isDisposed = true;
        }
        protected virtual void FreeManagedObjects()
        {
            if (_disposeDataContext)
            {
                if (_dataContext != null)
                {
                    _dataContext.Dispose();
                    _dataContext = null;
                }
            }

            if (_disposeConnectionScope)
            {
                if (_connectionScope != null)
                {
                    _connectionScope.Dispose();
                    _connectionScope = null;
                }
            }
        }
        #endregion 
    }
}
