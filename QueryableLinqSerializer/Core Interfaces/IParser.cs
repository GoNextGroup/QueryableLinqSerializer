using QueryableLinqSerializer.Core_Classes;
using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using QueryableLinqSerializer.Settings;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace QueryableLinqSerializer.Core_Interfaces
{
    public interface IParser<T, V, Z> where T:class
                                      where V:class
                                      where Z:ParserSettings
    {
        public Z Settings { get; }

        T Parse(V elem);
        S Parse<S>(V elem) where S:class => Parse(elem) as S;
    }

    public interface IExpressionParser : IParser<ExpressionNode, Expression, ExpressionParserSettings>//, ITypeCollection
    {
        /*
        ExpressionNode ParseNode(Expression expression);
        T ParseNode<T>(Expression expression) where T : ExpressionNode;
        */

        //public ExpressionParserSettings ExpressionParserSettings { get; }
    }

    public interface IMemberBingingParser : IParser<MemberBindingNode, MemberBinding, MemberBindingParserSettings>//, ITypeCollection
    {
        //public MemberBindingParserSettings MemberBindingnParserSettings { get; }
    }

    public interface IMemberInfoParser : IParser<MemberInfoNode, MemberInfo, MemberInfoParserSettings>//, ITypeCollection
    {
    }

    public interface ITypeParser : IParser<TypeNode, Type, TypeParserSettings>//, ITypeCollection
    {
    }
}
