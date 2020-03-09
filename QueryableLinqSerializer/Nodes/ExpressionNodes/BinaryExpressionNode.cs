using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Extensions;
using QueryableLinqSerializer.Nodes;
using QueryableLinqSerializer.Nodes.MemberInfoNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace QueryableLinqSerializer.Nodes.ExpressionNodes
{
    [Serializable, DataContract(Name = "BinaryExpression")]
    public class BinaryExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Conversion))]
        public virtual LambdaExpressionNode Conversion { get; set; } // <-- changed private set to public set for eliminating reflection usage during serialization/deserialization
        
        [DataMember(Name = nameof(IsLifToNull))]
        public virtual bool IsLifToNull { get; set; }
        
        [DataMember(Name = nameof(Left))]
        public virtual ExpressionNode Left { get; set; }        
        
        [DataMember(Name = nameof(Right))]
        public virtual ExpressionNode Right { get; set; }

        [DataMember(Name = nameof(Method))]
        public virtual MethodInfoNode Method { get; set; }

        protected BinaryExpressionNode() : base() { Console.WriteLine("BinaryExpression created"); }
        public BinaryExpressionNode(BinaryExpression expression, Container container) : base(expression, container)//, , [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var memberInfoParser = container.GetInstance<IMemberInfoParser>();

            Left = expressionParser.Parse(expression.Left); 
            Right = expressionParser.Parse(expression.Right); 
            Conversion = expressionParser.Parse<LambdaExpressionNode>(expression.Conversion); 
            Method = memberInfoParser.Parse<MethodInfoNode>(expression.Method);
            IsLifToNull = expression.IsLiftedToNull;
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);

            if (Method != default)
            {
                return Conversion != default ? Expression.MakeBinary(NodeType, Left.FromNode(container), Right.FromNode(container), IsLifToNull, Method.FromNode<MethodInfo>(container), Conversion.FromNode(container) as LambdaExpression) :
                                               Expression.MakeBinary(NodeType, Left.FromNode(container), Right.FromNode(container), IsLifToNull, Method.FromNode<MethodInfo>(container));

            }

            return Expression.MakeBinary(NodeType, Left.FromNode(container), Right.FromNode(container));
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Binary Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { Method?.GetType(), this.GetType() })
                                                 .Concat(Method?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Left?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Right?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Conversion?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}
