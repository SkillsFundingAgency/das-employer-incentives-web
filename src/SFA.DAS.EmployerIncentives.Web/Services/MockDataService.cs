using SFA.DAS.EmployerIncentives.Web.Models;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Services
{
    public class MockDataService : IDataService
    {
        public IEnumerable<ApprenticeshipModel> GetEligibleApprenticeships()
        {
            var data = new[]
            {
                 new ApprenticeshipModel
                    {FirstName = "Michael", LastName = "Johnson", CourseName = "Water Treatment Technician, Level: 3 (Standard)"},
                new ApprenticeshipModel
                    {FirstName = "Jack", LastName = "Roberts", CourseName = "Relationship Manager (Banking), Level: 6 (Standard)"},
                new ApprenticeshipModel
                {
                    FirstName = "Steven", LastName = "Smith",
                    CourseName = "Non-destructive testing (NDT) operator, Level: 2 (Standard)"
                },
                new ApprenticeshipModel
                {
                    FirstName = "Hubert", LastName = "Blaine Von Wolfeschlegelsteinhausenbergerdorff",
                    CourseName = "Water Treatment Technician, Level: 3 (Standard)"
                },
                new ApprenticeshipModel
                    {FirstName = "Rūšlāns", LastName = "Ščēļkunövs", CourseName = "Software Engineer, Level: 1 (Novice)"},
                new ApprenticeshipModel {FirstName = "Wassily", LastName = " Kandinsky", CourseName = "Painter, Level: 10 (Advanced)"},
            };

            foreach (var apprenticeshipModel in data)
            {
                apprenticeshipModel.Id = Guid.NewGuid().ToString();
            }

            return data;
        }
    }
}