using Customers.Data.Domains;
using Customers.Data.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class ViewAnalysisModel
    {
        public int CollectionId { get; set; }
        public CollectionAnalysisDto Analysis { get; set; }
        public vCollectionSOWList CollectionSOW { get; set; }
        public vCollectionProposalList CollectionProposal { get; set; }
    }
}
