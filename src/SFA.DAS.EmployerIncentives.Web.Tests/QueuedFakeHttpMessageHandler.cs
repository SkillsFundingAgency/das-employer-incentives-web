using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests
{
    public class QueuedFakeHttpMessageHandler : HttpMessageHandler
    {
        public List<HttpRequestMessage> RequestMesssages { get; private set; }

        private Queue<HttpResponseMessage> ResponseMessages { get; }
        
        public QueuedFakeHttpMessageHandler()
        {
            RequestMesssages = new List<HttpRequestMessage>();
            ResponseMessages = new Queue<HttpResponseMessage>();
        }

        public void AddResponse(HttpResponseMessage message)
        {
            ResponseMessages.Enqueue(message);
        }

        public virtual HttpResponseMessage Send(HttpRequestMessage request)
        {
            RequestMesssages.Add(request);
            var response = ResponseMessages.Dequeue();
            return response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }
}
