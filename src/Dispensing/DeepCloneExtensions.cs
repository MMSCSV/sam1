using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CareFusion.Dispensing
{
	public static class DeepCloneExtensions
	{
		/// <summary>
		/// This dictionary caches the delegates for each 'to-clone' type.
		/// </summary>
		static Dictionary<Type, Delegate> _cachedIL = new Dictionary<Type, Delegate>();

		public static T Clone<T>(this T source)
		{
			var sourceType = source.GetType();

			var fis = GetAllFields(sourceType);
			var clone = Activator.CreateInstance(sourceType);

			foreach (FieldInfo fi in fis)
			{
				fi.SetValue(clone, fi.GetValue(source));
			}

			return (T)clone;
		}

		static IEnumerable<FieldInfo> GetAllFields(Type t)
		{
			if (t == null)
			{
				return Enumerable.Empty<FieldInfo>();
			}

			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			return t.GetFields(flags).Union(GetAllFields(t.BaseType));
		}
	}
}
