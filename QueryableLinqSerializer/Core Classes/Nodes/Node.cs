using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.ExpressionNodes;
using QueryableLinqSerializer.Nodes.MemberBindingNodes;
using QueryableLinqSerializer.Nodes.MemberInfoNodes;
using QueryableLinqSerializer.Nodes.QueryableExpressionNodes;
using SimpleInjector;

namespace QueryableLinqSerializer.CoreClasses.Nodes
{
    [Serializable, DataContract(Name = "Node")]
    [KnownType(typeof(BinaryExpressionNode))]
    [KnownType(typeof(ConditionalExpressionNode))]
    [KnownType(typeof(ConstantExpressionNode))]
    [KnownType(typeof(QueryableConstantExpressionNode))]
    [KnownType(typeof(InvocationExpressionNode))]
    [KnownType(typeof(LambdaExpressionNode))]
    [KnownType(typeof(ListInitExpressionNode))]
    [KnownType(typeof(MemberExpressionNode))]
    [KnownType(typeof(MemberInitExpressionNode))]
    [KnownType(typeof(MethodCallExpressionNode))]
    [KnownType(typeof(NewArrayExpressionNode))]
    [KnownType(typeof(NewExpressionNode))]
    [KnownType(typeof(ParameterExpressionNode))]
    [KnownType(typeof(TypeBinaryExpressionNode))]
    [KnownType(typeof(UnaryExpressionNode))]
    [KnownType(typeof(MemberAssignmentNode))]
    [KnownType(typeof(MemberListBindingNode))]
    [KnownType(typeof(MemberMemberBindingNode))]
    [KnownType(typeof(MemberInfoNode))]
    [KnownType(typeof(PropertyInfoNode))]
    [KnownType(typeof(FieldInfoNode))]
    [KnownType(typeof(MethodInfoNode))]
    [KnownType(typeof(ConstructorInfoNode))]
    [JsonObject]
    public abstract class Node<T> : IGenerator<T>, ITypeCollector where T: class
    {
        public abstract T FromNode([Optional] Container container);
        public virtual V FromNode<V>([Optional] Container container) where V : class => FromNode() as V;
        public abstract ICollection<Type> GetKnownTypes([Optional] Container container);
    }
}
