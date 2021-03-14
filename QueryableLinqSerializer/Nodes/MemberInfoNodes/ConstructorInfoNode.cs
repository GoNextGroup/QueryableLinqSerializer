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
using MoreLinq.Extensions;

namespace QueryableLinqSerializer.Nodes.MemberInfoNodes
{
    [Serializable, DataContract(Name = "ConstructorInfo")]
    public class ConstructorInfoNode : MemberInfoNode
    {
        public ICollection<TypeNode> Arguments { get; set; }
        public ICollection<ParameterInfoNode> Parameters { get; set; }


        protected ConstructorInfoNode() : base() { Console.WriteLine("ConstructorInfo created"); }
        public ConstructorInfoNode(ConstructorInfo constructor, Container container) : base(constructor, container)
        {
            var typeParser = container.GetInstance<ITypeParser>();

            Arguments = constructor.IsGenericMethod ? constructor.GetGenericArguments()?.Select(e => typeParser.Parse(e)).ToList() : null;
            Parameters = constructor.GetParameters()?.Select(e => new ParameterInfoNode(e, container)).ToList();
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Constructor Info Node KnownType");

            return base.GetKnownTypes(container).Concat( new [] { this.GetType() }).ToList();
        }
        public override MemberInfo FromNode([Optional] Container container)
        {
            var constructorParamNames = Parameters.Select(e => e.Name);
            var constructorParamTypes = Parameters.Select(e => e.ParameterType.FromNode());

            var restoredType = Arguments != null ? DeclaringType.FromNode().MakeGenericType(Arguments.Select(e => e.FromNode()).ToArray()) : DeclaringType.FromNode();


            var constructor = restoredType.GetConstructors().Where(e => e.GetParameters().Select(f => f.Name).OrderBy(x => x).SequenceEqual(constructorParamNames.OrderBy(y => y)) &&
                                                                   e.GetParameters().Select(f => f.ParameterType).OrderBy(x => x).SequenceEqual(constructorParamTypes.OrderBy(y => y))).Single(e => e.Name == Name);

            return constructor;
        }
    }
}
