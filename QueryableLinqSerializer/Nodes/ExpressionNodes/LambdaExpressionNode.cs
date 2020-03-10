using Newtonsoft.Json;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Extensions;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace QueryableLinqSerializer.Nodes.ExpressionNodes
{
    [Serializable, DataContract(Name = "LambdaExpression")]
    public class LambdaExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Body))]
        public virtual ExpressionNode Body { get; set; }

        [DataMember(Name = nameof(Parameters))]
        public virtual List<ParameterExpressionNode> Parameters { get; set; }

        [DataMember(Name = nameof(ReturnType))]
        public virtual TypeNode ReturnType { get; set; }

        [DataMember(Name = nameof(TailCall))]
        public virtual bool TailCall { get; set; }

        protected LambdaExpressionNode() :base() { Console.WriteLine("LambdaExpression created"); }
        public LambdaExpressionNode(LambdaExpression expression, Container container) : base(expression, container)//, [Optional]IExpressionParser parser) : base(expression, parser)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var typeParser = container.GetInstance<ITypeParser>();

            TailCall = expression.TailCall;
            ReturnType = typeParser.Parse(expression.ReturnType);
            Parameters = expression.Parameters.Select(e => expressionParser.Parse<ParameterExpressionNode>(e)).ToList();
            Body = expressionParser.Parse(expression.Body);
        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);
            var parameterExpressions = Parameters.Select(e => e.FromNode<ParameterExpression>(container)).ToList();

            var restoredBody = Body.FromNode(container);
            var bodyParameters = restoredBody.GetNodes().OfType<ParameterExpression>().ToArray();

            for (var i = 0; i < parameterExpressions.Count; ++i)
            {
                var matchingParameter = bodyParameters.Where(p => p.Name == parameterExpressions[i].Name && p.Type == parameterExpressions[i].Type).ToArray();
                if (matchingParameter.Length == 1)
                    parameterExpressions[i] = matchingParameter.First();
            }

            return Expression.Lambda(Type.FromNode(), restoredBody, TailCall, parameterExpressions);
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Lambda Expression Node KnownType");

            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { ReturnType?.FromNode(), Parameters?.GetType(), this.GetType() })
                                                 .Concat(Body?.GetKnownTypes(container) ?? Enumerable.Empty<Type>())
                                                 .Concat(Parameters?.SelectMany(e => e?.GetKnownTypes(container)).ToList() ?? Enumerable.Empty<Type>())                                                 
                                                 .ToList();

            return totalTypes;
        }
    }
}
