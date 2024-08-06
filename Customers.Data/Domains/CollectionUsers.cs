using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("collection_users")]
    public class CollectionUsers
    {
        [Key]
        [Column("collection_users_id")]
        public int CollectionUsersId { get; set; }
        [Column("collection_id")]
        public int CollectionId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("created_by")]
        public int CreatedBy { get; set; }
        [Column("created_on")]
        public DateTime CreatedOn { get; set; }
    }
}
