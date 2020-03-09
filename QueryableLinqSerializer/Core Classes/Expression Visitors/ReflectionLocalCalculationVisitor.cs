using MoreLinq.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace QueryableLinqSerializer.Core_Classes.Expression_Visitors
{
    public class ReflectionLocalCalculationVisitor : ExpressionVisitor
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
                    return Expression.Constant(value);
                }
                if (member is PropertyInfo)
                {
                    object value = ((PropertyInfo)member).GetValue(container, null);
                    return Expression.Constant(value);
                }
            }
            return base.VisitMember(memberExpression);
        }
    }

    public class TstVisitor : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            Console.WriteLine("Expression Parameter: {0}, {1}", node.Name, node.Type);

            return base.VisitParameter(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Console.WriteLine("Expression Parameter: {0}, {1}", node.Name, node.ToString());
            node.Parameters.ForEach(e => Console.Write("{0}, ", e.Name));
            Console.WriteLine("\n");

            return base.VisitLambda(node);
        }
    }
}
