using System;
using System.Collections.Generic;
using System.Text;
using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using SimpleInjector;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using System.Collections;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using QueryableLinqSerializer.Extensions;
using EFCoreDataModel.DataClasses.Users.Base;
using MoreLinq.Extensions;

namespace QueryableLinqSerializer.Nodes.QueryableExpressionNodes
{
    [Serializable, DataContract(Name = "QueryableConstantExpression")]
    public class QueryableConstantExpressionNode : ExpressionNode
    {
        [DataMember(Name = nameof(Value))]
        public virtual object Value { get; set; }

        [DataMember(Name = nameof(IsEntityQueryable))]
        public virtual bool IsEntityQueryable { get; set; }

        [DataMember(Name = nameof(GenericTypes))]
        public virtual List<TypeNode> GenericTypes { get; set; }

        private QueryableConstantExpressionNode() : base() { Console.WriteLine("QueryableConstantExpression created"); }
        public QueryableConstantExpressionNode(ConstantExpression expression, Container container) : base(expression, container)
        {
            //var parser = container.GetInstance<IExpressionParser>();
            var typeParser = container.GetInstance<ITypeParser>();

            //Value = expression.Value.GetType().IsAssignableFrom(typeof(Expression)) ? parser.Parse(expression.Value as Expression) : expression.Value;
            //Value = expression.Value;

            //if (Value.GetType().GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryable<>)))
            if (expression.Value.GetType().IsGenericType && expression.Value.GetType().GetGenericTypeDefinition() == typeof(EntityQueryable<>))
            {
                var constantExpressionGenericTypes = expression.Value.GetType().GetGenericArguments();
                GenericTypes = constantExpressionGenericTypes.Select(e => typeParser.Parse(e)).ToList();
                
                var asEnumerableMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(constantExpressionGenericTypes);
                Value = asEnumerableMethod.Invoke(null, new object[] { expression.Value });                

                 IsEntityQueryable = true;
            }
            else
            {
                Console.WriteLine("{0}", expression.Value.GetType());
                expression.Value.GetType().GetInterfaces().ForEach(e => Console.WriteLine(e.ToString()));

                var valueType = expression.Value?.GetType();
                if (valueType != null && valueType.IsGenericType) { GenericTypes = valueType.GetGenericArguments().Select(e => typeParser.Parse(e)).ToList(); }

                /*
                var asEnumerableMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.AsEnumerable)).MakeGenericMethod(valueType.GetGenericArguments());
                Value = asEnumerableMethod.Invoke(null, new object[] { expression.Value });
                */

                
                Value = expression.Value;
                

                IsEntityQueryable = false;
            }

        }
        public override Expression FromNode([Optional] Container container)
        {
            Console.WriteLine(NodeType);

            if (IsEntityQueryable)
            {
                var queryProvider = container.GetInstance<IQueryProvider>();

                var entityQueryableType = typeof(EntityQueryable<>).MakeGenericType(GenericTypes.Select(e => e.FromNode()).ToArray());
                var  generatedEntity = Activator.CreateInstance(entityQueryableType, queryProvider, Expression.Constant(Value));//, QExpression.FromNode(container));

                ///Expression.Constant(Value);//(ConstantExpression)generatedExpression;///Expression.Constant(generatedEntity); //Type != null ? Expression.Constant(generatedEntity, Type.FromNode()) : Expression.Constant(generatedEntity);
                return Expression.Constant(generatedEntity); 
            }

            if (Value is string)
            {
                return Expression.Constant(Guid.TryParse((string)Value, out var guid) ? guid : Value);
            }

            if (Value.GetType().IsGenericEnumerable())
            {

                //var asEnumerableMethod = typeof(Queryable).GetMethods().Where(f => f.IsGenericMethod && f.Name == nameof(Queryable.AsQueryable)).First().MakeGenericMethod(GenericTypes.Select(e => e.FromNode()).ToArray());
                var asEnumerableMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.AsEnumerable)).MakeGenericMethod(GenericTypes.Select(e => e.FromNode()).ToArray());
                var asEnumerabled = asEnumerableMethod.Invoke(null, new object[] { Value });
                
                Console.WriteLine("{0}", Value.GetType());
                Value.GetType().GetInterfaces().ForEach(e => Console.WriteLine(e.ToString()));
                
                if (Value is IEnumerable<string>)
                {
                    asEnumerabled = ((IEnumerable<string>)asEnumerabled).All(e => Guid.TryParse(e, out _)) ? ((IEnumerable<string>)asEnumerabled).Select(e => Guid.Parse(e)).ToList() : asEnumerabled;

                    //return Expression.Constant(((IEnumerable<string>)Value).All(e => Guid.TryParse(e, out _)) ? ((IEnumerable<string>)Value).Select(e => Guid.Parse(e)).ToList() : Value);
                }

                return Expression.Constant(asEnumerabled);  //Type != null ? Expression.Constant(asEnumerabled, Type.FromNode()) : Expression.Constant(asEnumerabled);
            }
            
            /*   <------------------------------------------------ Need to realize correctly for IAsyncEnumerable
            if (Value.GetType().IsASyncGenericEnumerable())
            {
                var asAsyncEnumerableMethod = typeof(EntityFrameworkQueryableExtensions).GetMethod(nameof(EntityFrameworkQueryableExtensions.AsAsyncEnumerable)).MakeGenericMethod(GenericTypes.Select(e => e.FromNode()).ToArray());
                var asAsyncEnumerabled = asAsyncEnumerableMethod.Invoke(null, new object[] { Value });

                if (asAsyncEnumerabled is IAsyncEnumerable<string>)
                {
                    asAsyncEnumerabled = ((IAsyncEnumerable<string>)asAsyncEnumerabled).All(e => Guid.TryParse(e, out _)) ? ((IAsyncEnumerable<string>)asAsyncEnumerabled).Select(e => Guid.Parse(e)).ToListAsync().GetAwaiter().GetResult() : asAsyncEnumerabled;
                }

                return Expression.Constant(asAsyncEnumerabled); //Type != null ? Expression.Constant(asAsyncEnumerabled, Type.FromNode()) : Expression.Constant(asAsyncEnumerabled);
            }
            */

            return Type != null ? Expression.Constant(Value, Type.FromNode()) : Expression.Constant(Value);
        }
        public override ICollection<Type> GetKnownTypes([Optional] Container container)
        {
            Console.WriteLine("Queryable Constant Expression Node KnownType");

            Type genericType = null;
            Type underlyingType = null;

            var valueType = Value?.GetType();

            if (valueType != null ? valueType.IsGenericTypeDefinition : false)
            {

                genericType = valueType.MakeGenericType(valueType.GetGenericArguments());
            }

            if (valueType != null && valueType.IsGenericType)
            {
                underlyingType = valueType.GetGenericArguments().FirstOrDefault();
            }


            var totalTypes = base.GetKnownTypes(container).Concat(new Type[] { this.GetType(), valueType, genericType, underlyingType,GenericTypes?.GetType() })
                                                          .Concat(GenericTypes?.SelectMany(e => e.GetKnownTypes()).ToList() ?? Enumerable.Empty<Type>())
                                                          .ToList();

            return totalTypes;
        }
    }
}

