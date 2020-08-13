using SFA.DAS.EmployerIncentives.Web.Infrastructure.Configuration;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels
{
    public class ViewModel
    {
        public ViewModel(string title, WebConfigurationOptions webConfiguration)
        {
            Title = title;
            ZenDeskSnippetKey = webConfiguration.ZenDeskSnippetKey;
            ZenDeskSectionId = webConfiguration.ZenDeskSectionId;
            ZenDeskCobrowsingSnippetKey = webConfiguration.ZenDeskCobrowsingSnippetKey;
        }

        public string Title { get; }
        public string ZenDeskSnippetKey { get; set; }
        public string ZenDeskSectionId { get; set; }
        public string ZenDeskCobrowsingSnippetKey { get; set; }
    }
}