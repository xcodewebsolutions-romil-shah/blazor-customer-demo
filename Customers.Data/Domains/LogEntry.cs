using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Domains
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
