using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
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
    public class CollectionSOWService(IUnitOfWork unitOfWork, IMapper _mapper, IConfiguration _config,IActivityLogService _activityLogService) : ICollectionSOWService
    {
        public async Task<List<vCollectionSOWList>> GetDocumentByCollectionId(int id)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                return await context.vCollectionSOWList.Where(x => x.CollectionId == id && !x.IsArchived)
                .OrderByDescending(x => x.VersionNumber).ToListAsync();
            }
        }

        public async Task<vCollectionSOWList> GetActiveDocument(int collectionId)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                return await context.vCollectionSOWList
                .FirstOrDefaultAsync(x => x.CollectionId == collectionId &&
                !x.IsArchived && x.IsActive) ?? new();
            }
        }


        public async Task<bool> ImportSOW(UploadDocumentModel model)
        {
            var dbSOWs = await unitOfWork.CollectionSOWRepository.Query(x => x.CollectionId == model.CollectionId);
            var latestVersion = dbSOWs.Any() ? dbSOWs.Max(x => x.VersionNumber) + 1 : 1;

            var blobContainer = new StorageBlobProvider("releventmatch", _config["BlobConnectionString"]);
            var fileExtension = Path.GetExtension(model.FileName);
            var originalDocAdded = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = model.FileName.Replace(fileExtension, $"_V{latestVersion}{fileExtension}"),
                FileSizeBytes = model.FileSize,
                ImportedOn = DateTime.UtcNow,               
                DocumentName = model.FileName.Replace(fileExtension, $" version {latestVersion}"),
            });
            var originalDocUrl = blobContainer.UploadFile(
                model.FileData,
                model.FileName.Replace(fileExtension, $"_V{latestVersion}{fileExtension}"),
                model.CollectionId.ToString(), originalDocAdded.DocumentId.ToString());


            var sanitizedDocAdded = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                FileSizeBytes = model.FileSize,
                ImportedOn = DateTime.UtcNow,                
                DocumentName = model.FileName.Replace(fileExtension, $" version {latestVersion}"),
            });
            var textWordCountListString = blobContainer.UploadFile(
                Encoding.ASCII.GetBytes(model.TextWordCountListString),
                model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                model.CollectionId.ToString(),
                sanitizedDocAdded.DocumentId.ToString());

            var fileTextDocument = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                FileSizeBytes = model.FileSize,
                ImportedOn = DateTime.UtcNow,                
                DocumentName = model.FileName.Replace(fileExtension, $" version {latestVersion}"),
            });

            var textFromImportFileUrl = blobContainer.UploadFile(
                Encoding.ASCII.GetBytes(model.TextFromImportFile),
                model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                model.CollectionId.ToString(),
                fileTextDocument.DocumentId.ToString());

            var CleanedTextAdded = await unitOfWork.DocumentRepository.AddAsync(new Document
            {
                FileName = model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                FileSizeBytes = model.FileSize,
                ImportedOn = DateTime.UtcNow,                
                DocumentName = model.FileName.Replace(fileExtension, $" version {latestVersion}"),
            });
            var cleanedTextFromImportFileUrl = blobContainer.UploadFile(
                Encoding.ASCII.GetBytes(model.CleanedTextFromImportFile),
                model.FileName.Replace(fileExtension, $"_V{latestVersion}.txt"),
                model.CollectionId.ToString(),
                CleanedTextAdded.DocumentId.ToString());

            originalDocAdded.FileUrl = originalDocUrl;
            await unitOfWork.DocumentRepository.UpdateAsync(originalDocAdded);

            sanitizedDocAdded.FileUrl = textWordCountListString;
            await unitOfWork.DocumentRepository.UpdateAsync(sanitizedDocAdded);

            CleanedTextAdded.FileUrl = cleanedTextFromImportFileUrl;
            await unitOfWork.DocumentRepository.UpdateAsync(CleanedTextAdded);

            fileTextDocument.FileUrl = textFromImportFileUrl;
            await unitOfWork.DocumentRepository.UpdateAsync(fileTextDocument);

            var currentVersion = await unitOfWork.CollectionSOWRepository.Query(x => x.CollectionId == model.CollectionId);

            await unitOfWork.CollectionSOWRepository.SetCollectionSOWAsNotActive(model.CollectionId);
            var sowAdded = await unitOfWork.CollectionSOWRepository.AddAsync(new CollectionSOW
            {
                CollectionId = model.CollectionId,
                SanitizedDocumentId = sanitizedDocAdded.DocumentId,
                OriginalDocumentId = originalDocAdded.DocumentId,
                VersionNumber = currentVersion.Any() ? currentVersion.Max(x => x.VersionNumber) + 1 : 1,
                IsActive = true,
                IsArchived = false,
                CreatedOn = DateTime.UtcNow,
                CreatedById = model.CreatedByUserId,
                LastModifiedById = null,
                LastModifiedOn = null,
                CleanTextDocumentId = CleanedTextAdded.DocumentId,
                ImportedFileTextDocumentId = fileTextDocument.DocumentId,
                PageCount=model.PageCount,
                lineCount=model.LineCount,
                ParagraphCount=model.ParagraphCount,
                WordCount=model.WordCount,
                SpaceCount=model.SpaceCount,
                TabCount=model.TabCount
            });
            var collection = await unitOfWork.CollectionRepository.QueryFirstOrDefaultAsync(s=>s.CollectionId == model.CollectionId);
            await _activityLogService.AddActivityLog($"SOW {originalDocAdded.DocumentName} is imported to collection {collection?.Name ?? ""}");
            return sowAdded.CollectionSOWId > 0;
        }

        public async Task DeleteDocument(CollectionSOWDto doc)
        {
            await unitOfWork.DocumentRepository.DeleteAsync(_mapper.Map<Document>(doc));
        }

        public async Task<bool> ArchiveSOW(int collectionSOWId)
        {
            return await unitOfWork.CollectionSOWRepository.ArchiveCollection(collectionSOWId);
        }

        public async Task<bool> SetAsActive(int CollectionId, int CollectionSOWId)
        {
            return await unitOfWork.CollectionSOWRepository.SetCollectionSOWAsActive(CollectionSOWId, CollectionId);
        }
    }
}
