using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.AuxiliaryNodes
{
    [Serializable, DataContract(Name = "ParameterInfo")]
    public class ParameterInfoNode : IGenerator<ParameterInfo>, ITypeCollector
    {
        [DataMember(Name = nameof(Name))]
        public string? Name { get; set; }

        [DataMember(Name = nameof(ParameterType))]
        public Type ParameterType { get; set; }

        [DataMember(Name = nameof(Member))]
        public MemberInfoNode Member { get; set; }

        [DataMember(Name = nameof(HasDefaultValue))]
        public bool HasDefaultValue { get; set; }

        [DataMember(Name = nameof(DefaultValue))]
        public object? DefaultValue { get; set; }
        

        public ParameterInfoNode(ParameterInfo parameterInfo, Container container)
        {
            var parser = container.GetInstance<IMemberInfoParser>();

            ParameterType = parameterInfo.ParameterType;
            HasDefaultValue = parameterInfo.HasDefaultValue;
            DefaultValue = parameterInfo.DefaultValue;

            Member = parameterInfo.Member != null ? parser.Parse(parameterInfo.Member) : null;
        }

        public ICollection<Type> GetKnownTypes()
        {
            var methodTypes = new Type[] { Member.DeclaringType };
            var totalTypes = methodTypes.Concat(Member.GetKnownTypes()).ToList();

            return totalTypes;
        }

        public ParameterInfo FromNode()
        {
            GetTypeBuilder();

           return ParameterType.
        }
    }
}
