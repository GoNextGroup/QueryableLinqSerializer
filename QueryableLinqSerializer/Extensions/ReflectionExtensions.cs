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
            return @this.GetInterfaces().Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IEnumerable<>) /*e is IEnumerable*/ && !(e is string)).Any();
        }

        internal static bool IsAsyncGenericEnumerable(this Type @this)
        {
            return @this.GetInterfaces().Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>)).Any();
        }
    }
}
