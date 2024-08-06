using Customers.Data.Domains;
using Customers.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Customers.Infrastructure.Dtos
{
    public class CollectionDto
    {
        public int CollectionId { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        //public string FullName { get; set; }
        public string ShortName { get; set; }
        public string? Description { get; set; }
        public string AllowedPageCount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedById { get; set; }
        public DateTime? LastModifiedOn  { get; set; }
        public int? LastModifiedById { get; set; }
        public bool IsClosed { get; set; }
        public ApplicationUser? LastModifiedByUser { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public int? ArchivedById { get; set; }
        public DateTime? ArchivedOn { get; set; }
        public int? ClosedById { get; set; }
        public DateTime? ClosedOn { get; set; }
        public Customer? Customer { get; set; }
        public string? NameOfTheIssuer { get; set;}
        public string? IssuersShortName { get; set; }
        public string? IssuersAcronym { get; set; }
        public virtual List<CollectionSOW>? CollectionSOWs { get; set; }

    }

    public class AddCollectionDto : CollectionDto
    {
        public List<int> CollectionUsers { get; set; }
    }

    public class AccountUsers
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
