using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace CareFusion.Dispensing.DI.Unity
{
    public class UnityObjectFactory : IObjectFactory
    {
        private readonly IUnityContainer _container;
        private readonly bool _isContainerOwner;
        private readonly IInstanceLifetimeManager _manager;

        public IUnityContainer Container => _container;

        public UnityObjectFactory()
        {
            if (UnityContainerInstance.Container != null)
            {
                _container = UnityContainerInstance.Container;
                _isContainerOwner = false;
            }
            else
            {
                _container = new UnityContainer();
                _isContainerOwner = true;
            }

            _manager = new ExternallyControlledLifetimeManager();
            _container.RegisterInstance<IObjectFactory>(this, _manager);
        }

        private UnityObjectFactory(IUnityContainer container)
        {
            _container = container;
            _isContainerOwner = true;

            _manager = new ExternallyControlledLifetimeManager();
            _container.RegisterInstance<IObjectFactory>(this, _manager);
        }

        public IObjectFactory RegisterInstance(Type t, object instance)
        {
            try
            {
                _container.RegisterInstance(t, instance, new ContainerControlledLifetimeManager());
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterInstance(Type t, string name, object instance)
        {
            try
            {
                _container.RegisterInstance(t, name, instance, new ContainerControlledLifetimeManager());
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterInstance(Type t, object instance, Lifetime lifetime)
        {
            try
            {
                var lifetimeManager = GetLifetimeManager(lifetime);
                _container.RegisterInstance(t, instance, lifetimeManager);
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterInstance(Type t, string name, object instance, Lifetime lifetime)
        {
            try
            {
                var lifetimeManager = GetLifetimeManager(lifetime);
                _container.RegisterInstance(t, name, instance, lifetimeManager);
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterInstance<T>(T instance)
        {
            try
            {
                _container.RegisterInstance(instance, new ContainerControlledLifetimeManager());
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterInstance<T>(string name, T instance)
        {
            try
            {
                _container.RegisterInstance(name, instance, new ContainerControlledLifetimeManager());
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterInstance<T>(T instance, Lifetime lifetime)
        {
            try
            {
                var lifetimeManager = GetLifetimeManager(lifetime);
                _container.RegisterInstance(instance, lifetimeManager);
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterInstance<T>(string name, T instance, Lifetime lifetime)
        {
            try
            {
                var lifetimeManager = GetLifetimeManager(lifetime);
                _container.RegisterInstance(name, instance, lifetimeManager);
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType(Type t, params object[] constructorParameters)
        {
            try
            {
                if (constructorParameters != null && constructorParameters.Length > 0)
                {
                    _container.RegisterType(t, new InjectionConstructor(constructorParameters));
                }
                else
                {
                    _container.RegisterType(t);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType(Type t, string name, params object[] constructorParameters)
        {
            try
            {
                if (constructorParameters != null && constructorParameters.Length > 0)
                {
                    _container.RegisterType(t, name, new InjectionConstructor(constructorParameters));
                }
                else
                {
                    _container.RegisterType(t, name);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType(Type t, Lifetime lifetime, params object[] constructorParameters)
        {
            try
            {
                var manager = GetTypeLifetimeManager(lifetime);
                var injectionMembers = constructorParameters != null && constructorParameters.Length > 0 ?
                    new InjectionConstructor(constructorParameters) : null;

                if (lifetime == Lifetime.Singleton)
                {
                    RegisterSingleton(t, injectionMembers);
                }
                else
                {
                    _container.RegisterType(t, manager, injectionMembers);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType(Type t, string name, Lifetime lifetime, params object[] constructorParameters)
        {
            try
            {
                var manager = GetTypeLifetimeManager(lifetime);
                var injectionMembers = constructorParameters != null && constructorParameters.Length > 0 ?
                    new InjectionConstructor(constructorParameters) : null;

                if (lifetime == Lifetime.Singleton)
                {
                    RegisterSingleton(t, name, injectionMembers);
                }
                else
                {
                    _container.RegisterType(t, name);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType(Type from, Type to, params object[] constructorParameters)
        {
            try
            {
                if (constructorParameters != null && constructorParameters.Length > 0)
                {
                    _container.RegisterType(from, to, new InjectionConstructor(constructorParameters));
                }
                else
                {
                    _container.RegisterType(from, to);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType(Type from, Type to, string name, params object[] constructorParameters)
        {
            try
            {
                if (constructorParameters != null && constructorParameters.Length > 0)
                {
                    _container.RegisterType(from, to, name, new InjectionConstructor(constructorParameters));
                }
                else
                {
                    _container.RegisterType(from, to, name);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType(Type from, Type to, Lifetime lifetime, params object[] constructorParameters)
        {
            try
            {
                var manager = GetTypeLifetimeManager(lifetime);
                var injectionMembers = constructorParameters != null && constructorParameters.Length > 0 ?
                    new InjectionConstructor(constructorParameters) : null;

                if (lifetime == Lifetime.Singleton)
                {
                    RegisterSingleton(from, to, injectionMembers);
                }
                else
                {
                    _container.RegisterType(from, to, manager);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType(Type from, Type to, string name, Lifetime lifetime, params object[] constructorParameters)
        {
            try
            {
                var manager = GetTypeLifetimeManager(lifetime);
                var injectionMembers = constructorParameters != null && constructorParameters.Length > 0 ?
                    new InjectionConstructor(constructorParameters) : null;

                if (lifetime == Lifetime.Singleton)
                {
                    RegisterSingleton(from, to, name, injectionMembers);
                }
                else
                {
                    _container.RegisterType(from, to, name, manager);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType<T>(params object[] constructorParameters)
        {
            try
            {
                if (constructorParameters != null && constructorParameters.Length > 0)
                {
                    _container.RegisterType<T>(new InjectionConstructor(constructorParameters));
                }
                else
                {
                    _container.RegisterType<T>();
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType<T>(string name, params object[] constructorParameters)
        {
            try
            {
                if (constructorParameters != null && constructorParameters.Length > 0)
                {
                    _container.RegisterType<T>(name, new InjectionConstructor(constructorParameters));
                }
                else
                {
                    _container.RegisterType<T>(name);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType<T>(Lifetime lifetime, params object[] constructorParameters)
        {
            try
            {
                var manager = GetTypeLifetimeManager(lifetime);
                var injectionMembers = constructorParameters != null && constructorParameters.Length > 0 ?
                    new InjectionConstructor(constructorParameters) : null;

                if (lifetime == Lifetime.Singleton)
                {
                    RegisterSingleton<T>(injectionMembers);
                }
                else
                {
                    _container.RegisterType<T>(manager);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType<T>(string name, Lifetime lifetime, params object[] constructorParameters)
        {
            try
            {
                var manager = GetTypeLifetimeManager(lifetime);
                var injectionMembers = constructorParameters != null && constructorParameters.Length > 0 ?
                    new InjectionConstructor(constructorParameters) : null;

                if (lifetime == Lifetime.Singleton)
                {
                    RegisterSingleton<T>(name, injectionMembers);
                }
                else
                {
                    _container.RegisterType<T>(name, manager);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType<TFrom, TTo>(params object[] constructorParameters) where TTo : TFrom
        {
            try
            {
                if (constructorParameters != null && constructorParameters.Length > 0)
                {
                    _container.RegisterType<TFrom, TTo>(new InjectionConstructor(constructorParameters));
                }
                else
                {
                    _container.RegisterType<TFrom, TTo>();
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType<TFrom, TTo>(string name, params object[] constructorParameters) where TTo : TFrom
        {
            try
            {
                if (constructorParameters != null && constructorParameters.Length > 0)
                {
                    _container.RegisterType<TFrom, TTo>(name, new InjectionConstructor(constructorParameters));
                }
                else
                {
                    _container.RegisterType<TFrom, TTo>(name);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType<TFrom, TTo>(Lifetime lifetime, params object[] constructorParameters) where TTo : TFrom
        {
            try
            {
                var manager = GetTypeLifetimeManager(lifetime);
                var injectionMembers = constructorParameters != null && constructorParameters.Length > 0 ?
                    new InjectionConstructor(constructorParameters) : null;

                if (lifetime == Lifetime.Singleton)
                {
                    RegisterSingleton<TFrom, TTo>(injectionMembers);
                }
                else
                {
                    _container.RegisterType<TFrom, TTo>(manager);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public IObjectFactory RegisterType<TFrom, TTo>(string name, Lifetime lifetime, params object[] constructorParameters) where TTo : TFrom
        {
            try
            {
                var manager = GetTypeLifetimeManager(lifetime);
                var injectionMembers = constructorParameters != null && constructorParameters.Length > 0 ?
                    new InjectionConstructor(constructorParameters) : null;

                if (lifetime == Lifetime.Singleton)
                {
                    RegisterSingleton<TFrom, TTo>(name, injectionMembers);
                }
                else
                {
                    _container.RegisterType<TFrom, TTo>(name, manager);
                }
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }

            return this;
        }

        public T Get<T>()
        {
            T instance = default(T);

            try
            {
                instance = _container.Resolve<T>();
            }
            catch (Exception e)
            {
                ThrowNewDIException(e);
            }

            return instance;
        }

        public object Get(Type t)
        {
            object instance = null;

            try
            {
                instance = _container.Resolve(t);
            }
            catch (Exception e)
            {
                ThrowNewDIException(e);
            }

            return instance;
        }

        public T Get<T>(string name)
        {
            T instance = default(T);

            try
            {
                instance = _container.Resolve<T>(name);
            }
            catch (Exception e)
            {
                ThrowNewDIException(e);
            }

            return instance;
        }

        public object Get(Type t, string name)
        {
            object instance = null;

            try
            {
                instance = _container.Resolve(t, name);
            }
            catch (Exception e)
            {
                ThrowNewDIException(e);
            }

            return instance;
        }

        public IEnumerable<T> GetAll<T>()
        {
            var instance = Enumerable.Empty<T>();

            try
            {
                instance = _container.ResolveAll<T>();
            }
            catch (Exception e)
            {
                ThrowNewDIException(e);
            }

            return instance;
        }

        public IObjectFactory CreateChild()
        {
            var childContainer = _container.CreateChildContainer();
            var factory = new UnityObjectFactory(childContainer);
            return factory;
        }

        void ThrowNewDIException(Exception e)
        {
            var exceptionToThrow = e;
            if (e is ResolutionFailedException)
            {
                exceptionToThrow = e.GetBaseException();
            }
            throw new DIException(exceptionToThrow.Message, exceptionToThrow);
        }

        private static IInstanceLifetimeManager GetLifetimeManager(Lifetime lifetime)
        {
            IInstanceLifetimeManager manager;
            switch (lifetime)
            {
                case Lifetime.Transient:
                    throw new NotSupportedException($"{Lifetime.Transient} is no longer supported for the {nameof(RegisterInstance)} API.");
                case Lifetime.Singleton:
                    manager = new ContainerControlledLifetimeManager();
                    break;
                case Lifetime.External:
                    manager = new ExternallyControlledLifetimeManager();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime));
            }

            return manager;
        }

        private static ITypeLifetimeManager GetTypeLifetimeManager(Lifetime lifetime)
        {
            switch(lifetime)
            {
                case Lifetime.Transient: return TypeLifetime.Transient;
                case Lifetime.External: return TypeLifetime.External;
                case Lifetime.Singleton: return TypeLifetime.Singleton;
                default: throw new ArgumentOutOfRangeException(nameof(lifetime));
            }
        }

        #region IObjectFactory Members

        public bool IsRegistered<T>()
        {
            try
            {
                return _container.IsRegistered<T>();
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }
        }

        public bool IsRegistered(Type typeToCheck)
        {
            try
            {
                return _container.IsRegistered(typeToCheck);
            }
            catch (Exception e)
            {
                throw new DIException(e.Message, e);
            }
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
            if (_container != null && _isContainerOwner)
            {
                if (_container.IsRegistered<IObjectFactory>() && _container.Resolve<IObjectFactory>().Equals(this))
                {
                    var externallyControlledLifetimeManager = _manager as ExternallyControlledLifetimeManager;
                    externallyControlledLifetimeManager?.RemoveValue();
                }
                _container.Dispose();
            }
        }
        #endregion


        private void RegisterSingleton(Type t, InjectionConstructor injectionConstructor)
        {
            if (injectionConstructor != null)
            {
                _container.RegisterSingleton(t, injectionConstructor);
            }
            else
            {
                _container.RegisterSingleton(t);
            }
        }

        private void RegisterSingleton(Type t, string name, InjectionConstructor injectionConstructor)
        {
            if (injectionConstructor != null)
            {
                _container.RegisterSingleton(t, name, injectionConstructor);
            }
            else
            {
                _container.RegisterSingleton(t, name);
            }
        }

        private void RegisterSingleton(Type from, Type to, InjectionConstructor injectionConstructor)
        {
            if (injectionConstructor != null)
            {
                _container.RegisterSingleton(from, to, injectionConstructor);
            }
            else
            {
                _container.RegisterSingleton(from, to);
            }
        }

        private void RegisterSingleton(Type from, Type to, string name, InjectionConstructor injectionConstructor)
        {
            if (injectionConstructor != null)
            {
                _container.RegisterSingleton(from, to, name, injectionConstructor);
            }
            else
            {
                _container.RegisterSingleton(from, to, name);
            }
        }

        private void RegisterSingleton<T>(InjectionConstructor injectionConstructor)
        {
            if (injectionConstructor != null)
            {
                _container.RegisterSingleton<T>(injectionConstructor);
            }
            else
            {
                _container.RegisterSingleton<T>();
            }
        }

        private void RegisterSingleton<T>(string name, InjectionConstructor injectionConstructor)
        {
            if (injectionConstructor != null)
            {
                _container.RegisterSingleton<T>(name, injectionConstructor);
            }
            else
            {
                _container.RegisterSingleton<T>(name);
            }
        }

        private void RegisterSingleton<TFrom, TTo>(InjectionConstructor injectionConstructor) where TTo : TFrom
        {
            if (injectionConstructor != null)
            {
                _container.RegisterSingleton<TFrom, TTo>(injectionConstructor);
            }
            else
            {
                _container.RegisterSingleton<TFrom, TTo>();
            }
        }

        private void RegisterSingleton<TFrom, TTo>(string name, InjectionConstructor injectionConstructor) where TTo : TFrom
        {
            if (injectionConstructor != null)
            {
                _container.RegisterSingleton<TFrom, TTo>(name, injectionConstructor);
            }
            else
            {
                _container.RegisterSingleton<TFrom, TTo>(name);
            }
        }
    }
}
