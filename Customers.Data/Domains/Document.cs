using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("document")]
    public class Document
    {
        [Key]
        [Column("document_id")]
        public int DocumentId { get; set; }
        [Column("filename")]
        public string FileName { get; set; }
        [Column("filesize_bytes")]
        public decimal FileSizeBytes { get; set; }
        [Column("imported_on")]
        public DateTime ImportedOn { get; set; }
        //[Column("page_count")]
        //public int PageCount { get; set; }
        //[Column("word_count")]
        //public int WordCount { get; set; }
        //[Column("line_count")]
        //public int LineCount { get; set; }
        //[Column("paragraph_count")]
        //public int ParagraphCount { get; set; }
        //[Column("space_count")]
        //public int Spacecount { get; set; }
        //[Column("tab_count")]
        //public int TabCount { get; set; }
        [Column("document_name")]
        public string DocumentName { get; set; }
        [Column("file_url")]
        public string? FileUrl { get; set; }
    }
}
