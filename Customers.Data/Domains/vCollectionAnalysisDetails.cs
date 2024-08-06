using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("vcollectionanalysisdetail")]
    public class vCollectionAnalysisDetails
    {
        [Column("collection_analysis_id")]
        public int CollectionAnalysisId { get; set; }
        [Column("sow_sanitized_doc_id")]
        public int SowSanitizedDocId { get; set; }
        [Column("sow_sanitized_doc_file_name")]
        public string SowSanitizedDocName { get; set; }
        [Column("proposal_sanitized_doc_file_name")]
        public string ProposalSanitizedDocName { get; set; }
        [Column("proposal_sanitized_doc_id")]
        public int ProposalSanitizedDocId { get; set; }
        [Column("proposal_page_count")]
        public int ProposalPageCount { get; set; }
        [Column("collection_id")]
        public int CollectionId { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("analysis_status")]
        public string AnalysisStatus { get; set; }
        [Column("tc_score")]
        public decimal TCScore { get; set; }
        [Column("run_datetime")]
        public DateTime RunDateTime { get; set; }
        [Column("sow_doc_name")]
        public string SOWDocumentName { get; set; }
        [Column("sow_doc_id")]
        public int SOWDocumentId { get; set; }
        [Column("sow_version")]
        public int SOWVerison { get; set; }
        [Column("run_by")]
        public string RunByUser { get; set; }
        [Column("proposal_doc_name")]
        public string ProposalDocumentName { get; set; }
        [Column("proposal_doc_id")]
        public int ProposalDocumentId { get; set; }
        [Column("proposal_version")]
        public int ProposalVersion { get; set; }
        [Column("proposal_doc_imported_on")]
        public DateTime ProposalDocImportedOn { get; set; }
        [Column("sow_doc_imported_on")]
        public DateTime SowDocImportedOn { get; set; }
    }
}
