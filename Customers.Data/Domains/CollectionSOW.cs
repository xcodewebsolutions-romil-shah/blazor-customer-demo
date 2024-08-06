using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("collection_sow")]
    public class CollectionSOW
    {
        [Key]
        [Column("collection_sow_id")]
        public int CollectionSOWId { get; set; }
        [Column("collection_id")]
        public int CollectionId { get; set; }
        [ForeignKey("CollectionId")]
        public virtual Collection? Collection { get; set; }
        [Column("sanitized_document_id")]
        public int? SanitizedDocumentId { get; set; }
        [Column("original_document_id")]
        public int? OriginalDocumentId { get; set; }
        [Column("version_no")]
        public int VersionNumber { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
        [Column("is_archived")]
        public bool IsArchived { get; set; }
        [Column("created_on")]
        public DateTime CreatedOn { get; set; }
        [Column("created_by_id")]
        public int CreatedById { get; set; }
        [Column("last_modified_on")]
        public DateTime? LastModifiedOn { get; set; }
        [Column("last_modified_by_id")]
        public int? LastModifiedById { get; set; }
        [Column("clean_text_document_id")]
        public int? CleanTextDocumentId { get; set; }
        [Column("imported_file_text_document_id")]
        public int? ImportedFileTextDocumentId { get; set; }
        [Column("page_count")]
        public int? PageCount { get; set; }
        [Column("space_count")]
        public int? SpaceCount { get; set; }
        [Column("word_count")]
        public int? WordCount { get; set; }
        [Column("line_count")]
        public int? lineCount { get; set; }
        [Column("paragraph_count")]
        public int? ParagraphCount { get; set; }
        [Column("tab_count")]
        public int? TabCount { get; set; }
    }
}
