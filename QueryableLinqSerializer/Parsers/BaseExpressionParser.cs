using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.Nodes.ExpressionNodes;
using QueryableLinqSerializer.Parsers;
using QueryableLinqSerializer.Settings;
using SimpleInjector;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QueryableLinqSerializer.CoreClasses.Parsers
{
    
    public class BaseExpressionParser : ActionFactory<ExpressionType>, IExpressionParser
    {
        public ExpressionParserSettings Settings { get; }

        public BaseExpressionParser(ExpressionParserSettings settings) : base()
        {
            Settings = settings;
        }
        protected override void Initialize()
        {
            base.Initialize();

            //UnaryExpression block
            AddActivator(ExpressionType.Negate, typeof(UnaryExpressionNode));
            AddActivator(ExpressionType.NegateChecked, typeof(UnaryExpressionNode));
            AddActivator(ExpressionType.Not, typeof(UnaryExpressionNode));
            AddActivator(ExpressionType.Convert, typeof(UnaryExpressionNode));
            AddActivator(ExpressionType.ConvertChecked, typeof(UnaryExpressionNode));
            AddActivator(ExpressionType.ArrayLength, typeof(UnaryExpressionNode));
            AddActivator(ExpressionType.Quote, typeof(UnaryExpressionNode));
            AddActivator(ExpressionType.TypeAs, typeof(UnaryExpressionNode));

            //BinaryExpressionBlock
            AddActivator(ExpressionType.Add, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.AddChecked, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.Subtract, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.SubtractChecked, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.Multiply, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.MultiplyChecked, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.Divide, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.Modulo, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.And, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.AndAlso, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.Or, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.OrElse, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.LessThan, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.LessThanOrEqual, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.GreaterThan, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.GreaterThanOrEqual, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.Equal, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.NotEqual, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.Coalesce, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.ArrayIndex, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.RightShift, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.LeftShift, typeof(BinaryExpressionNode));
            AddActivator(ExpressionType.ExclusiveOr, typeof(BinaryExpressionNode));

            //TypeBinaryExpression block
            AddActivator(ExpressionType.TypeIs, typeof(TypeBinaryExpressionNode));

            //ConditionalExpression block
            AddActivator(ExpressionType.Conditional, typeof(ConditionalExpressionNode));

            //ConstantExpression block
            AddActivator(ExpressionType.Constant, typeof(ConstantExpressionNode));

            //ParameterExpression block
            AddActivator(ExpressionType.Parameter, typeof(ParameterExpressionNode));

            //MemberExpression block
            AddActivator(ExpressionType.MemberAccess, typeof(MemberExpressionNode));

            //MethodCallExpression block
            AddActivator(ExpressionType.Call, typeof(MethodCallExpressionNode));

            //LambdaExpression block
            AddActivator(ExpressionType.Lambda, typeof(LambdaExpressionNode));

            //NewExpression block
            AddActivator(ExpressionType.New, typeof(NewExpressionNode));

            //NewExpression block
            AddActivator(ExpressionType.NewArrayInit, typeof(NewArrayExpressionNode));
            AddActivator(ExpressionType.NewArrayBounds, typeof(NewArrayExpressionNode));

            //InvocationExpression block
            AddActivator(ExpressionType.Invoke, typeof(InvocationExpressionNode));

            //MemberInitExpression block
            AddActivator(ExpressionType.MemberInit, typeof(MemberInitExpressionNode));

            //ListInitExpression block
            AddActivator(ExpressionType.ListInit, typeof(ListInitExpressionNode));
        }

        
        public virtual ExpressionNode Parse(Expression elem) 
        {
            if (elem == null) { return null; }

            Console.WriteLine(string.Format("{0}    {1}", elem.NodeType, elem.Type));

            return this.Create<ExpressionNode>(elem.NodeType, elem, Settings.Container);
        }
        
    }
    
}
