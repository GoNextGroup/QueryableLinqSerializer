using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using QueryableLinqSerializer.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryableLinqSerializer.Parsers
{
    public class BaseTypeParser : ActionFactory<Type>, ITypeParser
    {
        public TypeParserSettings Settings { get; }

        public BaseTypeParser(TypeParserSettings settings) : base()
        {
            Settings = settings;
        }

        protected override void Initialize()
        {
            base.Initialize();

            AddActivator(typeof(Type), typeof(TypeNode));
        }

        public virtual TypeNode Parse(Type elem)
        {
            if (elem == null) { return null; }

            Console.WriteLine(string.Format("{0}   {1}", elem.DeclaringType, elem.Name));

            return this.Create<TypeNode>(/*elem.GetType()*/ typeof(Type), elem, Settings.Container);
        }
    }
}
