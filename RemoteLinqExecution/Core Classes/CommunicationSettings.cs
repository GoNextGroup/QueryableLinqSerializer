using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteLinqExecution.Core_Classes
{
    public class CommunicationSettings //<-- this settings class should be replaced by your own realization of this class
    {
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
    }
}
