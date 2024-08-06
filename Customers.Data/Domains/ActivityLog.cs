using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    [Table("activity_log")]
    public class ActivityLog
    {
        [Key]
        [Column("activity_log_id")]
        public int ActivityLogId { get; set; }
        [Column("customer_id")]
        public int? CustomerId { get; set; }
        [Column("user_id")]
        public int? UserId { get; set; }
        [Column("activity_time")]
        public DateTime ActivityTime { get; set; }
        [Column("message")]
        public string Message { get; set; }
        [Column("activity_source")]
        public string ActivitySource { get; set; }
        [Column("log_level")]
        public string LogLevel { get; set; }
    }
}
