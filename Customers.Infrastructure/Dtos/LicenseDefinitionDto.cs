using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class LicenseDefinitionDto
    {
        public string LicenseId { get; set; }
        public string LicenseName { get; set; }
        public string CollectionsAllowed { get; set; }
        public string UsersPerCollectionAllowed { get; set; }
        public string DaysAllowed { get; set; }
        public bool isTrial {  get; set; }
    }
}
