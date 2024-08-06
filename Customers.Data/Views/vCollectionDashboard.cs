using Customers.Data.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Views
{
    [Table("vcollection_dashboard")]
    public class vCollectionDashboard
    {
        [Column("collection_id")]
        public int CollectionId { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("shortname")]
        public string ShortName { get; set; }
        [Column("allowed_page_count")]
        public string AllowedPageCount { get; set; }
        [Column("due_date")]
        public DateTime DueDate { get; set; }
        [Column("created_on")]
        public DateTime CreatedOn { get; set; }
        [Column("sow_id")]
        public int? SOWId { get; set; }
        [Column("sow_document_name")]
        public string? SOWDocumentName { get; set; }
        [Column("sow_version")]
        public int? SOWVersion { get; set; }
        [Column("sow_imported_on")]
        public DateTime? SOWImportedOn { get; set; }
        [Column("sow_filename")]
        public string? SOWFileName { get; set; }
        [Column("proposal_document_name")]
        public string? ProposalDocumentName { get; set; }
        [Column("proposal_version")]
        public int? ProposalVersion { get; set; }
        [Column("proposal_imported_on")]
        public DateTime? ProposalImportedOn { get; set; }
        [Column("proposal_filename")]
        public string? ProposalFileName { get; set; }
        [Column("collection_archived")]
        public bool CollectionArchived { get; set; }
        [Column("collection_closed")]
        public bool CollectionClosed { get; set; }
    }
}
