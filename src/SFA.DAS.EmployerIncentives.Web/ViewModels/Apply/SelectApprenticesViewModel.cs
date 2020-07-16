using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class SelectApprenticesViewModel : ViewModel
    {
        public const string SelectApprenticesMessage = "Select the apprentices you want to apply for";

        public SelectApprenticesViewModel() : base(SelectApprenticesMessage)
        {
            Apprentices = GetSampleApprentices(); // TODO: Order by surname
            FirstCheckboxId = $"new-apprentices-{Apprentices.First().Id}";
            SelectedApprentices = new List<string>();
        }

        public IEnumerable<ApprenticeModel> Apprentices { get; set; }

        public bool HasSelectedApprentices => SelectedApprentices.Count > 0;
        public string FirstCheckboxId { get; }

        public List<string> SelectedApprentices { get; set; }

        private static IEnumerable<ApprenticeModel> GetSampleApprentices()
        {
            var data = new[]
            {
                new ApprenticeModel
                    {FullName = "Michael Johnson", CourseName = "Water Treatment Technician, Level: 3 (Standard)"},
                new ApprenticeModel
                    {FullName = "Jack Roberts", CourseName = "Relationship Manager (Banking), Level: 6 (Standard)"},
                new ApprenticeModel
                {
                    FullName = "Steven Smith",
                    CourseName = "Non-destructive testing (NDT) operator, Level: 2 (Standard)"
                },
                new ApprenticeModel
                {
                    FullName = "Hubert Blaine Wolfeschlegelsteinhausenbergerdorff",
                    CourseName = "Water Treatment Technician, Level: 3 (Standard)"
                },
                new ApprenticeModel
                    {FullName = "Rūšlāns Ščēļkunövs", CourseName = "Software Engineer, Level: 1 (Novice)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
                new ApprenticeModel {FullName = "Wassily Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
            };

            foreach (var apprenticeModel in data)
            {
                apprenticeModel.Id = Guid.NewGuid().ToString();
            }

            return data;
        }

    }

    public class ApprenticeModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string CourseName { get; set; }
    }
}