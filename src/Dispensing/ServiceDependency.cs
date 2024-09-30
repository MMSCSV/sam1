using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Mms.Logging;

namespace CareFusion.Dispensing
{
    public class Dependency
    {
        public int TimeoutInMilliseconds { get; set; }
        public string Description { get; private set; }
        public Func<bool> Verify  { get; private set; }

        /// <summary>
        /// Constructor to create Dependency. Default timeout for verifying the 
        /// dependency is 60 seconds. Different timeout can be assigned using the
        /// TimeoutInMilliseconds property.
        /// </summary>
        /// <param name="identify">
        /// Delegate to identify the dependency that needs to be verified
        /// </param>
        /// <param name="description">
        /// Description to identify the dependency. This information will be logged
        /// if the dependency could not be verified.
        /// </param>
        public Dependency(Func<bool> identify, string description)
        {
            if (identify == null)
                throw new ArgumentException("Dependency: identify parameter cannot be null");

            if (String.IsNullOrEmpty(description))
                throw new ArgumentException("Dependency: description parameter cannot be empty or null");

            Verify = identify;
            Description = description;
            TimeoutInMilliseconds = 60000; // Default timeout = 60 seconds.
        }
    }

    class DependencyDto
    {
        internal CancellationTokenSource CancellationTokenSource { get; set; }
        internal String DependencyDescription { get; set; }
    }

    /// <summary>
    /// This is a simple class designed to resolved dependencies in order for a particular
    /// service or component to run. 
    /// Dependencies need to be idenfiied by calling service / component.
    /// </summary>
    public static class ServiceDependency
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Default Dependency

        /// <summary>
        /// Default dependency will check if the database mentioned in the "DispensingDatabase"
        /// connection string is up and running.
        /// </summary>
        private static readonly Func<bool> defaultDependency = () =>
        {
            var check = false;

            try
            {
                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DispensingDatabase"].ConnectionString))
                using (var command = new SqlCommand("select 1", connection))
                {
                    connection.Open();
                    if (Convert.ToInt32(command.ExecuteScalar()) == 1)
                        check = true;
                }
            }
            catch (SqlException exception)
            {
                Log.Error(EventId.ServiceDependency, "SqlException received " + exception.Message);
            }

            return check;
        };

        #endregion

        /// <summary>
        /// Provide dependencies that need to be resolved.
        /// For instance
        /// To check if a required service running
        /// To check if database is up and running
        /// and others
        /// </summary>
        /// <param name="dependencies">
        /// List of dependencies that need to be resolved
        /// </param>
        public static bool Verify(IEnumerable<Dependency> dependencies)
        {
            var dependenciesVerified = false;

            foreach (var d in dependencies)
            {
                dependenciesVerified = Verify(d);
            }

            return dependenciesVerified;
        }

        /// <summary>
        /// Provide dependencies that need to be resolved.
        /// For instance
        /// To check if a required service running
        /// To check if database is up and running
        /// and others
        /// </summary>
        /// <param name="dependency">
        /// Dependency that need to be resolved
        /// </param>
        public static bool Verify(Dependency dependency)
        {
            if (dependency == null)
                throw new ArgumentException("ServiceDependency: resolution parameter cannot be null");

            var dependencyVerified = false;

            #region resolve
            var cancellationTokenSource = new CancellationTokenSource();
            var dependencyDto = new DependencyDto {CancellationTokenSource = cancellationTokenSource, DependencyDescription = dependency.Description};

            // Start a timer based on the timeout provided by dependency. 
            using (var timer = new System.Threading.Timer(CancelTask, dependencyDto, dependency.TimeoutInMilliseconds, 0))
            {
                // Create a task to verify the dependency
                var task = Task.Factory.StartNew(() => {
                    while (true)
                    {
                        dependencyVerified = dependency.Verify();

                        // Check if timer callback was called before the task could be verified
                        // or dependency was verified
                        if (cancellationTokenSource.IsCancellationRequested || dependencyVerified)
                            break;

                        Thread.Sleep(1000);
                    }
                });

                try
                {
                    // Wait for the task to complete. 
                    Task.WaitAll(new[] {task});

                    if(dependencyVerified)
                    {
                        LogDependencyVerified(String.Format("{0} dependency was verified.", dependency.Description));
                    }
                }
                catch (AggregateException e)
                {
                    e.Handle(ex => {
                        LogDependencyNotVerified(String.Format("{0} could not be verified.\n Reason: {1}", dependency.Description, ex.Message));
                        return true;
                    });
                }

                return dependencyVerified;
            }

            #endregion
        }

        /// <summary>
        /// Default dependency will check if the database mentioned in the "DispensingDatabase"
        /// connection string is up and running.
        /// </summary>
        public static Dependency DefaultDependency
        {
            get { return new Dependency(defaultDependency, "Default dependency: DispensingDatabase is up and running"); }
        }

        private static void CancelTask(object state)
        {
            try
            {
                Log.Debug("In timer cancel task");

                var dependencyDto = state as DependencyDto;

                if (dependencyDto != null)
                {
                    dependencyDto.CancellationTokenSource.Cancel();
                    LogDependencyNotVerified(String.Format("{0} could not be verified", dependencyDto.DependencyDescription));
                }
            }
            catch { }
        }

        private static void LogDependencyVerified(string message)
        {
            Log.Info(message);
        }

        private static void LogDependencyNotVerified(string message)
        {
            Log.Error(EventId.ServiceDependency, message);
        }
    }
}
