using System;
using System.Linq;

namespace SFA.DAS.EmployerIncentives.Web.Extensions
{
    public static class IQueryableExtensions
    {
        public static void ForEach<T>(this IQueryable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
