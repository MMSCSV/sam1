using System;
using System.Threading;

namespace CareFusion.Dispensing
{
	public static class SyncronizationContextExtensions
	{
		public static void ForwardCall(this SynchronizationContext context, Action action)
		{
			if (context != null && context != SynchronizationContext.Current)
			{
				context.Send(delegate
				{
					ForwardCall(context, action);
				}, null);
				return;
			}
			action();
		}
		public static void ForwardCallAsync(this SynchronizationContext context, Action action)
		{
			if (context != null && context != SynchronizationContext.Current)
			{
				context.Post(delegate
				{
					ForwardCallAsync(context, action);
				}, null);
				return;
			}
			action();
		}
	}
}
