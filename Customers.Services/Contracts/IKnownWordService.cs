using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    public interface IKnownWordService
    {
        Task<List<KnownWordDto>> GetKnownWords();
    }
}
