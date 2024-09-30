using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq.Expressions;

namespace CareFusion.Dispensing.Data
{
    internal static class CompiledQueries
    {
        private static readonly Dictionary<string, Delegate> _compiledQueries = new Dictionary<string, Delegate>();

        internal static Func<TArg0, TResult> Get<TArg0, TResult>(string key, Expression<Func<TArg0, TResult>> query) where TArg0 : DataContext
        {
            return (Func<TArg0, TResult>)InternalGet(key, () => CompiledQuery.Compile(query));
        }

        internal static Func<TArg0, TArg1, TResult> Get<TArg0, TArg1, TResult>(string key, Expression<Func<TArg0, TArg1, TResult>> query) where TArg0 : DataContext
        {
            return (Func<TArg0, TArg1, TResult>)InternalGet(key, () => CompiledQuery.Compile(query));
        }

        internal static Func<TArg0, TArg1, TArg2, TResult> Get<TArg0, TArg1, TArg2, TResult>(string key, Expression<Func<TArg0, TArg1, TArg2, TResult>> query) where TArg0 : DataContext
        {
            return (Func<TArg0, TArg1, TArg2, TResult>)InternalGet(key, () => CompiledQuery.Compile(query));
        }

        internal static Func<TArg0, TArg1, TArg2, TArg3, TResult> Get<TArg0, TArg1, TArg2, TArg3, TResult>(string key, Expression<Func<TArg0, TArg1, TArg2, TArg3, TResult>> query) where TArg0 : DataContext
        {
            return (Func<TArg0, TArg1, TArg2, TArg3, TResult>)InternalGet(key, () => CompiledQuery.Compile(query));
        }

        internal static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult> Get<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>(string key, Expression<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>> query) where TArg0 : DataContext
        {
            return (Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>)InternalGet(key, () => CompiledQuery.Compile(query));
        }

        private static Delegate InternalGet(string key, Func<Delegate> queryProvider)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            lock ((_compiledQueries.Keys as ICollection).SyncRoot)
            {
                Delegate d;
                if (_compiledQueries.TryGetValue(key, out d))
                    return d;
                    
                var result = queryProvider();
                _compiledQueries.Add(key, result);
                return result;
            }
        }

    }
}
