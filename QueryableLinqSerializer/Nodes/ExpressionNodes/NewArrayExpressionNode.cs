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
    [Serializable, DataContract(Name = "NewArrayExpression")]
    public class NewArrayExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Expressions))]
        public virtual List<ExpressionNode> Expressions { get; set; }

        protected NewArrayExpressionNode() : base() { Console.WriteLine("NewArrayExpression created"); }
        public NewArrayExpressionNode(NewArrayExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var parser = container.GetInstance<IExpressionParser>();

            Expressions = expression.Expressions.Select(e => parser.Parse(e)).ToList();
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return NodeType switch
            {
                ExpressionType.NewArrayBounds => Expression.NewArrayBounds(Type.FromNode(), Expressions.Select(e => e.FromNode(container))),
                ExpressionType.NewArrayInit   => Expression.NewArrayInit(Type.FromNode(), Expressions.Select(e => e.FromNode(container))),
                _                             => throw new NotImplementedException()
            };
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("New Array Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new [] { this.GetType(), Expressions?.GetType() })
                                                 .Concat(Expressions?.SelectMany(e => e?.GetKnownTypes(container)).ToList() ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}
