using Unity;

namespace CareFusion.Dispensing.DI.Unity
{
    /// <summary>
    /// Base class that provides a basic bootstrapping sequence that registers assets in
    /// a <see cref="IUnityContainer"/>
    /// </summary>
    public abstract class UnityBootstrapper
    {
        public static void ConfigureUnityContainer(IUnityContainer container, params UnityBootstrapper[] bootstrappers)
        {
            if (bootstrappers != null)
            {
                foreach (var bootstrapper in bootstrappers)
                {
                    bootstrapper.Configure(container);
                }
            }
        }

        protected abstract void Configure(IUnityContainer container);
    }
}
