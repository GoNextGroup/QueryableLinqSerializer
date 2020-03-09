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

namespace QueryableLinqSerializer.Nodes.ExpressionNodes
{
    [Serializable, DataContract(Name = "ParameterExpression")]
    public class ParameterExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(IsByRef))]
        public virtual bool IsByRef { get; set; }

        [DataMember(Name = nameof(Name))]
        public virtual string Name { get; set; }

        protected ParameterExpressionNode() : base() { Console.WriteLine("ParameterExpression created"); }
        public ParameterExpressionNode(ParameterExpression expression, Container container) : base(expression, container)
        {
            Name = expression.Name;
            IsByRef = expression.IsByRef;
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return IsByRef ? Expression.Parameter(Type.FromNode().MakeByRefType(), Name) : Expression.Parameter(Type.FromNode(), Name);
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Parameter Expression Node KnownType");

            return base.GetKnownTypes(container).Concat(new[] { this.GetType() }).ToList();
        }
    }
}
