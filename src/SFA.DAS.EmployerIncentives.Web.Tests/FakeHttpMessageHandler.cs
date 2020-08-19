using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerIncentives.Web.Tests
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        public int CalledSend { get; private set; } = 0;
        public HttpRequestMessage LastRequestMessage { get; private set; }
        public HttpResponseMessage ExpectedResponseMessage { get; set; }

        public virtual HttpResponseMessage Send(HttpRequestMessage request)
        {
            CalledSend++;
            LastRequestMessage = request;
            return ExpectedResponseMessage;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Send(request));
        }
    }
}
