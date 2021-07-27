using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class WithdrawRequest
    {
        public WithdrawalType WithdrawalType { get; }
        public long AccountId { get; }
        public long AccountLegalEntityId { get; }
        public long ULN { get; }
        public ServiceRequest ServiceRequest { get; }
        public string EmailAddress { get; }

        public WithdrawRequest(WithdrawalType type, long accountLegalEntityId, long uln, ServiceRequest serviceRequest, long accountId, string emailAddress)
        {
            WithdrawalType = type;
            AccountLegalEntityId = accountLegalEntityId;
            ULN = uln;
            ServiceRequest = serviceRequest;
            AccountId = accountId;
            EmailAddress = emailAddress;
        }    
    }

    public enum WithdrawalType
    {
        Employer = 1
    }

    public class ServiceRequest
    {
        public string TaskId { get; set; }
        public string DecisionReference { get; set; }
        public DateTime? TaskCreatedDate { get; set; }
    }
}
