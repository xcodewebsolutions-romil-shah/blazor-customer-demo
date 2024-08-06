using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("customer_users")]
    public class CustomerUsers
    {
        [Key]
        [Column("customer_user_id")]
        public int CustomerUseId { get; set; }
        [Column("customer_id")]
        public int CustomerId { get; set; }
        [Column("aspnet_userid")]
        public int AspNetUserId { get; set; }
        [ForeignKey("AspNetUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
