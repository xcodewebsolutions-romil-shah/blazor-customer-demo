using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class KnownWordDto
    {
        public int WordId { get; set; }
        public int? IndexNo { get; set; }
        public string Word { get; set; }
    }
}
