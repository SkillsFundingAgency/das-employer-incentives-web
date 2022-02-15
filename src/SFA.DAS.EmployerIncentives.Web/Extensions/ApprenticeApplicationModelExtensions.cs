using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;
using SFA.DAS.EmployerIncentives.Web.Models;

namespace SFA.DAS.EmployerIncentives.Web.Extensions
{
    public static class ApprenticeApplicationModelExtensions
    {
        public static IQueryable<ApprenticeApplicationModel> Sort(this IQueryable<ApprenticeApplicationModel> applications, string sortOrder, string sortField)
        {
            if (sortOrder == ApplicationsSortOrder.Descending)
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    applications = applications.OrderByDescending(sortField).ThenBy(x => x.ULN).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    applications = applications.OrderByDescending(sortField).ThenBy(x => x.ULN);
                }
            }
            else
            {
                if (sortField != ApplicationsSortField.ApprenticeName)
                {
                    applications = applications.OrderBy(sortField).ThenBy(x => x.ULN).ThenBy(x => x.ApprenticeName);
                }
                else
                {
                    applications = applications.OrderBy(sortField).ThenBy(x => x.ULN);
                }
            }

            return applications;
        }
        
        public static ApprenticeApplicationModel SetInLearning(this ApprenticeApplicationModel model)
        {
            if (model.FirstPaymentStatus != null)
            {
                model.FirstPaymentStatus.InLearning = true;
            }

            if (model.SecondPaymentStatus != null)
            {
                model.SecondPaymentStatus.InLearning = true;
            }

            return model;
        }

        public static ApprenticeApplicationModel SetClawedBackStatus(this ApprenticeApplicationModel model)
        {
            if (model.FirstPaymentStatus != null)
            {
                model.FirstPaymentStatus.IsClawedBack = model.FirstClawbackStatus != null && model.FirstClawbackStatus.IsClawedBack;
            }
            if (model.SecondPaymentStatus != null)
            {
                model.SecondPaymentStatus.IsClawedBack = model.SecondClawbackStatus != null && model.SecondClawbackStatus.IsClawedBack;
            }

            return model;
        }

        public static ApprenticeApplicationModel SetViewAgreementLink(this ApprenticeApplicationModel model, string viewAgreementLink)
        {
            if (model.FirstPaymentStatus != null)
            {
                model.FirstPaymentStatus.ViewAgreementLink = viewAgreementLink;
            }

            if (model.SecondPaymentStatus != null)
            {
                model.SecondPaymentStatus.ViewAgreementLink = viewAgreementLink;
            }

            return model;
        }

        public static ApprenticeApplicationModel SetEmploymentCheckFeatureToggle(this ApprenticeApplicationModel model, WebConfigurationOptions configuration)
        {
            var employmentCheckFeatureToggle = configuration.DisplayEmploymentCheckResult;

            if (model.FirstPaymentStatus != null)
            {
                model.FirstPaymentStatus.DisplayEmploymentCheckResult = employmentCheckFeatureToggle; 
            }

            if (model.SecondPaymentStatus != null)
            {
                model.SecondPaymentStatus.DisplayEmploymentCheckResult = employmentCheckFeatureToggle;
            }

            return model;
        }
    }
}
