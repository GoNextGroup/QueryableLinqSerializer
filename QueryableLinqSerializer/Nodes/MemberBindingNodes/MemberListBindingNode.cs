using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace QueryableLinqSerializer.Nodes.MemberBindingNodes
{
    [Serializable, DataContract(Name = "MemberListBinding")]
    public class MemberListBindingNode : MemberBindingNode
    {
        [DataMember(Name = nameof(Initializers))]
        public virtual List<ElementInitNode> Initializers { get; set; }

        protected MemberListBindingNode() : base() { Console.WriteLine("MemberListBinding created"); }
        public MemberListBindingNode(MemberListBinding memberListBinding, Container container) : base(memberListBinding, container)//, [Optional]IExpressionParser parser) : base(memberListBinding)
        {
            Initializers = memberListBinding.Initializers.Select(e => new ElementInitNode(e, container)).ToList();
        }
        public override MemberBinding FromNode([Optional] Container container)
        {
            Console.WriteLine(BindingType);
            return Expression.ListBind(Member.FromNode(container), Initializers.Select(e => e.FromNode(container)).ToList());
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Member List Binding Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new [] { this.GetType(), Initializers?.GetType() })
                                                 .Concat(Initializers?.SelectMany(e => e?.GetKnownTypes(container).ToList() ?? Enumerable.Empty<Type>()))
                                                 .ToList();

            return totalTypes;
        }
    }
}
