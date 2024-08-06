using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("license_definition")]
    public class LicenseDefinition
    {
        [Key]
        [Column("license_id")]
        public int LicenseId { get; set; }
        [Column("license_name")]
        public string LicenseName { get; set; }
        [Column("collections_allowed")]
        public int CollectionsAllowed { get; set; }
        [Column("users_per_collection_allowed")]
        public int UsersPerCollectionAllowed { get; set; }
        [Column("days_allowed")]
        public int DaysAllowed { get; set; }
        [Column("is_trial")]
        public bool isTrial {  get; set; }
    }
}
