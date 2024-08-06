using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Crmf;
using Customers.Data.Contracts;
using Customers.Data.Domains;

namespace Customers.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContextFactory<CustomersDBContext> _contextFactory;
        private ICollectionRepository<Collection> _collectionRepository;
        private ICollectionUsersRepository<CollectionUsers> _collectionUserRepository;
        private ICollectionSOWRepository<CollectionSOW> _collectionSowRepository;
        private IDocumentRepository<Document> _documentRepository;
        private ICustomerLicenseRepository<CustomerLicense> _customerLicenseRepository;
        private ICustomerRepository<Customer> _customerRepository;
        private ICustomerUsersRepository<CustomerUsers> _customerUsersRepository;
        private ICollectionAnalysisRepository<CollectionAnalysis> _collectionAnalysisRepository;
        private IAnalysisDetailsRepository<AnalysisDetail> _analysisDetailsRepository;
        private ICollectionProposalRepository<CollectionProposal> _collectionProposalRepository;
        private IKnownWordRepository<KnownWord> _knownWordRepository;        
        private ILicenseRepositary<LicenseDefinition> _licenseRepositary;
        private IAnalysisReportParameterRepository<AnalysisReportParameter> _analysisReportParameterRepository;
        private IActivityLogRepository<ActivityLog> _activityLogRepository;

        public UnitOfWork(IDbContextFactory<CustomersDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public IDbContextFactory<CustomersDBContext> DBContextFactory
        {
            get
            {
                return _contextFactory;
            }
        }
        public ICollectionRepository<Collection> CollectionRepository
        {
            get
            {
                if (_collectionRepository == null)
                    _collectionRepository = new CollectionRepository(_contextFactory);
                return _collectionRepository;
            }
        }

        public ICollectionUsersRepository<CollectionUsers> CollectionUsersRepository
        {
            get
            {
                if(_collectionUserRepository == null)
                    _collectionUserRepository = new CollectionUserRepository(_contextFactory);
                return _collectionUserRepository;
            }
        }

        public ICollectionSOWRepository<CollectionSOW> CollectionSOWRepository
        {
            get
            {
                if(_collectionSowRepository == null)
                    _collectionSowRepository = new CollectionSOWRepository(_contextFactory);
                return _collectionSowRepository;
            }
        }

        public IDocumentRepository<Document> DocumentRepository
        {
            get
            {
                if (_documentRepository == null)
                    _documentRepository = new DocumentRepository(_contextFactory);
                return _documentRepository;
            }
        }
        public ICustomerLicenseRepository<CustomerLicense> CustomerLicenseRepository
        {
            get
            {
                if (_customerLicenseRepository == null)
                    _customerLicenseRepository = new CustomerLicenseRepository(_contextFactory);
                return _customerLicenseRepository;
            }
        }
        public ICustomerRepository<Customer> CustomerRepository
        {
            get
            {
                if (_customerRepository == null)
                    _customerRepository = new CustomerRepository(_contextFactory);
                return _customerRepository;
            }
        }
        public ICustomerUsersRepository<CustomerUsers> CustomerUsersRepository
        {
            get
            {
                if (_customerUsersRepository == null)
                    _customerUsersRepository = new CustomerUsersRepository(_contextFactory);
                return _customerUsersRepository;
            }
        }
        public ICollectionAnalysisRepository<CollectionAnalysis> CollectionAnalysisRepository
        {
            get
            {
                if (_collectionAnalysisRepository == null)
                    _collectionAnalysisRepository = new CollectionAnalysisRepository(_contextFactory);
                return _collectionAnalysisRepository;
            }
        }
        public IAnalysisReportParameterRepository<AnalysisReportParameter> AnalysisReportParameterRepository
        {
            get
            {
                if (_analysisReportParameterRepository == null)
                    _analysisReportParameterRepository = new AnalysisReportParameterRepository(_contextFactory);
                return _analysisReportParameterRepository;
            }
        }
        public IAnalysisDetailsRepository<AnalysisDetail> AnalysisDetailsRepository
        {
            get
            {
                if(_analysisDetailsRepository == null)
                    _analysisDetailsRepository = new AnalysisDetailsRepository(_contextFactory);
                return _analysisDetailsRepository;
            }
        }

        public ICollectionProposalRepository<CollectionProposal> CollectionProposalRepository
        {
            get
            {
                if (_collectionProposalRepository == null)
                    _collectionProposalRepository = new CollectionProposalRepository(_contextFactory);
                return _collectionProposalRepository;
            }
        }
        public IKnownWordRepository<KnownWord> KnownWordRepository
        {
            get
            {
                if (_knownWordRepository == null)
                    _knownWordRepository = new KnownWordRepository(_contextFactory);
                return _knownWordRepository;
            }
        }

         public ILicenseRepositary<LicenseDefinition> LicenseRepositary
        {
            get
            {
                if (_licenseRepositary == null)
                    _licenseRepositary = new LicenseRepositary(_contextFactory);
                return _licenseRepositary;
            }
        }

        public IActivityLogRepository<ActivityLog> ActivityLogRepository
        {
            get
            {
                if (_activityLogRepository == null)
                    _activityLogRepository = new ActivityLogRepository(_contextFactory);
                return _activityLogRepository;
            }
        }

        public void Dispose()
        {
            //_context.Dispose();
        }
    }
}
