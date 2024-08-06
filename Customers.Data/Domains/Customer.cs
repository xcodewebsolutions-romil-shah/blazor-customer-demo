using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("customer")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("dun_number")]
        public string? DUNNumber { get; set; }
        [Column("short_name")]
        public string? ShortName { get; set; }
        [Column("acronym")]
        public string? Acronym { get; set; }
        [Column("address")]
        public string? Address { get; set; }
        [Column("city")]
        public string? City { get; set; }
        [Column("state")]
        public string? State { get; set; }
        [Column("postcode")]
        public int? PostCode { get; set; }
        [Column("created_on")]
        public DateTime CreatedOn { get; set; }
        [Column("created_by_id")]
        public int CreatedById { get; set; }
        [Column("last_modified_on")]
        public DateTime? LastModifiedOn { get; set; }
        [Column("last_modified_by_id")]
        public int? LastModifiedById { get; set; }
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
        [Column("logo")]
        public string? logo { get; set; }

    }
}
