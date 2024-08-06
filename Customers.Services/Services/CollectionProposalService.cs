using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Data.Repositories;
using Customers.Data.Views;
using Customers.Infrastructure.Dtos;
using Customers.Infrastructure.Helper;
using Customers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Services
{
    public class CollectionProposalService(IUnitOfWork unitOfWork, IConfiguration _config, IMapper _mapper,IActivityLogService _activityLogService) : ICollectionProposalService
    {
        public async Task<List<vCollectionProposalList>> GetDocumentByCollectionId(int id)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                return await context.vCollectionProposalList.Where(x => x.CollectionId == id && !x.IsArchived)
                .OrderByDescending(x => x.VersionNumber).ToListAsync();
            }
        }

        public async Task<Tuple<int, string>> ImportProposal(UploadDocumentModel model)
        {
            var dbProposals = await unitOfWork.CollectionProposalRepository.Query(x => x.CollectionId == model.CollectionId);
            var latestVersion = dbProposals.Any() ? dbProposals.Max(x => x.VersionNumber) + 1 : 1;

            var blobContainer = new StorageBlobProvider("releventmatch", _config["BlobConnectionString"]);
            var fileExtension = Path.GetExtension(model.FileName);

            var originalDocAdded = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = model.FileName.Replace(fileExtension, $"_V{latestVersion}{fileExtension}"),
                FileSizeBytes = model.FileSize,
                ImportedOn = DateTime.UtcNow,
                DocumentName = model.FileName.Replace(fileExtension, $" version {latestVersion}"),
                //FileUrl = originalDocUrl
            });
            var originalDocUrl = blobContainer.UploadFile(
                model.FileData,
                model.FileName.Replace(fileExtension, $"_V{latestVersion}{fileExtension}"),
                $"{model.CollectionId}",
                originalDocAdded.DocumentId.ToString());

            var sanitizedDocAdded = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                FileSizeBytes = model.FileSize,
                ImportedOn = DateTime.UtcNow,          
                DocumentName = model.FileName.Replace(fileExtension, $" version {latestVersion}"),
                //FileUrl = sanitizedDocurl
            });

            var sanitizedDocurl = blobContainer.UploadFile(
                Encoding.ASCII.GetBytes(model.TextWordCountListString),
                model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                $"{model.CollectionId}",
                sanitizedDocAdded.DocumentId.ToString());

            var fileTextDocument = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                FileSizeBytes = model.FileSize,
                ImportedOn = DateTime.UtcNow,              
                DocumentName = model.FileName.Replace(fileExtension, $" version {latestVersion}"),
                //FileUrl = sanitizedDocurl
            });
            var textFromImportFileUrl = blobContainer.UploadFile(
                Encoding.ASCII.GetBytes(model.TextFromImportFile),
                model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                $"{model.CollectionId}", fileTextDocument.DocumentId.ToString());

            var CleanedTextAdded = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                FileSizeBytes = model.FileSize,
                ImportedOn = DateTime.UtcNow,   
                DocumentName = model.FileName.Replace(fileExtension, $" version {latestVersion}"),
                //FileUrl = sanitizedDocurl
            });
            var cleanedTextFromImportFileUrl = blobContainer.UploadFile(
                Encoding.ASCII.GetBytes(model.CleanedTextFromImportFile),
                model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                $"{model.CollectionId}", CleanedTextAdded.DocumentId.ToString());


            originalDocAdded.FileUrl = originalDocUrl;
            await unitOfWork.DocumentRepository.UpdateAsync(originalDocAdded);

            sanitizedDocAdded.FileUrl = sanitizedDocurl;
            await unitOfWork.DocumentRepository.UpdateAsync(sanitizedDocAdded);

            CleanedTextAdded.FileUrl = cleanedTextFromImportFileUrl;
            await unitOfWork.DocumentRepository.UpdateAsync(CleanedTextAdded);

            fileTextDocument.FileUrl = textFromImportFileUrl;
            await unitOfWork.DocumentRepository.UpdateAsync(fileTextDocument);

            await unitOfWork.CollectionProposalRepository.SetCollectionProposalAsNotActive(model.CollectionId);
            var proposalAdded = await unitOfWork.CollectionProposalRepository.AddAsync(new CollectionProposal
            {
                CollectionId = model.CollectionId,
                SanitizedDocumentId = sanitizedDocAdded.DocumentId,
                OriginalDocumentId = originalDocAdded.DocumentId,
                VersionNumber = latestVersion,
                IsActive = true,
                IsArchived = false,
                CreatedOn = DateTime.UtcNow,
                CreatedById = model.CreatedByUserId,
                LastModifiedById = null,
                LastModifiedOn = null,
                CleanTextDocumentId = CleanedTextAdded.DocumentId,
                ImportedFileTextDocumentId = fileTextDocument.DocumentId,
                PageCount = model.PageCount,
                lineCount = model.LineCount,
                ParagraphCount = model.ParagraphCount,
                WordCount = model.WordCount,
                SpaceCount = model.SpaceCount,
                TabCount = model.TabCount
            });

            var collection = await unitOfWork.CollectionRepository.QueryFirstOrDefaultAsync(s => s.CollectionId == model.CollectionId);
            await _activityLogService.AddActivityLog($"Proposal {originalDocAdded.DocumentName} is imported to collection {collection?.Name ?? ""}");
            return new Tuple<int, string>(proposalAdded.CollectionProposalId, originalDocAdded.FileName);
        }

        public async Task<bool> ArchiveProposal(int collectionProposalId)
        {
            //var proposal = await unitOfWork.CollectionProposalRepository.QueryFirstOrDefaultAsync(x => x.CollectionProposalId == collectionProposalId);
            //var proposalDocumentIds = new int[] { proposal.OriginalDocumentId, proposal.SanitizedDocumentId, proposal.CleanTextDocumentId, proposal.ImportedFileTextDocumentId };

            //var documents = await unitOfWork.DocumentRepository.Query(d => proposalDocumentIds.Contains(d.DocumentId));

            //var blobContainer = new StorageBlobProvider("releventmatch", _config["BlobConnectionString"]);
            
            //foreach (var doc in documents)
            //{
            //    await blobContainer.DeleteAsync(doc.FileName, proposal.CollectionId, doc.DocumentId.ToString());
            //}
            //await unitOfWork.DocumentRepository.DeleteRangeAsync(documents);

            return await unitOfWork.CollectionProposalRepository.ArchiveProposal(collectionProposalId);
        }

        public async Task DeleteDocument(CollectionProposalDto doc)
        {
            await unitOfWork.DocumentRepository.DeleteAsync(_mapper.Map<Document>(doc));
        }

        public async Task<bool> SetAsActive(int CollectionId, int CollectionProposalId)
        {
            return await unitOfWork.CollectionProposalRepository.SetCollectionProposalAsActive(CollectionProposalId, CollectionId);
        }
    }
}
