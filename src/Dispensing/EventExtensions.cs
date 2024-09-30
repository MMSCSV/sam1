using System;

namespace CareFusion.Dispensing
{
	public static class EventExtensions
	{
		public static void SafeRaise<T>(this EventHandler<T> eventHandler, object source, T args)
			where T : EventArgs
		{
			if (eventHandler == null)
			{
				return;
			}
			eventHandler.Invoke(source, args);
		}
		public static void SafeRaise<T>(this EventHandler<T> eventHandler, T args)
			where T : EventArgs
		{
			SafeRaise(eventHandler, null, args);
		}
		public static void SafeRaise(this EventHandler eventHandler, object source, EventArgs args)
		{
			if (eventHandler == null)
			{
				return;
			}
			eventHandler.Invoke(source, args);
		}
		public static void SafeRaise(this EventHandler eventHandler, EventArgs args)
		{
			SafeRaise(eventHandler, null, args);
		}
		public static void SafeRaise(this EventHandler eventHandler)
		{
			SafeRaise(eventHandler, null, EventArgs.Empty);
		}
	}
}
