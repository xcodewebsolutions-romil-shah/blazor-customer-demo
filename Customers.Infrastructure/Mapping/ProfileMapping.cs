using AutoMapper;
using Customers.Data.Domains;
using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Infrastructure.Mapping
{
    public class ProfileMapping : Profile
    {
        public ProfileMapping()
        {
            CreateMap<CollectionSOW, CollectionSOWDto>().ReverseMap();
            CreateMap<Collection, CollectionDto>().ReverseMap();
            CreateMap<Collection, AddCollectionDto>().ReverseMap();
            CreateMap<CollectionDto, AddCollectionDto>().ReverseMap();
            CreateMap<AnalysisReportParameter, AnalysisReportParameterDto>().ReverseMap();
            CreateMap<CustomerLicense, CustomerLicenseDto>().ReverseMap();
            CreateMap<LicenseDefinition, LicenseDefinitionDto>().ReverseMap();
            CreateMap<Document, DocumentDto>().ReverseMap();
            CreateMap<AnalysisDetail,AnalysisDetailDto>().ReverseMap();
            CreateMap<CollectionAnalysis, CollectionAnalysisDto>().ReverseMap();
            CreateMap<KnownWord, KnownWordDto>().ReverseMap();
            CreateMap<ActivityLog, ActivityLogDto>().ReverseMap();
        }
    }
}
