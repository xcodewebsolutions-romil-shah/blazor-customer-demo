using Microsoft.VisualBasic;
using Customers.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Customers.Data.Domains
{
    [Table("collection")]
    public class Collection
    {    
        [Column("collection_id")]
        public int CollectionId { get; set; }
        [Column("customer_id")]
        public int CustomerId { get; set; }
        [Column("name")]
        public string Name { get; set; }
        //[Column("fullname")]
        //public string FullName { get; set; }
        [Column("shortname")]
        public string ShortName { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("allowed_page_count")]
        public string AllowedPageCount { get; set; }
        [Column("due_date")]
        public DateTime DueDate { get; set; }        
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
        [Column("is_closed")]
        public bool IsClosed { get; set; }
        [ForeignKey("LastModifiedById")]
        public ApplicationUser? LastModifiedByUser { get; set; }
        [ForeignKey("CreatedById")]
        public ApplicationUser CreatedByUser { get; set; }
        [Column("archived_by_id")]
        public int? ArchivedById { get; set; }
        [Column("archived_on")]
        public DateTime? ArchivedOn { get; set; }
        [Column("closed_by_id")]
        public int? ClosedById { get; set; }
        [Column("closed_on")]
        public DateTime? ClosedOn { get; set; }
        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
        [Column("name_of_the_issuer")]
        public string? NameOfTheIssuer { get; set; }
        [Column("issuer_short_name")]
        public string? IssuersShortName { get; set; }
        [Column("issuer_acronym")]
        public string? IssuersAcronym { get; set; }        
        public virtual ICollection<CollectionSOW>? CollectionSOWs { get; set; }

    }
}
