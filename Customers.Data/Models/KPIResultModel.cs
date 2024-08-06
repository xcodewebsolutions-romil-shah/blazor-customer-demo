using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Models
{
    public class KPIResultModel
    {
        public int DaysSinceLastProposal { get; set; }
        public float PercentageImprovement { get; set; }
        public decimal TCScore { get; set; }
        public int? ApproximatePageCount { get; set; }
        public int ProposalVersion { get; set; }
    }
}
