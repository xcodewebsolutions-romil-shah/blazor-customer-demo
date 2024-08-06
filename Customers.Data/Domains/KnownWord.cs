using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customers.Data.Domains
{
    [Table("known_word")]
    public class KnownWord
    {
        [Key]
        [Column("word_id")]
        public int WordId { get; set; }
        [Column("index_no")]
        public int? IndexNo { get; set; }
        [Column("word")]
        public string Word { get; set; }
    }
}
