using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QueryableLinqSerializer.Extensions
{
    public static class MemberGenericExtensions
    {
        public static Type? GetMemberGenericType(this MemberInfo member)
        {
            return member?.MemberType switch
            {
                
                MemberTypes.Field => ((FieldInfo)member).FieldType.IsGenericTypeDefinition ? ((FieldInfo)member).FieldType.MakeGenericType(((FieldInfo)member).FieldType.GetGenericArguments()) : null,
                MemberTypes.Property => ((PropertyInfo)member).PropertyType.IsGenericTypeDefinition ? ((PropertyInfo)member).PropertyType.MakeGenericType(((PropertyInfo)member).PropertyType.GetGenericArguments()) : null,
                MemberTypes.Method => ((MethodInfo)member).IsGenericMethodDefinition ? ((MethodInfo)member).MakeGenericMethod(((MethodInfo)member).GetGenericArguments()).GetMemberGenericType() : null,
                _ => null
            };
        }
    }
}