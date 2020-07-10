using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels
{
    public class ViewModel
    {
        public ViewModel(string title)
        {
            Title = title;
            _errors = new Dictionary<string, string>();
        }

        private Dictionary<string, string> _errors;
        public IReadOnlyDictionary<string, string> Errors => _errors;

        public string Title { get; }

        public bool Valid => !Errors.Any();

        public void AddError(string propertyName, string errorMessage)
        {
            _errors.Add(propertyName, errorMessage);
        }
    }
}
