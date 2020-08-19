using System.Collections.Generic;

namespace SFA.DAS.EmployerIncentives.Web.Models
{
  public class GoogleAnalyticsData
  {
    public string DataLoaded { get; set; } = "dataLoaded";
    public string UserId { get; set; }
    public string Acc { get; set; }
    public string Ale { get; set; }
    public string Vpv { get; set; }
    public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
  }
}
