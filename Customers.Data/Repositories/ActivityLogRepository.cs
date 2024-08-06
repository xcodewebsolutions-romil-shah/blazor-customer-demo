using Microsoft.EntityFrameworkCore;
using Customers.Data.Contracts;
using Customers.Data.Domains;

namespace Customers.Data.Repositories
{
    public class ActivityLogRepository : GenericRepository<ActivityLog>,IActivityLogRepository<ActivityLog>
    {
        public ActivityLogRepository(IDbContextFactory<CustomersDBContext> contextFactory) : base(contextFactory) { }
    }
}
