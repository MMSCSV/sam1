using System;
using System.Threading;
using System.Threading.Tasks;

namespace CareFusion.Dispensing
{
	/// <summary>
	/// Extension methods for the System.Threading.Tasks.Task object
	/// </summary>
	public static class TaskExtensionMethods
	{
		/// <summary>
		/// Attach exception handling continuation to a task. Will grab the current
		/// syncroniztion context to send any exception onto it synchronously.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		public static void AttachExceptionContinuation(this Task t)
		{
			// capture current syncronization context
			SynchronizationContext context = SynchronizationContext.Current;

			AttachExceptionContinuation(t, context);
		}

		/// <summary>
		/// Attach exception handling continuation to a task.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="context">Synchroniztion Content to send the exception to synchronously.</param>
		public static void AttachExceptionContinuation(this Task t, SynchronizationContext context)
		{
			// attach to the task if there are any errors, and forward them to the passed context (should be ui thread)
			t.ContinueWith(originalTask =>
				{
					// the handle method should return true for all errors that we handle, making sure the task scheduler
					// doesn't have to handle them and cause an exception for us
					originalTask.Exception.Handle(e =>
					{
						// forward to the context (should be the ui thread)
						context.Send(state =>
						{
							// throw the exception to be handled by the ui framework
							throw e;
						}, null);
						// return true to mark this exception as handled
						return true;
					});
				},
				// only invoke this continuation if the task throws an exception
				TaskContinuationOptions.OnlyOnFaulted
			);
		}

