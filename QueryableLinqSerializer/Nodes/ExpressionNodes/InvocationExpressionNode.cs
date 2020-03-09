using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.ExpressionNodes
{
    [Serializable, DataContract(Name = "InvocationExpression")]
    public class InvocationExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Arguments))]
        public virtual List<ExpressionNode> Arguments { get; set; }

        [DataMember(Name = nameof(IExpression))]
        public virtual ExpressionNode IExpression { get; set; }

        protected InvocationExpressionNode() : base() { Console.WriteLine("InvocationExpression created"); }
        public InvocationExpressionNode(InvocationExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var parser = container.GetInstance<IExpressionParser>();

            Arguments = expression.Arguments.Select(e => parser.Parse(e)).ToList();
            IExpression = parser.Parse(expression.Expression);
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return Expression.Invoke(IExpression.FromNode(container), Arguments.Select(e => e.FromNode(container)));
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Invocation Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new [] { this.GetType(), Arguments?.GetType() })
                                                 .Concat(IExpression?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Arguments?.SelectMany(e => e.GetKnownTypes(container)).ToList() ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}