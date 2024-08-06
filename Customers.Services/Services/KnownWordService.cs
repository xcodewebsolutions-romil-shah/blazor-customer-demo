using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Infrastructure.Dtos;
using Customers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Services
{
    public class KnownWordService(IUnitOfWork unitOfWork,IMapper mapper) : IKnownWordService
    {
        public async Task<List<KnownWordDto>> GetKnownWords()
        {
            IEnumerable<KnownWord>? data = await unitOfWork.KnownWordRepository.GetAllAsync();            
            return mapper.Map<List<KnownWordDto>>(data);
        }
    }
}
