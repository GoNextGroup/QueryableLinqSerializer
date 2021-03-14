using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Extensions;
using QueryableLinqSerializer.Nodes.MemberInfoNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.ExpressionNodes
{
    [Serializable, DataContract(Name = "NewExpression")]
    public class NewExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Arguments))]
        public virtual List<ExpressionNode> Arguments { get; set; }

        [DataMember(Name = nameof(Constructor))]
        public virtual ConstructorInfoNode Constructor { get; set; }
        
        [DataMember(Name = nameof(Members))]
        public virtual List<MemberInfoNode> Members { get; set; }

        protected NewExpressionNode() : base() { Console.WriteLine("NewExpression created"); }
        public NewExpressionNode(NewExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var memberInfoParser = container.GetInstance<IMemberInfoParser>();

            Arguments = expression?.Arguments.Select(e => expressionParser.Parse(e)).ToList();
            Constructor = memberInfoParser.Parse<ConstructorInfoNode>(expression.Constructor);
            Members = expression?.Members?.Select(e => memberInfoParser.Parse(e)).ToList();
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);

            if (Constructor != default)
            {
                if (Arguments?.Count > 0)
                {
                    if (Members?.Count > 0) { return Expression.New(Constructor.FromNode<ConstructorInfo>(container), Arguments.Select(e => e.FromNode(container)), Members.Select(e => e.FromNode(container))); }
                    else                    { return Expression.New(Constructor.FromNode<ConstructorInfo>(container), Arguments.Select(e => e.FromNode(container))); }
                }

                return Expression.New(Constructor.FromNode<ConstructorInfo>(container));
            }

            return Expression.New(Type.FromNode());
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("New Expresssion Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { this.GetType()})
                                                 .Concat(Constructor?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Arguments?.SelectMany(e => e?.GetKnownTypes(container)).ToList() ?? Enumerable.Empty<Type>())
                                                 .Concat(Members?.SelectMany(e => e?.GetKnownTypes(container)).ToList() ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}