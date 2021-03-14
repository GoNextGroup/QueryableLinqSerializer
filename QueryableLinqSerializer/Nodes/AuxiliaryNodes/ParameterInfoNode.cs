using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.AuxiliaryNodes
{
    [Serializable, DataContract(Name = "ParameterInfo")]
    public class ParameterInfoNode
    {
        [DataMember(Name = nameof(Name))]
        public virtual string? Name { get; set; }
        [DataMember(Name = nameof(ParameterType))]
        public virtual TypeNode ParameterType { get; set;  }
        //public virtual MemberInfo Member { get; }  --> possible this property needs to check in nested lists of the Expression Tree
        //public virtual bool HasDefaultValue { get; }
        //public virtual IEnumerable<CustomAttributeData> CustomAttributes { get; }
        //public virtual ParameterAttributes Attributes { get; }
        //public virtual object? DefaultValue { get; }

        protected ParameterInfoNode() : base() { Console.WriteLine("ParameterInfo created"); }
        public ParameterInfoNode(ParameterInfo parameter, Container container)
        {
            var typeParser = container.GetInstance<ITypeParser>();

            ParameterType = typeParser.Parse(parameter.ParameterType);
            Name = parameter.Name;
        }
        public virtual ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Parameter Info Node KnownType");

            return new Type[] { this.GetType(), this?.ParameterType?.FromNode() };
        }

        public virtual ParameterInfo FromNode([Optional] Container container) => null;
        /*
        public virtual ParameterInfo FromNode([Optional] Container container)
        {
            return DeclaringType.FromNode().GetFields().Single(e => e.FieldType == FieldType.FromNode() && e.Name == Name);
        }
        */


    }
}
