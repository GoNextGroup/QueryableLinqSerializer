using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.MemberInfoNodes;
using QueryableLinqSerializer.Settings;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace QueryableLinqSerializer.Parsers
{
    public class BaseMemberInfoParser : ActionFactory<MemberTypes>, IMemberInfoParser
    {
        public MemberInfoParserSettings Settings { get; }
        public BaseMemberInfoParser(MemberInfoParserSettings settings) : base()
        {
            Settings = settings;
         }

        protected override void Initialize()
        {
            base.Initialize();

            AddActivator(MemberTypes.Constructor, typeof(ConstructorInfoNode));
            AddActivator(MemberTypes.Field, typeof(FieldInfoNode));
            AddActivator(MemberTypes.Method, typeof(MethodInfoNode));
            AddActivator(MemberTypes.Property, typeof(PropertyInfoNode));
        }
        
        public virtual MemberInfoNode Parse(MemberInfo elem)
        {
            if (elem == null) { return null; }

            Console.WriteLine(string.Format("{0}   {1}", elem.DeclaringType, elem.Name));

            return this.Create<MemberInfoNode>(elem.MemberType, elem, Settings.Container);
        }
        
    }
}
