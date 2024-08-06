using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Customers.Infrastructure.Helper
{
    public class StorageBlobProvider
    {
        private readonly CloudBlobContainer _blobContainer;

        public StorageBlobProvider(string containerKey, string storageConnString)
        {
            var storage = CloudStorageAccount.Parse(storageConnString);
            _blobContainer = CreateOrAssignContainer(storage, containerKey);
        }

        private CloudBlobContainer CreateOrAssignContainer(CloudStorageAccount storage, string containerKey)
        {
            try
            {

                var blobClient = storage.CreateCloudBlobClient();
                BlobRequestOptions bro = new BlobRequestOptions()
                {
                    SingleBlobUploadThresholdInBytes = 1024 * 1024, //1MB, the minimum
                    ParallelOperationThreadCount = 4
                };
                blobClient.DefaultRequestOptions = bro;
                var blobContainer = blobClient.GetContainerReference(containerKey);

                blobContainer.CreateIfNotExistsAsync().Wait();
                // Set the permissions so the blobs are public.                 
                blobContainer.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                }).Wait();
                return blobContainer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string UploadImage(byte[] fileData, string fileName, int? orderID, string DirectoryName)
        {
            CloudBlobDirectory directory = _blobContainer.GetDirectoryReference(orderID.ToString());
            CloudBlobDirectory subDirectory = directory.GetDirectoryReference(DirectoryName);
            CloudBlockBlob cloudBlob = subDirectory.GetBlockBlobReference(fileName);
            using (var stream = new MemoryStream(fileData))
            {
                cloudBlob.UploadFromStreamAsync(stream);
            }
            return cloudBlob.Uri.ToString();
        }
        public string UploadFile(byte[] fileData, string fileName, string directoryNm, string subDirectory)
        {
            CloudBlobDirectory directory = _blobContainer.GetDirectoryReference(directoryNm);
            if (!string.IsNullOrEmpty(subDirectory))
                directory = directory.GetDirectoryReference(subDirectory);
            CloudBlockBlob cloudBlob = directory.GetBlockBlobReference(fileName);
            using (var stream = new MemoryStream(fileData))
            {
                cloudBlob.UploadFromStreamAsync(stream);
            }
            return cloudBlob.Uri.ToString();
        }

        public async Task<byte[]> DownloadFileStream(string fileName, string folderName, string directoryName)
        {
            byte[] bytes = null;
            CloudBlobDirectory directory = _blobContainer.GetDirectoryReference(folderName);
            if (!string.IsNullOrEmpty(directoryName))
                directory = directory.GetDirectoryReference(directoryName);
            //CloudBlobDirectory subDirectory = directory.GetDirectoryReference(directoryName);
            CloudBlockBlob cloudBlob = directory.GetBlockBlobReference(fileName);
            using (MemoryStream memStream = new MemoryStream())
            {
                await cloudBlob.DownloadToStreamAsync(memStream);
                bytes = memStream.ToArray();
            }

            return bytes;
        }

        public async Task DownloadAsync(string fileName, string folderName, string directoryName)
        {
            CloudBlobDirectory directory = _blobContainer.GetDirectoryReference(folderName);
            CloudBlobDirectory subDirectory = directory.GetDirectoryReference(directoryName);
            CloudBlockBlob cloudBlob = subDirectory.GetBlockBlobReference(fileName);
            await cloudBlob.DownloadToFileAsync("{{path}}", FileMode.OpenOrCreate);
        }

        public async Task DeleteAsync(string fileName, int? orderID, string directoryName)
        {
            CloudBlobDirectory directory = _blobContainer.GetDirectoryReference(orderID.ToString());
            CloudBlobDirectory subDirectory = directory.GetDirectoryReference(directoryName);
            CloudBlockBlob cloudBlob = subDirectory.GetBlockBlobReference(fileName);
            await cloudBlob.DeleteIfExistsAsync();
        }
    }
}
