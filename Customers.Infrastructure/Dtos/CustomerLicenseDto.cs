using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class CustomerLicenseDto
    {
        public int CustomerLicenseId { get; set; }
        public int CustomerId { get; set; }
        public int LicenseDefinitionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CollectionsAllowed { get; set; }
        public int UsersPerCollectionAllowed { get; set; }
        public int DaysAllowed { get; set; }
        public bool IsCurrent { get; set; }
    }
}

