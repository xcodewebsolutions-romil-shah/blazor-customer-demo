using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("collection_analysis")]
    public class CollectionAnalysis
    {
        [Key]
        [Column("collection_analysis_id")]
        public int CollectionAnalysisId { get; set; }
        [Column("name")]
        public string? Name { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("status")]
        public string Status { get; set; }
        [Column("collection_id")]
        public int CollectionId { get; set; }
        [Column("sow_document_id")]
        public int SOWDocumentId { get; set; }
        [ForeignKey("SOWDocumentId")]
        public virtual CollectionSOW? CollectionSOW { get; set; }
        [Column("proposal_document_id")]
        public int ProposalDocumentId { get; set; }
        [ForeignKey("ProposalDocumentId")]
        public virtual CollectionProposal? CollectionProposal { get; set; }
        [Column("run_number")]
        public int RunNumber { get; set; }
        [Column("run_datetime")]
        public DateTime RunDateTime { get; set; }
        [Column("run_by_userid")]
        public int RunByUserId { get; set; }
        [ForeignKey("RunByUserId")]
        public virtual ApplicationUser? RunByUser { get; set; }
        [Column("tc_score")]
        public decimal TCScore { get; set; }
        [Column("trend")]
        public string Trend { get; set; }
        [Column("tc_document_id")]
        public int? TCDocumentId { get; set; }
        [Column("cp_index")]
        public decimal? CPIndex { get; set; }
        [Column("array_index")]
        public decimal? ArrayIndex { get; set; }
        [Column("mcs_index")]
        public decimal? MCSIndex { get; set; }
        [Column("correl")]
        public decimal? Correl { get; set; }
    }
}
