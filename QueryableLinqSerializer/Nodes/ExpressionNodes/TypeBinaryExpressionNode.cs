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
    [Serializable, DataContract(Name = "TypeBinaryExpression")]
    public class TypeBinaryExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Expression))]
        public virtual ExpressionNode TBExpression { get; set; }

        [DataMember(Name = nameof(TypeOperand))]
        public virtual TypeNode TypeOperand { get; set; }

        protected TypeBinaryExpressionNode() : base() { Console.WriteLine("TypeBinaryExpression created"); }
        public TypeBinaryExpressionNode(TypeBinaryExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var typeParser = container.GetInstance<ITypeParser>();

            TBExpression = expressionParser.Parse(expression.Expression);
            TypeOperand = typeParser.Parse(expression.TypeOperand);
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return NodeType switch
            {
                ExpressionType.TypeIs    => Expression.TypeIs(TBExpression.FromNode(container), TypeOperand.FromNode()),
                ExpressionType.TypeEqual => Expression.TypeEqual(TBExpression.FromNode(container), TypeOperand.FromNode()),
                _                        => throw new NotImplementedException()
            };
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Type Binary Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { TypeOperand?.FromNode(), this.GetType() })
                                                 .Concat(TBExpression?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}

