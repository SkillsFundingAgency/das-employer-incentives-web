using Microsoft.Azure.Documents;

namespace SFA.DAS.EmployerIncentives.Web.Services.ReadStore
{
    public interface IDocumentClientFactory
    {
        IDocumentClient CreateDocumentClient();
    }
}
