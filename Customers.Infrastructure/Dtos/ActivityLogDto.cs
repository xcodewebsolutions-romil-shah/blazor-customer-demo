using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class ActivityLogDto
    {
        public int ActivityLogId { get; set; }
        public int? CustomerId { get; set; }
        public int? UserId { get; set; }
        public DateTime ActivityTime { get; set; }
        public string Message { get; set; }
        public string ActivitySource { get; set; }
        public string LogLevel { get; set; }
    }
}
