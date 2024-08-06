using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("analysis_report_parameter")]
    public class AnalysisReportParameter
    {
        [Key]
        [Column("analysis_report_parameter_id")]
        public int AnalysisReportParameterId { get; set; }
        [Column("collection_analysis_id")]
        public int CollectionAnalysisId { get; set; }
        [Column("include_radar_chart")]
        public bool IncludeRadarChart { get; set; }
        [Column("radar_sow")]
        public bool RadarSOW { get; set; }
        [Column("radar_proposal")]
        public bool RadarProposal { get; set; }
        [Column("radar_delta")]
        public bool RadarDelta { get; set; }
        [Column("include_match_chart")]
        public bool IncludeMatchChart { get; set; }
        [Column("match_sow")]
        public bool MatchSOW { get; set; }
        [Column("match_proposal")]
        public bool MatchProposal { get; set; }
        [Column("match_delta")]
        public bool MatchDelta { get; set; }
        [Column("include_sow_word")]
        public bool IncludeSOWWord { get; set; }
        [Column("sow_word_size")]
        public int SOWWordSize { get; set; }
        [Column("include_proposal_word")]
        public bool IncludeProposalWord { get; set; }
        [Column("proposal_word_size")]
        public int ProposalWordSize { get; set; }
        [Column("dualbar_filter")]
        public string DualBarFilter { get; set; }
        [Column("dualbar_orderby")]
        public string DualBarOrderBy { get; set; }
        [Column("created_by_id")]
        public int CreatedById { get; set; }
        [Column("last_modified_on")]
        public DateTime LastModifiedOn { get; set; }
    }
}