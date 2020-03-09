using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace QueryableLinqSerializer.Settings
{
    public abstract class ParserSettings
    {
        public Container Container { get; set; }
        public bool StoringKnownTypes { get; set; }
    }
}
