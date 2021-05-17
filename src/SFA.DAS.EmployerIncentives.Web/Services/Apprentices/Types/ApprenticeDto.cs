﻿using System;

namespace SFA.DAS.EmployerIncentives.Web.Services.Apprentices.Types
{
    public class ApprenticeDto
    {
        public long ApprenticeshipId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public DateTime StartDate { get; set; }
        public long Uln { get; set; }
        public string DisplayName => $"{FirstName} {LastName}";
    }
}
