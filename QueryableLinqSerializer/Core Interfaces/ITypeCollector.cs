using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace QueryableLinqSerializer.Core_Interfaces
{
    public interface ITypeCollector
    {
        public ICollection<Type> GetKnownTypes([Optional] Container container);
    }
}
