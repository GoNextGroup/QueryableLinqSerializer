using RemoteLinqExecution.Core_Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RemoteLinqExecution.Interfaces
{
    public interface IRemoteExecutable //<--this interface is placed here for information and should be replaced 
    {
        public Task<TResponse> Execute<TRequest, TResponse>(TRequest request, CommunicationSettings settings) where TRequest  : class
                                                                                                              where TResponse : class;
    }
}
