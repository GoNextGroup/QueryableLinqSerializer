using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Interfaces;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.AuxiliaryNodes
{
    [Serializable, DataContract(Name = "Type")]
    public class TypeNode : IGenerator<Type>, ITypeCollector
    {
        [DataMember(Name = nameof(AssemblyQualifiedName))]
        public virtual string AssemblyQualifiedName { get; set; }

        protected TypeNode() { { Console.WriteLine("Type created"); } }
        public TypeNode(Type type, Container _)
        {
            AssemblyQualifiedName = type?.AssemblyQualifiedName;
        }

        public virtual ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Type Node KnownType");

            return new Type [] { this.FromNode(), this.GetType() };
        }
        public virtual Type FromNode([Optional] Container container)
        {
            return AssemblyQualifiedName != null ? Type.GetType(AssemblyQualifiedName) : null;
        }
    }
}
