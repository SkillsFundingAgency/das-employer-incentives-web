﻿using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class ApprenticeApplicationModel
    {
        public long AccountId { get; set; }
        public Guid ApplicationId { get; set; }
        public string LegalEntityName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ApprenticeName { get { return $"{FirstName} {LastName}"; } }
        public DateTime ApplicationDate { get; set; }
        public decimal TotalIncentiveAmount { get; set; }
        public string Status { get; set; }
    }
}
