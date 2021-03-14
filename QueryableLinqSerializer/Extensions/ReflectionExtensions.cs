using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryableLinqSerializer.Extensions
{
    public static class EnumerableExtensions
    {
        internal static bool IsGenericEnumerable(this Type @this)
        {
            return @this.GetInterfaces().Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IEnumerable<>) && !(e is string)).Any();
        }

        internal static bool IsGenericPipeline(this Type @this)
        {
            return @this.GetInterfaces().Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IEnumerator<>)).Any();
        }

        internal static bool IsNongenericPipeline(this Type @this)
        {
            return @this.GetInterfaces().Where(e => e is IEnumerator && !e.IsGenericType).Any();
        }

        internal static bool IsAsyncGenericEnumerable(this Type @this)
        {
            return @this.GetInterfaces().Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)).Any();
        }

        internal static object AnonymousGenericEnumerableToList(object value)
        {
            var asEnumerableMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(value.GetType().GetGenericArguments().Last());
            var asEnumerabled = asEnumerableMethod.Invoke(null, new object[] { value });

            return asEnumerabled;
        }
    }
}
