using QueryableLinqSerializer.Core_Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryableLinqSerializer.Core_Classes.Parsers
{
    public class ParserContainer
    {
        public IExpressionParser ExpressionParser { get; set; }
        public IMemberBingingParser MemberBingingParser { get; set; }
        public IMemberInfoParser MemberInfoParser { get; set; }
        public ITypeParser TypeParser { get; set; }
    }
}
