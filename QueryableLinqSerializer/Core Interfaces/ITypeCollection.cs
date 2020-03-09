using System;
using System.Collections.Generic;
using System.Text;

namespace QueryableLinqSerializer.Core_Interfaces
{
    public interface ITypeCollection
    {
        public ICollection<Type> ParsedKnownTypes { get; }
    }
}
