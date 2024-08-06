using AutoMapper;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Repositories;
using Customers.Services.Contracts;

namespace Customers.Admin.Services
{
    public class ActivitiyLogService(IUnitOfWork unitOfWork,SecurityService Security):IActivityLogService
    {
        public async Task<int> AddActivityLog(string message, string log_level = "info")
        {
            var claims = await Security.GetUserClaims();
            await unitOfWork.ActivityLogRepository.AddAsync(new ActivityLog
            {
                CustomerId = claims.CustomerId == 0 ? null : claims.CustomerId,
                UserId = claims.UserId == 0 ? null : claims.UserId,
                ActivityTime = DateTime.UtcNow,
                Message = message,
                LogLevel = log_level,
                ActivitySource = "rmo web"
            });
            return 1;
        }
        public async Task<int> AddActivityLogForUser(string message,int userId,string log_level = "info")
        {
            var customerUsers = await unitOfWork.CustomerUsersRepository.QueryFirstOrDefaultAsync(x=>x.AspNetUserId == userId);
            await unitOfWork.ActivityLogRepository.AddAsync(new ActivityLog
            {
                CustomerId = customerUsers?.CustomerId,
                UserId = userId,
                ActivityTime = DateTime.UtcNow,
                Message = message,
                LogLevel = log_level,
                ActivitySource = "rmo web"
            });
            return 1;
        }
    }
}
