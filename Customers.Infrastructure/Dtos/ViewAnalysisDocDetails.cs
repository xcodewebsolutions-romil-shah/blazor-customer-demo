using Customers.Data.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Dtos
{
    public class ViewAnalysisDocDetails
    {
        public vCollectionProposalList Proposal { get; set; }
        public vCollectionSOWList SOW { get; set; }
    }
}
