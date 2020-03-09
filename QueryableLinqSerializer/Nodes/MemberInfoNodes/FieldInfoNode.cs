using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
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
    [Serializable, DataContract(Name = "FieldInfo")]
    public class FieldInfoNode : MemberInfoNode
    {
        [DataMember(Name = nameof(FieldType))]
        public virtual TypeNode FieldType { get; set; }

        protected FieldInfoNode() : base() { Console.WriteLine("FieldInfo created"); }
        public FieldInfoNode(FieldInfo field, Container container) : base(field, container)
        {
            var typeParser = container.GetInstance<ITypeParser>();

            FieldType = typeParser.Parse(field.FieldType);
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Field Info Node KnownType");

            return base.GetKnownTypes(container).Concat(new Type[] { this.GetType(), this?.FieldType?.FromNode() }).ToList();
        }
        public override MemberInfo FromNode([Optional] Container container)
        {
            return DeclaringType.FromNode().GetFields().Single(e => e.FieldType == FieldType.FromNode() && e.Name == Name);
        }
    }
}
