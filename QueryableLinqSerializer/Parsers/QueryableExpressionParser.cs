using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.CoreClasses.Parsers;
using QueryableLinqSerializer.Nodes.QueryableExpressionNodes;
using QueryableLinqSerializer.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace QueryableLinqSerializer.Parsers
{
    //Not realized
    public class QueryableExpressionParser : BaseExpressionParser
    {
        public QueryableExpressionParser(QueryableExpressionParserSettings settings) : base(settings)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            ChangeActivator(ExpressionType.Constant, typeof(QueryableConstantExpressionNode));
        }

        public override ExpressionNode Parse(Expression elem)
        {
            return base.Parse(elem);
        }
    }
}
