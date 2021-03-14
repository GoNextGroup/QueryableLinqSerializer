using QueryableLinqSerializer.Core_Interfaces;
using RemoteLinqExecution.Core_Classes;
using RemoteLinqExecution.Interfaces;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteLinqExecution.Implementations
{
    public static class RemoteClientLinqExecutionExtensions
    {
        public static async Task<List<TSource>> ToListAsync<TSource>([NotNull] this IQueryable<TSource> source, IRemoteExecutable executor,  CommunicationSettings settings, Container container, CancellationToken cancellationToken = default)
        {
            var expressionParser = container.GetInstance<IExpressionParser>();
            var typeParser = container.GetInstance<ITypeParser>();

            var executeDTO = new ExecuteDTO()
            {
                PipelineExpression = expressionParser.Parse(source.Expression),
                FinalizerLambdaExpression = null,
                IsLambdaFinalizer = false,
                MethodName = "ToListAsync",
                ReturnType = typeParser.Parse(typeof(TSource))
            };

            var result = await executor.Execute<ExecuteDTO, List<TSource>>(executeDTO, settings);
            return result;
        }
    }
}
