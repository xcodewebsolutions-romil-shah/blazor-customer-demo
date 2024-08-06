using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class AddAnalysisModel
    {
        public int CollectionAnalysisId { get; set; }
        public int CollectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SOWDocumentId { get; set; }
        public int ProposalDocumentId { get; set; }
    }
}
