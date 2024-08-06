using AutoMapper;
using CalculateTCIViaDLL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Data.Views;
using Customers.Infrastructure.Dtos;
using Customers.Infrastructure.Helper;
using Customers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Services
{
    public class CollectionAnalysisService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration _config,IActivityLogService _activityLogService) : ICollectionAnalysisService
    {
        public async Task<List<CollectionAnalysisDto>> GetAnalysesByCollectionId(int collectionId)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                return mapper.Map<List<CollectionAnalysisDto>>
                (await context.CollectionAnalyses
                .Include(x => x.RunByUser).Include(x => x.CollectionProposal)
                .Where(x => x.CollectionId == collectionId && x.Status == "active")
                .ToListAsync());
            }
        }

        public async Task<AnalysisReportParameterDto> GetReportParameterByAnalysisId(int collectionAnalysisId)
        {
            return mapper.Map<AnalysisReportParameterDto>(await unitOfWork.AnalysisReportParameterRepository
                .QueryFirstOrDefaultAsync(x=>x.CollectionAnalysisId == collectionAnalysisId));
        }        

        public async Task<bool> AddOrUpdateReportParameterForAnalysis(AnalysisReportParameterDto reportParameter)
        {
            reportParameter.LastModifiedOn = DateTime.UtcNow;
            var analysisReportParameter = mapper.Map<AnalysisReportParameter>(reportParameter);
            var dbParameters = await unitOfWork.AnalysisReportParameterRepository
                                        .QueryFirstOrDefaultAsync(x=>x.CollectionAnalysisId == reportParameter.CollectionAnalysisId);

            if(dbParameters is null || dbParameters.AnalysisReportParameterId == 0)
                await unitOfWork.AnalysisReportParameterRepository.AddAsync(analysisReportParameter);                
            else
                await unitOfWork.AnalysisReportParameterRepository.UpdateAsync(analysisReportParameter);
            return true;
        }

        public async Task<AnalysisDocumentCounts> GetAnalysisCount(int collectionAnalysisId)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                CollectionAnalysis? analysis = await context.CollectionAnalyses
                .Include(a => a.CollectionSOW)
                .Include(p => p.CollectionProposal)
                .FirstOrDefaultAsync(x => x.CollectionAnalysisId == collectionAnalysisId);

                if (analysis == null)
                    return new();

                var collection = await unitOfWork.CollectionRepository.QueryFirstOrDefaultAsync(x => x.CollectionId == analysis.CollectionId);

                var SOWParagraphPerPage = 0;
                if (analysis.CollectionSOW is null || analysis.CollectionSOW.PageCount == 0)
                    SOWParagraphPerPage = 0;
                else
                    SOWParagraphPerPage = analysis.CollectionSOW.ParagraphCount ?? 0 / analysis.CollectionSOW.PageCount ?? 1;

                var SOWWordsPerPage = 0;
                if (analysis.CollectionSOW is null || analysis.CollectionSOW.PageCount == 0)
                    SOWWordsPerPage = 0;
                else
                    SOWWordsPerPage = analysis.CollectionSOW.WordCount ?? 0/ analysis.CollectionSOW.PageCount ?? 1;

                return new()
                {
                    AllowablePageCount = Convert.ToInt32(collection?.AllowedPageCount ?? "0"),
                    TotalWords = analysis.CollectionProposal?.WordCount ?? 0,
                    SOWParagraphPerPage = SOWParagraphPerPage,
                    ApproxPageCount = analysis.CollectionProposal?.PageCount ?? 0,
                    TotalParagraphs = analysis.CollectionProposal?.ParagraphCount ?? 0,
                    SOWWordsPerPage = SOWWordsPerPage,
                };
            }
        }

        public async Task<vCollectionAnalysisDetails> GetAnalysisViewModel(int collectionAnalysisId)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                return await context.vcollectionanalysisdetail
                .FirstOrDefaultAsync(a => a.CollectionAnalysisId == collectionAnalysisId) ?? new();
            }
        }

        public async Task<KPIResultModel> GetKPIResultForAnalysis(int collectionId,int analysisId)
        {
            return await unitOfWork.CollectionAnalysisRepository.GetKPIResultForAnalysis(collectionId,analysisId);
        }

        public async Task<int> AddAnalysis(int userId, int sowId, int proposalId, int collectionId, string proposalName)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                var analysis = new CollectionAnalysis
                {
                    RunByUserId = userId,
                    CollectionId = collectionId,
                    Name = proposalName,
                    SOWDocumentId = sowId,
                    ProposalDocumentId = proposalId,
                    RunNumber = 1,
                    Status = "active",
                    RunDateTime = DateTime.UtcNow,
                    TCScore = 0,
                    Trend = "up",
                    TCDocumentId = null
                };
                var dbAnalysis = await unitOfWork.CollectionAnalysisRepository.AddAsync(analysis);


                var analysisView = await context.vcollectionanalysisdetail
               .FirstOrDefaultAsync(x => x.CollectionAnalysisId == dbAnalysis.CollectionAnalysisId);

                var collectionSOW = await unitOfWork.CollectionSOWRepository
                    .QueryFirstOrDefaultAsync(x => x.CollectionSOWId == sowId);

                var collectionProposal = await unitOfWork.CollectionProposalRepository
                    .QueryFirstOrDefaultAsync(x => x.CollectionProposalId == proposalId);


                if (analysisView == null || collectionProposal == null || collectionSOW == null)
                    return 0;

                var originalSOWDocument = await unitOfWork.DocumentRepository
                    .QueryFirstOrDefaultAsync(x => x.DocumentId == collectionSOW.OriginalDocumentId);

                var originalProposalDocument = await unitOfWork.DocumentRepository
                    .QueryFirstOrDefaultAsync(x => x.DocumentId == collectionProposal.OriginalDocumentId);

                var blobContainer = new StorageBlobProvider("releventmatch", _config["BlobConnectionString"]);

                var sowFile = await blobContainer.DownloadFileStream(
                    analysisView.SowSanitizedDocName,
                    analysis.CollectionId.ToString(),
                    analysisView.SowSanitizedDocId.ToString());

                var sowText = Encoding.UTF8.GetString(sowFile);


                var sowCleanedFile = await blobContainer.DownloadFileStream(
                    analysisView.SowSanitizedDocName,
                    analysis.CollectionId.ToString(),
                    collectionSOW.CleanTextDocumentId.ToString());

                var sowCleanedText = Encoding.UTF8.GetString(sowCleanedFile);

                var proposalFile = await blobContainer.DownloadFileStream(
                    analysisView.ProposalSanitizedDocName,
                    analysis.CollectionId.ToString(),
                    analysisView.ProposalSanitizedDocId.ToString());

                var proposalText = Encoding.UTF8.GetString(proposalFile);

                var proposalCleanedFile = await blobContainer.DownloadFileStream(
                 analysisView.ProposalSanitizedDocName,
                 analysis.CollectionId.ToString(),
                 collectionProposal.CleanTextDocumentId.ToString());

                var proposalCleanedText = Encoding.UTF8.GetString(proposalCleanedFile);

                List<AnalysisDetail>? analysisDetails = FileHelper.GetAnalysisDetails(sowText, proposalText);

                OutputClass outPut = FileHelper.CalculateTCI(analysisDetails.Select(ad => new WordCountComparisonList
                {
                    Word = ad.Word,
                    WordCountSOW = ad.SOWCount,
                    WordCountVP = ad.ProposalCount
                }).ToList(),
                new ActiveRecordSOW
                {
                    PagesCount = collectionSOW.PageCount,
                    ParagraphCount = collectionSOW.ParagraphCount == 0 ? 157 : collectionSOW.ParagraphCount,
                    CleanedText = sowCleanedText
                }, new ActiveRecordProposal
                {
                    PagesCount = collectionProposal.PageCount,
                    ParagraphCount = collectionProposal.ParagraphCount == 0 ? 91 : collectionProposal.ParagraphCount,
                    CleanedText = proposalCleanedText
                });

                double TCScore = outPut.TCI;

                analysisDetails = analysisDetails.Take(50).ToList();
                analysisDetails.ForEach(x => x.CollectionAnalysisId = dbAnalysis.CollectionAnalysisId);
                await unitOfWork.AnalysisDetailsRepository.AddRangeAsync(analysisDetails);

                var toUpdateAnalysis = await context.CollectionAnalyses.FindAsync(dbAnalysis.CollectionAnalysisId);
                if (toUpdateAnalysis == null)
                    return 0;

                var prevAnalysis = await unitOfWork.CollectionAnalysisRepository
                                    .Query(x => x.CollectionId == collectionId &&
                                        x.CollectionAnalysisId != dbAnalysis.CollectionAnalysisId &&
                                        x.Status == "active");

                if (prevAnalysis is null || prevAnalysis.Count() == 0)
                {
                    toUpdateAnalysis.Trend = "up";

                    if (TCScore is double.NaN)
                        toUpdateAnalysis.TCScore = 0;
                    else
                        toUpdateAnalysis.TCScore = decimal.Round(Convert.ToDecimal(TCScore), 2);
                }
                else
                {
                    var latestTCScore = prevAnalysis.MaxBy(x => x.CollectionAnalysisId)?.TCScore ?? 0;
                    if (TCScore is double.NaN)
                    {
                        toUpdateAnalysis.TCScore = 0;
                        toUpdateAnalysis.Trend = "down";
                    }
                    else
                    {
                        var currentTCScore = decimal.Round(Convert.ToDecimal(TCScore), 2);
                        toUpdateAnalysis.Trend = latestTCScore == currentTCScore ? "same" : (latestTCScore > currentTCScore ? "down" : "up");
                        toUpdateAnalysis.TCScore = decimal.Round(Convert.ToDecimal(TCScore), 2);
                    }
                    toUpdateAnalysis.RunNumber = prevAnalysis.Count() + 1;
                }

                toUpdateAnalysis.CPIndex = (decimal)outPut.CrossProduct;
                toUpdateAnalysis.ArrayIndex = (decimal)outPut.ArrayXY;
                toUpdateAnalysis.MCSIndex = (decimal)outPut.MCS;
                if (TCScore is not double.NaN)
                    toUpdateAnalysis.Correl = (decimal)(outPut.TCI / 100);
                else
                    toUpdateAnalysis.Correl = 0;

                await unitOfWork.CollectionAnalysisRepository.UpdateAsync(toUpdateAnalysis);
                await _activityLogService.AddActivityLog($"Analysis is generated with TC Score {toUpdateAnalysis.TCScore}");
                return dbAnalysis.CollectionAnalysisId;
            }
        }

        public async Task<int> SaveAnalysis(int collectionAnalysisId, byte[] FileBytes,string FileName)
        {
            var extension = Path.GetExtension(FileName);

            FileName = FileName.Replace(extension, ".pdf");

            var analysis = await unitOfWork.CollectionAnalysisRepository
                .QueryFirstOrDefaultAsync(x => x.CollectionAnalysisId == collectionAnalysisId);

            if (analysis is null) return 0;

            var blobContainer = new StorageBlobProvider("releventmatch", _config["BlobConnectionString"]);

            var DocAdded = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = FileName,
                FileSizeBytes = FileBytes.Length,
                ImportedOn = DateTime.UtcNow,         
                DocumentName = FileName,
                //FileUrl = originalDocUrl
            });

            var DocUrl = blobContainer.UploadFile(
                FileBytes,
                FileName,
                $"analysis/{collectionAnalysisId}",
                DocAdded.DocumentId.ToString());

            DocAdded.FileUrl = DocUrl;
            await unitOfWork.DocumentRepository.UpdateAsync(DocAdded);

            analysis.Status = "active";
            analysis.TCDocumentId = DocAdded.DocumentId;
            await unitOfWork.CollectionAnalysisRepository.UpdateAsync(analysis);
            return 1;
        }

        public async Task<ViewAnalysisDocDetails> GetCollectionAnalysisDocDetails(int collectionSOWId, int collectionProposalId)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                var collectionSOW = await context.vCollectionSOWList
                .FirstOrDefaultAsync(x => x.CollectionSOWId == collectionSOWId);

                var collectionProposal = await context.vCollectionProposalList
                    .FirstOrDefaultAsync(x => x.CollectionProposalId == collectionProposalId);

                return new()
                {
                    SOW = collectionSOW ?? new(),
                    Proposal = collectionProposal ?? new()
                };
            }
        }
        //public async Task<AnalysisReportParameter> GetAnalysis
    }
}
