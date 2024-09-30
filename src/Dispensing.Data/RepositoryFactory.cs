using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Data.Repositories;

namespace CareFusion.Dispensing.Data
{
    public static class RepositoryFactory
    {
        private static readonly Dictionary<Type, object> Instances = new Dictionary<Type, object>();
        internal static readonly Dictionary<Type, Type> DefaultInstances = new Dictionary<Type, Type>
            {
                { typeof(IReadOnlyRepository), typeof(LinqReadOnlyRepository) },
                { typeof(IAdtRepository), typeof(AdtRepository) },
                { typeof(ICdcRepository), typeof(CdcRepository) },
                { typeof(IExternalRepository), typeof(ExternalRepository) },
                { typeof(ICoreRepository), typeof(CoreRepository) },
                { typeof(IItemRepository), typeof(ItemRepository) },
                { typeof(ILocationRepository), typeof(LocationRepository) },
                { typeof(IStorageRepository), typeof(StorageRepository) },
                { typeof(ISyncRepository), typeof(SyncRepository) },
                { typeof(ITransactionRepository), typeof(TransactionRepository) },
                { typeof(IRxRepository), typeof(RxRepository) },
            };

        public static void RegisterInstance(Type t, object instance)
        {
            Guard.ArgumentNotNull(t, "t");
            Guard.ArgumentNotNull(instance, "instance");

            if (!t.IsAssignableFrom(instance.GetType()))
                throw new InvalidOperationException("The instance is not assignable to the specified type.");

            Instances[t] = instance;
        }

        public static void RegisterInstance<T>(T instance)
            where T : IRepository
        {
            Instances[typeof (T)] = instance;
        }

        public static void UnregisterInstance(Type t)
        {
            Instances.Remove(t);
        }

        public static void UnregisterInstance<T>()
        {
            Instances.Remove(typeof(T));
        }

        public static void ClearRegisteredInstances()
        {
            Instances.Clear();
        }

        public static T Create<T>()
        {
            return (T)Create(typeof (T));
        }

        public static object Create(Type t)
        {
            object repository;
            if (Instances.TryGetValue(t, out repository))
                return repository;

            Type instanceType;
            if (DefaultInstances.TryGetValue(t, out instanceType))
                return Activator.CreateInstance(instanceType);

           return null;
        }

        public static IReadOnlyRepository CreateReadOnly()
        {
            return Create<IReadOnlyRepository>();
        }
    }
}
