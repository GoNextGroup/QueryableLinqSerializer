using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QueryableLinqSerializer.Extensions
{
    internal static class ExpressionExtensions
    {
        internal static IEnumerable<Expression> GetLinkNodes(this Expression expression)
        {
            switch (expression)
            {
                case LambdaExpression lambdaExpression:
                    {
                        yield return lambdaExpression.Body;
                        foreach (var parameter in lambdaExpression.Parameters)
                            yield return parameter;
                        break;
                    }
                case BinaryExpression binaryExpression:
                    yield return binaryExpression.Left;
                    yield return binaryExpression.Right;
                    break;
                case ConditionalExpression conditionalExpression:
                    yield return conditionalExpression.IfTrue;
                    yield return conditionalExpression.IfFalse;
                    yield return conditionalExpression.Test;
                    break;
                case InvocationExpression invocationExpression:
                    {
                        yield return invocationExpression.Expression;
                        foreach (var argument in invocationExpression.Arguments)
                            yield return argument;
                        break;
                    }
                case ListInitExpression listInitExpression:
                    yield return listInitExpression.NewExpression;
                    break;
                case MemberExpression memberExpression:
                    yield return memberExpression.Expression;
                    break;
                case MemberInitExpression memberInitExpression:
                    yield return memberInitExpression.NewExpression;
                    break;
                case MethodCallExpression methodCallExpression:
                    {
                        foreach (var argument in methodCallExpression.Arguments)
                            yield return argument;
                        if (methodCallExpression.Object != null)
                            yield return methodCallExpression.Object;
                        break;
                    }
                case NewArrayExpression newArrayExpression:
                    {
                        foreach (var item in newArrayExpression.Expressions)
                            yield return item;
                        break;
                    }
                case NewExpression newExpression:
                    {
                        foreach (var item in newExpression.Arguments)
                            yield return item;
                        break;
                    }
                case TypeBinaryExpression typeBinaryExpression:
                    yield return typeBinaryExpression.Expression;
                    break;
                case UnaryExpression unaryExpression:
                    yield return unaryExpression.Operand;
                    break;
            }
        }

        internal static IEnumerable<Expression> GetNodes(this Expression expression)
        {
            foreach (var node in expression.GetLinkNodes())
            {
                foreach (var subNode in node.GetNodes())
                    yield return subNode;
            }
            yield return expression;
        }

        internal static IEnumerable<TExpression> GetNodes<TExpression>(this Expression expression) where TExpression : Expression
        {
            return expression.GetNodes().OfType<TExpression>();
        }
    }
}
