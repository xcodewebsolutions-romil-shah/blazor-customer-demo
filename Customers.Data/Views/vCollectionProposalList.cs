using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Views
{
    [Table("vCollectionProposalList")]
    public class vCollectionProposalList
    {

        [Column("collection_proposal_id")]
        public int CollectionProposalId { get; set; }
        [Column("collection_id")]
        public int CollectionId { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("is_archived")]
        public bool IsArchived { get; set; }
        [Column("sanitized_document_id")]
        public int SanitizedDocumentId { get; set; }
        [Column("original_document_id")]
        public int OriginalDocumentId { get; set; }
        [Column("document_name")]
        public string DocumentName { get; set; }
        [Column("filename")]
        public string FileName { get; set; }
        [Column("version_no")]
        public int VersionNumber { get; set; }
        [Column("created_on")]
        public DateTime CreatedOn { get; set; }
        [Column("username")]
        public string UserName { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("original_doc_url")]
        public string OriginalDocUrl { get; set; }
        [Column("sanitized_doc_url")]
        public string SanitizedDocUrl { get; set; }
        [Column("page_count")]
        public int PageCount { get; set; }
        [Column("word_count")]
        public int WordCount { get; set; }
        [Column("line_count")]
        public int LineCount { get; set; }
        [Column("paragraph_count")]
        public int ParagraphCount { get; set; }
        [Column("space_count")]
        public int SpaceCount { get; set; }
        [Column("tab_count")]
        public int TabCount { get; set; }
        [Column("filesize_bytes")]
        public decimal FileSize { get; set; }
    }
}
