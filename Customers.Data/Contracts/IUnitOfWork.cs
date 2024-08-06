using Microsoft.EntityFrameworkCore;
using Customers.Data.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContextFactory<CustomersDBContext> DBContextFactory { get; }
        ICollectionRepository<Collection> CollectionRepository { get; }
        ICollectionUsersRepository<CollectionUsers> CollectionUsersRepository { get; }
        ICollectionSOWRepository<CollectionSOW> CollectionSOWRepository { get; }
        IDocumentRepository<Document> DocumentRepository { get; }
        ICustomerLicenseRepository<CustomerLicense> CustomerLicenseRepository { get; }
        ICustomerRepository<Customer> CustomerRepository { get; }
        ICustomerUsersRepository<CustomerUsers> CustomerUsersRepository { get; }
        ICollectionAnalysisRepository<CollectionAnalysis> CollectionAnalysisRepository { get; }
        IAnalysisReportParameterRepository<AnalysisReportParameter> AnalysisReportParameterRepository { get; }
        IAnalysisDetailsRepository<AnalysisDetail> AnalysisDetailsRepository { get; }
        ICollectionProposalRepository<CollectionProposal> CollectionProposalRepository { get; }
        IKnownWordRepository<KnownWord> KnownWordRepository { get; }
        ILicenseRepositary<LicenseDefinition> LicenseRepositary { get; }
        IActivityLogRepository<ActivityLog> ActivityLogRepository { get; }
    }
}
