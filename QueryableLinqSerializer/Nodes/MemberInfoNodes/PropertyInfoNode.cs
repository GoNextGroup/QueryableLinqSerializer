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
    [Serializable, DataContract(Name = "PropertyInfo")]
    public class PropertyInfoNode : MemberInfoNode
    {
        [DataMember(Name = nameof(PropertyType))]
        public virtual TypeNode PropertyType { get; set; }

        protected PropertyInfoNode() : base() { Console.WriteLine("PropertyInfo created"); }
        public PropertyInfoNode(PropertyInfo property, Container container) : base(property, container)
        {
            var typeParser = container.GetInstance<ITypeParser>();

            PropertyType = typeParser.Parse(property.PropertyType);
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Property Info Node KnownType");

            return base.GetKnownTypes(container).Concat(new Type [] { this.GetType(), this?.PropertyType?.FromNode() }).ToList();
        }
        public override MemberInfo FromNode([Optional] Container container)
        {
            return DeclaringType.FromNode().GetProperties().Single(e => e.PropertyType == PropertyType.FromNode() && e.Name == Name);
        }
    }
}
