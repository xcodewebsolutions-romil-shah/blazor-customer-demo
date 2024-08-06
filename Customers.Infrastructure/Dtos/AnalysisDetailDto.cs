using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class AnalysisDetailDto
    {
        public int IndexId { get; set; }
        public int DetailId { get; set; }
        public int CollectionAnalysisId { get; set; }
        public string Word { get; set; }
        public int SOWCount { get; set; }
        public decimal SOWPercentage { get; set; }
        public int ProposalCount { get; set; }
        public decimal ProposalPercentage { get; set; }
        public int Delta { get; set; }
        public decimal DeltaPercentage { get; set; }
    }
    public class ActiveRecordSOW
    {
        public int? PagesCount { get; set; }
        public int? ParagraphCount { get; set; }
        public string CleanedText { get; set; }
    }
    public class ActiveRecordProposal
    {
        public int? PagesCount { get; set; }
        public int? ParagraphCount { get; set; }
        public string CleanedText { get; set; }
    }
}
