using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.MemberBindingNodes
{
    [Serializable, DataContract(Name = "MemberAssignment")]
    public class MemberAssignmentNode : MemberBindingNode //IGenerator<MemberAssignment>
    {
        [DataMember(Name = nameof(MAExpression))]
        public virtual ExpressionNode MAExpression { get; set; }

        protected MemberAssignmentNode() : base() { Console.WriteLine("MemberAssignment created"); }
        public MemberAssignmentNode(MemberAssignment memberAssignment, Container container) : base(memberAssignment, container)//, [Optional]IExpressionParser parser) : base(memberAssignment)
        {
            var parser = container.GetInstance<IExpressionParser>();

            MAExpression = parser.Parse(memberAssignment.Expression);
            BindingType = memberAssignment.BindingType;
        }
        public override MemberBinding FromNode([Optional] Container container)
        {
            Console.WriteLine(BindingType);
            return Expression.Bind(Member.FromNode(container), MAExpression.FromNode(container));
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Member Assignment Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(MAExpression?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}
