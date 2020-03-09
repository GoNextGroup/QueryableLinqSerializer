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
    [Serializable, DataContract(Name = "ConstantExpression")]
    public class ConstantExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Value))]
        public virtual object Value { get; set; }

        protected ConstantExpressionNode() : base() { Console.WriteLine("ConstantExpression created"); }
        public ConstantExpressionNode(ConstantExpression expression, Container container) : base(expression, container)
        {
            //var parser = container.GetInstance<IExpressionParser>();

            //Value = expression.Value.GetType().IsAssignableFrom(typeof(Expression)) ? parser.Parse(expression.Value as Expression) : expression.Value;
            Value = expression.Value;
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return Value != null ? Expression.Constant(Value, Type.FromNode()) : Expression.Constant(Value);
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Constant Expression Node KnownType");

            Type genericType = null;

            if ((Value?.GetType()).IsGenericTypeDefinition)
            {
                var valueType = Value.GetType();
                genericType = valueType.MakeGenericType(valueType.GetGenericArguments());
            }

            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { Value?.GetType(), genericType, this.GetType() })
                                                 .ToList();

            return totalTypes;
        }
    }
}