using System;
using System.Collections.Generic;
using Unity;

namespace CareFusion.Dispensing.DI
{
    public interface IObjectFactory : IDisposable
    {
        IUnityContainer Container { get; }
        IObjectFactory RegisterInstance(Type t, object instance);

        IObjectFactory RegisterInstance(Type t, string name, object instance);

        IObjectFactory RegisterInstance(Type t, object instance, Lifetime lifetime);

        IObjectFactory RegisterInstance(Type t, string name, object instance, Lifetime lifetime);

        IObjectFactory RegisterInstance<T>(T instance);

        IObjectFactory RegisterInstance<T>(string name, T instance);

        IObjectFactory RegisterInstance<T>(T instance, Lifetime lifetime);

        IObjectFactory RegisterInstance<T>(string name, T instance, Lifetime lifetime);

        IObjectFactory RegisterType(Type t, params object[] constructorParameters);

        IObjectFactory RegisterType(Type t, string name, params object[] constructorParameters);

        IObjectFactory RegisterType(Type t, Lifetime lifetime, params object[] constructorParameters);

        IObjectFactory RegisterType(Type t, string name, Lifetime lifetime, params object[] constructorParameters);

        IObjectFactory RegisterType(Type from, Type to, params object[] constructorParameters);

        IObjectFactory RegisterType(Type from, Type to, string name, params object[] constructorParameters);

        IObjectFactory RegisterType(Type from, Type to, Lifetime lifetime, params object[] constructorParameters);

        IObjectFactory RegisterType(Type from, Type to, string name, Lifetime lifetime, params object[] constructorParameters);

        IObjectFactory RegisterType<T>(params object[] constructorParameters);

        IObjectFactory RegisterType<T>(string name, params object[] constructorParameters);

        IObjectFactory RegisterType<T>(Lifetime lifetime, params object[] constructorParameters);

        IObjectFactory RegisterType<T>(string name, Lifetime lifetime, params object[] constructorParameters);

        IObjectFactory RegisterType<TFrom, TTo>(params object[] constructorParameters) where TTo : TFrom;

        IObjectFactory RegisterType<TFrom, TTo>(string name, params object[] constructorParameters) where TTo : TFrom;

        IObjectFactory RegisterType<TFrom, TTo>(Lifetime lifetime, params object[] constructorParameters) where TTo : TFrom;

        IObjectFactory RegisterType<TFrom, TTo>(string name, Lifetime lifetime, params object[] constructorParameters) where TTo : TFrom;

        bool IsRegistered<T>();

        bool IsRegistered(Type typeToCheck);

        T Get<T>();

        object Get(Type t);

        T Get<T>(string name);

        object Get(Type t, string name);

        IEnumerable<T> GetAll<T>();

        IObjectFactory CreateChild();
    }
}
