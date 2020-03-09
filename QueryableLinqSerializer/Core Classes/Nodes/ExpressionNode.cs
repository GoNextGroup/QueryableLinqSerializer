using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.CoreClasses;
using QueryableLinqSerializer.CoreClasses.Nodes;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using QueryableLinqSerializer.Nodes.ExpressionNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Core_Classes.Nodes
{
    [Serializable, DataContract(Name = "Expression")]
    public abstract class ExpressionNode : Node<Expression>
    {
        [DataMember(Name = nameof(NodeType))]
        public virtual ExpressionType NodeType { get; set; }

        [DataMember(Name = nameof(Type))]
        public virtual TypeNode Type { get; set; }

        protected ExpressionNode() : base() {  }
        public ExpressionNode([NotNull] Expression expression, Container container) :base()//, [Optional] IExpressionParser parser) : base()
        {
            var typeParser = container.GetInstance<ITypeParser>();

            NodeType = expression.NodeType;
            Type = typeParser.Parse(expression.Type);

        }
        public override ICollection<Type> GetKnownTypes([Optional]Container container)
        {
            Console.WriteLine("Expression Node KnownType");

            return new Type [] { Type?.FromNode(container), this.GetType(), NodeType.GetType() };
        }
    }   
}
