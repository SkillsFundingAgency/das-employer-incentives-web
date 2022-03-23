using System.Linq;
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

        public static IQueryable<ApprenticeApplicationModel> FilterByEmployerActions(this IQueryable<ApprenticeApplicationModel> applications)
        {
            return applications.Where(x => (x.FirstPaymentStatus != null && x.FirstPaymentStatus.EmploymentCheckPassed == false)
                                           || (x.SecondPaymentStatus != null && x.SecondPaymentStatus.EmploymentCheckPassed == false));
        }

        public static IQueryable<ApprenticeApplicationModel> FilterByPayments(this IQueryable<ApprenticeApplicationModel> applications)
        {
            return applications.Where(x => x.FirstPaymentStatus.ShowPayments()|| x.SecondPaymentStatus.ShowPayments());
        }

        public static IQueryable<ApprenticeApplicationModel> FilterByStoppedOrWithdrawn(this IQueryable<ApprenticeApplicationModel> applications)
        {
            return applications.Where(x => (x.FirstPaymentStatus != null && 
                                            (x.FirstPaymentStatus.PausePayments || x.FirstPaymentStatus.PaymentIsStopped ||
                                             x.FirstPaymentStatus.WithdrawnByCompliance || x.FirstPaymentStatus.WithdrawnByEmployer)) ||
                                             (x.SecondPaymentStatus != null && 
                                              (x.SecondPaymentStatus.PausePayments || x.SecondPaymentStatus.PaymentIsStopped ||
                                             x.SecondPaymentStatus.WithdrawnByCompliance || x.SecondPaymentStatus.WithdrawnByEmployer)));
        }

        private static bool ShowPayments(this PaymentStatusModel paymentStatus)
        {
            return (paymentStatus != null 
                        && (paymentStatus.PaymentSent || paymentStatus.PaymentSentIsEstimated) 
                        && paymentStatus.LearnerMatchFound && !paymentStatus.PausePayments && paymentStatus.InLearning && !paymentStatus.HasDataLock)
                        && (!paymentStatus.EmploymentCheckPassed.HasValue || paymentStatus.EmploymentCheckPassed.Value);
        }
    }
}
