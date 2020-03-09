using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.CoreClasses.Nodes;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Core_Classes.Nodes
{
    [Serializable, DataContract(Name = "MemberInfo")]
    public abstract class MemberInfoNode : Node<MemberInfo>
    {
        [DataMember(Name = nameof(MemberType))]
        public virtual MemberTypes MemberType { get; set; }

        [DataMember(Name = nameof(DeclaringType))]
        public virtual TypeNode DeclaringType { get; set;  }
        
        [DataMember(Name = nameof(Name))]
        public virtual string Name { get; set; }

        protected MemberInfoNode() { }
        public MemberInfoNode(MemberInfo member, Container container)
        {
            var typeParser = container.GetInstance<ITypeParser>();

            MemberType = member.MemberType;
            DeclaringType = typeParser.Parse(member?.DeclaringType);
            Name = member?.Name;
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Member Info Node KnownType");

            return new Type [] { DeclaringType?.FromNode(), MemberType.GetType(), this.GetType(), Name?.GetType() };
        }
    }
}
