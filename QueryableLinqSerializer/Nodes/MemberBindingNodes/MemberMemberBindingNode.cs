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
using System.Text;

namespace QueryableLinqSerializer.Nodes.MemberBindingNodes
{
    [Serializable, DataContract(Name = "MemberMemberBinding")]
    public class MemberMemberBindingNode : MemberBindingNode
    {
        [DataMember(Name = nameof(Bindings))]
        public virtual List<MemberBindingNode> Bindings { get; set; }

        protected MemberMemberBindingNode() : base() { Console.WriteLine("MemberMemberBinding created"); }
        public MemberMemberBindingNode(MemberMemberBinding memberMemberBindings, Container container) : base(memberMemberBindings, container)//, [Optional]IExpressionParser parser) : base(member)
        {
            var memberBindingParser = container.GetInstance<IMemberBingingParser>();

            Bindings = memberMemberBindings.Bindings.Select(e => memberBindingParser.Parse(e)).ToList();
        }
        public override MemberBinding FromNode([Optional] Container container)
        {
            Console.WriteLine(BindingType);
            return Expression.MemberBind(Member.FromNode(container), Bindings.Select(e => e.FromNode(container)));
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Member Member Binding Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new [] { this.GetType(), Bindings?.GetType() })
                                                 .Concat(Bindings?.SelectMany(e => e?.GetKnownTypes(container).ToList()) ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}
