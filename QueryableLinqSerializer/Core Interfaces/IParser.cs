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

    public interface IExpressionParser : IParser<ExpressionNode, Expression, ExpressionParserSettings> { }
    public interface IMemberBingingParser : IParser<MemberBindingNode, MemberBinding, MemberBindingParserSettings> { }
    public interface IMemberInfoParser : IParser<MemberInfoNode, MemberInfo, MemberInfoParserSettings> { }
    public interface ITypeParser : IParser<TypeNode, Type, TypeParserSettings> { }
}
