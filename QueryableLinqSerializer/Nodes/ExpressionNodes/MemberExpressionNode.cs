using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Extensions;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace QueryableLinqSerializer.Nodes.ExpressionNodes
{
    [Serializable, DataContract(Name = "MemberExpression")]
    public class MemberExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(MExpression))]
        public virtual ExpressionNode MExpression { get; set; }

        [DataMember(Name = nameof(Member))]
        public virtual MemberInfoNode Member { get; set; }

        protected MemberExpressionNode() : base() { Console.WriteLine("MemberExpression created"); }
        public MemberExpressionNode(MemberExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var memberInfoParser = container.GetInstance<IMemberInfoParser>();

            if (expression.Expression is ConstantExpression)
            //if (typeof(Derived).IsSubclassOf(typeof(SomeType)))
            {
                object underlyingValue = ((ConstantExpression)expression.Expression).Value;
                var member = expression.Member;
                if (member is FieldInfo)
                {
                    object value = ((FieldInfo)member).GetValue(underlyingValue);
                    MExpression = expressionParser.Parse(Expression.Constant(value));
                    
                    Member = memberInfoParser.Parse(value.GetType().GetField(((FieldInfo)member).Name));
                }
                if (member is PropertyInfo)
                {
                    object value = ((PropertyInfo)member).GetValue(underlyingValue, null);
                    MExpression = expressionParser.Parse(Expression.Constant(value));

                    Member = memberInfoParser.Parse(value.GetType().GetField(((PropertyInfo)member).Name));
                }
            }
            else
            {
                MExpression = expressionParser.Parse(expression.Expression);
                Member = memberInfoParser.Parse(expression.Member);
            }


            /*
            MExpression = expressionParser.Parse(expression.Expression);
            Member = memberInfoParser.Parse(expression.Member);
            */
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            return Expression.MakeMemberAccess(MExpression.FromNode(container), Member.FromNode(container));
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Member Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { this.GetType(), Member?.GetType() })
                                                 .Concat(Member?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(MExpression?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .ToList();

            return totalTypes;
        }
    }
}