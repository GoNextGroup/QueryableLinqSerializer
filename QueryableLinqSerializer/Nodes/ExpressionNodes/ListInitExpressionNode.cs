using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
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
    [Serializable, DataContract(Name = "ListInitExpression")]
    public class ListInitExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Initializers))]
        public virtual List<ElementInitNode> Initializers { get; set; }

        [DataMember(Name = nameof(NewExpression))]
        public virtual NewExpressionNode NewExpression { get; set;  }

        protected ListInitExpressionNode() : base() { Console.WriteLine("ListInitExpression created"); }
        public ListInitExpressionNode(ListInitExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var parser = container.GetInstance<IExpressionParser>();

            Initializers = expression.Initializers.Select(e => new ElementInitNode(e, container)).ToList();
            NewExpression = parser.Parse<NewExpressionNode>(expression.NewExpression);
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return Expression.ListInit((NewExpression)NewExpression.FromNode(container), Initializers.Select(e => e.FromNode(container)));
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("List Init Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new [] { this.GetType(), Initializers?.GetType()})
                                                 .Concat(NewExpression?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Initializers?.SelectMany(e => e?.GetKnownTypes(container)).ToList() ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}