using MediatR;
using System;
using System.Threading.Tasks;

namespace BlazorServerSide.Queue
{
    public interface IQueueManager
    {
        Task AddMessageAsync(string input);
        Task SendActionAsync<T>(IRequest<T> request);

        event EventHandler<MessageReceivedArgs> MessageReceived;
    }
}