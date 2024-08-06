using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Models
{
    public class UploadDocumentModel
    {
        public int CollectionId { get; set; }
        public byte[] FileData { get; set; }
        public string TextFromImportFile { get; set; }
        public string CleanedTextFromImportFile { get; set; }
        public string TextWordCountListString { get; set; }
        public string FileName { get; set; }    
        public decimal FileSize { get; set; }
        public int PageCount { get; set; }
        public int WordCount{ get; set; }
        public int LineCount { get; set; }
        public int ParagraphCount { get; set; }
        public int SpaceCount { get; set; }
        public int TabCount { get; set; }
        public string DocumentName { get; set; }
        public int CreatedByUserId { get; set; }
    }
}
