using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SFA.DAS.EmployerIncentives.Web.Authorisation.GovUserEmployerAccount
{
    public class GetUserAccountsResponse
    {
        public string EmployerUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonPropertyName("UserAccounts")]
        public List<EmployerIdentifier> UserAccounts { get; set; }
    }
    
    public class EmployerIdentifier
    {
        [JsonPropertyName("EncodedAccountId")]
        public string AccountId { get; set; }
        [JsonPropertyName("DasAccountName")]
        public string EmployerName { get; set; }
        [JsonPropertyName("Role")]
        public string Role { get; set; }
    }
}