namespace Customers.Infrastructure.Dtos
{
    public class DocumentDto
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public decimal FileSizeBytes { get; set; }
        public DateTime ImportedOn { get; set; }
        //public int PageCount { get; set; }
        //public int WordCount { get; set; }
        //public int LineCount { get; set; }
        //public int ParagraphCount { get; set; }
        //public int Spacecount { get; set; }
        //public int TabCount { get; set; }
    }
}
