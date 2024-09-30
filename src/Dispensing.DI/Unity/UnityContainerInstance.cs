using System;
using Unity;

namespace CareFusion.Dispensing.DI.Unity
{
    /// <summary>
    /// A static UnityContainer instance.
    /// </summary>
    public static class UnityContainerInstance
    {
        private static IUnityContainer _container;

        /// <summary>
        /// Gets or sets the container that is used in the application.
        /// </summary>
        public static IUnityContainer Container
        {
            get => _container;
            set
            {
                if (_container != null)
                {
                    throw new NotSupportedException("The static container already has an instance associated with it!");
                }

                _container = value;
            }
        }
    }
}
