using MoreLinq.Extensions;
using QueryableLinqSerializer.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace QueryableLinqSerializer.Core_Classes.Expression_Visitors
{

    public class ExpressionTransformationVisitor : ExpressionVisitor
    {
        protected override Expression VisitMember   // <-- Need to realize caching scenario by Expression and Compile for Expression Tree (see here: https://stackoverflow.com/questions/38528620/c-sharp-fieldinfo-reflection-alternatives)
            (MemberExpression memberExpression)
        {
            // Recurse down to see if we can simplify...
            var expression = Visit(memberExpression.Expression);

            // If we've ended up with a constant, and it's a property or a field,
            // we can simplify ourselves to a constant
            if (expression is ConstantExpression)
            {
                object container = ((ConstantExpression)expression).Value;
                var member = memberExpression.Member;
                if (member is FieldInfo)
                {
                    object value = ((FieldInfo)member).GetValue(container);

                    if (value.GetType().IsGenericPipeline()) { return Expression.Constant(EnumerableExtensions.AnonymousGenericEnumerableToList(value)); }

                    //if (value.GetType().GetInterfaces().Select(e => e.GetGenericTypeDefinition()).Contains(typeof(IEnumerable<>)))
                    //{
                    //    var asEnumerableMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(value.GetType().GetGenericArguments());
                    //    var asEnumerabled = asEnumerableMethod.Invoke(null, new object[] { value });
                    //
                    //    return Expression.Constant(asEnumerabled, asEnumerabled.GetType());
                    //}                       

                    return Expression.Constant(value);
                }
                if (member is PropertyInfo)
                {
                    object value = ((PropertyInfo)member).GetValue(container, null);

                    if (value.GetType().IsGenericEnumerable()) { return Expression.Constant(EnumerableExtensions.AnonymousGenericEnumerableToList(value)); }


                    return Expression.Constant(value);
                }
            }

            return base.VisitMember(memberExpression);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            /*
            if (!((node.Value?.GetType().IsGenericType ?? false) && node.Value?.GetType().GetGenericTypeDefinition() == typeof(EntityQueryable<>)))
            {
                if ((node.Value?.GetType().IsGenericEnumerable() ?? false) && !(node.Value is string))
                {
                    var asEnumerableMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(node.Value.GetType().GetGenericArguments());
                    var asEnumerabled = asEnumerableMethod.Invoke(null, new object[] { node.Value });

                    return Expression.Constant(asEnumerabled, asEnumerabled.GetType());  //////Type != null ? Expression.Constant(asEnumerabled, Type.FromNode()) : Expression.Constant(asEnumerabled);
                }
            }
            */

            return base.VisitConstant(node);
        }
    }
}
