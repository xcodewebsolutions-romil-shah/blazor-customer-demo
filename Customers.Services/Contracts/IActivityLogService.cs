using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    public interface IActivityLogService
    {
        Task<int> AddActivityLog(string message, string log_level = "info");
        Task<int> AddActivityLogForUser(string message, int userId, string log_level = "info");
    }
}
