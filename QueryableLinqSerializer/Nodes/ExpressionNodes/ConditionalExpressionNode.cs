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
    [Serializable, DataContract(Name = "ConditionalExpression")]
    public class ConditionalExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(IfFalse))]
        public virtual ExpressionNode IfFalse { get; set; }

        [DataMember(Name = nameof(IfTrue))]
        public virtual ExpressionNode IfTrue { get; set; }

        [DataMember(Name = nameof(Test))]
        public virtual ExpressionNode Test { get; set; }

        protected ConditionalExpressionNode() : base() { Console.WriteLine("ConditionalExpression created"); }
        public ConditionalExpressionNode(ConditionalExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var parser = container.GetInstance<IExpressionParser>();

            IfFalse = parser.Parse(expression.IfFalse);
            IfTrue = parser.Parse(expression.IfTrue);
            Test = parser.Parse(expression.Test);
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return Expression.Condition(Test.FromNode(container), IfTrue.FromNode(container), IfFalse.FromNode(container), Type.FromNode());
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Conditional Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new [] { this.GetType() })
                                                 .Concat(Test?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(IfTrue?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(IfFalse?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}
