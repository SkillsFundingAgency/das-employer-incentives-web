using System;
using System.Diagnostics;
using System.Text.Json;
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
            MockServer = WireMockServer.Start();
            BaseAddress = MockServer.Urls[0];
            MockServer.LogEntriesChanged += MockServer_LogEntriesChanged;
        }

        private void MockServer_LogEntriesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (LogEntry newItem in e.NewItems)
            {
                Debug.WriteLine("============================= TestEmployerIncentivesApi MockServer called ================================");
                Debug.WriteLine(JsonSerializer.Serialize(TestHelper.Map(newItem), new JsonSerializerOptions { WriteIndented = true }));
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
