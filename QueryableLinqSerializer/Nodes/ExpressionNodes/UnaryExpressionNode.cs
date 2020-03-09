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
    [Serializable, DataContract(Name = "UnaryExpression")]
    public class UnaryExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Operand))]
        public virtual ExpressionNode Operand { get; set; }

        [DataMember(Name = nameof(Method))]
        public virtual MethodInfoNode Method { get; set; }


        protected UnaryExpressionNode() : base() { Console.WriteLine("UnaryExpression created"); }
        public UnaryExpressionNode(UnaryExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var memberInfoParser = container.GetInstance<IMemberInfoParser>();

            Method = memberInfoParser.Parse<MethodInfoNode>(expression.Method);
            Operand = expressionParser.Parse(expression.Operand);
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);

            if (NodeType == ExpressionType.UnaryPlus)
            {

                return Method != default ? Expression.UnaryPlus(Operand.FromNode(), Method.FromNode<MethodInfo>()) :
                                           Expression.UnaryPlus(Operand.FromNode());
            }
            
            return Method != default ? Expression.MakeUnary(NodeType, Operand.FromNode(container), Type.FromNode(), Method.FromNode<MethodInfo>(container)) :
                                       Expression.MakeUnary(NodeType, Operand.FromNode(container), Type.FromNode());
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Unary Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { this.GetType() })
                                                 .Concat(Method?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Operand?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}
