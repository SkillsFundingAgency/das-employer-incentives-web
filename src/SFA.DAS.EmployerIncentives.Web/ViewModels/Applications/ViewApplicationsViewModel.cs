
using SFA.DAS.EmployerIncentives.Web.Models;
using SFA.DAS.NLog.Targets.Redis.DotNetCore;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Applications
{
    public class ViewApplicationsViewModel : ViewModel
    {
        private Dictionary<string, string> _fieldSortOrders;

        public ViewApplicationsViewModel() : base("Your hire a new apprentice payment applications")
        {
        }

        public IEnumerable<ApprenticeApplicationModel> Applications { get; set; }

        public void SetSortOrder(string fieldName, string sortOrder)
        {
            _fieldSortOrders = new Dictionary<string, string>
            {
                { ApplicationsSortField.ApplicationDate, ApplicationsSortOrder.None },
                { ApplicationsSortField.ApprenticeName, ApplicationsSortOrder.None },
                { ApplicationsSortField.TotalIncentiveAmount, ApplicationsSortOrder.None },
                { ApplicationsSortField.LegalEntityName, ApplicationsSortOrder.None }
            };

            _fieldSortOrders[fieldName] = sortOrder;

        }

        public string GetSortOrder(string fieldName)
        {
            var sortOrder = _fieldSortOrders[fieldName];
            if (string.IsNullOrWhiteSpace(sortOrder))
            {
                return ApplicationsSortOrder.None;
            }

            return sortOrder;
        }

        public string SortField { get; set; }

        public string ToggleSortOrder(string fieldName)
        {
            var sortOrder = GetSortOrder(fieldName);
            if (String.IsNullOrWhiteSpace(sortOrder))
            {
                return "asc";
            }

            if (sortOrder.ToLower() == "asc")
            {
                return "desc";
            }

            return "asc";
        }
    }
}
