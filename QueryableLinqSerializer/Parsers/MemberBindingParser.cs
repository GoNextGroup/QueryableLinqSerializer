using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.MemberBindingNodes;
using QueryableLinqSerializer.Settings;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryableLinqSerializer.Parsers
{
    public class BaseMemberBindingParser: ActionFactory<MemberBindingType>, IMemberBingingParser
    {
        public MemberBindingParserSettings Settings { get; }

        public BaseMemberBindingParser(MemberBindingParserSettings settings) : base()
        {
            Settings = settings;
        }

        protected override void Initialize()
        {
            base.Initialize();

            AddActivator(MemberBindingType.Assignment, typeof(MemberAssignmentNode));
            AddActivator(MemberBindingType.MemberBinding, typeof(MemberListBindingNode));
            AddActivator(MemberBindingType.ListBinding, typeof(MemberMemberBindingNode));
        }
        public virtual MemberBindingNode Parse(MemberBinding elem)
        {
            if (elem == null) { return null; }

            Console.WriteLine(string.Format("{0}   {1}", elem.BindingType, elem.Member?.DeclaringType));

            return this.Create<MemberBindingNode>(elem.BindingType, elem, Settings.Container);
        }
    }
}
