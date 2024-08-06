using System.ComponentModel.DataAnnotations.Schema;

namespace Customers.Data.Domains
{
    public class CollectionProposalDto
    {
        public int CollectionProposalId { get; set; }
        public int CollectionId { get; set; }
        public int SanitizedDocumentId { get; set; }
        public int OriginalDocumentId { get; set; }
        public int VersionNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int? LastModifiedById { get; set; }
        public int? CleanTextDocumentId { get; set; }
        public int? ImportedFileTextDocumentId { get; set; }
        public int? PageCount { get; set; }
        public int? SpaceCount { get; set; }
        public int? WordCount { get; set; }
        public int? lineCount { get; set; }
        public int? ParagraphCount { get; set; }
        public int? TabCount { get; set; }
    }
}
