using Newtonsoft.Json;
using System;
using System.Diagnostics;
using WireMock.Logging;
using WireMock.Server;

namespace SFA.DAS.EmployerIncentives.Web.SystemAcceptanceTests.Services
{
    public class TestEmployerIncentivesApi : IDisposable
    {
        private bool isDisposed;

        public string BaseAddress { get; private set; }

        public WireMockServer MockServer { get; private set; }

        public TestEmployerIncentivesApi()
        {
            MockServer = WireMockServer.Start(ssl: true);
            BaseAddress = MockServer.Urls[0];
            MockServer.LogEntriesChanged += MockServer_LogEntriesChanged;
        }

        private void MockServer_LogEntriesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(LogEntry newItem in e.NewItems)
            {
                Debug.WriteLine("============================= TestEmployerIncentivesApi MockServer called ================================");
                Debug.WriteLine(JsonConvert.SerializeObject(TestHelper.Map(newItem), Formatting.Indented));
                Debug.WriteLine("==========================================================================================================");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                if (MockServer.IsStarted)
                {
                    MockServer.Stop();
                }
                MockServer.Dispose();
            }

            isDisposed = true;
        }

        

    }
}
