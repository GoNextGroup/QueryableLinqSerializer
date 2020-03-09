using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using QueryableLinqSerializer.Nodes.MemberBindingNodes;
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
    [Serializable, DataContract(Name = "MemberInitExpression")]
    public class MemberInitExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Bindings))]
        public virtual List<MemberBindingNode> Bindings { get; set; }

        [DataMember(Name = nameof(NewExpression))]
        public virtual NewExpressionNode NewExpression { get; set; }

        protected MemberInitExpressionNode() : base() { Console.WriteLine("MemberInitExpression created"); }
        public MemberInitExpressionNode(MemberInitExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser)  : base(expression, parser)
        {
            var memberBindingParser = container.GetInstance<IMemberBingingParser>();
            var expressionParser = container.GetInstance<IExpressionParser>();

            Bindings = expression.Bindings.Select(e => memberBindingParser.Parse(e)).ToList(); 
            NewExpression = expressionParser.Parse<NewExpressionNode>(expression.NewExpression); 
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return Expression.MemberInit(NewExpression.FromNode<NewExpression>(container), Bindings.Select(e => e.FromNode(container)));
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Member Init Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new[] { this.GetType(), Bindings?.GetType()})
                                                 .Concat(NewExpression?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Bindings?.SelectMany(e => e?.GetKnownTypes(container)).ToList() ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}
