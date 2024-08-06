using Org.BouncyCastle.Asn1.Mozilla;
using Customers.Data.Domains;
using Customers.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customers.Infrastructure.Dtos
{
    public class CollectionAnalysisDto
    {
        public int CollectionAnalysisId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public int CollectionId { get; set; }
        public int SOWDocumentId { get; set; }
        public virtual CollectionSOW? CollectionSOW { get; set; }
        public int ProposalDocumentId { get; set; }
        public virtual CollectionProposal? CollectionProposal { get; set; }
        public int RunNumber { get; set; }
        public DateTime RunDateTime { get; set; }
        public int RunByUserId { get; set; }
        public ApplicationUser RunByUser { get; set; }
        public decimal TCScore { get; set; }
        public string Trend { get; set; }
        public int? TCDocumentId { get; set; }
        public decimal? CPIndex { get; set; }
        public decimal? ArrayIndex { get; set; }
        public decimal? MCSIndex { get; set; }
        public decimal? Correl { get; set; }
        public string ProposalVersionNumber
        {
            get
            {
                return CollectionProposal == null ? "0" : $"V{CollectionProposal.VersionNumber}";
            }
        }
    }

    public class AnalysisDocumentCounts
    {
        public int AllowablePageCount { get; set; }
        public int TotalWords { get; set; }
        public float SOWParagraphPerPage { get; set; }
        public int ApproxPageCount { get; set; }
        public int TotalParagraphs { get; set; }
        public int SOWWordsPerPage { get; set; }
    }
}
