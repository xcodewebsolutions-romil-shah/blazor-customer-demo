using Customers.Data.Domains;
using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string? DUNNumber { get; set; }
        public string? ShortName { get; set; }
        public string? Acronym { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int? PostCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public int? LastModifiedById { get; set; }
        public bool IsDeleted { get; set; }
        public string logo { get; set; }
    }
}
