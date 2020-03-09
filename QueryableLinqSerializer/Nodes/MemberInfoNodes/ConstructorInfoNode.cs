using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes.Nodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.MemberInfoNodes
{
    [Serializable, DataContract(Name = "ConstructorInfo")]
    public class ConstructorInfoNode : MemberInfoNode
    {
        protected ConstructorInfoNode() : base() { Console.WriteLine("ConstructorInfo created"); }
        public ConstructorInfoNode(ConstructorInfo constructor, Container container) : base(constructor, container)
        {
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Constructor Info Node KnownType");

            return base.GetKnownTypes(container).Concat( new [] { this.GetType() }).ToList();
        }
        public override MemberInfo FromNode([Optional] Container container)
        {
            return DeclaringType.FromNode().GetConstructors().Single(e => e.Name == Name);
        }
    }
}
