using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services.Applications.Types
{
    public class WithdrawRequest
    {
        public WithdrawalType WithdrawalType { get; }
        public long AccountId { get; }
        public IEnumerable<Application> Applications { get; }
        public ServiceRequest ServiceRequest { get; }
        public string EmailAddress { get; }

        public WithdrawRequest(WithdrawalType withdrawalType, IEnumerable<Application> applications, ServiceRequest serviceRequest, long accountId, string emailAddress)
        {
            WithdrawalType = withdrawalType;
            Applications = applications;
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
