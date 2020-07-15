using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.ViewModels.Apply
{
    public class SelectApprenticesViewModel : ViewModel
    {
        // TODO
        public SelectApprenticesViewModel() : base("Select the apprentices you want to apply for")
        {
            Apprentices = GetSampleApprentices();
        }

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

        public IEnumerable<ApprenticeModel> Apprentices { get; set; }
    }

    public class ApprenticeModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string CourseName { get; set; }
    }
}