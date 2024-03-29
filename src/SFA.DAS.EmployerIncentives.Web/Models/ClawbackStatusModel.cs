﻿using System;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
    public class ClawbackStatusModel
    {
        public decimal ClawbackAmount { get; set; }
        public DateTime? ClawbackDate { get; set; }
        public DateTime? OriginalPaymentDate { get; set; }
        public bool IsClawedBack => ClawbackDate.HasValue;
    }
}
