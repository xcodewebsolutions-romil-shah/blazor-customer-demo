using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class AnalysisReportParameterDto
    {
        public int AnalysisReportParameterId { get; set; }
        public int CollectionAnalysisId { get; set; }        
        public bool IncludeRadarChart { get; set; }        
        public bool RadarSOW { get; set; }
        public bool RadarProposal { get; set; }        
        public bool RadarDelta { get; set; }        
        public bool IncludeMatchChart { get; set; }        
        public bool MatchSOW { get; set; }        
        public bool MatchProposal { get; set; }        
        public bool MatchDelta { get; set; }        
        public bool IncludeSOWWord { get; set; }        
        public int SOWWordSize { get; set; }        
        public bool IncludeProposalWord { get; set; }        
        public int ProposalWordSize { get; set; }
        public string DualBarFilter { get; set; }        
        public string DualBarOrderBy { get; set; }
        public int CreatedById { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
}
