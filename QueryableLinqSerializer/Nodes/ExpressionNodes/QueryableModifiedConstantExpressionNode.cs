using QueryableLinqSerializer.Core_Classes.Nodes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.ExpressionNodes
{
    [Serializable, DataContract(Name = "ConstantExpression")]
    public sealed class QueryableModifiedConstantExpressionNode : ExpressionNode
    {
        //[DataMember(Name = nameof(Value))]
        public object Value { get; set; }

        [DataMember(Name = nameof(QueryableValueString))]
        public string QueryableValueString { get; set; }

        [DataMember(Name = nameof(UnderlyingObjectValue))]
        public object UnderlyingObjectValue { get; set; }


        public QueryableModifiedConstantExpressionNode(ConstantExpression expression, Container container) : base(expression)
        {
            Value = expression.Value;
           // ValueObjectString = expression.Value.GetType().AssemblyQualifiedName;
        }
        public override Expression FromNode()
        {
            Console.WriteLine(NodeType);
            return Type != null ? Expression.Constant(Value, Type) : Expression.Constant(Value);
        }

        public override ICollection<Type> GetKnownTypes()
        {
            Type genericType = null;

            if ((Value?.GetType()).IsGenericTypeDefinition)
            {
                var valueType = Value.GetType();
                genericType = valueType.MakeGenericType(valueType.GetGenericArguments());
            }


            var totalTypes = base.GetKnownTypes().Concat(new Type?[] { Value?.GetType(), genericType })
                                                 .ToList();

            return totalTypes;
        }
    }
}