		/// <summary>
		/// Creates a continuation that executes when the target System.Threading.Tasks.Task
		/// completes, running on the continuation on the TaskScheduler.FromCurrentSynchronizationContext()
		/// result.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="continuationAction">An action to run when the System.Threading.Tasks.Task completes. When run,
		/// the delegate will be passed the completed task as an argument.</param>
		/// <returns>A new continuation System.Threading.Tasks.Task.</returns>
		public static Task ContinueWithOnScheduler(this Task t, Action<Task> continuationAction)
		{
			return t.ContinueWith(continuationAction, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
		}

		/// <summary>
		/// Creates a continuation that executes when the target System.Threading.Tasks.Task
		/// completes, running on the continuation on the TaskScheduler.FromCurrentSynchronizationContext()
		/// result.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="continuationAction">An action to run when the System.Threading.Tasks.Task completes. When run,
		/// the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The System.Threading.Tasks.Task.CancellationToken that will be assigned to
		/// the new continuation task.</param>
		/// <returns>A new continuation System.Threading.Tasks.Task.</returns>
		public static Task ContinueWithOnScheduler(this Task t, Action<Task> continuationAction, CancellationToken cancellationToken)
		{
			return t.ContinueWith(continuationAction, cancellationToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
		}

		/// <summary>
		/// Creates a continuation that executes when the target System.Threading.Tasks.Task
		/// completes, running on the continuation on the TaskScheduler.FromCurrentSynchronizationContext()
		/// result.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="continuationAction">An action to run when the System.Threading.Tasks.Task completes. When run,
		/// the delegate will be passed the completed task as an argument.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes
		/// criteria, such as System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled,
		/// as well as execution options, such as System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously.</param>
		/// <returns>A new continuation System.Threading.Tasks.Task.</returns>
		public static Task ContinueWithOnScheduler(this Task t, Action<Task> continuationAction, TaskContinuationOptions continuationOptions)
		{
			return t.ContinueWith(continuationAction, CancellationToken.None, continuationOptions, TaskScheduler.FromCurrentSynchronizationContext());
		}

		/// <summary>
		/// Creates a continuation that executes when the target System.Threading.Tasks.Task
		/// completes, running on the continuation on the TaskScheduler.FromCurrentSynchronizationContext()
		/// result.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="continuationAction">An action to run when the System.Threading.Tasks.Task completes. When run,
		/// the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The System.Threading.Tasks.Task.CancellationToken that will be assigned to
		/// the new continuation task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes
		/// criteria, such as System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled,
		/// as well as execution options, such as System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously.</param>
		/// <returns>A new continuation System.Threading.Tasks.Task.</returns>
		public static Task ContinueWithOnScheduler(this Task t, Action<Task> continuationAction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
		{
			return t.ContinueWith(continuationAction, cancellationToken, continuationOptions, TaskScheduler.FromCurrentSynchronizationContext());
		}

		/// <summary>
		/// Creates a continuation that executes when the target System.Threading.Tasks.Task
		/// completes, running on the continuation on the TaskScheduler.FromCurrentSynchronizationContext()
		/// result.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="continuationFunction">An function to run when the System.Threading.Tasks.Task completes. When run,
		/// the delegate will be passed the completed task as an argument.</param>
		/// <returns>A new continuation System.Threading.Tasks.Task.</returns>
		public static Task<TResult> ContinueWithOnScheduler<TResult>(this Task t, Func<Task, TResult> continuationFunction)
		{
			return t.ContinueWith(continuationFunction, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
		}

		/// <summary>
		/// Creates a continuation that executes when the target System.Threading.Tasks.Task
		/// completes, running on the continuation on the TaskScheduler.FromCurrentSynchronizationContext()
		/// result.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="continuationFunction">An function to run when the System.Threading.Tasks.Task completes. When run,
		/// the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The System.Threading.Tasks.Task.CancellationToken that will be assigned to
		/// the new continuation task.</param>
		/// <returns>A new continuation System.Threading.Tasks.Task.</returns>
		public static Task<TResult> ContinueWithOnScheduler<TResult>(this Task t, Func<Task, TResult> continuationFunction, CancellationToken cancellationToken)
		{
			return t.ContinueWith(continuationFunction, cancellationToken, TaskContinuationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
		}

		/// <summary>
		/// Creates a continuation that executes when the target System.Threading.Tasks.Task
		/// completes, running on the continuation on the TaskScheduler.FromCurrentSynchronizationContext()
		/// result.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="continuationFunction">An function to run when the System.Threading.Tasks.Task completes. When run,
		/// the delegate will be passed the completed task as an argument.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes
		/// criteria, such as System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled,
		/// as well as execution options, such as System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously.</param>
		/// <returns>A new continuation System.Threading.Tasks.Task.</returns>
		public static Task<TResult> ContinueWithOnScheduler<TResult>(this Task t, Func<Task, TResult> continuationFunction, TaskContinuationOptions continuationOptions)
		{
			return t.ContinueWith(continuationFunction, CancellationToken.None, continuationOptions, TaskScheduler.FromCurrentSynchronizationContext());
		}

		/// <summary>
		/// Creates a continuation that executes when the target System.Threading.Tasks.Task
		/// completes, running on the continuation on the TaskScheduler.FromCurrentSynchronizationContext()
		/// result.
		/// </summary>
		/// <param name="t">Task to attach the exception continuation to.</param>
		/// <param name="continuationFunction">An function to run when the System.Threading.Tasks.Task completes. When run,
		/// the delegate will be passed the completed task as an argument.</param>
		/// <param name="cancellationToken">The System.Threading.Tasks.Task.CancellationToken that will be assigned to
		/// the new continuation task.</param>
		/// <param name="continuationOptions">Options for when the continuation is scheduled and how it behaves. This includes
		/// criteria, such as System.Threading.Tasks.TaskContinuationOptions.OnlyOnCanceled,
		/// as well as execution options, such as System.Threading.Tasks.TaskContinuationOptions.ExecuteSynchronously.</param>
		/// <returns>A new continuation System.Threading.Tasks.Task.</returns>
		public static Task<TResult> ContinueWithOnScheduler<TResult>(this Task t, Func<Task, TResult> continuationFunction, CancellationToken cancellationToken, TaskContinuationOptions continuationOptions)
		{
			return t.ContinueWith(continuationFunction, cancellationToken, continuationOptions, TaskScheduler.FromCurrentSynchronizationContext());
		}
	}
}
