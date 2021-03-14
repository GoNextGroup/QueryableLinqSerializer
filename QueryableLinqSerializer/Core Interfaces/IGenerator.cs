using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;

namespace QueryableLinqSerializer.Core_Interfaces
{
    public interface IGenerator<T> 
    {
        public abstract T FromNode([Optional] Container container);
        //virtual T FromNode() where T : Expression => (T)FromNode();
        //public virtual T FromNode<T, V>(Func<V, T> func, V arg) => (T)func(arg);
    }
}
