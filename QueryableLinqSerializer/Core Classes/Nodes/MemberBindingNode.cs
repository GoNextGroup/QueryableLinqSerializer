using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.CoreClasses;
using QueryableLinqSerializer.CoreClasses.Nodes;
using QueryableLinqSerializer.Nodes.MemberBindingNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Core_Classes.Nodes
{
    [Serializable, DataContract(Name = "MemberBinding")]
    public abstract class MemberBindingNode : Node<MemberBinding>
    {
        [DataMember(Name = nameof(BindingType))]
        public virtual MemberBindingType BindingType { get; set; }

        [DataMember(Name = nameof(Member))]
        public virtual MemberInfoNode Member { get; set; }

        protected MemberBindingNode() { }
        public MemberBindingNode(MemberBinding member, Container container)
        {
            var memberInfoParser = container.GetInstance<IMemberInfoParser>();

            Member = memberInfoParser.Parse(member.Member);
            BindingType = member.BindingType;
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Member Bindings Node KnownType");

            return (new Type[] { Member?.GetType(), this.GetType(), BindingType.GetType() }).Concat(Member?.GetKnownTypes() ?? Enumerable.Empty<Type>()).ToList();
        }
    }
}
