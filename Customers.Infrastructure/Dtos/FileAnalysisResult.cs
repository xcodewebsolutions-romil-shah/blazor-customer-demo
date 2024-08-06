namespace Customers.Infrastructure.Dtos
{
    public class FileAnalysisResult
    {
        public int? NumParagraphs { get; set; }
        public int? NumWords { get; set; }
        public int? NumSpaces { get; set; }
        public int? NumPages { get; set; }
        public int? LineCount { get; set; }
    }
}
