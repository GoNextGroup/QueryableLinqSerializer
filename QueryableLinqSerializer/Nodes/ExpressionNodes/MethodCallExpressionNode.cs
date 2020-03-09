using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Extensions;
using QueryableLinqSerializer.Nodes.MemberInfoNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.ExpressionNodes
{
    [Serializable, DataContract(Name = "MethodCallExpression")]
    public class MethodCallExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Arguments))]
        public virtual List<ExpressionNode> Arguments { get; set; }

        [DataMember(Name = nameof(Object))]
        public virtual ExpressionNode Object { get; set; }

        [DataMember(Name = nameof(Method))]
        public virtual MethodInfoNode Method { get; set; }

        protected MethodCallExpressionNode() : base() { Console.WriteLine("MethodCallExpression created"); }
        public MethodCallExpressionNode(MethodCallExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var memberInfoParser = container.GetInstance<IMemberInfoParser>();

            Arguments = expression.Arguments.Select(e => expressionParser.Parse(e)).ToList();
            Object = expression.Object != null ? expressionParser.Parse(expression.Object) : null;
            Method = memberInfoParser.Parse<MethodInfoNode>(expression.Method);
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            if (Arguments?.Count > 0)
            {
                return Object != null ? Expression.Call(Object.FromNode(container), Method.FromNode<MethodInfo>(container), Arguments.Select(e => e.FromNode(container)).ToList()) :
                                        Expression.Call(Method.FromNode<MethodInfo>(container), Arguments.Select(e => e.FromNode(container)).ToList());
            }

            return Object != null ? Expression.Call(Object.FromNode(container), Method.FromNode<MethodInfo>(container)) :
                                    Expression.Call(Method.FromNode<MethodInfo>(container));
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Method Call Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { this.GetType(), Arguments?.GetType() })
                                                 .Concat(Object?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Method?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Arguments?.SelectMany(e => e?.GetKnownTypes(container)).ToList() ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}
