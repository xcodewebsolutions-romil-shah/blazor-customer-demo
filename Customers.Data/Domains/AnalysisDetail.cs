using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Customers.Data.Domains
{
    [Table("analysis_detail")]
    public class AnalysisDetail
    {
        [Column("detail_id")]
        [Key]
        public int DetailId { get; set; }
        [Column("collection_analysis_id")]
        public int CollectionAnalysisId { get; set; }
        [Column("word")]
        public string Word { get; set; }
        [Column("sow_count")]
        public int SOWCount { get; set; }
        [Column("sow_percentage")]
        public float SOWPercentage { get; set; }
        [Column("proposal_count")]
        public int ProposalCount { get; set; }
        [Column("proposal_percentage")]
        public float ProposalPercentage { get; set; }
        [Column("delta")]
        public int Delta { get; set; }
        [Column("delta_percentage")]
        public float DeltaPercentage { get; set; }
    }
}
