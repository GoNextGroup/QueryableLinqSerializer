using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Extensions;
using QueryableLinqSerializer.Nodes.MemberInfoNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.AuxiliaryNodes
{
    [Serializable, DataContract(Name = "ElementInit")]
    public class ElementInitNode : IGenerator<ElementInit>, ITypeCollector
    {
        [DataMember(Name = nameof(AddMethod))]
        public virtual MethodInfoNode AddMethod { get; set; }

        [DataMember(Name = nameof(Arguments))]
        public virtual List<ExpressionNode> Arguments { get; set; }

        protected ElementInitNode() { Console.WriteLine("ElementInit created"); }
        public ElementInitNode(ElementInit element, Container container)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var memberInfoParser = container.GetInstance<IMemberInfoParser>();

            Arguments = element.Arguments.Select(e => expressionParser.Parse(e)).ToList();
            AddMethod = memberInfoParser.Parse<MethodInfoNode>(element.AddMethod);
        }
        public ElementInit FromNode([Optional] Container container)
        {
            return Expression.ElementInit(AddMethod.FromNode<MethodInfo>(container), Arguments.Select(e => e.FromNode(container)));
        }
        public virtual ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Element Init Node KnownType");

            var methodTypes = new Type[] { Arguments?.GetType(), this.GetType() };
            var totalTypes = methodTypes.Concat(Arguments?.Select(e => e.Type.FromNode()) ?? Enumerable.Empty<Type>()).Concat(AddMethod?.GetKnownTypes(container) ?? Enumerable.Empty<Type>()).ToList();

            return totalTypes;
        }
    }
}
