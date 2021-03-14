using RemoteLinqExecution.Core_Classes;
using RemoteLinqExecution.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteLinqExecution.Implementations.Server
{
    public class ServerLinqExecutionService : IRemoteExecutable
    {
        public Task<TResponse> Execute<TRequest, TResponse>(TRequest request, CommunicationSettings settings) where TRequest  : class
                                                                                                              where TResponse : class
        {
            throw new NotImplementedException();
        }
    }
}
