using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.MemberInfoNodes
{
    [Serializable, DataContract(Name = "MethodInfo")]
    public class MethodInfoNode : MemberInfoNode
    {
        /*
        [DataMember(Name = nameof(ReturnParameter))]
        public virtual ParameterInfoNode ReturnParameter { get; set; }
        */

        [DataMember(Name = nameof(ReturnType))]
        public virtual TypeNode ReturnType { get; set; }

        [DataMember(Name = nameof(IsGenericMethod))]
        public virtual bool IsGenericMethod { get; set; }

        [DataMember(Name = nameof(GenericArguments))]
        public virtual List<TypeNode> GenericArguments { get; set; }

        protected MethodInfoNode()  : base() { Console.WriteLine("MethodInfo created"); }
        public MethodInfoNode(MethodInfo method, Container container) : base(method, container)
        {
            var typeParser = container.GetInstance<ITypeParser>();

            ReturnType = typeParser.Parse(method.ReturnType);

            IsGenericMethod = method.IsGenericMethod;
            GenericArguments = IsGenericMethod ? method.GetGenericArguments().Select(e => typeParser.Parse(e)).ToList() : null;
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Method Info Node KnownType");

            return base.GetKnownTypes(container).Concat(new Type[] { this.GetType(), this?.ReturnType?.FromNode(), GenericArguments?.GetType() }
                                                .Concat(GenericArguments?.Select(e => e?.FromNode()) ?? Enumerable.Empty<Type>())).ToList();
        }
        public override MemberInfo FromNode([Optional] Container container)
        {
            var concreteMethodInfo = DeclaringType.FromNode().GetMethods().Where(e => e.Name == Name).FirstOrDefault();

            return IsGenericMethod ? concreteMethodInfo.MakeGenericMethod(GenericArguments.Select(e => e.FromNode()).ToArray()) : concreteMethodInfo;
        }
    }
}
