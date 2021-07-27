using SFA.DAS.EmployerIncentives.Web.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Applications
{
    public class ViewApplicationsViewModel : IViewModel
    {
        private Dictionary<string, string> _fieldSortOrders;

        public IEnumerable<ApprenticeApplicationModel> Applications { get; set; }

        public void SetSortOrder(string fieldName, string sortOrder)
        {
            _fieldSortOrders = new Dictionary<string, string>
            {
                { ApplicationsSortField.ApplicationDate, ApplicationsSortOrder.None },
                { ApplicationsSortField.ApprenticeName, ApplicationsSortOrder.None },
                { ApplicationsSortField.TotalIncentiveAmount, ApplicationsSortOrder.None },
                { ApplicationsSortField.CourseName, ApplicationsSortOrder.None }
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

        public string GetAriaSortOrder(string fieldName)
        {
            var sortOrder = _fieldSortOrders[fieldName];
            if (sortOrder == ApplicationsSortOrder.None)
            {
                return sortOrder;
            }

            return $"{sortOrder.ToLower()}ending";
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

        public string AccountId { get; set; }
        public string AccountLegalEntityId { get; set; }

        public bool ShowBankDetailsInReview { get; set; }
        public bool ShowAddBankDetailsCalltoAction { get; set; }
        public bool ShowAcceptNewEmployerAgreement { get; set; }
        public bool ShowCancelLink { get; set; }

        public string AddBankDetailsLink { get; set; }
        public string ViewAgreementLink { get; set; }
        public string CancelLink { get; set; }

        public string Title => "Hire a new apprentice payment applications";

        public string OrganisationName { get; set; }    }
}
