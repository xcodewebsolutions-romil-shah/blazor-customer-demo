using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("customer_license")]
    public class CustomerLicense
    {
        [Column("customer_license_id")]
        public int CustomerLicenseId { get; set; }
        [Column("customer_id")]
        public int CustomerId { get; set; }
        [Column("license_definition_id")]
        public int LicenseDefinitionId { get; set; }
        [Column("start_date")]
        public DateTime StartDate { get; set; }
        [Column("end_date")]
        public DateTime EndDate { get; set; }
        [Column("collections_allowed")]
        public int CollectionsAllowed { get; set; }
        [Column("users_per_collection_allowed")]
        public int UsersPerCollectionAllowed { get; set; }
        [Column("days_allowed")]
        public int DaysAllowed { get; set; }
        [Column("is_current")]
        public bool IsCurrent { get; set; }
    }
}
