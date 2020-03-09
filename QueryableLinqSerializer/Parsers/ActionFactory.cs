using QueryableLinqSerializer.Core_Interfaces;
using QueryableLinqSerializer.CoreClasses.Parsers;
using QueryableLinqSerializer.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QueryableLinqSerializer.Parsers
{
    public abstract class ActionFactory<T>
    {
        /*
        static ActionFactory() 
        {
            var defaultSetting = new ParserFactorySettings() { };
            DefaultFactory = new ActionFactory(defaultSetting);

            DefaultFactory.AddActivator<IExpressionParser>(typeof(ExpressionParser));
        } 
        */

        protected static ActionFactory<T> defaultFactory;
        protected IDictionary<T, Type> activators;
        //protected BaseSettings baseSettings;

        public ActionFactory()//[Optional]BaseSettings baseSettings)
        {
            Initialize();
        }
        protected virtual void Initialize() 
        {
            activators = new ConcurrentDictionary<T, Type>();
        }

        
        public virtual V Create<V>(T key, params object[] arguments) where V:class
        {
            var instance = activators.TryGetValue(key, out var type) ? (V)Activator.CreateInstance(type, arguments) : throw new NotImplementedException("Not implemented");
            return instance;
        }
        public virtual object Create(T key, params object[] arguments) => Create<object>(key, arguments);


        //public virtual void AddActivator<V>(T concreteType) => activators.Add(typeof(V), concreteType);
        public virtual void AddActivator(T underlying, Type concreteType) => activators.Add(underlying, concreteType);
        public virtual void ChangeActivator(T underlying, Type concreteType)
        {   
            if (activators.ContainsKey(underlying)) { activators[underlying] = concreteType; }
            else { activators.Add(underlying, concreteType); }
        }
        public virtual void RemoveActivator(T underlying) => activators.Remove(underlying);
    }
}
