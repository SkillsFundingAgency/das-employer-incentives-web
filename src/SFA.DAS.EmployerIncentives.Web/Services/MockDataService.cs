using SFA.DAS.EmployerIncentives.Web.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services
{
    public class MockDataService : IDataService
    {
        public IEnumerable<ApprenticeModel> GetSampleApprentices()
        {
            var data = new[]
            {
                 new ApprenticeModel
                    {FirstName = "Michael", LastName = "Johnson", CourseName = "Water Treatment Technician, Level: 3 (Standard)"},
                new ApprenticeModel
                    {FirstName = "Jack", LastName = "Roberts", CourseName = "Relationship Manager (Banking), Level: 6 (Standard)"},
                new ApprenticeModel
                {
                    FirstName = "Steven", LastName = "Smith",
                    CourseName = "Non-destructive testing (NDT) operator, Level: 2 (Standard)"
                },
                new ApprenticeModel
                {
                    FirstName = "Hubert", LastName = "Blaine Von Wolfeschlegelsteinhausenbergerdorff",
                    CourseName = "Water Treatment Technician, Level: 3 (Standard)"
                },
                new ApprenticeModel
                    {FirstName = "Rūšlāns", LastName = "Ščēļkunövs", CourseName = "Software Engineer, Level: 1 (Novice)"},
                new ApprenticeModel {FirstName = "Wassily", LastName = " Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
            };

            foreach (var apprenticeModel in data)
            {
                apprenticeModel.Id = Guid.NewGuid().ToString();
            }

            return data;
        }
    }
}