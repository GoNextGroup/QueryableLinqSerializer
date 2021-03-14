using QueryableLinqSerializer.Core_Classes.Nodes;
using QueryableLinqSerializer.Nodes.AuxiliaryNodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteLinqExecution.Core_Classes
{
    internal class ExecuteDTO
    {
        public ExpressionNode PipelineExpression { get; set; }
        public ExpressionNode FinalizerLambdaExpression { get; set; }
        public bool IsLambdaFinalizer { get; set; }
        public string MethodName { get; set; }
        public TypeNode ReturnType { get; set; }
    }
}
