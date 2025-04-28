using System.Collections.Generic;
using System.Text.Json.Serialization;
using SFA.DAS.GovUK.Auth.Employer;

namespace SFA.DAS.EmployerIncentives.Web.Authorisation.GovUserEmployerAccount
{
    public class GetUserAccountsResponse
    {
        [JsonPropertyName("employerUserId")]
        public string EmployerUserId { get; set; }
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        [JsonPropertyName("userAccounts")]
        public List<EmployerIdentifier> UserAccounts { get; set; }

        [JsonPropertyName("isSuspended")]
        public bool IsSuspended { get; set; }
    }
    
    public class EmployerIdentifier
    {
        [JsonPropertyName("encodedAccountId")]
        public string AccountId { get; set; }
        [JsonPropertyName("dasAccountName")]
        public string EmployerName { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("apprenticeshipEmployerType")]
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    }
}